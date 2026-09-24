using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace AllClassesRebalanced;

public class ItemSummonerGear : Item
{
    private const float USE_TIME = 3.0f;
    private const string REWARD_HANDLED = "AllClassesRebalanced_reward_handled";


    public override void OnHeldInteractStart(
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel,
        EntitySelection entitySel,
        bool firstEvent,
        ref EnumHandHandling handling)
    {
        api.Logger.Notification(
            $"[AllClassesRebalanced] SummonerGear START CALLBACK | " +
            $"firstEvent={firstEvent} | " +
            $"blockSel={(blockSel != null ? "YES" : "NULL")} | " +
            $"face={(blockSel?.Face?.Code ?? "NULL")} | " +
            $"entitySel={(entitySel != null ? "YES" : "NULL")}"
        );

        if (!firstEvent || slot?.Itemstack == null)
        {
            return;
        }

        slot.Itemstack.TempAttributes.RemoveAttribute(REWARD_HANDLED);

        if (blockSel == null)
        {
            api.Logger.Debug(
                "[AllClassesRebalanced] SummonerGear START REJECTED: no BlockSelection."
            );

            return;
        }

        api.Logger.Notification(
            $"[AllClassesRebalanced] SummonerGear TARGET | " +
            $"pos={blockSel.Position} | " +
            $"face={blockSel.Face.Code} | " +
            $"hit={blockSel.HitPosition}"
        );

        if (blockSel.Face != BlockFacing.UP)
        {
            api.Logger.Debug(
                $"[AllClassesRebalanced] SummonerGear START REJECTED: wrong face = {blockSel.Face.Code}"
            );

            return;
        }

        handling = EnumHandHandling.PreventDefault;

        api.Logger.Notification(
            "[AllClassesRebalanced] SummonerGear START ACCEPTED - interaction is active."
        );
    }


    public override bool OnHeldInteractStep(
        float secondsUsed,
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel,
        EntitySelection entitySel)
    {
        if (secondsUsed >= USE_TIME)
        {
            api.Logger.Notification(
                $"[AllClassesRebalanced] SummonerGear STEP COMPLETE | " +
                $"secondsUsed={secondsUsed:F2}"
            );
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
        api.Logger.Notification(
            $"[AllClassesRebalanced] SummonerGear CANCEL | " +
            $"secondsUsed={secondsUsed:F2} | " +
            $"reason={cancelReason}"
        );

        return true;
    }


    public override void OnHeldInteractStop(
        float secondsUsed,
        ItemSlot slot,
        EntityAgent byEntity,
        BlockSelection blockSel,
        EntitySelection entitySel)
    {
        api.Logger.Notification(
            $"[AllClassesRebalanced] SummonerGear STOP | " +
            $"secondsUsed={secondsUsed:F2} | " +
            $"blockSel={(blockSel != null ? "YES" : "NULL")} | " +
            $"face={(blockSel?.Face?.Code ?? "NULL")}"
        );

        if (secondsUsed < USE_TIME)
        {
            api.Logger.Debug(
                "[AllClassesRebalanced] SummonerGear STOP: interaction cancelled before completion."
            );

            return;
        }

        if (slot?.Itemstack == null)
        {
            api.Logger.Warning(
                "[AllClassesRebalanced] SummonerGear STOP: itemstack is missing."
            );

            return;
        }

        if (slot.Itemstack.TempAttributes.GetBool(REWARD_HANDLED))
        {
            api.Logger.Debug(
                "[AllClassesRebalanced] SummonerGear STOP: reward already handled, ignoring duplicate callback."
            );

            return;
        }

        if (api.Side != EnumAppSide.Server)
        {
            api.Logger.Debug(
                "[AllClassesRebalanced] SummonerGear STOP: skipping reward on client side."
            );

            return;
        }

        if (byEntity is not EntityPlayer player)
        {
            api.Logger.Warning(
                $"[AllClassesRebalanced] SummonerGear STOP: entity is not EntityPlayer. " +
                $"entityId={byEntity?.EntityId}"
            );

            return;
        }

        if (slot.Itemstack.Collectible != this)
        {
            api.Logger.Warning(
                "[AllClassesRebalanced] SummonerGear STOP: held item is no longer SummonerGear."
            );

            return;
        }
        //tmp reward
        Item? rewardItem = api.World.GetItem(
            new AssetLocation("game:rawbrick-blue")
        );

        if (rewardItem == null)
        {
            api.Logger.Error(
                "[AllClassesRebalanced] SummonerGear REWARD FAILED: " +
                "could not find item game:rawbrick-blue."
            );

            return;
        }

        ItemStack rewardStack = new ItemStack(rewardItem, 1);

        api.Logger.Notification(
            $"[AllClassesRebalanced] SummonerGear REWARD: attempting to give " +
            $"game:rawbrick-blue to player {player.PlayerUID}."
        );

        bool rewardGiven = player.Player.InventoryManager.TryGiveItemstack(
            rewardStack,
            slotNotifyEffect: true
        );

        if (!rewardGiven)
        {
            api.Logger.Warning(
                $"[AllClassesRebalanced] SummonerGear REWARD FAILED: inventory full " +
                $"for player {player.PlayerUID}. Carrot will NOT be removed."
            );

            return;
        }

        slot.Itemstack.TempAttributes.SetBool(
            REWARD_HANDLED,
            true
        );

        ItemStack? removed = slot.TakeOut(1);
        slot.MarkDirty();

        if (removed == null)
        {
            api.Logger.Warning(
                $"[AllClassesRebalanced] SummonerGear REWARD WARNING: " +
                $"reward was given but carrot could not be removed " +
                $"for player {player.PlayerUID}."
            );

            return;
        }

        api.Logger.Notification(
            $"[AllClassesRebalanced] SummonerGear REWARD SUCCESS | " +
            $"player={player.PlayerUID} | " +
            $"given=game:rawbrick-blue x1 | " +
            $"carrot removed=1"
        );
    }
}