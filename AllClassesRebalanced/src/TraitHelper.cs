using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace AllClassesRebalanced;

public static class TraitHelper
{
    //to remember, double check names with those in assets/allrebalanced/config/traits
    public const string GlowingTrait = "beacon_ar";
    public const string ProjectileResistanceTrait = "projectile_resistance_ar";
    public const string ExtraInventoryTrait = "mule_ar";

    public static bool HasTrait(IPlayer player, string traitCode)
    {
        if (player?.Entity == null)
        {
            return false;
        }

        CharacterSystem characterSystem =
            player.Entity.Api.ModLoader.GetModSystem<CharacterSystem>();

        return characterSystem.HasTrait(player, traitCode);
    }

    public static bool IsGlowing(IPlayer player)
    {
        return HasTrait(player, GlowingTrait);
    }

    public static bool HasExtraInventory(IPlayer player)
    {
        return HasTrait(player, ExtraInventoryTrait);
    }
}