using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace SimplePoliceJail.Commands
{
    public class CommandUncuff : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "uncuff";
        public string Help => "Uncuff a player";
        public string Syntax => "/uncuff <player>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "policejail.police" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length < 1)
            {
                UnturnedChat.Say(caller, "Usage: /uncuff <player>");
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(args[0]);
            if (target == null)
            {
                UnturnedChat.Say(caller, "Player not found.");
                return;
            }

            SimplePoliceJail.Instance.UncuffPlayer(target);
        }
    }
}
