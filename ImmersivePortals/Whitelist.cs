using MelonLoader;

[assembly: MelonInfo(typeof(ImmersivePortals.Whitelist), "ImmersivePortals", "1.0.0", "leona", null)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace ImmersivePortals
{
    public class Whitelist : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}