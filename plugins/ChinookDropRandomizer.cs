using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Chinook Drop Randomizer", "shinnova/Arainrr", "1.4.1")]
    [Description("Make the chinook drop location more random")]
    public class ChinookDropRandomizer : RustPlugin
    {
        private Dictionary<string, Vector3> monumentPosition = new Dictionary<string, Vector3>();
        private List<CH47HelicopterAIController> blockedChinooks = new List<CH47HelicopterAIController>();

        private readonly Dictionary<string, float> monumentSizes = new Dictionary<string, float>
        {
            ["AbandonedCabins"] = 50f,
            ["Airfield"] = 200f,
            ["BanditCamp"] = 80f,
            ["Compound"] = 115f,
            ["Dome"] = 65f,
            ["GasStation"] = 40f,
            ["GasStation1"] = 40f,
            ["Harbor1"] = 125f,
            ["Harbor2"] = 125f,
            ["Junkyard"] = 150f,
            ["Launchsite"] = 265f,
            ["Lighthouse"] = 40f,
            ["Lighthouse1"] = 40f,
            ["Lighthouse2"] = 40f,
            ["MilitaryTunnel"] = 120f,
            ["MiningOutpost"] = 40f,
            ["MiningOutpost1"] = 40f,
            ["MiningOutpost2"] = 40f,
            ["PowerPlant"] = 150f,
            ["QuarryHQM"] = 30f,
            ["QuarryStone"] = 30f,
            ["QuarrySulfur"] = 30f,
            ["Satellite"] = 95f,
            ["SewerBranch"] = 80f,
            ["SuperMarket"] = 30f,
            ["SuperMarket1"] = 30f,
            ["Trainyard"] = 130f,
            ["WaterTreatment"] = 190f,
            ["Excavator"] = 180f,
        };

        private void OnServerInitialized()
        {
            if (configData.checkMonument) FindMonuments();
            UnityEngine.Object.FindObjectsOfType<CH47HelicopterAIController>().ToList().ForEach(x => OnEntitySpawned(x));
        }

        private void OnEntitySpawned(CH47HelicopterAIController chinook)
        {
            if (chinook == null) return;
            if (chinook.landingTarget != Vector3.zero) return;
            timer.Once(configData.spawnedDelay, () => TryDropCrate(chinook));
        }

        private object CanHelicopterDropCrate(CH47HelicopterAIController chinook)
        {
            if (configData.blockFP && blockedChinooks.Contains(chinook)) return false;
            return null;
        }

        private void FindMonuments()
        {
            int miningoutpost = 0;
            int lighthouse = 0;
            int gasstation = 0;
            int supermarket = 0;

            foreach (var monumentInfo in UnityEngine.Object.FindObjectsOfType<MonumentInfo>())
            {
                var pos = monumentInfo.transform.position;
                if (monumentInfo.name.Contains("swamp_c"))
                {
                    monumentPosition.Add("AbandonedCabins", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("airfield_1"))
                {
                    monumentPosition.Add("Airfield", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("bandit_town"))
                {
                    monumentPosition.Add("BanditCamp", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("compound"))
                {
                    monumentPosition.Add("Compound", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("sphere_tank"))
                {
                    monumentPosition.Add("Dome", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("gas_station_1") && gasstation == 0)
                {
                    monumentPosition.Add("GasStation", pos);
                    gasstation++;
                    continue;
                }
                if (monumentInfo.name.Contains("gas_station_1") && gasstation == 1)
                {
                    monumentPosition.Add("GasStation1", pos);
                    gasstation++;
                    continue;
                }
                if (monumentInfo.name.Contains("harbor_1"))
                {
                    monumentPosition.Add("Harbor1", pos);
                    continue;
                }

                if (monumentInfo.name.Contains("harbor_2"))
                {
                    monumentPosition.Add("Harbor2", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("junkyard"))
                {
                    monumentPosition.Add("Junkyard", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("launch_site"))
                {
                    monumentPosition.Add("Launchsite", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("lighthouse") && lighthouse == 0)
                {
                    monumentPosition.Add("Lighthouse", pos);
                    lighthouse++;
                    continue;
                }

                if (monumentInfo.name.Contains("lighthouse") && lighthouse == 1)
                {
                    monumentPosition.Add("Lighthouse1", pos);
                    lighthouse++;
                    continue;
                }

                if (monumentInfo.name.Contains("lighthouse") && lighthouse == 2)
                {
                    monumentPosition.Add("Lighthouse2", pos);
                    lighthouse++;
                    continue;
                }

                if (monumentInfo.name.Contains("military_tunnel_1"))
                {
                    monumentPosition.Add("MilitaryTunnel", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("powerplant_1"))
                {
                    monumentPosition.Add("PowerPlant", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("mining_quarry_c"))
                {
                    monumentPosition.Add("QuarryHQM", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("mining_quarry_b"))
                {
                    monumentPosition.Add("QuarryStone", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("mining_quarry_a"))
                {
                    monumentPosition.Add("QuarrySulfur", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("radtown_small_3"))
                {
                    monumentPosition.Add("SewerBranch", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("satellite_dish"))
                {
                    monumentPosition.Add("Satellite", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("supermarket_1") && supermarket == 0)
                {
                    monumentPosition.Add("SuperMarket", pos);
                    supermarket++;
                    continue;
                }

                if (monumentInfo.name.Contains("supermarket_1") && supermarket == 1)
                {
                    monumentPosition.Add("SuperMarket1", pos);
                    supermarket++;
                    continue;
                }
                if (monumentInfo.name.Contains("trainyard_1"))
                {
                    monumentPosition.Add("Trainyard", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("warehouse") && miningoutpost == 0)
                {
                    monumentPosition.Add("MiningOutpost", pos);
                    miningoutpost++;
                    continue;
                }

                if (monumentInfo.name.Contains("warehouse") && miningoutpost == 1)
                {
                    monumentPosition.Add("MiningOutpost1", pos);
                    miningoutpost++;
                    continue;
                }

                if (monumentInfo.name.Contains("warehouse") && miningoutpost == 2)
                {
                    monumentPosition.Add("MiningOutpost2", pos);
                    miningoutpost++;
                    continue;
                }
                if (monumentInfo.name.Contains("water_treatment_plant_1"))
                {
                    monumentPosition.Add("WaterTreatment", pos);
                    continue;
                }
                if (monumentInfo.name.Contains("excavator_1"))
                {
                    monumentPosition.Add("Excavator", pos);
                    continue;
                }
            }
        }

        private Vector3 CheckDown(Vector3 sourcePos)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(sourcePos, Vector3.down, out hitInfo, 500f, LayerMask.GetMask("Terrain", "World", "Construction", "Deployed")))
                sourcePos.y = hitInfo.point.y;
            return sourcePos;
        }

        private bool AboveWater(Vector3 Position)
        {
            if (CheckDown(Position).y > 0)
                return false;
            return true;
        }

        private bool AboveMonument(Vector3 location)
        {
            Vector3 collision = CheckDown(location);
            foreach (var entry in monumentPosition)
            {
                var monumentName = entry.Key;
                if (configData.monumentsToCheck[monumentName] && monumentSizes.ContainsKey(monumentName))
                {
                    var monumentVector = entry.Value;
                    float realdistance = monumentSizes[monumentName];
                    monumentVector.y = collision.y;
                    float dist = Vector3.Distance(collision, monumentVector);
                    if (dist < realdistance)
                        return true;
                }
            }
            return false;
        }

        private void TryDropCrate(CH47HelicopterAIController chinook)
        {
            float randomtime = UnityEngine.Random.Range(configData.minTime, configData.maxTime);
            timer.Once(randomtime, () =>
            {
                if (chinook.IsDestroyed || chinook == null) return;
                if (chinook.CanDropCrate() || blockedChinooks.Contains(chinook))
                {
                    if ((configData.checkWater && !AboveWater(chinook.transform.position)) || !configData.checkWater)
                    {
                        if ((configData.checkMonument && !AboveMonument(chinook.transform.position)) || !configData.checkMonument)
                        {
                            if (BasePlayer.activePlayerList.Count >= configData.minPlayers)
                                chinook.DropCrate();
                            if (chinook.numCrates == 0)
                            {
                                if (blockedChinooks.Contains(chinook))
                                    blockedChinooks.Remove(chinook);
                                return;
                            }
                        }
                    }
                    TryDropCrate(chinook);
                }
            });
        }

        #region ConfigurationFile

        private ConfigData configData;

        public class ConfigData
        {
            [JsonProperty(PropertyName = "Prevent the game from handling chinook drops")]
            public bool blockFP;

            [JsonProperty(PropertyName = "Start trying to drop delay (in seconds)")]
            public float spawnedDelay;

            [JsonProperty(PropertyName = "Minimum time until drop (in seconds)")]
            public float minTime;

            [JsonProperty(PropertyName = "Maximum time until drop (in seconds)")]
            public float maxTime;

            [JsonProperty(PropertyName = "Minimal players to drop")]
            public int minPlayers;

            [JsonProperty(PropertyName = "Don't drop above water")]
            public bool checkWater;

            [JsonProperty(PropertyName = "Don't drop above monuments")]
            public bool checkMonument;

            [JsonProperty(PropertyName = "What monuments to check (only works if monument checking is enabled)")]
            public Dictionary<string, bool> monumentsToCheck = new Dictionary<string, bool>();

            public static ConfigData DefaultConfig()
            {
                return new ConfigData()
                {
                    blockFP = false,
                    spawnedDelay = 300f,
                    minTime = 30f,
                    maxTime = 300f,
                    minPlayers = 0,
                    checkWater = true,
                    checkMonument = false,
                    monumentsToCheck = new Dictionary<string, bool>
                    {
                        ["AbandonedCabins"] = true,
                        ["Airfield"] = true,
                        ["BanditCamp"] = true,
                        ["Compound"] = true,
                        ["Dome"] = true,
                        ["GasStation"] = true,
                        ["GasStation1"] = true,
                        ["Harbor1"] = true,
                        ["Harbor2"] = true,
                        ["Junkyard"] = true,
                        ["Launchsite"] = true,
                        ["Lighthouse"] = true,
                        ["Lighthouse1"] = true,
                        ["Lighthouse2"] = true,
                        ["MilitaryTunnel"] = true,
                        ["MiningOutpost"] = true,
                        ["MiningOutpost1"] = true,
                        ["MiningOutpost2"] = true,
                        ["PowerPlant"] = true,
                        ["QuarryHQM"] = true,
                        ["QuarryStone"] = true,
                        ["QuarrySulfur"] = true,
                        ["Satellite"] = true,
                        ["SewerBranch"] = true,
                        ["SuperMarket"] = true,
                        ["SuperMarket1"] = true,
                        ["Trainyard"] = true,
                        ["WaterTreatment"] = true,
                        ["Excavator"] = true,
                    },
                };
            }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                configData = Config.ReadObject<ConfigData>();
                if (configData == null)
                    LoadDefaultConfig();
            }
            catch
            {
                PrintError("Config has corrupted or incorrectly formatted");
                LoadDefaultConfig();
            }
            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file");
            configData = ConfigData.DefaultConfig();
        }

        protected override void SaveConfig() => Config.WriteObject(configData);

        #endregion ConfigurationFile
    }
}