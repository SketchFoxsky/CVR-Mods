using ABI_RC.Core.Util.AssetFiltering;
using ABI_RC.Systems.Movement;
using ECM2;
using UnityEngine;
using HarmonyLib;
using MelonLoader;

namespace Sketch.LocalMomentumTeleport
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            SharedFilter.SpawnableWhitelist.Add(typeof(LocalMomentumTeleport));
            SharedFilter.LocalComponentWhitelist.Add(typeof(LocalMomentumTeleport));
            SharedFilter.AllowedEventComponents.Add(typeof(LocalMomentumTeleport));
            WorldFilter._Base.Add(typeof(LocalMomentumTeleport));
            MelonLogger.Msg("Initialized, now whitelisting modded component!");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                var bbcc = BetterBetterCharacterController.Instance;
                Vector3 currentVel = bbcc.characterMovement.velocity;

                // Rotate and scale velocity to get a stronger push
                Vector3 testVel = Quaternion.Euler(0, 90, 0) * currentVel * 3f;

                // Set velocity directly for immediate effect
                bbcc.SetVelocity(testVel);

                MelonLogger.Msg($"[Test] Velocity forcibly rotated and scaled: {testVel}");
            }
        }

    }
}
