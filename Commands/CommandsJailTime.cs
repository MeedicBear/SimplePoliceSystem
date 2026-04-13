using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePoliceJail.Commands
{
    public class CommandJailTime : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "jailtime";
        public string Help => "Check remaining jail time";
        public string Syntax => "/jailtime";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (!SimplePoliceJail.Instance.IsJailed(player))
            {
                UnturnedChat.Say(player, "You are not jailed.");
                return;
            }

            float remaining = SimplePoliceJail.Instance.GetRemainingJailTime(player);
            UnturnedChat.Say(player, $"Time remaining: {Mathf.CeilToInt(remaining)} seconds.");
        }
    }
}
