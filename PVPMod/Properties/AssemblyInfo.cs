using MelonLoader;
using PVPMod.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

[assembly: MelonInfo(
    typeof(Sketch.PVPMod.Main),
    nameof(Sketch.PVPMod),
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/SketchFoxsky/CVR-Mods/tree/main/SourceMovement"
)]

[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: MelonColor(255, 43, 173, 100)]
[assembly: MelonAuthorColor(255, 40, 144, 209)]
[assembly: HarmonyDontPatchAll]

namespace PVPMod.Properties
{
    internal class AssemblyInfoParams
    {
        public const string Version = "1.0.0";
        public const string Author = "SketchFoxsky";
    }
}
