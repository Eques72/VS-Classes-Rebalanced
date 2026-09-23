using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace AllClassesRebalanced;

[HarmonyPatch(typeof(CharacterSystem))]
public static class CharacterSystemPatch
{
    [HarmonyPatch(nameof(CharacterSystem.setCharacterClass))]
    [HarmonyPostfix]
    private static void SetCharacterClassPostfix(EntityPlayer eplayer, string classCode, bool initializeGear)
    {
        if (eplayer?.World == null || eplayer.World.Side != EnumAppSide.Server)
        {
            return;
        }

        eplayer.WatchedAttributes.SetBool(
            AllClassesRebalancedModSystem.BeaconWatchedAttribute,
            TraitHelper.IsGlowing(eplayer.Player)
        );
    }
}
