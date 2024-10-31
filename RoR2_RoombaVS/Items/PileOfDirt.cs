using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2_Roomba.Items
{
    public class PileOfDirt
    {
        public static SpawnCard scPileOfDirt;

        public static GameObject CreatePileOfDirtSpawnPrefab(GameObject pileOfDirtPrefab, Material glassMaterial)
        {
            Transform model = pileOfDirtPrefab.transform.Find("Model");
            Transform hurtBoxTransform = pileOfDirtPrefab.transform.Find("Model/Hurtbox");

            pileOfDirtPrefab.transform.Find("Model/mdlPileOfDirt/Glass1").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;
            pileOfDirtPrefab.transform.Find("Model/mdlPileOfDirt/Glass2").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;
            pileOfDirtPrefab.transform.Find("Model/mdlPileOfDirt/Glass3").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;

            #region PileOfDirtSpawn

            #region NetworkIdentity
            pileOfDirtPrefab.AddComponent<NetworkIdentity>();
            #endregion

            #region SkillLocator
            pileOfDirtPrefab.AddComponent<SkillLocator>();
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if (!pileOfDirtPrefab.TryGetComponent<TeamComponent>(out teamComponent))
            {
                teamComponent = pileOfDirtPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.Neutral; // TODO: maybe switch to monster if enemies keep attacking it
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if (!pileOfDirtPrefab.TryGetComponent<CharacterBody>(out characterBody))
            {
                characterBody = pileOfDirtPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ROOMBA_BODY_PILE_OF_DIRT";
            characterBody.bodyFlags = CharacterBody.BodyFlags.Masterless | CharacterBody.BodyFlags.ImmuneToVoidDeath;
            characterBody.baseMaxHealth = 30f;
            characterBody.autoCalculateLevelStats = false;
            characterBody.hullClassification = HullClassification.Human;
            #endregion

            #region HealthComponent
            var healthComponent = pileOfDirtPrefab.AddComponent<HealthComponent>();
            healthComponent.globalDeathEventChanceCoefficient = 1f;
            healthComponent.dontShowHealthbar = true;
            #endregion

            #region ModelLocator
            var modelLocator = pileOfDirtPrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = model;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = true;
            modelLocator.normalizeToFloor = true;
            #endregion

            #region EntityStateMachine_Body
            var esmBody = pileOfDirtPrefab.AddComponent<EntityStateMachine>();
            esmBody.customName = "Body";
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            esmBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            #endregion

            #region NetworkStateMachine
            var networkStateMachine = pileOfDirtPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new EntityStateMachine[] { esmBody };
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = pileOfDirtPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(RoR2_Roomba.States.PileOfDirtDeath));
            #endregion

            #endregion

            #region Hurtbox
            var hurtbox = hurtBoxTransform.gameObject.AddComponent<HurtBox>();
            hurtbox.healthComponent = healthComponent;
            #endregion

            #region Model

            #region HurtBoxGroup
            var hurtboxGroup = model.gameObject.AddComponent<HurtBoxGroup>();
            hurtboxGroup.mainHurtBox = hurtbox;
            hurtboxGroup.hurtBoxes = new HurtBox[] { hurtbox };
            #endregion

            #endregion

            PrefabAPI.RegisterNetworkPrefab(pileOfDirtPrefab);

            return pileOfDirtPrefab;
        }

        public static SpawnCard CreatePileOfDirtSpawnCard(GameObject pileOfDirtPrefab)
        {
            var card = ScriptableObject.CreateInstance<SpawnCard>();

            card.prefab = pileOfDirtPrefab;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.Human;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoChestSpawn;
            card.directorCreditCost = 1;
            card.occupyPosition = false;

            return card;
        }

        public static ItemDef CreateItemDef(GameObject pilePickup, Material glassMaterial, Sprite icon)
        {
            pilePickup.transform.Find("mdlPileOfDirt/Glass1").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;
            pilePickup.transform.Find("mdlPileOfDirt/Glass2").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;
            pilePickup.transform.Find("mdlPileOfDirt/Glass3").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;

            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.tier = ItemTier.Boss;
            itemDef.deprecatedTier = ItemTier.Boss;
            itemDef.name = "PileOfDirt";
            itemDef.nameToken = "ROOMBA_ITEM_PILE_OF_DIRT_NAME";
            itemDef.pickupToken = "ROOMBA_ITEM_PILE_OF_DIRT_PICKUP";
            itemDef.descriptionToken = "ROOMBA_ITEM_PILE_OF_DIRT_DESCRIPTION";
            itemDef.loreToken = "ROOMBA_ITEM_PILE_OF_DIRT_LORE";
            itemDef.pickupModelPrefab = pilePickup;
            itemDef.canRemove = true;
            itemDef.tags = new ItemTag[] { ItemTag.WorldUnique, ItemTag.CannotSteal, ItemTag.CannotDuplicate, ItemTag.CannotCopy, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist };
            itemDef.pickupIconSprite = icon;

            return itemDef;
        }
    }
}
