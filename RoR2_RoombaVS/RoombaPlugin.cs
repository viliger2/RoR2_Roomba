using BepInEx;
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
using R2API;
using RoR2_Roomba.Items;
using System.Collections.Generic;

namespace RoR2_Roomba
{
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    public class RoombaPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "Roomba";
        public const string Version = "1.0.0";
        public const string GUID = "com." + Author + "." + ModName;

        private void Awake()
        {

#if DEBUG == true
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
#endif
            Log.Init(Logger);

            RoombaConfigs.PopulateConfigs(Config);

            // fixing hopoo's shit
            var neutralTeam = RoR2.TeamCatalog.GetTeamDef(TeamIndex.Neutral);
            if (neutralTeam != null)
            {
                if (!neutralTeam.levelUpEffect)
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

            Hooks();
        }

        private void Hooks()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            RoR2.Language.onCurrentLanguageChanged += Language.Language_onCurrentLanguageChanged;
            if (RoombaConfigs.CustomItems.Value)
            {
                On.RoR2.GlobalEventManager.OnHitEnemy += Maxwell.GlobalEventManager_OnHitEnemy;
                RecalculateStatsAPI.GetStatCoefficients += Poster.RecalculateStatsAPI_GetStatCoefficients;
            }
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

            TrySpawnRoomba(sceneDirector);
            TrySpawnPileOfDirt(sceneDirector);
        }

        private static void TrySpawnPileOfDirt(SceneDirector sceneDirector)
        {
            float value = sceneDirector.rng.RangeFloat(0f, 100f);
            if (value < RoombaSpawnChance.Value)
            {
                var spawnRequest = new DirectorSpawnRequest(
                    Items.PileOfDirt.scPileOfDirt,
                    new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    },
                    sceneDirector.rng);
                spawnRequest.teamIndexOverride = TeamIndex.Neutral;
                spawnRequest.ignoreTeamMemberLimit = true;

                RoR2.DirectorCore.instance.TrySpawnObject(spawnRequest);
            }
        }

        private static void TrySpawnRoomba(SceneDirector sceneDirector)
        {
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

        public void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Language"));
        }
    }
}
