using System;
using System.Linq;
using System.Reflection;
using MelonLoader;
using Steamworks;
using UnityEngine;
using ABI_RC.Systems.GameEventSystem; // For CVRGameEventSystem

[assembly: MelonInfo(typeof(VoiceReconnectionButton.Core), "VoiceReconnectionButton", "1.0.0", "leona", null)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace VoiceReconnectionButton
{
    public class Core : MelonMod
    {
        private object commsClientInstance;
        private MethodInfo disconnectMethod;
        private MethodInfo connectMethod;
        private float nextAllowedReconnectTime = 0f; // Cooldown timer

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("VoiceReconnectionButton initialized.");

            CVRGameEventSystem.Instance.OnConnected.AddListener(_ =>
            {
                LoggerInstance.Msg("Game connected, delaying Comms_Client setup by 2 seconds...");
                MelonCoroutines.Start(DelayedSetup());
            });
        }

        private System.Collections.IEnumerator DelayedSetup()
        {
            yield return new WaitForSeconds(2f);
            SetupCommsClient();
        }

        private void SetupCommsClient()
        {
            LoggerInstance.Msg("SetupCommsClient started.");

            var commsManagerObj = GameObject.Find("Comms_Manager");
            LoggerInstance.Msg($"Comms_Manager GameObject found? {(commsManagerObj != null)}");
            if (commsManagerObj == null)
            {
                LoggerInstance.Error("Comms_Manager GameObject not found in scene!");
                return;
            }

            var commsAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");
            LoggerInstance.Msg($"Assembly-CSharp.dll found? {(commsAssembly != null)}");
            if (commsAssembly == null)
            {
                LoggerInstance.Error("Assembly-CSharp.dll not found!");
                return;
            }

            var commsClientType = commsAssembly.GetType("ABI_RC.Systems.Communications.Networking.Comms_Client");
            LoggerInstance.Msg($"Comms_Client type found? {(commsClientType != null)}");
            if (commsClientType == null)
            {
                LoggerInstance.Error("Failed to find ABI_RC.Systems.Communications.Networking.Comms_Client type.");
                return;
            }

            commsClientInstance = commsManagerObj.GetComponent(commsClientType);
            LoggerInstance.Msg($"Comms_Client component found on Comms_Manager? {(commsClientInstance != null)}");
            if (commsClientInstance == null)
            {
                LoggerInstance.Error("Comms_Client component not found on Comms_Manager GameObject!");
                return;
            }

            // Explicitly get parameterless methods
            disconnectMethod = commsClientType.GetMethod("Disconnect", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            connectMethod = commsClientType.GetMethod("Connect", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

            if (disconnectMethod == null || connectMethod == null)
            {
                LoggerInstance.Warning("Failed to find parameterless Disconnect or Connect methods.");
                return;
            }

            LoggerInstance.Msg("Successfully hooked parameterless Disconnect and Connect methods.");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (Time.time < nextAllowedReconnectTime)
                {
                    var remaining = Mathf.Ceil(nextAllowedReconnectTime - Time.time);
                    LoggerInstance.Warning($"Reconnect on cooldown. Please wait {remaining} seconds.");
                    return;
                }

                if (commsClientInstance != null && disconnectMethod != null && connectMethod != null)
                {
                    LoggerInstance.Msg("F5 pressed: Disconnecting and reconnecting voice...");
                    disconnectMethod.Invoke(commsClientInstance, null);
                    connectMethod.Invoke(commsClientInstance, null);

                    nextAllowedReconnectTime = Time.time + 60f; // 1-minute cooldown
                }
                else
                {
                    LoggerInstance.Warning("Comms_Client not ready or methods missing.");
                }
            }
        }
    }
}
