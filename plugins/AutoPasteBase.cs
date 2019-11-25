using Facepunch;
using Oxide.Core.Libraries.Covalence;
using System.Collections.Generic;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Auto paste base", "Wasdik", "0.1.0")]
    [Description("Auto paste base")] 
    class AutoPasteBase : RustPlugin
    {
		[PluginReference] private Plugin CopyPaste;
		
        private void Init()
        {
			// Server.Broadcast
        }
		
		private object PasteBase(string buildingName, Vector3 pos, float height)
		{
			
			var options = new List<string>{ "blockcollision", "1", "auth", "false", "height", height.ToString()};
			Puts("-----buildingName-----: "+buildingName);
			Puts("-----pos-----: "+pos.ToString());
			Puts("-----height-----: "+height.ToString());
			var success = CopyPaste.Call("TryPasteFromVector3", pos, 0f, buildingName, options.ToArray());
			return success;
		}

		
		private void PasteAllBases(BasePlayer player)
		{
			// Base #1 
			var buildingName = "home102";
			var pos = new Vector3(-1324f, 0f, -1431f);
			var height = 2;
			
			var success = PasteBase(buildingName, pos, height);

			if(success is string)
			{
				SendReply(player, "Can't place the building here: "+success);
				Puts("Can't place the building here: "+success);
			}
			else
			{
				SendReply(player, "You've successfully bought this building");
				Puts("You've successfully bought this building");
			}
			
			// Base #2 
			buildingName = "home200";
			pos = new Vector3(900f, 0f, 80);
			height = 1;
			
			//success = PasteBase(buildingName, pos, height);

			if(success is string)
			{
				//SendReply(player, "Can't place the building here: "+success);
				//Puts("Can't place the building here: "+success);
			}
			else
			{
				//SendReply(player, "You've successfully bought this building");
				//Puts("You've successfully bought this building");
			}
		}
		
		
		[ChatCommand("autopaste")]
		private void TestUpdateCommand(BasePlayer player, string command, string[] args)
		{
			//PasteAllBases(player);
		}
		
		
		[ConsoleCommand("autopaste")]
        private void cmdConsoleAutoPaste(ConsoleSystem.Arg arg)
        {
            PasteAllBases(arg.Player());
        }
		
    }
}