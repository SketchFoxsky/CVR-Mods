using MelonLoader;
using Sketch.SourceMovement.Properties;
using System.Reflection;

[assembly: AssemblyVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyFileVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyInformationalVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyTitle(nameof(Sketch.SourceMovement))]
[assembly: AssemblyCompany(AssemblyInfoParams.Author)]
[assembly: AssemblyProduct(nameof(Sketch.SourceMovement))]

[assembly: MelonInfo(
    typeof(Sketch.SourceMovement.SourceMovement),
    nameof(Sketch.SourceMovement),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "N/A"
)]

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(21, 140, 95, 255)] // Green-Blue
[assembly: MelonAuthorColor(23, 148, 194, 255)] // Blue
[assembly: HarmonyDontPatchAll]

namespace Sketch.SourceMovement.Properties;
internal static class AssemblyInfoParams
{
    public const string Version = "1.0.0";
    public const string Author = "SketchFoxsky";
}