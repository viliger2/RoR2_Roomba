using R2API;
using RoR2;
using RoR2.Audio;
using RoR2_Roomba.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2_Roomba.Items
{
    public class Maxwell
    {
        public static GameObject EvilMaxwellPrefab;

        public static ModdedProcType MaxwellProcType;

        public static ItemDef CreateItemDef(GameObject maxwellPickupItem, Sprite icon)
        {
            Transform focusPoint = maxwellPickupItem.transform.Find("FocusPoint");
            Transform cameraPosition = maxwellPickupItem.transform.Find("CameraPosition");
            if (focusPoint && cameraPosition)
            {
                var modelPanel = maxwellPickupItem.AddComponent<ModelPanelParameters>();
                modelPanel.focusPointTransform = focusPoint;
                modelPanel.cameraPositionTransform = cameraPosition;
                modelPanel.minDistance = 1.5f;
                modelPanel.maxDistance = 3f;
            }

            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.tier = ItemTier.Tier3;
            itemDef.deprecatedTier = ItemTier.Tier3;
            itemDef.name = "Maxwell";
            itemDef.nameToken = "ROOMBA_ITEM_MAXWELL_NAME";
            itemDef.pickupToken = "ROOMBA_ITEM_MAXWELL_PICKUP";
            itemDef.descriptionToken = "ROOMBA_ITEM_MAXWELL_DESCRIPTION";
            itemDef.loreToken = "ROOMBA_ITEM_MAXWELL_LORE";
            itemDef.pickupModelPrefab = maxwellPickupItem;
            itemDef.canRemove = true;
            itemDef.pickupIconSprite = icon;
            itemDef.tags = new ItemTag[] { ItemTag.WorldUnique };

            return itemDef;
        }

        public static GameObject CreateEvilMaxwellPrefab(GameObject evilMaxwellPrefab, GameObject grenadeExplosion)
        {
            #region NetworkIdentity
            evilMaxwellPrefab.AddComponent<NetworkIdentity>();
            #endregion

            #region TeamFilter
            evilMaxwellPrefab.AddComponent<TeamFilter>();
            #endregion

            #region DelayBlast
            var delayBlast = evilMaxwellPrefab.AddComponent<ModdedDelayBlast>();
            delayBlast.explosionEffect = grenadeExplosion;
            delayBlast.timerStagger = 0f;
            #endregion

            PrefabAPI.RegisterNetworkPrefab(evilMaxwellPrefab);

            return evilMaxwellPrefab;
        }


        public static void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            var damageInfo = obj.damageInfo;
            var victim = obj.victim;

            if (!damageInfo.attacker || !damageInfo.attacker.TryGetComponent<CharacterBody>(out var attackerBody))
            {
                return;
            }

            if (!attackerBody.master)
            {
                return;
            }

            if (!damageInfo.procChainMask.HasModdedProc(MaxwellProcType))
            {
                var maxwellCount = attackerBody.master.inventory.GetItemCount(ContentProvider.Items.Maxwell);
                if (maxwellCount > 0 && Util.CheckRoll((RoombaConfigs.MaxwellExplosionChance.Value) * damageInfo.procCoefficient, attackerBody.master))
                {
                    Vector3 victimCorePosition = victim.GetComponent<CharacterBody>().corePosition;
                    float damageCoefficient = RoombaConfigs.MaxwellExplosionDamage.Value / 100f + ((RoombaConfigs.MaxwellExplosionDamagePerStack.Value / 100f) * (maxwellCount - 1));
                    float baseDamage = Util.OnKillProcDamage(attackerBody.damage, damageCoefficient);
                    GameObject evilMaxwellCopy = UnityEngine.Object.Instantiate(EvilMaxwellPrefab, victimCorePosition, Quaternion.identity);
                    ModdedDelayBlast delayBlast = evilMaxwellCopy.GetComponent<ModdedDelayBlast>();
                    if ((bool)delayBlast)
                    {
                        delayBlast.position = victimCorePosition;
                        delayBlast.baseDamage = baseDamage;
                        delayBlast.baseForce = 2000f;
                        delayBlast.bonusForce = Vector3.up * 1000f;
                        delayBlast.radius = RoombaConfigs.MaxwellExplosionRadius.Value;
                        delayBlast.attacker = damageInfo.attacker;
                        delayBlast.inflictor = null;
                        delayBlast.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
                        delayBlast.maxTimer = 3f;
                        delayBlast.damageColorIndex = DamageColorIndex.Item;
                        delayBlast.falloffModel = BlastAttack.FalloffModel.None; 

                        delayBlast.procChainMask = damageInfo.procChainMask;
                        delayBlast.procChainMask.AddModdedProc(MaxwellProcType);
                    }
                    TeamFilter teamFilter = evilMaxwellCopy.GetComponent<TeamFilter>();
                    if ((bool)teamFilter)
                    {
                        teamFilter.teamIndex = attackerBody.GetComponent<TeamComponent>().teamIndex;
                    }

                    NetworkServer.Spawn(evilMaxwellCopy);
                    EntitySoundManager.EmitSoundServer((AkEventIdArg)"Roomba_BadToTheBone_Play", evilMaxwellCopy);
                }
            }
        }
    }
}
