using Sketch.YTdlpNightly.Properties;
using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(
    typeof(Sketch.YTdlpNightly.Main),
    nameof(Sketch.YTdlpNightly),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/SketchFoxsky/CVR-Mods/tree/main/SourceMovement"
)]

[assembly: MelonGame("ChilloutVR", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(255, 173, 100, 255)]
[assembly: MelonAuthorColor(255, 40, 144, 209)] 
[assembly: HarmonyDontPatchAll]

namespace Sketch.YTdlpNightly.Properties
{
    internal static class AssemblyInfoParams
    {
        public const string Version = "1.0.0";
        public const string Author = "SketchFoxsky";
    }
}