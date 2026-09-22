using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace AllClassesRebalanced;

[HarmonyPatch(typeof(Entity), nameof(Entity.ReceiveDamage))]
public static class ProjectileResistancePatch
{
    private const float DamageMultiplier = 0.75f;

    [HarmonyPrefix]
    private static void Prefix(Entity __instance, DamageSource damageSource, ref float damage)
    {
        if (__instance.World.Side != EnumAppSide.Server
            || __instance is not EntityPlayer player
            || !TraitHelper.HasProjectileResistance(player.Player))
        {
            return;
        }

        if (damageSource.SourceEntity is not IProjectile || damage <= 0)
            return;

        damage *= DamageMultiplier;
    }
}
