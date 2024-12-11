using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.UI;
using ABI_RC.Systems.GameEventSystem;

namespace Sketch.HexReticle
{
    public class HexReticle : MelonMod
    {
        // Throwing Reticle Info
        public Image ThrowReticle;
        public float throwfill;
        private float ThrowStart = 0.1f;
        public float _Time = 0f;
        public float elapsedTime;

        //asset bundle info
        private static Sprite _hexagonempty;
        private static Sprite _hexagonfilled;
        private static Material _hexagonMaterial;
        private static string HexReticleBundleName = "reticlehexagon.assetbundle";
        private static string EmptyHexagonSpritePath = "Assets/Scenes/Hex.png";
        private static string FilledHexagonSpritePath = "Assets/Scenes/HexHighlight.png";
        private static string HexagonMaterialPath = "Assets/Scenes/Reticle.mat";

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");

            // load reticle assets
            #region AssetBundle
            try
            {

                LoggerInstance.Msg($"Loading the asset bundle...");
                using Stream resourceStream = MelonAssembly.Assembly.GetManifestResourceStream(HexReticleBundleName);
                using MemoryStream memoryStream = new MemoryStream();
                if (resourceStream == null)
                {
                    MelonLogger.Error($"Failed to load {HexReticleBundleName}!");
                    return;
                }

                resourceStream.CopyTo(memoryStream);
                AssetBundle assetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
                if (assetBundle == null)
                {
                    LoggerInstance.Error($"Failed to load {HexReticleBundleName}! Asset bundle is null!");
                    return;
                }

                // Load empty hex
                Sprite HexagonEmpty = assetBundle.LoadAsset<Sprite>(EmptyHexagonSpritePath);
                HexagonEmpty.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                if ( HexagonEmpty == null )
                {
                    LoggerInstance.Error("$Failed to load EmptyHexagon Sprite!");
                }
                else
                {
                    LoggerInstance.Msg("Loaded EmptyHexagon Sprite!");
                    _hexagonempty = HexagonEmpty;
                }

                // Load filled hex
                Sprite HexagonFilled = assetBundle.LoadAsset<Sprite>(FilledHexagonSpritePath);
                HexagonFilled.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                if (HexagonFilled == null)
                {
                    LoggerInstance.Error("$Failed to load FilledHexagon Sprite!");
                }
                else
                {
                    LoggerInstance.Msg("Loaded FilledHexagon Sprite!");
                    _hexagonfilled = HexagonFilled;
                }

                //load material
                Material HexagonMaterial = assetBundle.LoadAsset<Material>(HexagonMaterialPath);
                HexagonMaterial.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                if (HexagonMaterial == null)
                {
                    LoggerInstance.Error("$Failed to load Hexagon Material!");
                }
                else
                {
                    LoggerInstance.Msg("Loaded Hexagon Material!");
                    _hexagonMaterial = HexagonMaterial;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to Load the asset bundle: " + ex.Message);
                return;
            }
            #endregion AssetBundle
            ApplyPatches(typeof(ControllerRay_Patches));
            CVRGameEventSystem.Initialization.OnPlayerSetupStart.AddListener(OnPlayerSetup);
        }

        private void ApplyPatches(Type type)
        {
            try
            {
                HarmonyInstance.PatchAll(type);
            }
            catch (Exception e)
            {
                LoggerInstance.Msg($"Failed while patching {type.Name}!");
                LoggerInstance.Error(e);
            }
        }

        private void OnPlayerSetup()
        {
            GameObject Crosshair;
            Crosshair = GameObject.Find("_PLAYERLOCAL/[CameraRigDesktop]/Camera/Canvas/Image");
            if (Crosshair == null)
            {
                LoggerInstance.Msg($"The reticle is null");
            }
            else
            {
                var CrosshairPos = Crosshair.transform;
                GameObject.Instantiate(Crosshair, CrosshairPos);
                var ThrowMeter = GameObject.Find("_PLAYERLOCAL/[CameraRigDesktop]/Camera/Canvas/Image/Image(Clone)");
                ThrowReticle = ThrowMeter.GetComponent<Image>();
                var throwcolor = ThrowReticle.GetComponent<Graphic>();

                ThrowReticle.sprite = _hexagonempty;
                ThrowReticle.type = Image.Type.Filled;
                throwcolor.color = new Color32(255, 255, 255, 45);
                ThrowReticle.material = _hexagonMaterial;
                ThrowMeter.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                ThrowReticle.fillAmount = 0f;
            }
        }

        #region Patches

        // Thank you NotAKid for letting me use Smart Reticles patches!

        private static class ControllerRay_Patches
        {
            private static Transform _mainMenuTransform;
            private static Transform _quickMenuTransform;
            private static float _lastDisplayedTime;
            private static Image DesktopReticle;
            private static Graphic ReticleColor;

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ControllerRay), nameof(ControllerRay.Start))]
            private static void Postfix_ControllerRay_Start()
            {
                _mainMenuTransform = ViewManager.Instance.transform;
                _quickMenuTransform = CVR_MenuManager.Instance.transform;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ControllerRay), nameof(ControllerRay.DisplayAuraHighlight))]
            private static void Postfix_ControllerRay_DisplayAuraHighlight(ref ControllerRay __instance)
            {
                GameObject pointer;
                if (__instance.isDesktopRay) // in desktop mode
                    pointer = CohtmlHud.Instance.desktopPointer;
                else if (__instance.isHeadRay) // in VR mode with no controllers
                    pointer = __instance.backupCrossHair;
                else
                    return;

                DesktopReticle = pointer.GetComponent<Image>();

                if (DesktopReticle == null)
                {
                    MelonLogger.Msg("Fail to get Pointer Image");
                }
                else
                {
                    ReticleColor = DesktopReticle.GetComponent<Graphic>();
                    ReticleColor.color = new Color32(255, 255, 255, 45);
                    DesktopReticle.material = _hexagonMaterial;
                    pointer.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                }

                if (!pointer.activeSelf)
                {
                    _lastDisplayedTime = 0; // reset time
                    return; // pointing at menu or cursor / controllers active
                }

                bool spritetype = (__instance._interact  // pressing mouse1 or mouse2
                                               || __instance._isTryingToPickup
                                               // using some tool/utility
                                               || (PlayerSetup.Instance.GetCurrentPropSelectionMode()
                                                   != PlayerSetup.PropSelectionMode.None)
                                               // hit something- other than the two menus
                                               || (__instance._objectWasHit
                                                   && (__instance.hitTransform != _mainMenuTransform
                                                       && __instance.hitTransform != _quickMenuTransform)));

                if (spritetype)
                {
                    _lastDisplayedTime = Time.time;
                    DesktopReticle.sprite = _hexagonfilled;
                    return;
                }

                if (Time.time - _lastDisplayedTime > 0f)
                    DesktopReticle.sprite = _hexagonempty;
            }
        }

        #endregion Patches

        public override void OnUpdate()
        {
            if (ThrowReticle != null)
            {
                if (Input.GetKey(KeyCode.G))
                {
                    _Time = _Time + Time.deltaTime;
                    if (_Time > ThrowStart)
                    {
                        elapsedTime += Time.deltaTime;
                        float percentage = (elapsedTime / 1.9f);
                        throwfill = Mathf.Lerp(ThrowStart, 1f, percentage);
                        ThrowReticle.fillAmount = throwfill;
                    }
                }
                else
                {
                    _Time = 0f;
                    elapsedTime = 0f;
                    throwfill = 0f;
                    ThrowReticle.fillAmount = throwfill;
                }
            }

        }
    }
}