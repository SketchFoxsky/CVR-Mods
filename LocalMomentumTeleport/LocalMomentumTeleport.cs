using ABI_RC.Systems.Movement;
using UnityEngine;
using System.Collections;
using ECM2;
using MelonLoader;

namespace Sketch.LocalMomentumTeleport
{
    public class LocalMomentumTeleport : MonoBehaviour
    {
        public GameObject EntryPortal;
        public GameObject ExitPortal;
        public float MomentumFling = 1.1f;
        private Collider LocalPlayer;

        private static bool isOnCooldown = false;

        public void Start()
        {
            GameObject localPlayerGO = GameObject.Find("_PLAYERLOCAL");
            if (localPlayerGO != null && localPlayerGO.CompareTag("LocalPlayer"))
            {
                LocalPlayer = localPlayerGO.GetComponent<Collider>();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other != LocalPlayer || isOnCooldown || EntryPortal == null || ExitPortal == null)
                return;

            StartCoroutine(TeleportPlayer());
        }

        private IEnumerator TeleportPlayer()
        {
            isOnCooldown = true;
            var bbcc = BetterBetterCharacterController.Instance;

            if (bbcc == null || EntryPortal == null || ExitPortal == null)
            {
                MelonLogger.Msg("[LocalMomentumTeleport] Missing references!");
                yield break;
            }

            // Debug portal forward vectors
            MelonLogger.Msg($"EntryPortal forward: {EntryPortal.transform.forward}");
            MelonLogger.Msg($"ExitPortal forward: {ExitPortal.transform.forward}");

            // Calculate speed and force-align velocity to exit forward
            Vector3 originalVel = bbcc.CharacterMovement.velocity;
            float speed = originalVel.magnitude;
            Vector3 redirectedVelocity = ExitPortal.transform.forward * speed * MomentumFling;

            MelonLogger.Msg($"Velocity before: {originalVel}");
            MelonLogger.Msg($"Redirected (aligned) velocity: {redirectedVelocity}");

            // Calculate rotation difference for player orientation
            Quaternion rotationDifference = ExitPortal.transform.rotation * Quaternion.Inverse(EntryPortal.transform.rotation);

            // Teleport player to exit
            bbcc.TeleportPlayerTo(ExitPortal.transform.position, false, true, true, rotationDifference);

            // Apply redirected velocity next frame to override internal resets
            yield return new WaitForEndOfFrame();
            bbcc.SetVelocity(redirectedVelocity);

            yield return new WaitForSeconds(0.3f); // cooldown
            isOnCooldown = false;
        }
    }
}
