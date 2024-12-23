﻿using KinematicCharacterController;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Networking;
using RoR2_Roomba.States;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.Skills.ComboSkillDef;

namespace RoR2_Roomba
{
    public class RoombaFactory
    {
        public static CharacterSpawnCard cscRoomba { internal set; get; }
        public static CharacterSpawnCard cscRoombaMaxwell { internal set; get; }
        public static CharacterSpawnCard cscRoombaTV { internal set; get; }

        public GameObject CreateRoombaBody(GameObject roombaPrefab, GameObject bombEffect)
        {
            Transform modelBase = roombaPrefab.transform.Find("ModelBase");
            Transform aimOrigin = roombaPrefab.transform.Find("ModelBase/AimOrigin");
            Transform modelTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba");
            Transform mainHurtBoxTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba/MainHurtBox");
            Transform focusPointTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba/LogBookTarget");
            Transform cameraPositionTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba/LogBookTarget/LogBookCamera");
            Transform maxwell = roombaPrefab.transform.Find("ModelBase/mdlRoomba/maxwell");
            Transform tv = roombaPrefab.transform.Find("ModelBase/mdlRoomba/TVPrefab");
            Transform bombTransform = roombaPrefab.transform.Find("ModelBase/mdlRoomba/SmokeBomb");

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
            characterMotor.airControl = 0.05f;
            characterMotor.generateParametersOnAwake = true;
            #endregion

            #region InputBankTest
            roombaPrefab.AddComponent<InputBankTest>();
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if (!roombaPrefab.TryGetComponent<CharacterBody>(out characterBody))
            {
                characterBody = roombaPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ROOMBA_ROOMBA_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage & CharacterBody.BodyFlags.ImmuneToGoo & CharacterBody.BodyFlags.ImmuneToExecutes & CharacterBody.BodyFlags.OverheatImmune & CharacterBody.BodyFlags.Mechanical & CharacterBody.BodyFlags.HasBackstabImmunity;
            characterBody.mainRootSpeed = 33;

            characterBody.baseMaxHealth = 200;
            characterBody.baseMoveSpeed = 5;
            characterBody.baseAcceleration = 180;
            characterBody.baseJumpPower = 16;
            characterBody.baseDamage = 0;
            characterBody.baseAttackSpeed = 1;
            characterBody.baseVisionDistance = 1;
            characterBody.baseJumpCount = 1; // TODO: maybe 0?

            characterBody.sprintingSpeedMultiplier = 2;
            characterBody.autoCalculateLevelStats = false;

            characterBody.levelMaxHealth = 60;

            characterBody.aimOriginTransform = aimOrigin;
            characterBody.hullClassification = HullClassification.Human;
            characterBody.bodyColor = Color.white;
            characterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));
            #endregion

            #region CameraTargetParams
            roombaPrefab.AddComponent<CameraTargetParams>().cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandard.asset").WaitForCompletion();
            #endregion

            #region ModelLocator
            var modelLocator = roombaPrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBase;
            modelLocator.autoUpdateModelTransform = true;
            modelLocator.normalizeToFloor = true;
            modelLocator.normalSmoothdampTime = 0.1f;
            modelLocator.normalMaxAngleDelta = 60f;
            #endregion

            #region EntityStateMachine_Body
            var entityStateMachineBody = roombaPrefab.AddComponent<EntityStateMachine>();
            entityStateMachineBody.customName = "Body";
            entityStateMachineBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(RoombaSpawnState));
            entityStateMachineBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));
            #endregion

            #region EntityStateMachine_Weapon
            var entityStateMachineWeapon = roombaPrefab.AddComponent<EntityStateMachine>();
            entityStateMachineWeapon.customName = "Weapon";
            entityStateMachineWeapon.initialStateType = new EntityStates.SerializableEntityStateType(typeof(InvincibleRoombaState));
            entityStateMachineWeapon.mainStateType = new EntityStates.SerializableEntityStateType(typeof(InvincibleRoombaState));
            #endregion

            #region SkillLocator
            roombaPrefab.AddComponent<SkillLocator>();
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if (!roombaPrefab.TryGetComponent<TeamComponent>(out teamComponent))
            {
                teamComponent = roombaPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.Neutral; // TODO: maybe switch to monster if enemies keep attacking it
            #endregion

            #region HealthComponent
            var healthComponent = roombaPrefab.AddComponent<HealthComponent>();
            healthComponent.globalDeathEventChanceCoefficient = 1f;
            healthComponent.dontShowHealthbar = true;
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
            if (maxwell)
            {
                sfxLocator.aliveLoopStart = "Roomba_Maxwell_Play";
                sfxLocator.aliveLoopStop = "Roomba_Maxwell_Stop";
            }
            else if (tv)
            {
                sfxLocator.aliveLoopStart = "Roomba_TV_Play";
                sfxLocator.aliveLoopStop = "Roomba_TV_Stop";
            }
            else
            {
                sfxLocator.aliveLoopStart = "Roomba_Roomba_Play";
                sfxLocator.aliveLoopStop = "Roomba_Roomba_Stop";
            }
            #endregion

            #region KinematicCharacterMotor
            var capsuleCollider = roombaPrefab.GetComponent<CapsuleCollider>();

            var kinematicCharacterMotor = roombaPrefab.AddComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor._attachedRigidbody = roombaPrefab.GetComponent<Rigidbody>();
            kinematicCharacterMotor.CapsuleRadius = capsuleCollider.radius;
            kinematicCharacterMotor.CapsuleHeight = capsuleCollider.height;
            if (capsuleCollider.center != Vector3.zero)
            {
                Log.Error("CapsuleCollider for " + roombaPrefab + " has non-zero center. This WILL result in pathing issues for AI.");
            }
            kinematicCharacterMotor.CapsuleYOffset = 0f;

            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.StableGroundLayers = LayerIndex.world.mask;
            kinematicCharacterMotor.DiscreteCollisionEvents = false;

            kinematicCharacterMotor.StepHandling = StepHandlingMethod.Standard;
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
            kinematicCharacterMotor.AllowSteppingWithoutStableGrounding = false;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;

            kinematicCharacterMotor.LedgeAndDenivelationHandling = true;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.MaxVelocityForLedgeSnap = 0f;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;

            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.RigidbodyInteractionType= RigidbodyInteractionType.None;
            kinematicCharacterMotor.SimulatedCharacterMass = 1f;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;

            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = Vector3.forward;

            kinematicCharacterMotor.CheckMovementInitialOverlaps = true;
            kinematicCharacterMotor.KillVelocityWhenExceedMaxMovementIterations = true;
            kinematicCharacterMotor.KillRemainingMovementWhenExceedMaxMovementIterations = true;
            kinematicCharacterMotor.timeUntilUpdate = 0f;
            kinematicCharacterMotor.playerCharacter = false;
            #endregion

            #region DropItemOnDeath
            if (RoombaConfigs.RoombaCanDropItems.Value)
            {
                var dropItemOnDeath = roombaPrefab.AddComponent<DropItemOnDeath>();
                dropItemOnDeath.dropRandomChest1Item = true;
                dropItemOnDeath.dropAngle = 0f;
            }
            #endregion

            #region RoombaInteractableManager
            var interactableManager = roombaPrefab.AddComponent<RoombaInteractableManager>();
            interactableManager.body = characterBody;
            interactableManager.smokeBombPrefab = bombEffect;
            #endregion

            #region EntityLocator
            roombaPrefab.AddComponent<EntityLocator>().entity = roombaPrefab;
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
            var childLocator = modelGameObject.AddComponent<ChildLocator>();
            childLocator.transformPairs = new ChildLocator.NameTransformPair[] { new ChildLocator.NameTransformPair { name = "SmokeBomb", transform = bombTransform } };
            #endregion

            #endregion

            #region Maxwell
            if (maxwell && RoombaConfigs.CustomItems.Value)
            {
                var itemDropMaxwell = roombaPrefab.AddComponent<DropItemOnDeath>();
                itemDropMaxwell.itemToDrop = ContentProvider.Items.Maxwell;
                itemDropMaxwell.dropAngle = 270f;
            }
            #endregion

            #region TV
            if (tv && RoombaConfigs.CustomItems.Value)
            {
                var itemDropTV = roombaPrefab.AddComponent<DropItemOnDeath>();
                itemDropTV.itemToDrop = ContentProvider.Items.Poster;
                itemDropTV.dropAngle = 180f;
            }
            #endregion

            PrefabAPI.RegisterNetworkPrefab(roombaPrefab);

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
            characterMaster.destroyOnBodyDeath = true;
            characterMaster.preventGameOver = true;
            #endregion

            #region Inventory
            roombaMaster.AddComponent<Inventory>();
            #endregion

            #region EntityStateMachine_AI
            var entityStateMachineAI = roombaMaster.AddComponent<EntityStateMachine>();
            entityStateMachineAI.customName = "AI";
            entityStateMachineAI.initialStateType = new EntityStates.SerializableEntityStateType(typeof(WanderFar));
            entityStateMachineAI.mainStateType = new EntityStates.SerializableEntityStateType(typeof(WanderFar));
            #endregion

            #region BaseAI
            var baseAI = roombaMaster.AddComponent<BaseAI>();
            baseAI.fullVision = false;
            baseAI.neverRetaliateFriendlies = true;
            baseAI.enemyAttentionDuration = 0f;
            baseAI.desiredSpawnNodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            baseAI.stateMachine = entityStateMachineAI;
            baseAI.scanState = new EntityStates.SerializableEntityStateType(typeof(WanderFar));
            baseAI.isHealer = false;
            baseAI.enemyAttention = 0f;
            baseAI.aimVectorDampTime = 0.05f;
            baseAI.aimVectorMaxSpeed = 180f;
            #endregion

            roombaMaster.RegisterNetworkPrefab();

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

            return cscRoombaNew;
        }
    }
}
