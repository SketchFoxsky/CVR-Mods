using MelonLoader;
using UnityEngine;
using HarmonyLib;
using ABI_RC.Systems.Movement;
using ABI_RC.Core.UI;
using ABI.CCK.Components;

namespace Sketch.SourceMovement
{
    public class SourceMovement : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
        }
    }
}