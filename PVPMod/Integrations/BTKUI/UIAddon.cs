using BTKUILib.UIObjects;
using BTKUILib;
using MelonLoader;
using System.Reflection;
using Sketch.PVPMod;


namespace PVPMod.Integrations
{
    public static partial class BTKUIAddon
    {
        private static Page _rootPage;
        private static string _rootPageElementID;

        public static void Initialize()
        {
            SetupIcons();
            SetupPVP_ModTab();
        }

        private static void SetupIcons()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = assembly.GetName().Name;

            QuickMenuAPI.PrepareIcon("PVP Mod", "Swords", GetIconStream("Swords.png"));

            Stream GetIconStream(string iconName) => assembly.GetManifestResourceStream($"{assemblyName}.Resources.{iconName}");
            MelonLogger.Msg("BTKUI Intialization completed.");
            return;
        }

        private static void SetupPVP_ModTab()
        {
            _rootPage = new Page("PVP Mod", "PVP Mod Settings", true, "Swords")
            {
                MenuTitle = "PVP Mod",
                MenuSubtitle = "Fight with those around you, Of course if they want to!"
            };

            _rootPageElementID = _rootPage.ElementID;

            var category = _rootPage.AddCategory("PVP Mod");
            var Warningcategory = _rootPage.AddCategory("WARNING");
            var PVPtoggle = category.AddToggle("Enable PVP", "Click to toggle PVP", (Sketch.PVPMod.Main.EnablePVP.Value));
            var Warning = Warningcategory.AddTextBlock
                ("Due to a game bug, when disabling PVP you can still take damage. If you go down; you'll need to respawn.");
            var Warning2 = Warningcategory.AddTextBlock
                ("To re-enable PVP after going down, please rejoin the world.");
            PVPtoggle.OnValueUpdated += PVP =>
            {
                if (PVP == true)
                {
                    Sketch.PVPMod.Main.EnablePVP.Value = true;
                }
                else
                {
                    Sketch.PVPMod.Main.EnablePVP.Value = false;
                }
            };
        }
    }
}
