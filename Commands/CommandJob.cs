using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace SimplePoliceJail.Commands
{
    public class CommandJob : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "job";
        public string Help => "Perform an inmate job";
        public string Syntax => "/job";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            SimplePoliceJail.Instance.TryPerformJob(player);
        }
    }
}
