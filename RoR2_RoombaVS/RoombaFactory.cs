using EntityStates;
using KinematicCharacterController;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2_Roomba
{
    public class RoombaFactory
    {
        public static CharacterSpawnCard cscRoomba { private set; get; }
       
        public GameObject CreateRoombaBody(GameObject roombaPrefab)
        {
            //foreach(Transform child in roombaPrefab.transform)
            //{
            //    Debug.Log(child);
            //}


            Transform modelBase = roombaPrefab.transform.Find("ModelBase");
            //if (!modelBase) { Debug.Log("oioioioioi"); };
            Transform aimOrigin = roombaPrefab.transform.Find("ModelBase/AimOrigin");
            Transform modelTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba");
            Transform mainHurtBoxTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba/MainHurtBox");
            Transform focusPointTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba/LogBookTarget");
            Transform cameraPositionTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba/LogBookTarget/LogBookCamera");

            Renderer renderer = roombaPrefab.transform.Find("ModelBase/mdlRoomba/roomba").gameObject.GetComponent<MeshRenderer>();

            #region RoombaBody

            #region NetworkIdentity
            roombaPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterDirection
            var characterDirection = roombaPrefab.AddComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBase;
            characterDirection.turnSpeed = 720f;
            #endregion

            #region CharacterMotor
            var characterMotor = roombaPrefab.AddComponent<CharacterMotor>();
            characterMotor.characterDirection = characterDirection;
            characterMotor.muteWalkMotion = true;
            characterMotor.mass = 100f;
            characterMotor.airControl = 0.25f;
            characterMotor.generateParametersOnAwake = true;
            #endregion

            #region InputBankTest
            roombaPrefab.AddComponent<InputBankTest>();
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if(!roombaPrefab.TryGetComponent<CharacterBody>(out characterBody))
            {
                characterBody = roombaPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ROOMBA_ROOMBA_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage & CharacterBody.BodyFlags.ImmuneToGoo & CharacterBody.BodyFlags.ImmuneToExecutes & CharacterBody.BodyFlags.OverheatImmune & CharacterBody.BodyFlags.Mechanical & CharacterBody.BodyFlags.HasBackstabImmunity;
            characterBody.mainRootSpeed = 33;

            characterBody.baseMaxHealth = 200;
            characterBody.baseMoveSpeed = 10;
            characterBody.baseAcceleration = 180;
            characterBody.baseJumpPower = 16;
            characterBody.baseDamage = 0;
            characterBody.baseAttackSpeed = 1;
            characterBody.baseVisionDistance = 1;
            characterBody.baseJumpCount = 1; // TODO: maybe 0?

            characterBody.sprintingSpeedMultiplier = 2;
            characterBody.autoCalculateLevelStats = true;

            characterBody.levelMaxHealth = 60;

            characterBody.aimOriginTransform = aimOrigin;
            characterBody.hullClassification = HullClassification.Human;
            characterBody.bodyColor = Color.white;
            characterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));
            #endregion

            #region CameraTargetParams
            roombaPrefab.AddComponent<CameraTargetParams>();
            #endregion

            #region ModelLocator
            var modelLocator = roombaPrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBase;
            modelLocator.autoUpdateModelTransform = true;
            modelLocator.normalizeToFloor = true;
            modelLocator.normalSmoothdampTime = 0.1f;
            modelLocator.normalMaxAngleDelta = 90f;
            #endregion

            #region EntityStateMachine_Body
            var entityStateMachineBody = roombaPrefab.AddComponent<EntityStateMachine>();
            entityStateMachineBody.customName = "Body";
            entityStateMachineBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(RoR2_Roomba.RoombaSpawnState));
            entityStateMachineBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));
            #endregion

            #region EntityStateMachine_Weapon
            var entityStateMachineWeapon = roombaPrefab.AddComponent<EntityStateMachine>();
            entityStateMachineWeapon.customName = "Weapon";
            entityStateMachineWeapon.initialStateType = new EntityStates.SerializableEntityStateType(typeof(RoR2_Roomba.InvincibleRoombaState));
            entityStateMachineWeapon.mainStateType = new EntityStates.SerializableEntityStateType(typeof(RoR2_Roomba.InvincibleRoombaState));
            #endregion

            #region SkillLocator
            roombaPrefab.AddComponent<SkillLocator>();
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if(!roombaPrefab.TryGetComponent<TeamComponent>(out teamComponent))
            {
                teamComponent = roombaPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.Neutral; // TODO: maybe switch to monster if enemies keep attacking it
            #endregion

            #region HealthComponent
            var healthComponent = roombaPrefab.AddComponent<HealthComponent>();
            healthComponent.globalDeathEventChanceCoefficient = 1f;
            #endregion

            #region Interactor
            roombaPrefab.AddComponent<Interactor>().maxInteractionDistance = 3f;
            #endregion

            #region InteractionDriver
            roombaPrefab.AddComponent<InteractionDriver>();
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = roombaPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = entityStateMachineBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterDeath));
            #endregion

            #region CharacterNetworkTransform
            var characterNetworkTransform = roombaPrefab.AddComponent<CharacterNetworkTransform>();
            characterNetworkTransform.positionTransmitInterval = 0.1f;
            characterNetworkTransform.interpolationFactor = 2f;
            #endregion

            #region NetworkStateMachine
            var networkStateMachine = roombaPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new EntityStateMachine[] { entityStateMachineBody, entityStateMachineWeapon };
            #endregion

            #region SfxLocator
            var sfxLocator = roombaPrefab.AddComponent<SfxLocator>();
            // TODO: populate these
            //sfxLocator.aliveLoopStart = ;
            //sfxLocator.aliveLoopStop = ;
            #endregion

            #region KinematicCharacterMotor
            var kinematicCharacterMotor = roombaPrefab.AddComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = roombaPrefab.GetComponent<CapsuleCollider>();
            kinematicCharacterMotor.Rigidbody = roombaPrefab.GetComponent<Rigidbody>();

            kinematicCharacterMotor.MaxStepHeight = 0.4f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;

            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;

            kinematicCharacterMotor.StepHandling = StepHandlingMethod.Standard;
            kinematicCharacterMotor.LedgeHandling = true;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.SafeMovement = false;
            #endregion

            #endregion

            #region mdlRoomba
            var modelGameObject = modelTransform.gameObject;
            var mainHurtBox = mainHurtBoxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;

            #region HurtBoxGroup
            var hurtBoxGroup = modelGameObject.AddComponent<HurtBoxGroup>();
            hurtBoxGroup.hurtBoxes = new HurtBox[] { mainHurtBox };
            hurtBoxGroup.mainHurtBox = mainHurtBox;
            #endregion

            #region DestroyOnUnseen
            modelGameObject.AddComponent<DestroyOnUnseen>();
            #endregion

            #region CharacterModel
            var characterModel = modelGameObject.AddComponent<CharacterModel>();
            characterModel.body = characterBody;
            //characterModel.itemDisplayRuleSet = ; hopefully I don't need it
            characterModel.autoPopulateLightInfos = true;
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = renderer,
                    defaultMaterial = renderer.materials[0],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                }
            };
            #endregion

            #region ModelPanelParameters
            var modelPanelParameters = modelGameObject.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = focusPointTransform;
            modelPanelParameters.cameraPositionTransform = cameraPositionTransform;
            modelPanelParameters.modelRotation = new Quaternion(0f, 0f, 0f, 1f);
            modelPanelParameters.minDistance = 1.5f;
            modelPanelParameters.maxDistance = 2.5f;
            #endregion

            #region ChildLocator
            modelGameObject.AddComponent<ChildLocator>();
            #endregion

            #endregion

            return roombaPrefab;
        }

        public GameObject CreateRoombaMaster(GameObject roombaMaster, GameObject roombaBody)
        {
            #region NetworkIdentity
            roombaMaster.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterMaster
            var characterMaster = roombaMaster.AddComponent<CharacterMaster>();
            characterMaster.bodyPrefab = roombaBody;
            characterMaster.spawnOnStart = false;
            characterMaster.teamIndex = TeamIndex.Neutral; // TODO: swap to monster if enemies attack it
            //characterMaster.OnBodyDeath  TODO: maybe do something funny with it
            characterMaster.destroyOnBodyDeath = true;
            characterMaster.preventGameOver = true;
            #endregion

            #region Inventory
            roombaMaster.AddComponent<Inventory>();
            #endregion

            #region EntityStateMachine_AI
            var entityStateMachineAI = roombaMaster.AddComponent<EntityStateMachine>();
            entityStateMachineAI.customName = "AI";
            entityStateMachineAI.initialStateType = new EntityStates.SerializableEntityStateType(typeof(RoR2_Roomba.WanderFar));
            entityStateMachineAI.mainStateType = new EntityStates.SerializableEntityStateType(typeof(RoR2_Roomba.WanderFar));
            #endregion

            #region BaseAI
            var baseAI = roombaMaster.AddComponent<BaseAI>();
            baseAI.fullVision = false;
            baseAI.neverRetaliateFriendlies = true;
            baseAI.enemyAttentionDuration = 0f;
            baseAI.desiredSpawnNodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            baseAI.stateMachine = entityStateMachineAI;
            baseAI.scanState = new EntityStates.SerializableEntityStateType(typeof(RoR2_Roomba.WanderFar));
            baseAI.isHealer = false;
            baseAI.enemyAttention = 0f;
            baseAI.aimVectorDampTime = 0.05f;
            baseAI.aimVectorMaxSpeed = 180f;
            #endregion

            return roombaMaster;
        }

        public CharacterSpawnCard CreateCharacterSpawnCard(GameObject roombaMaster)
        {
            var cscRoombaNew = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            cscRoombaNew.prefab = roombaMaster;
            cscRoombaNew.sendOverNetwork = true;
            cscRoombaNew.hullSize = HullClassification.Human;
            cscRoombaNew.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            cscRoombaNew.requiredFlags = RoR2.Navigation.NodeFlags.None;
            cscRoombaNew.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            cscRoombaNew.directorCreditCost = 1;
            cscRoombaNew.occupyPosition = true;
            cscRoombaNew.eliteRules = SpawnCard.EliteRules.Default;
            cscRoombaNew.noElites = true;
            cscRoombaNew.forbiddenAsBoss = true;

            cscRoomba = cscRoombaNew;

            return cscRoombaNew;
        }

    }
}
