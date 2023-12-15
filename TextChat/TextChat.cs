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
    [PluginInit("TextChat", "Maniac Devil Knuckles", "1.0.0")]
    public static class TextChat
    {
        public static ICommand ChatCommand { get; private set; }

        [PluginEnable]
        public static void Enable()
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
        public static void Disable()
        {
            Config.IsEnabled = false;
            RemoteAdmin.QueryProcessor.DotCommandHandler.UnregisterCommand(ChatCommand);
            ChatCommand = null;
        }

        [EventMethod(RoundEvents.Start)]
        public static void OnRoundStart()
        {
            if (!Config.IsEnabled) return;
            if (string.IsNullOrEmpty(Config.SendMessageWhenRoundIsStarted)) return;
            foreach (Player player in Player.List)
            {
                player.Client.SendConsole(Config.SendMessageWhenRoundIsStarted, "red");
            }
        }
    }


    public static class Config
    {
        public static bool IsEnabled { get; internal set; } = true;

        public static string NameofCommand { get; private set; } = "chat";

        public static string SendMessageWhenRoundIsStarted { get; internal set; } = "Вы можете отправлять сообщения через наш текстовый чат на кнопку [Ё].\n Разговаривайте друг с другом!";

        private static readonly JsonConfig jsonConfig = new JsonConfig("TextChat");

        internal static bool Load()
        {
            IsEnabled = jsonConfig.SafeGetValue("IsEnabled", IsEnabled);
            SendMessageWhenRoundIsStarted = jsonConfig.SafeGetValue("SendMessageWhenRoundIsStarted", SendMessageWhenRoundIsStarted);
            NameofCommand = jsonConfig.SafeGetValue("NameOfCommand", NameofCommand, "COmmand chat");
            JsonConfig.UpdateFile();
            return IsEnabled;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ChatCommand : ICommand, IHelpProvider
    {
        public string Command => Config.NameofCommand;

        public string[] Aliases => null;

        public string Description => "Chatting with players";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "You did not write";
            Player ply = Extensions.GetPlayer(sender as CommandSender);
            if (arguments.Count == 0 || ply.IsHost || ply == null) return false;
            else if (!ply.RoleInfomation.IsAlive)
            {
                foreach (Player player in Player.List.Where(p => !p.RoleInfomation.IsAlive))
                    if (!player.IsHost) player.Client.SendConsole($"{ply.UserInfomation.Nickname}: {string.Join(" ", arguments)}", $"{Color[UnityEngine.Random.Range(0, Color.Count)]}");
            }
            else if (ply.RoleInfomation.IsScp)
            {
                foreach (Player player in Player.List.Where(p => p.RoleInfomation.IsScp))
                    if (!player.IsHost) player.Client.SendConsole($"{ply.UserInfomation.Nickname}: {string.Join(" ", arguments)}", $"{Color[UnityEngine.Random.Range(0, Color.Count)]}");
            }
            else
            {
                foreach (Player player in Player.List.Where(p => p.DistanceTo(ply) <= 10f))
                    if (!player.IsHost) player.Client.SendConsole($"{ply.UserInfomation.Nickname}: {string.Join(" ", arguments)}", $"{Color[UnityEngine.Random.Range(0, Color.Count)]}");
            }
            response = "Sended";
            return true;
        }

        public string GetHelp(ArraySegment<string> arguments) => ".chat <message>";

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