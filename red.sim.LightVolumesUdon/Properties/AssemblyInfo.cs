using red.sim.LightVolumesUdon.Properties;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(
    typeof(red.sim.LightVolumesUdon.Main),
    nameof(red.sim.LightVolumesUdon),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "N/A"
)]

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(255, 3, 252, 78)]
[assembly: MelonAuthorColor(255, 40, 144, 209)] 
[assembly: HarmonyDontPatchAll]

namespace red.sim.LightVolumesUdon.Properties
{
    internal static class AssemblyInfoParams
    {
        public const string Version = "1.0.0";
        public const string Author = "REDSIM , SketchFoxsky";
    }
}