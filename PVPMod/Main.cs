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
        public GameObject PVPModHud;
        public GameObject ModdedCombatSystem;
        public CombatSystem _moddedcombat;
        private Vector3 PlayerVRCam;
        private Vector3 PlayerDesktopCam;
        private GameObject ThirdPersonCam;
        private Vector3 ThirdPersonCamPos;
        private bool godmode;

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
            propWhitelist.Add(typeof(GunController));
#endregion PropWhitelisting

            CVRGameEventSystem.Instance.OnConnected.AddListener(_ => OnConnected());
            BTKUIAddon.Initialize();
        }

        // BBCC Doesnt update on world load, so we need to do flight checks at OnConnected, besides props cant spawn in offline instances anyways.
        public void OnConnected()
        {
            if (BetterBetterCharacterController.Instance == null)
                return;

            var _WorldCombat = UnityEngine.Object.FindObjectOfType(typeof(CombatSystem));

            if ((_WorldCombat == null) && (BetterBetterCharacterController.Instance.FlightAllowedInWorld == true))
            {
                GameObject.Instantiate(_pvpprefab);
                LoggerInstance.Msg("PVP Prefab has been spawned in.");
                PVPModHud = GameObject.Find("PVP Mod Prefab(Clone)/PVPMODHitIndicator");
                ThirdPersonCam = GameObject.Find("_PLAYERLOCAL/[CameraRigDesktop]/Camera/ThirdPersonCameraObj");
                ModdedCombatSystem = GameObject.Find("PVP Mod Prefab(Clone)");
                _moddedcombat = ModdedCombatSystem.GetComponent<CombatSystem>();
            }
            else
            {
                if (_WorldCombat != null)
                {
                    LoggerInstance.Msg("World already has combat system, not spawning PVP prefab.");
                }
                if (BetterBetterCharacterController.Instance.FlightAllowedInWorld == false)
                {
                    LoggerInstance.Msg("World has flight restrictions, not spawning PVP prefab.");
                }
                // Just to make sure when moving to a new instance the objects are nulled when PVPMod is not allowed.
                ModdedCombatSystem = GameObject.Find("PVP Mod Prefab(Clone)");
                _moddedcombat = ModdedCombatSystem.GetComponent<CombatSystem>();
                PVPModHud = GameObject.Find("PVP Mod Prefab(Clone)/PVPMODHitIndicator");
            }
        } 

        public override void OnUpdate()
        {
            if (ModdedCombatSystem != null)
            {
                if (!EnablePVP.Value)
                {
                    // due to a game bug I cannot disable damage components, however we can keep the player health at 100
                    // Theyll only be killed by taking 10000+ in a single frame.
                    _moddedcombat.currentHealth = 10000;
                    godmode = true;
                }
                else if (EnablePVP.Value && godmode)
                {
                    // set the players health back to 100
                    _moddedcombat.currentHealth = 100;
                    godmode = false;
                }
                PVPModHud.SetActive(EnablePVP.Value);
            }

            if (PVPModHud != null)
            {
                //Keep the PVP mod Hud on the player

                // VR
                if (MetaPort.Instance.isUsingVr == true)
                {
                    PlayerVRCam = PlayerSetup.Instance.vrCameraRig.transform.position;
                    PVPModHud.transform.position = PlayerVRCam;
                }
                // Third Person Mod support
                else if ((!MetaPort.Instance.isUsingVr) && (ThirdPersonCam != null) && (ThirdPersonCam.activeSelf))
                {
                    ThirdPersonCamPos = ThirdPersonCam.transform.position;
                    PVPModHud.transform.position = ThirdPersonCamPos;
                }
                // Desktop
                else if ((!MetaPort.Instance.isUsingVr) && ((ThirdPersonCam == null) || (!ThirdPersonCam.activeSelf)))
                {
                    PlayerDesktopCam = PlayerSetup.Instance.desktopCameraRig.transform.position;
                    PVPModHud.transform.position = PlayerDesktopCam;
                }
            }
        }

    }
}