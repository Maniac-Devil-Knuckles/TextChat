using System;
using Qurre;
using Qurre.API.Events;
using System.Collections.Generic;

using Player = Qurre.API.Player;

namespace TextChat
{
    public class TextChat : Plugin
    {
        public override string Developer => "KoToXleB#4663";
        public override string Name => "TextChat";
        public override Version Version => new Version(1, 0, 0);
        public override int Priority => int.MinValue;
        public override void Enable()
        {
            Qurre.Events.Server.SendingConsole += OnSendConsole;
            Qurre.Events.Round.Start += OnRoundStart;
        }
        public override void Disable()
        {
            Qurre.Events.Server.SendingConsole -= OnSendConsole;
            Qurre.Events.Round.Start -= OnRoundStart;
        }
        public void OnRoundStart()
        {

            foreach (Player player in Player.List)
            {
                player.SendConsoleMessage("Вы можете отправлять сообщения через наш текстовый чат на кнопку [Ё].\n Разговаривайте друг с другом!\nПример: .chat Ку!", "red");
            }
        }
        public void OnSendConsole(SendingConsoleEvent ev)
        {
         if(ev.Name.ToLower() == "chat" || ev.Name.ToLower() == "чат") foreach (Player player in Player.List)
            {
                if (!player.IsHost)
                {
                    player.SendConsoleMessage($"{ev.Player.Nickname}: {string.Join(' ', ev.Args)}", $"{Color[UnityEngine.Random.Range(0, Color.Count)]}");
                    ev.Allowed = false;
                }
            }
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
