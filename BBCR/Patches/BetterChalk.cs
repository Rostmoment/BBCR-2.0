using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBCR.Patches
{
    [HarmonyPatch(typeof(ChalkEraser))]
    internal class BetterChalk
    {
        [HarmonyPatch("Use")]
        [HarmonyPostfix]
        private static void FixMirroredChalkEraser(ChalkEraser __instance)
        {
            if (CoreGameManager.Instance.mirrorMode)
            {
                __instance.gameObject.transform.localScale = new UnityEngine.Vector3(-__instance.gameObject.transform.localScale.x,
                __instance.gameObject.transform.localScale.y, -__instance.gameObject.transform.localScale.z);
            }
            ParticleSystemRenderer p = __instance.gameObject.GetComponent<ParticleSystemRenderer>();
            foreach (Material mat in p.materials) 
            {
                if (!mat.mainTexture.IsNull())
                    Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), mat.mainTexture);
            }
            foreach (Material mat in p.sharedMaterials)
            {
                if (!mat.mainTexture.IsNull())
                    Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), mat.mainTexture);
            }
            if (!p.sharedMaterial.mainTexture.IsNull())
                Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), p.sharedMaterial.mainTexture);
            if (!p.material.mainTexture.IsNull())
                Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), p.material.mainTexture);


            ParticleSystemRenderer p2 = __instance.GetComponent<ParticleSystemRenderer>();
            foreach (Material mat in p2.materials)
            {
                if (!mat.mainTexture.IsNull())
                    Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), mat.mainTexture);
            }
            foreach (Material mat in p2.sharedMaterials)
            {
                if (!mat.mainTexture.IsNull())
                    Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), mat.mainTexture);
            }

            if (!p2.sharedMaterial.mainTexture.IsNull())
                Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), p2.sharedMaterial.mainTexture);
            if (!p2.material.mainTexture.IsNull())
                Graphics.CopyTexture(BasePlugin.assets.Get<Texture2D>("NewChalkCloud"), p2.material.mainTexture);
        }
    }
}
