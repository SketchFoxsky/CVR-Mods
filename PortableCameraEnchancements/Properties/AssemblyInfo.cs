using Sketch.PortableCameraEnchancements.Properties;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(
    typeof(Sketch.PortableCameraEnchancements.PortableCameraEnchancements),
    nameof(Sketch.PortableCameraEnchancements),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/SketchFoxsky/CVR-Mods/tree/main/SourceMovement"
)]

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(255, 255, 196, 0)]
[assembly: MelonAuthorColor(255, 40, 144, 209)] 
[assembly: HarmonyDontPatchAll]

namespace Sketch.PortableCameraEnchancements.Properties
{
    internal static class AssemblyInfoParams
    {
        public const string Version = "1.0.0";
        public const string Author = "SketchFoxsky";
    }
}