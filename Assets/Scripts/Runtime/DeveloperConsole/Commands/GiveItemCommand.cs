using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "GiveItemCommand", menuName = "Console Commands/Player/GiveItem")]
    internal sealed class GiveItemCommand : PlayerCommand
    {
        private void TryToGetItemByID(string input, out Item item)
        {
            item = null;
            if (!int.TryParse(input, out int id)) return;
            item = PaperSoulsGameManager.ItemDatabase.GetItem(id);
        }

        private void TryToGetItemByName(string input, out Item item)
        {
            item = null;
            item = PaperSoulsGameManager.ItemDatabase.GetItem(input);
        }

        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            if (args.Length <= 1)
            {
                msg = new($"Need to specify an item ID or name and an amount ex. '{Command} HealthPotion 10' or '{Command} 1 10'.", ResponseType.Warning);
                return false;
            }

            string arg = args[0];
            string amountArg = args[1];

            if (!int.TryParse(amountArg, out int amount))
            {
                msg = new($"'{amountArg}' is an invaild amount.", ResponseType.Error);
                return false;
            }

            TryToGetItemByID(arg, out Item item);
            if (item == null) TryToGetItemByName(arg, out item);

            if (item == null)
            {
                msg = new($"'{arg}' is an invaild Item Name or ID.", ResponseType.Error);
                return false;
            }


            if (!_player.ItemInventory.InventoryManger.AddToInventory(item, amount))
            {
                msg = new($"Player's inventory is full, cannot add item.", ResponseType.Warning);
                return false;
            }

            msg = new(ResponseType.None);

            return true;
        }
    }
}
