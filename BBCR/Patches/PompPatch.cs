using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
namespace BBCR.Patches.NPC
{
    [HarmonyPatch(typeof(NoLateTeacher))]
    class PompPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void FixSubtitles(NoLateTeacher __instance)
        {
            __instance.audSpot.soundKey = "You there!";
            __instance.audInTime.soundKey = "Just in time!";
            __instance.audScream.soundKey = "WHYYYYYYY WEREN'T YOU AT MY CLAAAAAAASSS?!?!?!";
            __instance.audTimesUp.soundKey = "Time's up...";
            __instance.audDismissed.soundKey = "Class dismissed!";
            __instance.audIntro.soundKey = "Just reminding you to be at my class in";
            for (int i = 0; i<__instance.audNumbers.Length; i++)
            {
                __instance.audNumbers[i].soundKey = i.ToString();

            }
            __instance.audMinutes.soundKey = "minutes.";
            __instance.audMinutesLeft.soundKey = "minutes left.";
        }
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RemoveMapAudioClip(IEnumerable<CodeInstruction> instructions)
        {
            List<int> indexes = new List<int>() { 181, 182, 183, 184, 185 };
            List<CodeInstruction> result = new List<CodeInstruction>();
            int x = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!indexes.Contains(x)) result.Add(instruction);
                x++;
            }
            return result;
        }
    }
}
