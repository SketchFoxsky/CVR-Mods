using BTKUILib;
using BTKUILib.UIObjects;
using Sketch.SourceMovement;
using System;
using System.IO;
using System.Reflection;

namespace SourceMovement.Integrations
{
    public static partial class BTKKUIAddon
    {
        private static Page _rootPage;
        private static string _rootPageElementID;
        private static bool _isSMTabOpened;

        public static void Initialize()
        {
            SetupIcons();
            SetupSM_ModTab();
        }

        private static void SetupIcons()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = assembly.GetName().Name;

            QuickMenuAPI.PrepareIcon("Source Movement", "Crowbar", GetIconStream("Crowbar.png"));

            return;
            Stream GetIconStream(string iconName) => assembly.GetManifestResourceStream($"{assemblyName}.Resources.{iconName}");
        }

        private static void SetupSM_ModTab()
        {
            _rootPage = new Page("Source Movement", "Source Movement Settings", true, "Crowbar")
            {
                MenuTitle = "Source Movement",
                MenuSubtitle = "Move around like Gordon Freeman, or dont; up to you."
            };

            _rootPageElementID = _rootPage.ElementID;

            QuickMenuAPI.OnTabChange += OnTabChange;
        }

        private static DateTime lastTime = DateTime.Now;

        private static void OnTabChange(string newTab, string previousTab)
        {
            _isSMTabOpened = newTab == _rootPageElementID;
            if (!_isSMTabOpened) return;

            TimeSpan timeDifference = DateTime.Now - lastTime;
            if (timeDifference.TotalSeconds <= 0.5)
            {
                if (Sketch.SourceMovement.SourceMovement.UseDoubleClickTab.Value)
                {
                    ToggleSourceMovement();
                }
                return;
            }
            lastTime = DateTime.Now;
        }

        private static void ToggleSourceMovement()
        {
            if (Sketch.SourceMovement.SourceMovement.EntryUseSourceMovement.Value)
            {
                //Find a way to change the UseSourceMovement setting
            }
            else
            {
                //Find a way to change the UseSourceMovement setting
            }
        }
    }
}
