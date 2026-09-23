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
        float? fruitLevel = ReadWatchedFruitLevel(player.Entity);
        UpdateGlowAttribute(player.Entity, fruitLevel?.CompareTo(GlowFruitLevelThreshold) >= 0 || fruitLevel is null);

        player.Entity.WatchedAttributes.RegisterModifiedListener("hunger", () =>
        {
            float? fruitLevel = ReadWatchedFruitLevel(player.Entity);
            UpdateGlowAttribute(player.Entity, fruitLevel?.CompareTo(GlowFruitLevelThreshold) >= 0 || fruitLevel is null);
        });
    }

    private static float? ReadWatchedFruitLevel(EntityPlayer eplayer)
    {
        ITreeAttribute hunger = eplayer.WatchedAttributes.GetTreeAttribute("hunger");
        if (hunger == null)
        {
            return null;
        }
        return hunger.GetFloat("fruitLevel");
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
