using CommandSystem;
using System.Collections.Generic;
using Qurre.API;
using Qurre.API.Attributes;
using Qurre.Events;
using Player = Qurre.API.Player;
using System;
using System.Linq;
using Qurre.API.Addons;


namespace TextChat
{
    [PluginInit("TextChat","Maniac Devil Knuckles","1.0.0")]
    public class TextChat 
    {
        public static ChatCommand ChatCommand { get; private set; } 

        [PluginEnable]
        public void Enable()
        {
            if (!Config.Load())
            {
                Log.Warn("TextChat is disabled");
                return;
            }
            ChatCommand = new ChatCommand();
            RemoteAdmin.QueryProcessor.DotCommandHandler.RegisterCommand(ChatCommand);           
        }

        [PluginDisable]
        public void Disable()
        {
            RemoteAdmin.QueryProcessor.DotCommandHandler.UnregisterCommand(ChatCommand);
            ChatCommand = null;
        }

        [EventMethod(RoundEvents.Start)]
        public void OnRoundStart()
        {
            if (!Config.IsEnabled) return;
            foreach (Player player in Player.List)
            {
                player.Client.SendConsole(Config.SendMessageWhenRoundIsStarted, "red");
            }
        }
    }

    public static class Config
    {
        public static bool IsEnabled { get; private set; } = true;

        public static string SendMessageWhenRoundIsStarted { get; private set; } = "Вы можете отправлять сообщения через наш текстовый чат на кнопку [Ё].\n Разговаривайте друг с другом!";

        public static string NameOfCommand { get; private set; } = "chat";

        private static readonly JsonConfig jsonConfig = new JsonConfig("TextChat");

        internal static bool Load()
        {
            IsEnabled = jsonConfig.SafeGetValue("IsEnabled", IsEnabled);
            SendMessageWhenRoundIsStarted = jsonConfig.SafeGetValue("SendMessageWhenRoundIsStarted", SendMessageWhenRoundIsStarted);
            NameOfCommand = jsonConfig.SafeGetValue("NameOfCommand", NameOfCommand);
            return IsEnabled;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ChatCommand : ICommand
    {
        public string Command => Config.NameOfCommand;

        public string[] Aliases => new string[] { };

        public string Description => "Chatting with players";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "You did not write";
            Player ply = Extensions.GetPlayer(sender as CommandSender);
            if (arguments.Count == 0 || ply.IsHost|| ply == null) return false;
            if (!ply.RoleInfomation.IsAlive)
            {
                foreach (Player player in Player.List.Where(p=>!p.RoleInfomation.IsAlive))
                    if (!player.IsHost) player.Client.SendConsole($"{ply.UserInfomation.Nickname}: {string.Join(" ", arguments)}", $"{Color[UnityEngine.Random.Range(0, Color.Count)]}");
            }
            else if (ply.RoleInfomation.IsScp)
            {
                foreach (Player player in Player.List.Where(p => p.RoleInfomation.IsScp))
                    if (!player.IsHost) player.Client.SendConsole($"{ply.UserInfomation.Nickname}: {string.Join(" ", arguments)}", $"{Color[UnityEngine.Random.Range(0, Color.Count)]}");
            }
            else
            {
                foreach (Player player in Player.List.Where(p => p.DistanceTo(ply) <= 5f))
                    if (!player.IsHost) player.Client.SendConsole($"{ply.UserInfomation.Nickname}: {string.Join(" ", arguments)}", $"{Color[UnityEngine.Random.Range(0, Color.Count)]}");
            }
            response = "Sended";
            return true;
        }

        public List<string> Color = new List<string>()
        {
            "red",
            "cyan",
            "yellow",
            "magenta",
            "green",
            "white"
        };
    }

}
