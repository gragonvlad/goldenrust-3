using Newtonsoft.Json;
using System.Collections.Generic;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Instant Craft", "Orange", "2.1.1")]
    [Description("Allows players to instantly craft items with features")]
    public class InstantCraft : RustPlugin
    {
        #region Vars

        private const string permUse = "instantcraft.use";

        #endregion
        
        #region Oxide Hooks

        private void Init()
        {
            permission.RegisterPermission(permUse, this);
        }
        
        private object OnItemCraft(ItemCraftTask item)
        {
            return OnCraft(item);
        }

        #endregion

        #region Core

        private object OnCraft(ItemCraftTask task)
        {
            var player = task.owner;
            var target = task.blueprint.targetItem;
            var name = target.shortname;
            
            if (permission.UserHasPermission(player.UserIDString, permUse) == false)
            {
                return null;
            }

            if (IsBlocked(name))
            {
                task.cancelled = true;
                Message(player, "Blocked");
                GiveRefund(player, task.takenItems);
                return null;
            }

            var stacks = GetStacks(target, task.amount * task.blueprint.amountToCreate);
            var slots = FreeSlots(player);

            if (HasPlace(slots, stacks) == false)
            {
                task.cancelled = true;
                Message(player, "Slots", stacks.Count, slots);
                GiveRefund(player, task.takenItems);
                return null;
            }
            
            if (IsNormalItem(name))
            {
                Message(player, "Normal");
                return null;
            }
            
            GiveItem(player, task, target, stacks, task.skinID);
            task.cancelled = true;
            return null;
        }

        private void GiveItem(BasePlayer player, ItemCraftTask task, ItemDefinition def, List<int> stacks, int taskSkinID)
        {
            var skin = ItemDefinition.FindSkin(def.itemid, taskSkinID);
            
            if (config.split == false)
            {
                var final = 0;

                foreach (var stack in stacks)
                {
                    final += stack;
                }
                
                var item = ItemManager.Create(def, final, skin);
                player.GiveItem(item);
                Interface.CallHook("OnItemCraftFinished", task, item);
            }
            else
            {
                foreach (var stack in stacks)
                {
                    var item = ItemManager.Create(def, stack, skin);
                    player.GiveItem(item);
                    Interface.CallHook("OnItemCraftFinished", task, item);
                }
            }
        }

        private int FreeSlots(BasePlayer player)
        {
            var slots = player.inventory.containerMain.capacity + player.inventory.containerBelt.capacity;
            var taken = player.inventory.containerMain.itemList.Count + player.inventory.containerBelt.itemList.Count;
            return slots - taken;
        }

        private void GiveRefund(BasePlayer player, List<Item> items)
        {
            foreach (var item in items)
            {
                player.GiveItem(item);
            }
        }

        private List<int> GetStacks(ItemDefinition item, int amount) 
        {
            var list = new List<int>();
            var maxStack = item.stackable;

            while (amount > maxStack)
            {
                amount -= maxStack;
                list.Add(maxStack);
            }
            
            list.Add(amount);
            
            return list; 
        }

        private bool IsNormalItem(string name)
        {
            return config.normal?.Contains(name) ?? false;
        }

        private bool IsBlocked(string name)
        {
            return config.blocked?.Contains(name) ?? false;
        }

        private bool HasPlace(int slots, List<int> stacks)
        {
            if (config.checkPlace == false)
            {
                return true;
            }

            if (config.split && slots - stacks.Count < 0)
            {
                return false;
            }

            return slots > 0;
        }

        #endregion

        #region Localization 1.1.1
        
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                {"Blocked", "Crafting of that item is blocked!"},
                {"Slots", "You don't have enough place to craft! Need {0}, have {1}!"},
                {"Normal", "Item will be crafted with normal speed."}
            }, this);
        }

        private void Message(BasePlayer player, string messageKey, params object[] args)
        {
            if (player == null)
            {
                return;
            }

            var message = GetMessage(messageKey, player.UserIDString, args);
            player.SendConsoleCommand("chat.add", (object) 0, (object) message);
        }

        private string GetMessage(string messageKey, string playerID, params object[] args)
        {
            return string.Format(lang.GetMessage(messageKey, this, playerID), args);
        }

        #endregion
        
        #region Configuration 1.1.0

        private static ConfigData config;

        private class ConfigData
        {
            [JsonProperty(PropertyName = "Check for free place")]
            public bool checkPlace;
            
            [JsonProperty(PropertyName = "Normal Speed")]
            public List<string> normal;

            [JsonProperty(PropertyName = "Blacklist")]
            public List<string> blocked;
            
            [JsonProperty(PropertyName = "Split crafted stacks")]
            public bool split;
        }

        private ConfigData GetDefaultConfig()
        {
            return new ConfigData
            {
                normal = new List<string>
                {
                    "hammer",
                    "put item shortname here"
                },
                blocked = new List<string>
                {
                    "rock",
                    "put item shortname here"
                },
                checkPlace = false,
                split = false
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                config = Config.ReadObject<ConfigData>();

                if (config == null)
                {
                    LoadDefaultConfig();
                }
            }
            catch
            {
                PrintError("Configuration file is corrupt! Unloading plugin...");
                Interface.Oxide.RootPluginManager.RemovePlugin(this);
                return;
            }

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            config = GetDefaultConfig();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(config);
        }

        #endregion
    }
}