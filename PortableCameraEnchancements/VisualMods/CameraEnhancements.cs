using ABI_RC.Systems.Camera;
using ABI_RC.Systems.Camera.VisualMods;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sketch.PortableCameraEnchancements.VisualMods
{
    public class CameraEnhancements
    {
        public static CameraEnhancements Instance;

        private bool UseNewLookTarget;
        private bool UseBlending;
        private PortableCameraSetting setting_MinDistance;
        private PortableCameraSetting setting_MaxDistance;

        public void Setup(PortableCamera __instance)
        {
            Instance = this;
            __instance.@interface.AddAndGetHeader(null, typeof(CameraEnhancements), "Player Tracking Settings");

            //Settings

            //UseNewLookTarget
            PortableCameraSetting setting_UseNewLookTarget = __instance.@interface.AddAndGetSetting(PortableCameraSettingType.Bool);
            setting_UseNewLookTarget.BoolChanged = new Action<bool>(value => UpdateCameraSettingBool("UseNewLookTarget", value));
            setting_UseNewLookTarget.SettingName = "UseNewLookTarget";
            setting_UseNewLookTarget.DisplayName = "Use New Look Target";
            setting_UseNewLookTarget.OriginType = typeof(CameraEnhancements);
            setting_UseNewLookTarget.DefaultValue = true;
            setting_UseNewLookTarget.Load();

            //UseBlending
            PortableCameraSetting setting_UseBlending = __instance.@interface.AddAndGetSetting(PortableCameraSettingType.Bool);
            setting_UseBlending.BoolChanged = new Action<bool>(value => UpdateCameraSettingBool("UseBlending", value));
            setting_UseBlending.SettingName = "UseBlending";
            setting_UseBlending.DisplayName = "Blend Between Targets";
            setting_UseBlending.OriginType = typeof(CameraEnhancements);
            setting_UseBlending.DefaultValue = true;
            setting_UseBlending.Load();

            //MinDistance
            setting_MinDistance = __instance.@interface.AddAndGetSetting(PortableCameraSettingType.Float);
            setting_MinDistance.FloatChanged = new Action<float>(value => UpdateCameraSettingFloat("MinDistance", value));
            setting_MinDistance.SettingName = "MinDistance";
            setting_MinDistance.DisplayName = "Min Distance";
            setting_MinDistance.isExpertSetting = true;
            setting_MinDistance.OriginType = typeof(CameraEnhancements);
            setting_MinDistance.DefaultValue = 1f;
            setting_MinDistance.MinValue = 0.01f;
            setting_MinDistance.MaxValue = 2.99f;
            setting_MinDistance.Load();

            //MaxDistance
            setting_MaxDistance = __instance.@interface.AddAndGetSetting(PortableCameraSettingType.Float);
            setting_MaxDistance.FloatChanged = new Action<float>(value => UpdateCameraSettingFloat("MaxDistance", value));
            setting_MaxDistance.SettingName = "MaxDistance";
            setting_MaxDistance.DisplayName = "Max Distance";
            setting_MaxDistance.isExpertSetting = true;
            setting_MaxDistance.OriginType = typeof(CameraEnhancements);
            setting_MaxDistance.DefaultValue = 3.01f;
            setting_MaxDistance.MinValue = 3f;
            setting_MaxDistance.MaxValue = 10f;
            setting_MaxDistance.Load();
        }

        public void OnUpdateOptionsDisplay(bool expertMode = true)
        {
            if (!expertMode)
                return;

            setting_MinDistance.settingsObject.SetActive(true);
            setting_MaxDistance.settingsObject.SetActive(true);
        }

        private void UpdateCameraSettingBool(string setting, bool value)
        {
                switch (setting)
                {
                    //Tracking Settings
                    case "UseNewLookTarget":
                        PortableCameraEnchancements.UseNewLookTarget = value;
                        break;
                    //Internal Settings
                    case "UseBlending":
                            PortableCameraEnchancements.AutoBlendTargets = value;
                        break;
                }
        }

        private void UpdateCameraSettingFloat(string setting, float value)
        {
                switch (setting)
                {
                    //Camera Settings
                    case "MinDistance":
                        PortableCameraEnchancements.BlendMinDistance = value;
                        break;
                    case "MaxDistance":
                        PortableCameraEnchancements.BlendMaxDistance = value;
                        break;
                }

        }

    }
}
