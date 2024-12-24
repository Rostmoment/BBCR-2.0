using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBCR.API
{
    public abstract class ConditionalPatch : Attribute
    {
        public abstract bool ShouldPatch();
    }
    public class ConditionalPatchMod : ConditionalPatch
    {
        public string modKey;

        public ConditionalPatchMod(string mod)
        {
            modKey = mod;
        }

        public override bool ShouldPatch()
        {
            return Chainloader.PluginInfos.ContainsKey(modKey);
        }
    }
}
