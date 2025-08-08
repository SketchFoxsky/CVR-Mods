using ABI_RC.API;
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
            WorldFilter._Base.Add(typeof(LightVolume));
            WorldFilter._Base.Add(typeof(LightVolumeData));
            WorldFilter._Base.Add(typeof(LightVolumeDataSorter));
            WorldFilter._Base.Add(typeof(LightVolumeInstance));
            WorldFilter._Base.Add(typeof(LightVolumeManager));
            WorldFilter._Base.Add(typeof(LightVolumeSetup));
            WorldFilter._Base.Add(typeof(PointLightVolume));
            WorldFilter._Base.Add(typeof(PointLightVolumeInstance));
            //Prop Whitelist
            SharedFilter.SpawnableWhitelist.Add(typeof(PointLightVolumeInstance));
            SharedFilter.SpawnableWhitelist.Add(typeof(PointLightVolume));
            MelonLogger.Msg("Initialized, now whitelisting modded components!");
        }
    }
}