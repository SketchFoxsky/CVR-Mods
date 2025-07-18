using MelonLoader;
using UnityEngine;
using ABI_RC;
using ABI_RC.Systems.GameEventSystem;
using ABI.CCK.Components;
using ABI_RC.API;
using ABI_RC.Core.Player;

namespace Sketch.PortableCameraEnchancements
{
    public class PortableCameraEnchancements : MelonMod
    {
        #region MelonLoader Preferences
        private const string SettingsCategory = nameof(PortableCameraEnchancements);

        private static readonly MelonPreferences_Category Category = MelonPreferences.CreateCategory(SettingsCategory);

        internal static MelonPreferences_Entry<bool> LogAviChanges =
            Category.CreateEntry("Log_Avatar_Changes", true, "Log_Avatar_Changes", description: "Print Avatar changes to the Log");

        internal static MelonPreferences_Entry<bool> UseNewLookTarget =
            Category.CreateEntry("Use_Tracking_Target", true, "Use New Tracking Target", description: "Toggle the new tracking behavior.");

        internal static MelonPreferences_Entry<bool> AutoBlendTargets =
            Category.CreateEntry("Enable_Auto_Blend", true, "Auto Blend Tracking Targets", description: "Blends the tracking target based on how close you are to the camera");

        internal static MelonPreferences_Entry<float> BlendMinDistance =
            Category.CreateEntry("BlendMinDistance", 1f, "Blend Min Distance", description: "No blending below this distance (uses default target)");

        internal static MelonPreferences_Entry<float> BlendMaxDistance =
            Category.CreateEntry("BlendMaxDistance", 3f, "Blend Max Distance", description: "Full blend to modded target at this distance");

        #endregion

        public static Vector3 ModdedTarget;
        public static Vector3 BlendedTarget;

        private Animator AvatarAnimator;
        private bool UseDefaultTarget;

        public static Transform Head;
        public static Transform Hips;
        public static Transform Chest;
        public static Transform FootL;
        public static Transform FootR;

        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll();
            LoggerInstance.Msg("Initialized.");

            CVRGameEventSystem.Avatar.OnLocalAvatarClear.AddListener(_ =>
            {
                if (LogAviChanges.Value)
                {
                    LoggerInstance.Msg("Local Avatar has been cleared");
                }
                
                ClearLocalAvatar();
            });

            CVRGameEventSystem.Avatar.OnLocalAvatarLoad.AddListener(_ =>
            {
                if (LogAviChanges.Value)
                {
                    LoggerInstance.Msg("Local Avatar has been loaded");
                }
                
                LoadLocalAvatar();
            });
        }

        private void ClearLocalAvatar()
        {
            UseDefaultTarget = true;
            Head = Hips = Chest = FootL = FootR = null;
        }

        private void LoadLocalAvatar()
        {
            AvatarAnimator = PlayerSetup.Instance.Animator;
            if (!AvatarAnimator.isHuman)
            {
                UseDefaultTarget = true;
                return;
            }

            UseDefaultTarget = false;

            Head = AvatarAnimator.GetBoneTransform(HumanBodyBones.Head);
            Hips = AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
            Chest = AvatarAnimator.GetBoneTransform(HumanBodyBones.Chest)
                    ?? AvatarAnimator.GetBoneTransform(HumanBodyBones.Spine);
            FootL = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
            FootR = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
        }

        public override void OnUpdate()
        {
            if (Head == null || Chest == null || Hips == null || FootL == null || FootR == null)
                return;
            Vector3 feet =(FootL.position + FootR.position) / 2f;
            ModdedTarget = (Head.position + Chest.position + Hips.position + feet) / 4f;
        }
    }
}