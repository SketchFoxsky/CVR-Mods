using MelonLoader;
using UnityEngine;
using ABI_RC.Systems.Movement;
using ABI.CCK.Components;
using ABI_RC.Systems.GameEventSystem;
using SourceMovement.Integrations;


namespace Sketch.SourceMovement
{
    public class SourceMovement : MelonMod
    {
        #region Melon Loader Preferences

        private const string SettingsCategory = nameof(SourceMovement);

        private static readonly MelonPreferences_Category Category = MelonPreferences.CreateCategory(SettingsCategory);

        internal static MelonPreferences_Entry<bool> EntryUseSourceMovement =
            Category.CreateEntry("Use_source_movement", false, "Use Source Movement", description: "Toggle source movement.");
        internal static readonly MelonPreferences_Entry<bool> UseDoubleClickTab =
            Category.CreateEntry("Use_DoubleClick", false, "Use Double Click Tab", description: "Allows double clicking Crowbar Icon to toggle the  Use Source Movement' setting");

        #endregion Melon Loader Preferences

        #region Default Settings
        // Original values reference the BBCC value on world load.
        // Falling Speed
        private static float _currentMaxFallSpeed = 40f; //this is curently default.
        float _OriginalMaxFallSpeed = 40f;
        private static float _currentLateralFallingFriction = 2f;
        float _OriginalLateralFallingFriction = 0f;

        //Acceleration and Walking
        private static float _currentMaxAcceleration = 15f;
        float _OriginalMaxAcceleration =25f;
        private static float _currentWalkSpeed = 100f;
        float _OriginalWalkSpeed;

        //Friction
        //These original Values are not touched by game code and must be manually reapplied.
        private static float _currentGroundFriction = 0f;
        private static float _OriginalGroundFriction = 8f;
        private static float _currentbrakingDecelerationWalking = 0f;
        private static float _OriginalbrakingDecelerationWalking = 25f;

        bool _PassedWorldCheck = false;

        #endregion Default Settings


        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            //Add listeners to call world load and unload so we can apply world movement settings.

            //this can be spammed by world author toggling the gamobject with CVRWorld, needs a fix.
            CVRGameEventSystem.World.OnUnload.AddListener(_ => OnWorldUnload());
            CVRWorld.GameRulesUpdated += OnApplyMovementSettings;
            BTKUIAddon.Initialize();
        }

        // Get World Movement Settings, This will also work on runtime with animated CVR World Settings!
        private void OnApplyMovementSettings()
        {
            _OriginalMaxAcceleration = BetterBetterCharacterController.Instance.maxAcceleration;
            _OriginalWalkSpeed = BetterBetterCharacterController.Instance.maxWalkSpeed;
            LoggerInstance.Msg("Original World Movement Settings acquired");

            //Prevent OnUpdate() from overiding the original values the same frame they change.
            _PassedWorldCheck = true;
        }

        private void OnWorldUnload()
        {
            // Prevent loading old friction data from another world.
            BetterBetterCharacterController.Instance.groundFriction = _OriginalGroundFriction;
            BetterBetterCharacterController.Instance.brakingDecelerationWalking = _OriginalbrakingDecelerationWalking;
            LoggerInstance.Msg("Removed Old World movement.");
            _PassedWorldCheck = false;
        }

        public override void OnUpdate()
        {
            if (BetterBetterCharacterController.Instance == null)
                return;

            if (((!EntryUseSourceMovement.Value)
                || BetterBetterCharacterController.Instance.IsFlying()
                || BetterBetterCharacterController.Instance.FlightAllowedInWorld == false
                || BetterBetterCharacterController.Instance.fallingTime <= 0.01f)
                && (_PassedWorldCheck == true))
            {
                BetterBetterCharacterController.Instance.groundFriction = _OriginalGroundFriction;
                BetterBetterCharacterController.Instance.brakingDecelerationWalking = _OriginalbrakingDecelerationWalking;
                BetterBetterCharacterController.Instance.fallingLateralFriction = _OriginalLateralFallingFriction;
                BetterBetterCharacterController.Instance.maxFallSpeed = _OriginalMaxFallSpeed;
                BetterBetterCharacterController.Instance.maxWalkSpeed = _OriginalWalkSpeed;
            }
            //Apply Source Movement settings when jumping/falling
            if ((EntryUseSourceMovement.Value) && BetterBetterCharacterController.Instance.fallingTime > 0.25f
                && BetterBetterCharacterController.Instance.FlightAllowedInWorld == true)
            {
                BetterBetterCharacterController.Instance.fallingLateralFriction = _currentLateralFallingFriction;
                BetterBetterCharacterController.Instance.maxFallSpeed = _currentMaxFallSpeed;
                BetterBetterCharacterController.Instance.maxWalkSpeed = _currentWalkSpeed;
                BetterBetterCharacterController.Instance.maxAcceleration = _currentMaxAcceleration;
                BetterBetterCharacterController.Instance.groundFriction = _currentGroundFriction;
                BetterBetterCharacterController.Instance.brakingDecelerationWalking = _currentbrakingDecelerationWalking;
            }

            //Disable Ground constraint when the player is over a certain speed
            if ((BetterBetterCharacterController.Instance.speed >= 50f) && !BetterBetterCharacterController.Instance.IsGrounded()
                && EntryUseSourceMovement.Value)
            {
                BetterBetterCharacterController.Instance.EnableGroundConstraint(false);
            }
            else
            {
                BetterBetterCharacterController.Instance.EnableGroundConstraint(true);
            }
        }
    }
}