using ABI.CCK.Components;
using ABI_RC.API;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Util.AssetFiltering;
using MelonLoader;
using VRCLightVolumes;

namespace red.sim.LightVolumesUdon

{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            //World Whitelist
            WorldFilter._VolumetricFogAndMist.Add(typeof(LightVolumeInstance));
            WorldFilter._VolumetricFogAndMist.Add(typeof(LightVolumeManager));
            WorldFilter._VolumetricFogAndMist.Add(typeof(PointLightVolumeInstance));

            MelonLogger.Msg("Initialized, now whitelisting modded components!");
        }
    }
}