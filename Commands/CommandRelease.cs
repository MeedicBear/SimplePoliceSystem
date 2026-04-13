using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace SimplePoliceJail.Commands
{
    public class CommandRelease : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "release";
        public string Help => "Release a jailed player";
        public string Syntax => "/release <player>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "policejail.police" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length < 1)
            {
                UnturnedChat.Say(caller, "Usage: /release <player>");
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(args[0]);
            if (target == null)
            {
                UnturnedChat.Say(caller, "Player not found.");
                return;
            }

            SimplePoliceJail.Instance.ReleasePlayer(target.CSteamID.m_SteamID);
        }
    }
}
