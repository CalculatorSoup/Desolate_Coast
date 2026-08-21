using R2API;
using RoR2;
using SS2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesolateCoast
{
    public class Starstorm2Compat
    {
        public static string FindEnemyConfig(string monsterName) //Lamp, Lamp Boss, Acid Bug
        {
            var defstring = "00 - Enemy Disabling.Disable Enemy: " + monsterName;
            var monsterConfig = SS2Config.ConfigMonster.GetConfigEntries();
            foreach (var entry in monsterConfig)
            {
                if (entry.Definition.ToString() == defstring)
                {
                    var configValue = entry.GetSerializedValue();
                    return configValue;
                }
            }
            return "false";

        }

        public static void AddEnemies()
        {
            // Wayfarer
            var wayfarerValue = FindEnemyConfig("Lamp Boss");

            if (DesolateCoast.toggleWayfarer.Value && wayfarerValue == "false")
            {
                var wayfarerCard = new RoR2.DirectorCard()
                {
                    spawnCard = SS2Assets.LoadAsset<RoR2.SpawnCard>("scLampBoss", (SS2Bundle)17),
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                    selectionWeight = 1
                };

                var wayfarerHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = wayfarerCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(wayfarerHolder, false, DirectorAPI.Stage.Custom, DesolateCoast.mapName);
                DirectorAPI.Helpers.AddNewMonsterToStage(wayfarerHolder, false, DirectorAPI.Stage.Custom, DesolateCoast.simuName);
                Log.Info("Wayfarer added to Desolate Coast's spawn pool.");
            }

        }

    }

}
