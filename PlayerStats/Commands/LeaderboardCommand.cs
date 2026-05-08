using RestoreMonarchy.PlayerStats.Helpers;
using RestoreMonarchy.PlayerStats.Models;
using Rocket.API;
using System.Collections.Generic;
using System.Linq;

namespace RestoreMonarchy.PlayerStats.Commands
{
    public class LeaderboardCommand : IRocketCommand
    {
        private PlayerStatsPlugin pluginInstance => PlayerStatsPlugin.Instance;
        private PlayerStatsConfiguration configuration => pluginInstance.Configuration.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            int amount = 5;
            ThreadHelper.RunAsynchronously(() =>
            {
                List<PlayerRanking> playerRankings = pluginInstance.Database.GetPlayerRankings(amount).Where(x => !x.IsUnranked()).ToList();
                ThreadHelper.RunSynchronously(() =>
                {
                    if (playerRankings.Count == 0)
                    {
                        pluginInstance.SendMessageToPlayer(caller, "NoLeaderboardPlayersFound");
                        return;
                    }

                    if (configuration.ActualStatsMode == StatsMode.Both || configuration.ActualStatsMode == StatsMode.PVP)
                    {
                        pluginInstance.SendMessageToPlayer(caller, "LeaderboardListHeaderPVP", amount);
                        foreach (PlayerRanking playerRanking in playerRankings)
                        {
                            string rank = playerRanking.Rank.ToString();
                            string name = playerRanking.Name;
                            string kills = playerRanking.Kills.ToString("N0");
                            pluginInstance.SendMessageToPlayer(caller, "LeaderboardListItemPVP", rank, name, kills);
                        }
                    }
                    else
                    {
                        pluginInstance.SendMessageToPlayer(caller, "LeaderboardListHeaderPVE", amount);
                        foreach (PlayerRanking playerRanking in playerRankings)
                        {
                            string rank = playerRanking.Rank.ToString();
                            string name = playerRanking.Name;
                            string zombies = playerRanking.Zombies.ToString("N0");
                            pluginInstance.SendMessageToPlayer(caller, "LeaderboardListItemPVE", rank, name, zombies);
                        }
                    }
                });
            });
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "leaderboard";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => ["lb"];

        public List<string> Permissions => [];
    }
}
