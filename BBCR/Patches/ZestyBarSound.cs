using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
namespace BBCR.Patches
{
    [HarmonyPatch(typeof(ITM_ZestyBar))]
    class ZestyBarSound
    {
        [HarmonyPatch("Use")]
        [HarmonyPrefix]
        private static void AddSound() => CoreGameManager.Instance.audMan.PlaySingle(BasePlugin.assets.Get<SoundObject>("ZestyBarEat"));
    }
}
