using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace AllClassesRebalanced;

public class AllClassesRebalancedModSystem : ModSystem
{
    public static ILogger Logger { get; private set; } = null!;

    public const string BeaconWatchedAttribute = "beacon-allrebalanced";

    private const float GlowFruitLevelThreshold = 450f;

    public override void Start(ICoreAPI api)
    {
        Logger = api.Logger;
        new Harmony("allrebalanced").PatchAll();
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Event.PlayerJoin += OnPlayerJoin;
    }

    private void OnPlayerJoin(IServerPlayer player)
    {
        if (player?.Entity == null)
        {
            return;
        }

        UpdateGlowAttribute(player.Entity);
        player.Entity.WatchedAttributes.RegisterModifiedListener("hunger", () =>
        {
            ITreeAttribute hunger = player.Entity.WatchedAttributes.GetTreeAttribute("hunger");
            if (hunger == null)
            {
                return;
            }

            float fruit = hunger.GetFloat("fruitLevel");
            UpdateGlowAttribute(player.Entity, fruit >= GlowFruitLevelThreshold);
        });
    }

    private static void UpdateGlowAttribute(EntityPlayer player, bool shouldGlow = true)
    {
        if (player == null)
        {
            return;
        }

        player.WatchedAttributes.SetBool(
            BeaconWatchedAttribute,
            TraitHelper.IsGlowing(player.Player) && shouldGlow
        );
    }
}
