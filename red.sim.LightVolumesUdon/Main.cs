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
            //Prop Whitelist (Not sure if itll work or be worth it but having interior lighting would be nice for ships)
            SharedFilter.SpawnableWhitelist.Add(typeof(PointLightVolumeInstance));
            SharedFilter.SpawnableWhitelist.Add(typeof(LightVolumeInstance));
            SharedFilter.SpawnableWhitelist.Add(typeof(LightVolumeManager));
            MelonLogger.Msg("Initialized, now whitelisting modded components!");
        }
    }
}