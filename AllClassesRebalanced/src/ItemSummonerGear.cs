using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using System;
using System.Collections.Generic;


namespace AllClassesRebalanced;

public class ItemSummonerGear : Item
{
    private readonly Random random = new();

    private const float USE_TIME = 3.5f;

    private class SpawnEntry
    {
        public string Entity { get; set; } = "";
        public int Min { get; set; }
        public int Max { get; set; }
    }

    private Dictionary<string, List<SpawnEntry>>? spawningByType;

    private const string REWARD_GUARD_KEY =
        "allclassesrebalanced_reward_handled";

    private static readonly AssetLocation USE_SOUND =
        new("game:sounds/effect/metalgrinding");

    private const float PARTICLE_INTERVAL = 0.15f;

    private const float PARTICLE_RADIUS = 0.12f;
    private const int PARTICLE_QUANTITY = 2;
    private const float PARTICLE_SCALE = 0.4f;

    private const float CAMERA_SHAKE_MIN = 0.015f;
    private const float CAMERA_SHAKE_MAX = 0.04f;

    private const string ROTATION_GUARD_KEY =
        "allclassesrebalanced_rotation_active";
    private const string ROTATION_ANGLE =
        "allclassesrebalanced_rotation_angle";

    private const float ROTATION_MIN_SPEED = 60f;
    private const float ROTATION_MAX_SPEED = 560f;

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);

        JsonObject spawningConfig = Attributes?["spawnEntries"];
        if (spawningConfig == null || !spawningConfig.Exists)
        {
            api.Logger.Warning(
                "[allrebalanced] SummonerGear has no spawnEntries configuration."
            );

            return;
        }

        spawningByType = new Dictionary<string, List<SpawnEntry>>();

        foreach (JsonObject entry in spawningConfig.AsArray())
        {
            string selector = entry["type"].AsString("");
            string entity = entry["entity"].AsString("");

            if (string.IsNullOrWhiteSpace(selector) ||
                string.IsNullOrWhiteSpace(entity))
            {
                api.Logger.Warning(
                    "[allrebalanced] SummonerGear has a spawnEntries entry without type or entity."
                );
                continue;
            }

            if (!spawningByType.TryGetValue(selector, out List<SpawnEntry>? entries))
            {
                entries = new List<SpawnEntry>();
                spawningByType[selector] = entries;
            }

            entries.Add(new SpawnEntry
            {
                Entity = entity,
                Min = entry["min"].AsInt(1),
                Max = entry["max"].AsInt(1)
            });
        }

        if (spawningByType.Count == 0)
        {
            api.Logger.Warning(
                "[allrebalanced] SummonerGear has no usable spawnEntries configuration."
            );
        }
    }

    private List<SpawnEntry>? GetSpawnEntries(ItemStack itemstack)
    {
        if (spawningByType == null || spawningByType.Count == 0)
        {
            return null;
        }

        string itemCode = itemstack.Collectible.Code.ToString();

        if (spawningByType.TryGetValue(
                itemCode,
                out List<SpawnEntry>? exact))
        {
            return exact;
        }

        foreach (KeyValuePair<string, List<SpawnEntry>> entry
                 in spawningByType)
        {
            string selector = entry.Key;

            if (!selector.StartsWith("*", StringComparison.Ordinal))
            {
                continue;
            }

            string suffix = selector[1..];

            if (itemCode.EndsWith(
                    suffix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return entry.Value;
            }
        }

        foreach (KeyValuePair<string, List<SpawnEntry>> entry
                 in spawningByType)
        {
            string selector = entry.Key;

            if (!selector.EndsWith("*", StringComparison.Ordinal))
            {
                continue;
            }

            string prefix = selector[..^1];

            if (itemCode.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                return entry.Value;
            }
        }

        if (spawningByType.TryGetValue(
                "*",
                out List<SpawnEntry>? fallback))
        {
            return fallback;
        }

        return null;
    }

    public override void OnHeldInteractStart(
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel,
        EntitySelection entitySel,
        bool firstEvent,
        ref EnumHandHandling handling)
    {
        if (!firstEvent || slot?.Itemstack == null)
        {
            return;
        }

        IPlayer? player = (byEntity as EntityPlayer)?.Player;

        if (player == null)
        {
            return;
        }

        // Trait restriction currently disabled intentionally WIP
        if (!TraitHelper.HasTrait(player, TraitHelper.SummonerTrait) && 1 == 2)
        {
            return;
        }

        slot.Itemstack.TempAttributes.RemoveAttribute(
            REWARD_GUARD_KEY
        );

        slot.Itemstack.TempAttributes.RemoveAttribute(
            "allclassesrebalanced_particle_time"
        );

        slot.Itemstack.TempAttributes.SetBool(
            ROTATION_GUARD_KEY,
            false
        );

        slot.Itemstack.TempAttributes.RemoveAttribute(
            ROTATION_ANGLE
        );

        if (blockSel == null || blockSel.Face != BlockFacing.UP)
        {
            return;
        }

        handling = EnumHandHandling.PreventDefaultAction;

        if (api.Side == EnumAppSide.Server)
        {
            try
            {
                api.World.PlaySoundAt(
                    USE_SOUND,
                    blockSel.FullPosition.X,
                    blockSel.FullPosition.Y,
                    blockSel.FullPosition.Z,
                    null,
                    false,
                    32f,
                    1.0f
                );
            }
            catch
            {
                api.Logger.Warning(
                "[allrebalanced] SummonerGear sound could not be played"
                );
            }
        }

        slot.Itemstack.TempAttributes.SetBool(
            ROTATION_GUARD_KEY,
            true
        );

        slot.Itemstack.TempAttributes.SetFloat(
            ROTATION_ANGLE,
            0f
        );
    }

    public override bool OnHeldInteractStep(
        float secondsUsed,
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel,
        EntitySelection entitySel)
    {
        if (api.Side == EnumAppSide.Client)
        {
            ICoreClientAPI clientApi = (ICoreClientAPI)api;

            float progress = GameMath.Clamp(
                secondsUsed / USE_TIME,
                0f,
                1f
            );

            float shakeStrength = GameMath.Lerp(
                CAMERA_SHAKE_MIN,
                CAMERA_SHAKE_MAX,
                progress
            );

            clientApi.World.AddCameraShake(shakeStrength);
        }

        if (api.Side == EnumAppSide.Server && blockSel != null)
        {
            float currentInterval =
                (float)Math.Floor(
                    secondsUsed / PARTICLE_INTERVAL
                ) * PARTICLE_INTERVAL;

            float lastInterval =
                slot.Itemstack?.TempAttributes.GetFloat(
                    "allclassesrebalanced_particle_time"
                ) ?? -1f;

            if (currentInterval > lastInterval)
            {
                slot.Itemstack.TempAttributes.SetFloat(
                    "allclassesrebalanced_particle_time",
                    currentInterval
                );

                SpawnTargetParticles(
                    blockSel,
                    byEntity
                );
            }
        }

        if (slot?.Itemstack != null)
        {
            float progress = GameMath.Clamp(
                secondsUsed / USE_TIME,
                0f,
                1f
            );

            _ = progress;

            float angle =
                ROTATION_MIN_SPEED * secondsUsed +
                0.5f * (ROTATION_MAX_SPEED - ROTATION_MIN_SPEED)
                * secondsUsed * secondsUsed;

            angle %= 360f;

            slot.Itemstack.TempAttributes.SetFloat(
                ROTATION_ANGLE,
                angle
            );

            slot.Itemstack.TempAttributes.SetBool(
                ROTATION_GUARD_KEY,
                true
            );
        }

        if (secondsUsed >= USE_TIME)
        {
            return false;
        }

        return true;
    }

    public override bool OnHeldInteractCancel(
        float secondsUsed,
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel,
        EntitySelection entitySel,
        EnumItemUseCancelReason cancelReason)
    {
        if (slot?.Itemstack != null)
        {
            slot.Itemstack.TempAttributes.SetBool(
                ROTATION_GUARD_KEY,
                false
            );

            slot.Itemstack.TempAttributes.RemoveAttribute(
                ROTATION_ANGLE
            );

            slot.Itemstack.TempAttributes.RemoveAttribute(
                "allclassesrebalanced_particle_time"
            );
        }

        return true;
    }

    public override void OnHeldInteractStop(
        float secondsUsed,
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel,
        EntitySelection entitySel)
    {
        if (slot?.Itemstack != null)
        {
            slot.Itemstack.TempAttributes.SetBool(
                ROTATION_GUARD_KEY,
                false
            );

            slot.Itemstack.TempAttributes.RemoveAttribute(
                ROTATION_ANGLE
            );

            slot.Itemstack.TempAttributes.RemoveAttribute(
                "allclassesrebalanced_particle_time"
            );
        }

        if (secondsUsed < USE_TIME)
        {
            return;
        }

        if (slot?.Itemstack == null)
        {
            api.Logger.Warning(
                "[allrebalanced] SummonerGear OnHeldInteractStop: itemstack is missing."
            );
            return;
        }

        if (slot.Itemstack.TempAttributes.GetBool(REWARD_GUARD_KEY) || api.Side != EnumAppSide.Server || byEntity is not EntityPlayer player || slot.Itemstack.Collectible != this)
        {
            return;
        }

        slot.Itemstack.TempAttributes.SetBool(
            REWARD_GUARD_KEY,
            true
        );

        ItemStack? removed = slot.TakeOut(1);

        slot.MarkDirty();

        if (removed == null)
        {
            api.Logger.Warning(
                $"[allrebalanced] SummonerGear WRN: " +
                $"gear could not be removed " +
                $"for player {player.PlayerUID}."
            );
            return;
        }


        if (blockSel != null)
        {
            SpawnEntitiesAtTarget(
                blockSel,
                byEntity,
                removed
            );
        }
    }

    private void SpawnEntitiesAtTarget(
        BlockSelection blockSel,
        EntityAgent byEntity,
        ItemStack itemstack)
    {
        if (api.Side != EnumAppSide.Server)
        {
            return;
        }

        if (blockSel == null)
        {
            return;
        }

        List<SpawnEntry>? entries =
            GetSpawnEntries(itemstack);

        if (entries == null || entries.Count == 0)
        {
            return;
        }

        SpawnEntry selectedEntry =
            entries[
                byEntity.World.Rand.Next(entries.Count)
            ];

        if (string.IsNullOrWhiteSpace(
                selectedEntry.Entity))
        {
            return;
        }

        int min = Math.Max(
            0,
            selectedEntry.Min
        );

        int max = Math.Max(
            min,
            selectedEntry.Max
        );

        int spawnCount =
            byEntity.World.Rand.Next(
                min,
                max + 1
            );

        if (spawnCount <= 0)
        {
            return;
        }

        EntityProperties? type =
            byEntity.World.GetEntityType(
                new AssetLocation(
                    selectedEntry.Entity
                )
            );

        if (type == null)
        {
            api.Logger.Error(
                "[allrebalanced] SummonerGear: " +
                $"entity '{selectedEntry.Entity}' was not found."
            );

            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Entity? entity =
                byEntity.World.ClassRegistry.CreateEntity(
                    type
                );

            if (entity == null)
            {
                api.Logger.Warning(
                    "[allrebalanced] SummonerGear: " +
                    $"CreateEntity returned null for " +
                    $"'{selectedEntry.Entity}'."
                );
                continue;
            }

            entity.Pos.X = blockSel.FullPosition.X;
            entity.Pos.Y = blockSel.FullPosition.Y;
            entity.Pos.Z = blockSel.FullPosition.Z;

            entity.Pos.Yaw =
                (float)byEntity.World.Rand.NextDouble()
                * 2f
                * GameMath.PI;

            entity.PositionBeforeFalling.X =
                entity.Pos.X;

            entity.PositionBeforeFalling.Y =
                entity.Pos.Y;

            entity.PositionBeforeFalling.Z =
                entity.Pos.Z;

            byEntity.World.SpawnEntity(entity);
        }
    }

    public override void OnBeforeRender(
        ICoreClientAPI capi,
        ItemStack itemstack,
        EnumItemRenderTarget target,
        ref ItemRenderInfo renderinfo)
    {
        base.OnBeforeRender(
            capi,
            itemstack,
            target,
            ref renderinfo
        );

        if (!itemstack.TempAttributes.GetBool(
                ROTATION_GUARD_KEY))
        {
            return;
        }

        if (target != EnumItemRenderTarget.HandTp)
        {
            return;
        }

        float angle =
            itemstack.TempAttributes.GetFloat(
                ROTATION_ANGLE
            );

        if (renderinfo.Transform == null)
        {
            return;
        }

        renderinfo.Transform =
            renderinfo.Transform.Clone();

        renderinfo.Transform.Rotation.Y += angle;
    }

    private void SpawnTargetParticles(
        BlockSelection blockSel,
        EntityAgent byEntity)
    {
        if (blockSel == null)
        {
            return;
        }

        Vec3d pos = new Vec3d(
            blockSel.Position.X +
                blockSel.HitPosition.X,

            blockSel.Position.Y +
                blockSel.HitPosition.Y,

            blockSel.Position.Z +
                blockSel.HitPosition.Z
        );

        SimpleParticleProperties particles =
            new SimpleParticleProperties(
                1,
                2,
                ColorUtil.ToRgba(
                    150,
                    183,
                    65,
                    14
                ),
                pos,
                pos,
                new Vec3f(
                    -0.03f,
                    0.05f,
                    -0.03f
                ),
                new Vec3f(
                    0.03f,
                    0.18f,
                    0.03f
                ),
                1.5f,
                0.0f,
                0.15f,
                0.4f,
                EnumParticleModel.Quad
            );

        particles.AddPos.Set(
            0.12,
            0.05,
            0.12
        );

        particles.RandomVelocityChange = true;

        particles.OpacityEvolve =
            EvolvingNatFloat.create(
                EnumTransformFunction.QUADRATIC,
                -3f
            );

        particles.SizeEvolve =
            EvolvingNatFloat.create(
                EnumTransformFunction.LINEAR,
                0.5f
            );

        api.World.SpawnParticles(
            particles
        );
    }
}
