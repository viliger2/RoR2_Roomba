﻿using BepInEx;
using BepInEx.Configuration;
using EntityStates.GameOver;
using RoR2;
using System.IO;
using RoR2.ContentManagement;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using static RoR2_Roomba.RoombaConfigs;

namespace RoR2_Roomba
{
    [BepInPlugin(GUID, ModName, Version)]
    public class RoombaPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "Roomba";
        public const string Version = "1.0.0";
        public const string GUID = "com." + Author + "." + ModName;



        private void Awake()
        {
            Log.Init(Logger);

            RoombaConfigs.PopulateConfigs(Config);

            // fixing hopoo's shit
            var neutralTeam = RoR2.TeamCatalog.GetTeamDef(TeamIndex.Neutral);
            if(neutralTeam != null)
            {
                if(!neutralTeam.levelUpEffect)
                {
                    neutralTeam.levelUpEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/LevelUpEffectEnemy");
                }
            }

            // adding BypassArmor to all void death projectiles, surely it won't break anything
            if (MakeVoidExplosionBypassArmor.Value)
            {
                AddBypassArmorToProjectileDamage(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierDeathBombProjectile.prefab").WaitForCompletion());
                AddBypassArmorToProjectileDamage(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDeathBombProjectile.prefab").WaitForCompletion());
                AddBypassArmorToProjectileDamage(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDeathBombProjectileStill.prefab").WaitForCompletion());
                AddBypassArmorToProjectileDamage(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombletsProjectile.prefab").WaitForCompletion());
                AddBypassArmorToProjectileDamage(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombProjectile.prefab").WaitForCompletion());
            }

            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
        }

        private static void AddBypassArmorToProjectileDamage(GameObject deathBomb)
        {
            if (deathBomb && deathBomb.TryGetComponent<ProjectileDamage>(out var projectileDamage))
            {
                projectileDamage.damageType |= DamageType.BypassArmor;
            }
        }

        private void SceneDirector_onPostPopulateSceneServer(RoR2.SceneDirector sceneDirector)
        {
            if (!RoR2.SceneInfo.instance.countsAsStage)
            {
                return;
            }

            float value = sceneDirector.rng.RangeFloat(0f, 100f);
            if (value < RoombaSpawnChance.Value)
            {
                WeightedSelection<CharacterSpawnCard> roombaSelection = new WeightedSelection<CharacterSpawnCard>();
                roombaSelection.AddChoice(RoombaFactory.cscRoomba, RoombaNothingWeight.Value);
                roombaSelection.AddChoice(RoombaFactory.cscRoombaMaxwell, RoombaMaxwellWeight.Value);
                roombaSelection.AddChoice(RoombaFactory.cscRoombaTV, RoombaTVWeight.Value);

                var spawnCard = roombaSelection.Evaluate(sceneDirector.rng.nextNormalizedFloat);

                var spawnRequest = new DirectorSpawnRequest(
                    spawnCard, 
                    new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    }, 
                    sceneDirector.rng);
                spawnRequest.teamIndexOverride = TeamIndex.Neutral; // TODO: swap to Monster if blablabla
                spawnRequest.ignoreTeamMemberLimit = true;

                RoR2.DirectorCore.instance.TrySpawnObject(spawnRequest);
            }
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentProvider());
        }
    }
}
