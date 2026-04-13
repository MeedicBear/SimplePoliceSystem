using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace SimplePoliceJail.Commands
{
    public class CommandArrest : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "arrest";
        public string Help => "Arrest a player for X seconds";
        public string Syntax => "/arrest <player> <seconds>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "policejail.police" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length < 2)
            {
                UnturnedChat.Say(caller, "Usage: /arrest <player> <seconds>");
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(args[0]);
            if (target == null)
            {
                UnturnedChat.Say(caller, "Player not found.");
                return;
            }

            if (!int.TryParse(args[1], out int seconds))
            {
                UnturnedChat.Say(caller, "Invalid time.");
                return;
            }

            SimplePoliceJail.Instance.JailPlayer(target, seconds);
        }
    }
}
