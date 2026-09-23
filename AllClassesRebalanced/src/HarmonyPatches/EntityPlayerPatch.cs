using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;

namespace AllClassesRebalanced;

[HarmonyPatch(typeof(EntityPlayer))]
public static class EntityPlayerLightPatch
{
    private static readonly byte[] BeaconLightHsv = { 42, 5, 11 };
    private static readonly byte[] NoLight = { 0, 0, 0 };
    private static readonly int GlowLevel = 220;

    [HarmonyPatch("LightHsv", MethodType.Getter)]
    [HarmonyPostfix]
    private static void Postfix(EntityPlayer __instance, ref byte[] __result)
    {
        if (!__instance.WatchedAttributes.GetBool(AllClassesRebalancedModSystem.BeaconWatchedAttribute))
        {
            return;
        }

        __instance.Properties.Client.GlowLevel = GlowLevel;
        if (__result == null || (__result.Length == 3 && __result.SequenceEqual(NoLight)))
        {
            __result = (byte[])BeaconLightHsv.Clone();
        }
    }
}
