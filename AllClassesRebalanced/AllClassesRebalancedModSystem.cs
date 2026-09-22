using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using HarmonyLib;
using Vintagestory.API.Datastructures;
using System.Linq;

namespace AllClassesRebalanced;

[HarmonyPatch(typeof(EntityPlayer))]
public static class EntityPlayerLightPatch
{
    private static readonly byte[] SailorLightHsv = { 42, 5, 11 };
    private static readonly byte[] NoLight = { 0,0,0 };

    [HarmonyPatch("LightHsv", MethodType.Getter)]
    [HarmonyPostfix]
    private static void Postfix(
        EntityPlayer __instance,
        ref byte[] __result
    )
    {
        if (!__instance.WatchedAttributes.GetBool(
            AllClassesRebalancedModSystem.BeaconWatchedAttribute
        ))
        {
            return;
        }

        __instance.Properties.Client.GlowLevel = 220;
        if (__result == null || (__result.Length == 3 && __result.SequenceEqual(NoLight)))
        {
            AllClassesRebalancedModSystem.Logger.Notification(
                "[EntityPlayerLightPatch] Iside IF"
        );
            __result = (byte[])SailorLightHsv.Clone();

        }

        return;
    }
}

[HarmonyPatch(typeof(CharacterSystem))]
public static class CharacterSystemPatch
{
    [HarmonyPatch(nameof(CharacterSystem.setCharacterClass))]
    [HarmonyPostfix]
    private static void SetCharacterClassPostfix(
        EntityPlayer eplayer,
        string classCode,
        bool initializeGear
    )
    {
        if (eplayer == null)
        {
            return;
        }

        if (eplayer.World.Side != EnumAppSide.Server)
        {
            return;
        }

        // AllClassesRebalancedModSystem.Logger.Notification(
        //     "[CharacterPatch] setCharacterClass finished. EntityId={0}, UID={1}, NewClass={2}",
        //     eplayer.EntityId,
        //     eplayer.PlayerUID,
        //     classCode
        // );

        bool hasGlow = TraitHelper.IsGlowing(eplayer.Player);
        AllClassesRebalancedModSystem.Logger.Notification(
            "[CharacterPatch] Has glowing trait = {0}",
            hasGlow
        );

        eplayer.WatchedAttributes.SetBool(
            AllClassesRebalancedModSystem.BeaconWatchedAttribute,
            hasGlow
        );
        AllClassesRebalancedModSystem.Logger.Notification(
            "[CharacterPatch] Watched glow = {0}",
            eplayer.WatchedAttributes.GetBool(
                AllClassesRebalancedModSystem.BeaconWatchedAttribute
            )
        );
    }
}

[HarmonyPatch(typeof(Entity), nameof(Entity.ReceiveDamage))]
public static class ProjectileResistancePatch
{
    private const float DamageMultiplier = 0.75f;

    [HarmonyPrefix]
    private static void Prefix(
        Entity __instance,
        DamageSource damageSource,
        ref float damage
    )
    {
        if (__instance.World.Side != EnumAppSide.Server 
            || __instance is not EntityPlayer player
            || !TraitHelper.HasTrait(player.Player,TraitHelper.ProjectileResistanceTrait))
            return;

        if (damageSource.SourceEntity is not IProjectile || damage <= 0)
            return;

        damage *= DamageMultiplier;
    }
}

public class AllClassesRebalancedModSystem : ModSystem
{
    public static ILogger Logger  {get; private set;} = null!;
    // public const string ExtraInventoryIdPrefix = "mule-allrebalanced";

    public const string BeaconWatchedAttribute = "beacon-allrebalanced";

    private static readonly byte[] GlowLight = { 7, 4, 90 };

    public override void Start(ICoreAPI api)
    {
        Logger = api.Logger;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        Mod.Logger.Notification("Hello AllClassesRebalanced: " + api.Side);

        api.Event.PlayerJoin += OnPlayerJoin;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        Mod.Logger.Notification("Hello AllClassesRebalanced: " + api.Side);

        new Harmony("allrebalanced").PatchAll(); //to Start()?
        // api.Event.PlayerEntitySpawn += RegisterGlowListener;
    }

    //Server side
    private void OnPlayerJoin(IServerPlayer player)
    {
        Mod.Logger.Notification("AllClassesRebalanced: OnPlayerJoin");
        if (player == null)
            return;

        UpdateGlowAttribute(player.Entity);
player.Entity.WatchedAttributes.RegisterModifiedListener("hunger", () =>
{
    ITreeAttribute hunger =
        player.Entity.WatchedAttributes.GetTreeAttribute("hunger");

    float fruit = hunger.GetFloat("fruitLevel");
            Mod.Logger.Notification("AllClassesRebalanced: FRUIT LEVEL ", fruit.ToString());
            if (fruit < 750){
                UpdateGlowAttribute(player.Entity, false);
            }
            else
                UpdateGlowAttribute(player.Entity, true);
        });
    }

    private void ApplyGlow(IServerPlayer player)
    {
        if (!TraitHelper.HasTrait(
                player,
                TraitHelper.GlowingTrait))
        {
            player.Entity.WatchedAttributes.RemoveAttribute(
                "allclassesrebalanced-glowing"
            );

            return;
        }

        player.Entity.WatchedAttributes.SetBool(
            "allclassesrebalanced-glowing",
            true
        );
    }

    //Client side


    //Common
    private static void UpdateGlowAttribute(EntityPlayer player, bool shouldGlow = true)
    {
        bool hasGlow = TraitHelper.IsGlowing(player.Player) && shouldGlow;
        Logger.Notification("AllClassesRebalanced: Has Glow? ", hasGlow.ToString());
        player.WatchedAttributes.SetBool(
            BeaconWatchedAttribute,
            hasGlow
        );
    }



    private void ApplyGlow(IClientPlayer player)
    {
        // if (player.Entity == null)
        // {
        //     return;
        // }

        // bool glowing =
        //     player.Entity.WatchedAttributes.GetBool(
        //         "allclassesrebalanced-glowing"
        //     );

        // player.Entity.LightHsv = glowing
        //     ? new byte[] { 10, 10, 50 }
        //     : null;
    }

    private void RegisterGlowListener(IClientPlayer player)
    {
        player.Entity.WatchedAttributes.RegisterModifiedListener(
            "allclassesrebalanced-glowing",
            () => ApplyGlow(player)
        );

        // ApplyGlow(player);
    }

    // private void ApplyExtraInventory(IPlayer player)
    // {
    //     Mod.Logger.Notification("AllClassesRebalanced: ApplyExtraInventory");
    //     // capi.Logger.Debug(
    //     // "AllClassesRebalanced Extra inventory: {0}",
    //     // extraInventory?.InventoryID ?? "NULL");
    //     if (!TraitHelper.HasExtraInventory(player)
    //         || player.InventoryManager.GetOwnInventory(ExtraInventoryIdPrefix) != null)
    //         return;

    //     string inventoryId =
    //         player.InventoryManager.GetInventoryName(
    //             ExtraInventoryIdPrefix);

    //     RebalancedPlayerInventory inventory =
    //         new RebalancedPlayerInventory(
    //             ExtraInventoryIdPrefix,
    //             player.PlayerUID,
    //             player.Entity.Api
    //         );
    //     Mod.Logger.Notification("AllClassesRebalanced: ApplyExtraInventory APPLIED");
    //     player.InventoryManager.Inventories[inventoryId] = inventory;
    // }
}

// using Vintagestory.API.Client;
// using Vintagestory.API.Server;
// using Vintagestory.API.Config;
// using Vintagestory.API.Common;

// namespace AllClassesRebalanced;

// public class AllClassesRebalancedModSystem : ModSystem
// {
//     // Called on server and client
//     // Useful for registering block/entity classes on both sides
//     public override void Start(ICoreAPI api)
//     {
//         Mod.Logger.Notification("Hello from template mod: " + api.Side);
//     }

//     public override void StartServerSide(ICoreServerAPI api)
//     {
//         Mod.Logger.Notification("Hello from template mod server side: " + Lang.Get("allclassesrebalanced:hello"));
//     }

//     public override void StartClientSide(ICoreClientAPI api)
//     {
//         Mod.Logger.Notification("Hello from template mod client side: " + Lang.Get("allclassesrebalanced:hello"));
//     }
// }
