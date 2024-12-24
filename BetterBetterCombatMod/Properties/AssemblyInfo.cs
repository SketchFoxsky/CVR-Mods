using Sketch.BetterBetterCombatMod.Properties;
using MelonLoader;

[assembly: MelonInfo(
    typeof(Sketch.BetterBetterCombatMod.Main),
    nameof(Sketch.BetterBetterCombatMod),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/SketchFoxsky/CVR-Mods/tree/main/BetterBetterCombatMod"
)]

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(255, 228, 58, 0)]
[assembly: MelonAuthorColor(255, 40, 144, 209)] 
[assembly: HarmonyDontPatchAll]

namespace Sketch.BetterBetterCombatMod.Properties
{
    internal static class AssemblyInfoParams
    {
        public const string Version = "1.0.0";
        public const string Author = "SketchFoxsky";
    }
}