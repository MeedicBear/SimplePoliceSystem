using System.Collections.Generic;

namespace SimplePoliceJail.JobSystem
{
    public class InmateJobDefinition
    {
        public string JobName;
        public string JobType; // ZONE, ITEM, DELIVERY

        public float TimeReduction;
        public int RewardXP;
        public int RewardMoney;
        public float CooldownSeconds;

        public List<JobZone> Zones;

        public ushort RequiredItemID;

        public JobZone DeliveryZone;
    }
}
