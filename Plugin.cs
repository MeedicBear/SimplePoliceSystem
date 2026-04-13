using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using SimplePoliceJail.JobSystem;

namespace SimplePoliceJail
{
    public class JailConfig : IRocketPluginConfiguration
    {
        public Vector3 JailPosition;
        public float JailYaw;
        public Vector3 ReleasePosition;
        public float ReleaseYaw;

        public bool FreezeMovement;

        public int ImpoundFee;
        public Vector3 ImpoundLocation;
        public float ImpoundYaw;

        public List<InmateJobDefinition> Jobs;

        public void LoadDefaults()
        {
            JailPosition = new Vector3(0, 0, 0);
            JailYaw = 0;
            ReleasePosition = new Vector3(10, 0, 10);
            ReleaseYaw = 180;

            FreezeMovement = true;

            ImpoundFee = 250;
            ImpoundLocation = new Vector3(20, 0, 20);
            ImpoundYaw = 0;

            Jobs = new List<InmateJobDefinition>()
            {
                new InmateJobDefinition()
                {
                    JobName = "TrashPickup",
                    JobType = "ZONE",
                    TimeReduction = 5f,
                    RewardXP = 10,
                    RewardMoney = 25,
                    CooldownSeconds = 10f,
                    Zones = new List<JobZone>()
                    {
                        new JobZone()
                        {
                            Name = "TrashArea",
                            Position = new Vector3(5,0,5),
                            Radius = 4f
                        }
                    }
                }
            };
        }
    }

    public class ImpoundedVehicle
    {
        public ushort VehicleId;
        public ulong OwnerId;
        public uint OriginalFuel;
    }

    public class SimplePoliceJail : RocketPlugin<JailConfig>
    {
        public static SimplePoliceJail Instance;

        private Dictionary<ulong, JailData> jailedPlayers = new Dictionary<ulong, JailData>();
        private HashSet<ulong> handcuffed = new HashSet<ulong>();
        internal Dictionary<ushort, ImpoundedVehicle> impounded = new Dictionary<ushort, ImpoundedVehicle>();

        protected override void Load()
        {
            Instance = this;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnect;
            Logger.Log("SimplePoliceJail loaded.");
        }

        protected override void Unload()
        {
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnect;
            Logger.Log("SimplePoliceJail unloaded.");
        }

        private void OnPlayerDisconnect(UnturnedPlayer player)
        {
            ulong id = player.CSteamID.m_SteamID;
            if (jailedPlayers.ContainsKey(id))
                jailedPlayers.Remove(id);
            if (handcuffed.Contains(id))
                handcuffed.Remove(id);
        }

        private void FixedUpdate()
        {
            List<ulong> toRelease = new List<ulong>();

            foreach (var entry in jailedPlayers)
            {
                entry.Value.TimeRemaining -= Time.deltaTime;
                if (entry.Value.TimeRemaining <= 0)
                    toRelease.Add(entry.Key);
            }

            foreach (ulong steamId in toRelease)
                ReleasePlayer(steamId);
        }

        public void JailPlayer(UnturnedPlayer target, int seconds)
        {
            ulong id = target.CSteamID.m_SteamID;

            if (jailedPlayers.ContainsKey(id))
                jailedPlayers.Remove(id);

            jailedPlayers.Add(id, new JailData
            {
                OriginalPosition = target.Position,
                OriginalRotation = target.Rotation,
                TimeRemaining = seconds,
                LastJobTime = 0,
                JobsCompleted = 0
            });

            target.Teleport(Configuration.Instance.JailPosition, Configuration.Instance.JailYaw);

            if (Configuration.Instance.FreezeMovement)
                target.Player.movement.sendPluginSpeedMultiplier(0);

            UnturnedChat.Say($"{target.CharacterName} has been jailed for {seconds} seconds.");
        }

        public void ReleasePlayer(ulong steamId)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(steamId));
            if (player == null) return;

            if (!jailedPlayers.ContainsKey(steamId)) return;

            player.Player.movement.sendPluginSpeedMultiplier(1f);

            player.Teleport(Configuration.Instance.ReleasePosition, Configuration.Instance.ReleaseYaw);

            jailedPlayers.Remove(steamId);

            UnturnedChat.Say($"{player.CharacterName} has been released from jail.");
        }

        public bool IsJailed(UnturnedPlayer player)
        {
            return jailedPlayers.ContainsKey(player.CSteamID.m_SteamID);
        }

        public float GetRemainingJailTime(UnturnedPlayer player)
        {
            if (!IsJailed(player)) return 0;
            return jailedPlayers[player.CSteamID.m_SteamID].TimeRemaining;
        }

        public void TryPerformJob(UnturnedPlayer player)
        {
            ulong id = player.CSteamID.m_SteamID;

            if (!jailedPlayers.ContainsKey(id))
            {
                UnturnedChat.Say(player, "You are not jailed.");
                return;
            }

            var data = jailedPlayers[id];

            foreach (var job in Configuration.Instance.Jobs)
            {
                if (JobEngine.TryPerformJob(player, data, job))
                    return;
            }

            UnturnedChat.Say(player, "No job available here.");
        }

        public void CuffPlayer(UnturnedPlayer target)
        {
            ulong id = target.CSteamID.m_SteamID;

            if (!handcuffed.Contains(id))
                handcuffed.Add(id);

            target.Player.movement.sendPluginSpeedMultiplier(0.1f);
            target.Player.equipment.dequip();

            UnturnedChat.Say($"{target.CharacterName} has been handcuffed.");
        }

        public void UncuffPlayer(UnturnedPlayer target)
        {
            ulong id = target.CSteamID.m_SteamID;

            if (handcuffed.Contains(id))
                handcuffed.Remove(id);

            target.Player.movement.sendPluginSpeedMultiplier(1f);

            UnturnedChat.Say($"{target.CharacterName} has been uncuffed.");
        }

        public bool IsCuffed(UnturnedPlayer player)
        {
            return handcuffed.Contains(player.CSteamID.m_SteamID);
        }

        public void ImpoundVehicle(UnturnedPlayer officer)
        {
            if (officer.CurrentVehicle == null)
            {
                UnturnedChat.Say(officer, "You are not in a vehicle.");
                return;
            }

            InteractableVehicle vehicle = officer.CurrentVehicle;
            ushort id = vehicle.instanceID;

            if (impounded.ContainsKey(id))
            {
                UnturnedChat.Say(officer, "This vehicle is already impounded.");
                return;
            }

            ulong owner = vehicle.lockedOwner.m_SteamID;

            impounded.Add(id, new ImpoundedVehicle()
            {
                VehicleId = id,
                OwnerId = owner,
                OriginalFuel = vehicle.fuel
            });

            vehicle.transform.position = Configuration.Instance.ImpoundLocation;
            vehicle.transform.rotation = Quaternion.Euler(0, Configuration.Instance.ImpoundYaw, 0);

            UnturnedChat.Say($"Vehicle impounded. Owner must pay ${Configuration.Instance.ImpoundFee} to reclaim.");
        }

        public bool TryReclaimVehicle(UnturnedPlayer player)
        {
            foreach (var entry in new List<ImpoundedVehicle>(impounded.Values))
            {
                if (entry.OwnerId == player.CSteamID.m_SteamID)
                {
                    if (Rocket.Unturned.U.Economy == null)
                    {
                        UnturnedChat.Say(player, "Economy is not available.");
                        return false;
                    }

                    var balance = Rocket.Unturned.U.Economy.GetBalance(player.CSteamID);
                    if (balance < Configuration.Instance.ImpoundFee)
                    {
                        UnturnedChat.Say(player, "You cannot afford the impound fee.");
                        return false;
                    }

                    Rocket.Unturned.U.Economy.DecreaseBalance(player.CSteamID, Configuration.Instance.ImpoundFee);

                    InteractableVehicle vehicle = VehicleManager.getVehicle(entry.VehicleId);
                    if (vehicle != null)
                    {
                        vehicle.transform.position = player.Position + player.Player.transform.forward * 5f;
                        vehicle.fuel = entry.OriginalFuel;
                    }

                    impounded.Remove(entry.VehicleId);

                    UnturnedChat.Say(player, "Your vehicle has been reclaimed.");
                    return true;
                }
            }

            UnturnedChat.Say(player, "You have no impounded vehicles.");
            return false;
        }

        public override TranslationList DefaultTranslations => new TranslationList();
    }
}
