using RestoreMonarchy.PlayerStats.Models;
using Rocket.API;
using System;

namespace RestoreMonarchy.PlayerStats
{
    public class PlayerStatsConfiguration : IRocketPluginConfiguration
    {
        public string MessageColor { get; set; }
        public string MessageIconUrl { get; set; }

        // Database Settings
        public string DatabaseProvider { get; set; }
        public string JsonFilePath { get; set; }
        public string MySQLConnectionString { get; set; }
        public string PlayerStatsTableName { get; set; }
        public float SaveIntervalSeconds { get; set; }

        // UI Settings
        public bool EnableUIEffect { get; set; }
        public ushort UIEffectId { get; set; }
        public bool ShowUIEffectByDefault { get; set; }

        // Feature Toggles
        public bool EnableJoinMessages { get; set; }
        public bool EnableLeaveMessages { get; set; }
        public bool DisableAdminJoinMessages { get; set; }
        public bool DisableAdminLeaveMessages { get; set; }

        // Legacy Stats Settings
        public bool EnablePVPStats { get; set; }
        public bool EnablePVEStats { get; set; }
        public bool PVPRanking { get; set; }
        public bool PVPRewards { get; set; }
        public bool PVPUI { get; set; }

        // New Stats Settings
        public string StatsMode { get; set; }
        public bool ShowCombinedDeaths { get; set; } = false;

        // Ranking & Rewards
        public int MinimumRankingTreshold { get; set; }
        public bool EnableRewards { get; set; }
        public Reward[] Rewards { get; set; }
        public bool EnableAutomaticBans { get; set; } = false;
        public AutomaticBan[] AutomaticBans { get; set; } = 
        [
            new AutomaticBan
            {
                Reason = "Cheating (AB)",
                Conditions =
                [
                    new AutomaticBanCondition
                    {
                        Comparer = "greater",
                        Stat = nameof(PlayerStatsData.Kills),
                        Value = 30
                    },
                    new AutomaticBanCondition
                    {
                        Comparer = "greater",
                        Stat = "Accuracy",
                        Value = 80
                    },
                    new AutomaticBanCondition
                    {
                        Comparer = "less",
                        Stat = nameof(PlayerStatsData.Playtime),
                        Value = 3600
                    }
                ]
            },
        ];

        // Only hide legacy properties when using new StatsMode
        public bool ShouldSerializeEnablePVPStats() => StatsMode == null;
        public bool ShouldSerializeEnablePVEStats() => StatsMode == null;
        public bool ShouldSerializePVPRanking() => StatsMode == null;
        public bool ShouldSerializePVPRewards() => StatsMode == null;
        public bool ShouldSerializePVPUI() => StatsMode == null;

        internal StatsMode? actualStatsMode;

        public StatsMode ActualStatsMode
        {
            get
            {
                if (actualStatsMode.HasValue)
                {
                    return actualStatsMode.Value;
                }
                
                
                if (!string.IsNullOrEmpty(StatsMode))
                {
                    actualStatsMode = (StatsMode)Enum.Parse(typeof(StatsMode), StatsMode, true);
                } else
                {
                    if (!EnablePVPStats && !EnablePVEStats)
                    {
                        actualStatsMode = Models.StatsMode.Both;
                    } else if (EnablePVPStats && !EnablePVEStats)
                    {
                        actualStatsMode = Models.StatsMode.PVP;
                    }
                    else if (!EnablePVPStats && EnablePVEStats)
                    {
                        actualStatsMode = Models.StatsMode.PVE;
                    }
                    else
                    {
                        actualStatsMode = Models.StatsMode.Both;
                    }
                }                

                return actualStatsMode.Value;
            }
        }

        public void LoadDefaults()
        {
            MessageColor = "#5E794D";
            MessageIconUrl = "";

            DatabaseProvider = "mysql"; 
            JsonFilePath = "{rocket_directory}/Plugins/PlayerStats/PlayerStats.json";
            MySQLConnectionString = "Server=127.0.0.1;Port=3306;Database=unturned;Uid=root;Pwd=passw;";
            PlayerStatsTableName = "PlayerStats";
            SaveIntervalSeconds = 300;

            EnableUIEffect = false;
            UIEffectId = 22512;
            ShowUIEffectByDefault = true;

            EnableJoinMessages = true;
            EnableLeaveMessages = true;
            DisableAdminJoinMessages = true;
            DisableAdminLeaveMessages = true;

            // Legacy defaults
            EnablePVPStats = true;
            EnablePVEStats = true;
            PVPRanking = true;
            PVPRewards = true;
            PVPUI = true;

            // New system defaults to null to use legacy system
            StatsMode = Models.StatsMode.Both.ToString();
            ShowCombinedDeaths = true;

            MinimumRankingTreshold = 5;
            EnableRewards = false;
            Rewards = [
                new Reward
                {
                    Name = "VIP Rank",
                    PermissionGroup = "vip",
                    Treshold = 50
                },
                new Reward
                {
                    Name = "MVP Rank",
                    PermissionGroup = "mvp",
                    Treshold = 125
                }
            ];
        }
    }
}