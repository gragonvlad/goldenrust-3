using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Plugins;
using static ConVar.Admin;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("RaidAlerts", "birthdates", "0.1.5", ResourceId = 0)]
    [Description("you can spot a raid without even moving!")]
    class RaidAlerts : RustPlugin
    {

        public List<BasePlayer> admins = new List<BasePlayer>();
        public List<BasePlayer> toggledo = new List<BasePlayer>();

        void OnRocketLaunched(BasePlayer player, BaseEntity entity)
        {
            if(!player.IsAdmin)
            {
                foreach (BasePlayer b in admins)
                    if (!b.IsAdmin)
                    {
                        admins.Remove(b);
                    }
                    else
                    {
                        if (!toggledo.Contains(b))
                        {
                            SendReply(b, "<color=#add8e6ff>(RAID ALERTS) </color><color=#a52a2aff>" + player.displayName + "</color> might be <color=#ffa500ff>RAIDING!</color> (Rocket fired)");
                        }
                    }
                {
                }
            }
        }

        void OnExplosiveThrown(BasePlayer player, BaseEntity entity)
        {
            if (!player.IsAdmin)
            {
                foreach (BasePlayer b in admins)
                    if (!b.IsAdmin)
                    {
                        admins.Remove(b);
                    }
                    else
                    {
                        if (!toggledo.Contains(b))
                        {
                            SendReply(b, "<color=#add8e6ff>(RAID ALERTS) </color><color=#a52a2aff>" + player.displayName + "</color> might be <color=#ffa500ff>RAIDING!</color> (C4 Thrown)");
                        }
                    }
                {
                }
            }
        }

        [ChatCommand("raidalerts")]
        public void raidalerts(IPlayer player, string command, string[] args)
        {
            BasePlayer b = player as BasePlayer;
                if (!toggledo.Contains(b))
            {
                SendReply(b, "You have toggled raid alerts off.");
                toggledo.Add(b);
                return;
            }
            else
            {
                SendReply(b, "You have toggled raid alerts on.");
                toggledo.Remove(b);
            }
        }

        void OnPlayerInit(BasePlayer player)
        {
            if (!player.IsAdmin && admins.Contains(player))
            {
                admins.Remove(player);
            }
            if (player.IsAdmin)
            {
                admins.Add(player);
            }
        }


    }
}