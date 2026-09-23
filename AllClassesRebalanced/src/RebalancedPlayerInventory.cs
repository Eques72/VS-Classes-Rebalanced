// using Vintagestory.API.Common;
// using Vintagestory.API.Datastructures;

// namespace AllClassesRebalanced;

// public class RebalancedPlayerInventory : InventoryBasePlayer
// {
//     public const int SlotCount = 4;

//     private ItemSlot[] slots;

//     public override int Count => slots.Length;

//     public override ItemSlot this[int slotId]
//     {
//         get
//         {
//             if (slotId < 0 || slotId >= slots.Length)
//             {
//                 return null;
//             }

//             return slots[slotId];
//         }
//         set
//         {
//             if (slotId < 0 || slotId >= slots.Length)
//             {
//                 return;
//             }

//             slots[slotId] = value;
//         }
//     }

//     public RebalancedPlayerInventory(
//         string inventoryID,
//         string playerUID,
//         ICoreAPI api
//     ) : base(inventoryID, playerUID, api)
//     {
//         slots = new ItemSlot[SlotCount];

//         for (int i = 0; i < SlotCount; i++)
//         {
//             slots[i] = new ItemSlot(this);
//         }
//     }

//     public override void FromTreeAttributes(ITreeAttribute tree)
//     {
//         slots = SlotsFromTreeAttributes(tree, slots);

//         if (slots == null || slots.Length != SlotCount)
//         {
//             ItemSlot[] oldSlots = slots;

//             slots = new ItemSlot[SlotCount];

//             for (int i = 0; i < SlotCount; i++)
//             {
//                 slots[i] = new ItemSlot(this);

//                 if (oldSlots != null && i < oldSlots.Length)
//                 {
//                     slots[i].Itemstack = oldSlots[i].Itemstack;
//                 }
//             }
//         }
//     }

//     public override void ToTreeAttributes(ITreeAttribute tree)
//     {
//         SlotsToTreeAttributes(slots, tree);
//     }
// }