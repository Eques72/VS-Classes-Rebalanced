using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using System;
using System.Collections.Generic;


namespace AllClassesRebalanced;

public class ItemSummonerGear : Item
{
    private const float USE_TIME = 3.5f;
    private const string PARTICLE_TIME_KEY =
        "allclassesrebalanced_particle_time";

    public class SummonSpawnEntry
    {
        public string Entity { get; set; } = "";
        public int Min { get; set; }
        public int Max { get; set; }
        public float Temp_Loss {get; set;} = 0.0f;
    }

    private Dictionary<string, List<SummonSpawnEntry>>? spawningByType;

    private const string REWARD_GUARD_KEY =
        "allclassesrebalanced_reward_handled";

    private static readonly AssetLocation USE_SOUND =
        new("game:sounds/effect/metalgrinding");

    private const float PARTICLE_INTERVAL = 0.15f;

    private const float PARTICLE_RADIUS = 0.12f;
    private const int PARTICLE_QUANTITY = 4;
    private const float PARTICLE_SCALE = 0.3f;

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

        spawningByType = JsonHelper.ExtractSummonEntriesFromAttributes(Attributes, api);
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

        // Temporarely disabled, waits for config implementation
        // if (!TraitHelper.HasTrait(player, TraitHelper.SummonerTrait) && 1 == 2)
        // {
        //     return;
        // }

        ResetInteractionState(slot.Itemstack);
        slot.Itemstack.TempAttributes.RemoveAttribute(REWARD_GUARD_KEY);

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

        SetRotationState(slot.Itemstack, 0f);
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

            float shakeStrength = GameMath.Lerp(
                CAMERA_SHAKE_MIN,
                CAMERA_SHAKE_MAX,
                GetUseProgress(secondsUsed)
            );

            clientApi.World.AddCameraShake(shakeStrength);
        }

        if (api.Side == EnumAppSide.Server && blockSel != null)
        {
            SpawnParticlesAtInterval(secondsUsed, slot, blockSel, byEntity);
        }

        if (slot?.Itemstack != null)
        {
            UpdateRotation(slot.Itemstack, secondsUsed);
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
            ResetInteractionState(slot.Itemstack);
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
            ResetInteractionState(slot.Itemstack);
        }

        if (secondsUsed < USE_TIME)
        {
            return;
        }

        TrySummonAtTarget(slot, byEntity, blockSel);
    }

    private void TrySummonAtTarget(
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel)
    {
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

    private static float GetUseProgress(float secondsUsed)
    {
        return GameMath.Clamp(
            secondsUsed / USE_TIME,
            0f,
            1f
        );
    }

    private static void ResetInteractionState(ItemStack itemstack)
    {
        itemstack.TempAttributes.SetBool(
            ROTATION_GUARD_KEY,
            false
        );

        itemstack.TempAttributes.RemoveAttribute(ROTATION_ANGLE);
        itemstack.TempAttributes.RemoveAttribute(PARTICLE_TIME_KEY);
    }

    private static void SetRotationState(ItemStack itemstack, float angle)
    {
        itemstack.TempAttributes.SetBool(
            ROTATION_GUARD_KEY,
            true
        );

        itemstack.TempAttributes.SetFloat(
            ROTATION_ANGLE,
            angle
        );
    }

    private static void UpdateRotation(ItemStack itemstack, float secondsUsed)
    {
        float angle =
            ROTATION_MIN_SPEED * secondsUsed +
            0.5f * (ROTATION_MAX_SPEED - ROTATION_MIN_SPEED)
            * secondsUsed * secondsUsed;

        SetRotationState(itemstack, angle % 360f);
    }

    private void SpawnParticlesAtInterval(
        float secondsUsed,
        ItemSlot slot,
        BlockSelection blockSel,
        EntityAgent byEntity)
    {
        if (slot.Itemstack == null)
        {
            return;
        }

        float currentInterval =
            (float)Math.Floor(
                secondsUsed / PARTICLE_INTERVAL
            ) * PARTICLE_INTERVAL;

        float lastInterval = slot.Itemstack.TempAttributes.GetFloat(
            PARTICLE_TIME_KEY
        );

        if (currentInterval <= lastInterval)
        {
            return;
        }

        slot.Itemstack.TempAttributes.SetFloat(
            PARTICLE_TIME_KEY,
            currentInterval
        );

        SpawnTargetParticles(blockSel, byEntity);
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

        List<SummonSpawnEntry>? entries =
            JsonHelper.GetSpawnEntries(itemstack, spawningByType);

        if (entries == null || entries.Count == 0)
        {
            return;
        }

        SummonSpawnEntry selectedEntry =
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

        var temporalBehavior = byEntity.GetBehavior<EntityBehaviorTemporalStabilityAffected>();
        if(temporalBehavior != null)
            temporalBehavior.OwnStability = temporalBehavior.OwnStability - selectedEntry.Temp_Loss;
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
                2,
                PARTICLE_QUANTITY,
                ColorUtil.ToRgba(
                    120,
                    183,
                    65,
                    14
                ),
                pos,
                pos,
                new Vec3f(
                    -0.25f,
                    0.02f,
                    -0.25f
                ),
                new Vec3f(
                    0.25f,
                    0.06f,
                    0.25f
                ),
                1.8f,
                0.0f,
                0.10f,
                PARTICLE_SCALE,
                EnumParticleModel.Quad
            );

        particles.AddPos.Set(
            PARTICLE_RADIUS,
            0.05,
            PARTICLE_RADIUS
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
