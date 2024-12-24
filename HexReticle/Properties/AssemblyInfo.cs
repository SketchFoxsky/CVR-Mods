using Sketch.HexReticle.Properties;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(
    typeof(Sketch.HexReticle.HexReticle),
    nameof(Sketch.HexReticle),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/SketchFoxsky/CVR-Mods/tree/main/HexReticle"
)]

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(255, 227, 73, 25)]
[assembly: MelonAuthorColor(255, 40, 144, 209)] 
[assembly: HarmonyDontPatchAll]

namespace Sketch.HexReticle.Properties
{
    internal static class AssemblyInfoParams
    {
        public const string Version = "1.0.1";
        public const string Author = "SketchFoxsky";
    }
}