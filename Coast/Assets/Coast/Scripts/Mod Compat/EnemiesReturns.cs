using EnemiesReturns;
using EnemiesReturns.Configuration;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.MechanicalSpider.Enemy;
using EnemiesReturns.Enemies.SandCrab;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.Enemies.Swift;
using R2API;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesolateCoast
{
    public class EnemiesReturnsCompat
    {
        public static void AddEnemies()
        {
            // Sand Crab
            if (DesolateCoast.toggleSandCrab.Value && General.EnableSandCrab.Value)
            {
                var card = new RoR2.DirectorCard()
                {
                    spawnCard = (RoR2.SpawnCard)(object)SandCrabBody.SpawnCards.cscSandCrabDefault,
                    spawnDistance = RoR2.DirectorCore.MonsterSpawnDistance.Standard,
                    selectionWeight = SandCrab.SelectionWeight.Value,
                    minimumStageCompletions = SandCrab.MinimumStageCompletion.Value
                };

                var holder = new DirectorAPI.DirectorCardHolder
                {
                    Card = card,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
                };

                if (!SandCrab.DefaultStageList.Value.Contains(DesolateCoast.mapName)) //Checking whether default stage list has this enemy to avoid adding a duplicate spawn card
                {
                    DirectorAPI.Helpers.AddNewMonsterToStage(holder, false, DirectorAPI.Stage.Custom, DesolateCoast.mapName);
                    Log.Info("Sand Crab added to Desolate Coast's spawn pool.");
                }
                if (!SandCrab.DefaultStageList.Value.Contains(DesolateCoast.simuName))
                {
                    DirectorAPI.Helpers.AddNewMonsterToStage(holder, false, DirectorAPI.Stage.Custom, DesolateCoast.simuName);
                    Log.Info("Sand Crab added to Desolate Coast's simulacrum spawn pool.");
                }

            }
        }
    }
}