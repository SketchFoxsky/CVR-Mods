using MelonLoader;

[assembly: MelonInfo(typeof(HexRetacle.Core), "HexRetacle", "1.0.0", "leona", null)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]

namespace HexRetacle
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}