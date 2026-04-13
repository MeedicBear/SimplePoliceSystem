using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace SimplePoliceJail.Commands
{
    public class CommandReclaim : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "reclaim";
        public string Help => "Reclaim your impounded vehicle";
        public string Syntax => "/reclaim";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            SimplePoliceJail.Instance.TryReclaimVehicle(player);
        }
    }
}
