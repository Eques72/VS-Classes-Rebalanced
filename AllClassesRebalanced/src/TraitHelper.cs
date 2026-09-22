using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace AllClassesRebalanced;

public static class TraitHelper
{
    public const string GlowingTrait = "beacon_ar";
    public const string ProjectileResistanceTrait = "projectile_resistance_ar";

    public static bool HasTrait(IPlayer player, string traitCode)
    {
        if (player?.Entity?.Api == null)
        {
            return false;
        }

        CharacterSystem characterSystem =
            player.Entity.Api.ModLoader.GetModSystem<CharacterSystem>();

        return characterSystem != null && characterSystem.HasTrait(player, traitCode);
    }

    public static bool IsGlowing(IPlayer player)
    {
        return HasTrait(player, GlowingTrait);
    }

    public static bool HasProjectileResistance(IPlayer player)
    {
        return HasTrait(player, ProjectileResistanceTrait);
    }
}