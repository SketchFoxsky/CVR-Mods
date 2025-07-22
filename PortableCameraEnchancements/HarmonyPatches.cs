using HarmonyLib;
using UnityEngine;
using ABI_RC.Systems.Camera.VisualMods;
using ABI_RC.Systems.Movement;
using System.Reflection;
using System;
using ABI_RC.Systems.Camera;
using MelonLoader;
using Sketch.PortableCameraEnchancements.VisualMods;

namespace Sketch.PortableCameraEnchancements.HarmonyPatches
{
    [HarmonyPatch(typeof(PlayerFaceTracking), nameof(PlayerFaceTracking.Update))]
    public static class PlayerFaceTracking_Update_PrefixPatch
    {
        static FieldInfo cameraField = AccessTools.Field(typeof(PlayerFaceTracking), "_camera");
        static FieldInfo isTrackingField = AccessTools.Field(typeof(PlayerFaceTracking), "_isTracking");

        static bool Prefix(PlayerFaceTracking __instance)
        {
            bool isTracking = (bool)isTrackingField.GetValue(__instance);
            if (!isTracking)
                return false; // skip original if not tracking

            // Get the _camera Transform from the instance (private field)
            Transform cameraTransform = cameraField.GetValue(__instance) as Transform;
            if (cameraTransform == null)
                return false; // skip original to avoid errors

            // Original target position
            Vector3 defaultTarget = BetterBetterCharacterController.Instance.RotationPivot.position;

            // Use mod settings and bones
            if (!PortableCameraEnchancements.UseNewLookTarget || PortableCameraEnchancements.Head == null)
            {
                // Just do original behavior (look at default target)
                RotateCameraTowards(cameraTransform, defaultTarget);
                return false; // skip original
            }

            // Calculate blend factor based on distance from original target to camera
            float distance = Vector3.Distance(cameraTransform.position, defaultTarget);

            // Scale with player height cause duh 1m for tiny people is not the same for big people
            float avatarHeight = PortableCameraEnchancements.GetAvatarHeight();
            float scale = avatarHeight / 1.75f;

            float min = PortableCameraEnchancements.BlendMinDistance * scale;
            float max = PortableCameraEnchancements.BlendMaxDistance * scale;

            // Clamp and normalize between min and max
            float t = Mathf.InverseLerp(min, max, distance); // 0 = close, 1 = far
            float blendFactor = Mathf.Clamp01(t);
            blendFactor = blendFactor * blendFactor * (3f - 2f * blendFactor); // SMOOOOTH


            Vector3 blendedTarget;

            if (PortableCameraEnchancements.AutoBlendTargets)
            {
                blendedTarget = Vector3.Lerp(defaultTarget, PortableCameraEnchancements.ModdedTarget, blendFactor);
                PortableCameraEnchancements.BlendedTarget = blendedTarget;
            }
            else
            {
                blendedTarget = PortableCameraEnchancements.ModdedTarget;
            }

            RotateCameraTowards(cameraTransform, blendedTarget);

            return false; // skip original Update()
        }

        private static void RotateCameraTowards(Transform cameraTransform, Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - cameraTransform.position).normalized;
            Vector3 upVector = BetterBetterCharacterController.Instance.GetUpVector();

            Quaternion targetRotation = Quaternion.LookRotation(direction, upVector);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PortableCamera), "Start")]
        private static void Postfix_PortableCamera_Start(ref PortableCamera __instance)
        {
            VisualMods.CameraEnhancements mainMod = new();
            mainMod.Setup(__instance);
            __instance.RegisterMod(new LocalAttachment());
            MelonLogger.Msg("Registered LocalAttachment mod.");

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PortableCamera), nameof(PortableCamera.UpdateOptionsDisplay))]
        private static void Postfix_PortableCamera_UpdateOptionsDisplay(ref bool ____showExpertSettings)
        {
            VisualMods.CameraEnhancements.Instance?.OnUpdateOptionsDisplay(____showExpertSettings);
        }

        [HarmonyPatch(typeof(CameraAttachment), nameof(CameraAttachment.Enable))]
        public static class CameraAttachment_Enable_Patch
        {
            static void Postfix(CameraAttachment __instance)
            {
                var portableCameraField = AccessTools.Field(typeof(CameraAttachment), "_portableCamera");
                var portableCamera = portableCameraField.GetValue(__instance) as PortableCamera;

                if (portableCamera != null)
                {
                    portableCamera.DisableModByType(typeof(LocalAttachment));
                }
            }
        }
    }

}
