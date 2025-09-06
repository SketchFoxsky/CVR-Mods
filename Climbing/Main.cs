using MelonLoader;
using ABI_RC.Core.Util.AssetFiltering;
using ECM2.Examples.Ladders;
using ABI_RC.Systems.Movement;
using HarmonyLib;
using UnityEngine;
using ABI_RC.Systems.InputManagement;
using ABI_RC.Core.Player;
using System.Runtime.CompilerServices;

namespace Sketch.Climbing
{
    public class Main : MelonMod
    {
        // Configurable settings
        private const float DefaultFlingStrength = 6f;
        private const float MinUpwardBoost = 0.2f;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Climbing Mod Initialized.");
            WorldFilter._VolumetricFogAndMist.Add(typeof(Ladder));
            WorldFilter._VolumetricFogAndMist.Add(typeof(LadderClimbAbility));
            HarmonyInstance.PatchAll();
        }

        // Ensure the player has LadderClimbAbility
        [HarmonyPatch(typeof(BetterBetterCharacterController), nameof(BetterBetterCharacterController.OnEnable))]
        public static class BBC_LadderInit_Patch
        {
            static void Postfix(BetterBetterCharacterController __instance)
            {
                var ability = __instance.GetComponent<LadderClimbAbility>();
                if (!ability)
                    ability = __instance.gameObject.AddComponent<LadderClimbAbility>();

                if (ability.ladderMask.value == 0)
                {
                    int mask = LayerMask.GetMask("Ladder");
                    ability.ladderMask = (mask != 0) ? (LayerMask)mask : Physics.AllLayers;
                }
            }
        }

        // Handle desktop climbing input
        [HarmonyPatch(typeof(BetterBetterCharacterController), nameof(BetterBetterCharacterController.HandleInput))]
        public static class BBC_LadderInput_Patch
        {
            private class PressState { public bool held; }
            private static readonly ConditionalWeakTable<BetterBetterCharacterController, PressState> _states = new();

            static void Postfix(BetterBetterCharacterController __instance)
            {
                var ability = __instance.GetComponent<LadderClimbAbility>();
                if (!ability) return;

                // --- Desktop: Grab / Release with E ---
                bool heldNow = Input.GetKey(KeyCode.E);
                var state = _states.GetValue(__instance, _ => new PressState());
                bool pressed = !state.held && heldNow;
                bool released = state.held && !heldNow;
                state.held = heldNow;

                if (pressed) ability.Climb();
                if (released) ability.StopClimbing();

                // --- Jump to drop + fling ---
                if (ability.IsClimbing() && Input.GetButtonDown("Jump"))
                {
                    ability.StopClimbing();

                    Vector3 forward = __instance.cameraTransform.forward;
                    forward.y = Mathf.Max(forward.y, MinUpwardBoost);
                    forward.Normalize();

                    __instance.LaunchCharacter(forward * DefaultFlingStrength, overrideVerticalVelocity: true);
                }

                // --- Update Animator ---
                var animator = PlayerSetup.Instance?.Animator;
                if (animator)
                    animator.SetBool("Climbing", ability.IsClimbing());
            }
        }
    }
}
