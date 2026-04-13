using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace SimplePoliceJail.Commands
{
    public class CommandImpound : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "impound";
        public string Help => "Impound the vehicle you're sitting in";
        public string Syntax => "/impound";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "policejail.police" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            SimplePoliceJail.Instance.ImpoundVehicle(player);
        }
    }
}
