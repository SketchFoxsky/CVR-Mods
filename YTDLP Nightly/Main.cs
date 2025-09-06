using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;

namespace Sketch.YTdlpNightly
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            HarmonyInstance.PatchAll();
            MelonLogger.Msg("YtDlp Nightly Updater loaded!");
        }
    }

    [HarmonyPatch(typeof(ABI_RC.VideoPlayer.YoutubeDl), "UpdateYoutubeDl")]
    public static class YoutubeDlPatch
    {
        // Replace original coroutine entirely
        public static bool Prefix(ref IEnumerator __result)
        {
            __result = RunNightlyUpdate();
            return false; // skip original
        }

        private static IEnumerator RunNightlyUpdate()
        {
            string exePath = Path.Combine(Application.dataPath, "youtube-dl.exe");
            string nightlyUrl = "https://github.com/yt-dlp/yt-dlp-nightly-builds/releases/latest/download/yt-dlp.exe";

            MelonLogger.Msg("Checking for yt-dlp nightly update...");

            bool shouldDownload = true;

            // --- Step 1: Check local binary version ---
            if (File.Exists(exePath))
            {
                try
                {
                    Process proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = exePath,
                            Arguments = "--version",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    proc.Start();
                    string version = proc.StandardOutput.ReadToEnd().Trim();
                    proc.WaitForExit();

                    MelonLogger.Msg($"Local yt-dlp version: {version}");

                    // Nightly builds usually have timestamp or "-nightly"
                    if (version.Contains("nightly") || version.Split('.').Length > 3)
                    {
                        shouldDownload = false; // Already nightly, no need to replace
                    }
                    else
                    {
                        MelonLogger.Msg("Local yt-dlp is NOT nightly, forcing update.");
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning("Could not check local yt-dlp version: " + ex.Message);
                }
            }

            // --- Step 2: If not nightly, or missing, fetch new one ---
            if (shouldDownload)
            {
                using UnityWebRequest uwr = UnityWebRequest.Get(nightlyUrl);
                uwr.downloadHandler = new DownloadHandlerFile(exePath, true);
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    MelonLogger.Error("Failed to update yt-dlp nightly: " + uwr.error);
                }
                else
                {
                    MelonLogger.Msg("yt-dlp nightly downloaded successfully.");
                }
            }
            else
            {
                MelonLogger.Msg("yt-dlp is already a nightly build.");
            }
        }
    }
}
