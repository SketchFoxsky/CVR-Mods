using System;
using System.Reflection;
using MelonLoader;
using ABI_RC.Systems.Communications;
using Steamworks;
using UnityEngine; // For Input handling

[assembly: MelonInfo(typeof(VoiceReconnectionButton.Core), "VoiceReconnectionButton", "1.0.0", "leona", null)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace VoiceReconnectionButton
{
    public class Core : MelonMod
    {
        private object commsClient;
        private MethodInfo disconnectMethod;
        private MethodInfo connectMethod;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");

            // Get the assembly containing Networking
            var commsAssembly = typeof(Networking).Assembly;

            // Get the Networking type
            var networkingType = commsAssembly.GetType("ABI_RC.Systems.Communications.Networking");
            if (networkingType == null)
            {
                LoggerInstance.Error("Failed to find Networking type.");
                return;
            }

            // Get the internal static Comms_Client field or property
            var clientProperty = networkingType.GetProperty("Comms_Client", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            commsClient = clientProperty?.GetValue(null);

            if (commsClient == null)
            {
                var clientField = networkingType.GetField("Comms_Client", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                commsClient = clientField?.GetValue(null);
            }

            if (commsClient == null)
            {
                LoggerInstance.Error("Failed to retrieve Comms_Client.");
                return;
            }

            // Get Disconnect and Connect methods
            var commsClientType = commsClient.GetType();
            disconnectMethod = commsClientType.GetMethod("Disconnect", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            connectMethod = commsClientType.GetMethod("Connect", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (disconnectMethod == null || connectMethod == null)
                LoggerInstance.Warning("Failed to find Disconnect or Connect method.");
        }

        public override void OnUpdate()
        {
            // When F5 is pressed
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (commsClient != null && disconnectMethod != null && connectMethod != null)
                {
                    LoggerInstance.Msg("F5 pressed: Reconnecting voice...");
                    disconnectMethod.Invoke(commsClient, null);
                    connectMethod.Invoke(commsClient, null);
                }
                else
                {
                    LoggerInstance.Warning("Comms_Client not ready or methods missing.");
                }
            }
        }
    }
}
