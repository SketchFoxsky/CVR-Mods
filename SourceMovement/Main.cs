using MelonLoader;
using UnityEngine;
using ABI_RC.Systems.Movement;


namespace Sketch.SourceMovement;

public class SourceMovement : MelonMod
{
    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg("Initialized.");
    }
}