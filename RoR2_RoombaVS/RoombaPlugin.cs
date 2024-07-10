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

namespace RoR2_Roomba
{
    [BepInPlugin(GUID, ModName, Version)]
    public class RoombaPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "Roomba";
        public const string Version = "1.0.0";
        public const string GUID = "com." + Author + "." + ModName;

        public static ConfigEntry<float> RoombaSpawnChance;
        public static ConfigEntry<bool> MakeVoidExplosionBypassArmor;

        private void Awake()
        {
            PopulateConfigs();

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

        private void PopulateConfigs()
        {
            RoombaSpawnChance = Config.Bind("Roomba", "Chance To Spawn", 30f, "Chance of Roomba to spawn on stage start.");
            MakeVoidExplosionBypassArmor = Config.Bind("Void Death", "Make Void Explosion Bypass Armor", true, "Add BypassArmor flag to all void explosions that result in instant death. Used so you can kill Roomba with it.");
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
                var spawnRequest = new DirectorSpawnRequest(
                    RoombaFactory.cscRoomba, 
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
