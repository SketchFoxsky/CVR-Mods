using MelonLoader;
using UnityEngine;
using ABI.CCK.Components;
using ABI_RC.Systems.GameEventSystem;
using ABI_RC.Systems.Movement;
using ABI_RC.Core.Util.AssetFiltering;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Player;
using PVPMod.Integrations;


namespace Sketch.PVPMod
{
    public class Main : MelonMod
    {
        private static string PVPBundleName = "pvpmod.assetbundle";
        private static string PVPPrefab = "Assets/PVPMOD/PVP Mod Prefab.prefab";
        private GameObject _pvpprefab;
        private UnityEngine.Object _WorldCombat;
        public GameObject PVPModHud;
        private Vector3 PlayerVRCam;
        private Vector3 PlayerDesktopCam;
        private GameObject ThirdPersonCam;
        private Vector3 ThirdPersonCamPos;

#region MelonPrefs

        private const string SettingsCategory = nameof(PVPMod);

        private static readonly MelonPreferences_Category Category = MelonPreferences.CreateCategory(SettingsCategory);

        internal static MelonPreferences_Entry<bool> EnablePVP =
        Category.CreateEntry("EnablePVP", true, "Enable PVP", description: "Enables the PVP mod.");

#endregion MelonPrefs

        public override void OnInitializeMelon()
        {
#region AssetBundleLoad
            LoggerInstance.Msg("Loading PVP prefab.");
            try
            {

                LoggerInstance.Msg($"Loading the asset bundle...");
                using Stream resourceStream = MelonAssembly.Assembly.GetManifestResourceStream(PVPBundleName);
                using MemoryStream memoryStream = new MemoryStream();
                if (resourceStream == null)
                {
                    MelonLogger.Error($"Failed to load {PVPBundleName}!");
                    return;
                }

                resourceStream.CopyTo(memoryStream);
                AssetBundle assetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
                if (assetBundle == null)
                {
                    LoggerInstance.Error($"Failed to load {PVPBundleName}! Asset bundle is null!");
                    return;
                }

                // Load the Prefab
                GameObject CombatPrefab = assetBundle.LoadAsset<GameObject>(PVPPrefab);
                CombatPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                if (CombatPrefab == null)
                {
                    LoggerInstance.Error("$Failed to load PVP Prefab!");
                }
                else
                {
                    LoggerInstance.Msg("Loaded PVP Prefab");
                    _pvpprefab = CombatPrefab;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to Load the asset bundle: " + ex.Message);
                return;
            }
            #endregion AssetBundleLoad

#region PropWhitelisting
            var propWhitelist = SharedFilter.SpawnableWhitelist;
            propWhitelist.Add(typeof(Damage));
#endregion PropWhitelisting

            CVRGameEventSystem.World.OnUnload.AddListener(_ => OnWorldUnload());
            CVRGameEventSystem.World.OnLoad.AddListener(_ => OnWorldload());
            CVRGameEventSystem.Initialization.OnPlayerSetupStart.AddListener(OnPlayerSetup);
            CVRWorld.GameRulesUpdated += OnApplyMovementSettings;
            BTKUIAddon.Initialize();
        }

        public void OnPlayerSetup()
        {
            ThirdPersonCam = GameObject.Find("[CameraRigDesktop]/Camera/ThirdPersonCameraObj");
        }
        public void OnWorldload()
        {
            _WorldCombat = UnityEngine.Object.FindObjectOfType(typeof(CombatSystem));
        }
        public void OnWorldUnload()
        {
            _WorldCombat = null;
        }

        //When Movement Settings are changed try to spawn the PVP prefab
        //This is because Movement is updated AFTER world loads, which means flight checks CANNOT be done during world load.
        public void OnApplyMovementSettings()
        {
            SpawnPVP();
        }

        // Due to the BBCC updating AFTER world load, we do flight check here when the PVP mod tries to spawn its prefab.
        // This will also check if it spawned itself incase the CVRWorld item is toggled on and off updating its movment settings.
        public void SpawnPVP()
        {
            var PVP = GameObject.Find("PVP Mod Prefab(Clone)");
            if ((_WorldCombat == null) && (BetterBetterCharacterController.Instance.FlightAllowedInWorld == true) && (EnablePVP.Value == true)
                && (PVP == null))
            {
                GameObject.Instantiate(_pvpprefab);
                LoggerInstance.Msg("PVP Prefab has been spawned in.");
                PVPModHud = GameObject.Find("PVP Mod Prefab(Clone)/PVPMODHitIndicator");
            }
            else
            {
                if (_WorldCombat != null)
                {
                    LoggerInstance.Msg("World already has combat, not spawning PVP prefab.");
                }
                if (BetterBetterCharacterController.Instance.FlightAllowedInWorld == false)
                {
                    LoggerInstance.Msg("World has flight restrictions, not spawning PVP prefab.");
                }
                if (EnablePVP.Value == false)
                {
                    LoggerInstance.Msg("PVP Mod is currently Disabled.");
                }
            }
        }       

        public override void OnUpdate()
        {
            if (GameObject.Find("PVP Mod Prefab(Clone)") != null)
            {
                var ModdedCombatSystem = (GameObject.Find("PVP Mod Prefab(Clone)"));
                ModdedCombatSystem.SetActive(EnablePVP.Value);
            }
            if (PVPModHud != null)
            {
                //Keep the PVP mod Hud on the player
                if (MetaPort.Instance.isUsingVr == true)
                {
                    PlayerVRCam = PlayerSetup.Instance.vrCameraRig.transform.position;
                    PVPModHud.transform.position = PlayerVRCam;
                }
                else if ((MetaPort.Instance.isUsingVr == false) && (ThirdPersonCam != null) && (ThirdPersonCam.activeSelf == true))
                {
                    ThirdPersonCamPos = ThirdPersonCam.transform.position;
                    PVPModHud.transform.position = ThirdPersonCamPos;
                }
                else
                {
                    PlayerDesktopCam = PlayerSetup.Instance.desktopCameraRig.transform.position;
                    PVPModHud.transform.position = PlayerDesktopCam;
                }
            }
        }

    }
}