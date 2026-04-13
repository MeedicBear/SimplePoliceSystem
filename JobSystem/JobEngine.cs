using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using UnityEngine;

namespace SimplePoliceJail.JobSystem
{
    public static class JobEngine
    {
        public static bool TryPerformJob(UnturnedPlayer player, JailData data, InmateJobDefinition job)
        {
            if (Time.realtimeSinceStartup - data.LastJobTime < job.CooldownSeconds)
                return false;

            switch (job.JobType.ToUpper())
            {
                case "ZONE":
                    return CheckZoneJob(player, data, job);

                case "ITEM":
                    return CheckItemJob(player, data, job);

                case "DELIVERY":
                    return CheckDeliveryJob(player, data, job);
            }

            return false;
        }

        private static bool CheckZoneJob(UnturnedPlayer player, JailData data, InmateJobDefinition job)
        {
            if (job.Zones == null) return false;

            foreach (var zone in job.Zones)
            {
                if (Vector3.Distance(player.Position, zone.Position) <= zone.Radius)
                {
                    CompleteJob(player, data, job);
                    return true;
                }
            }

            return false;
        }

        private static bool CheckItemJob(UnturnedPlayer player, JailData data, InmateJobDefinition job)
        {
            if (player.Player.equipment.itemID == job.RequiredItemID)
            {
                CompleteJob(player, data, job);
                return true;
            }

            return false;
        }

        private static bool CheckDeliveryJob(UnturnedPlayer player, JailData data, InmateJobDefinition job)
        {
            if (player.Player.equipment.itemID != job.RequiredItemID)
                return false;

            if (Vector3.Distance(player.Position, job.DeliveryZone.Position) <= job.DeliveryZone.Radius)
            {
                CompleteJob(player, data, job);
                return true;
            }

            return false;
        }

        private static void CompleteJob(UnturnedPlayer player, JailData data, InmateJobDefinition job)
        {
            data.LastJobTime = Time.realtimeSinceStartup;
            data.JobsCompleted++;

            data.TimeRemaining -= job.TimeReduction;
            if (data.TimeRemaining < 0)
                data.TimeRemaining = 0;

            player.Experience += (uint)job.RewardXP;

            if (job.RewardMoney > 0 && Rocket.Unturned.U.Economy != null)
                Rocket.Unturned.U.Economy.IncreaseBalance(player.CSteamID, job.RewardMoney);

            UnturnedChat.Say(player,
                $"Job '{job.JobName}' completed! -{job.TimeReduction}s | +{job.RewardXP} XP | +${job.RewardMoney}");
        }
    }
}
