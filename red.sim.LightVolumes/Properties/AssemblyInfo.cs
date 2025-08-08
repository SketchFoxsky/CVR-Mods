using red.sim.LightVolumes.Properties;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(
    typeof(red.sim.LightVolumes.Main),
    nameof(red.sim.LightVolumes),
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

namespace red.sim.LightVolumes.Properties
{
    internal static class AssemblyInfoParams
    {
        public const string Version = "1.0.0";
        public const string Author = "REDSIM , SketchFoxsky";
    }
}