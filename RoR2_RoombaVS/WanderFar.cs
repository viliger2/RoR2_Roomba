﻿using EntityStates.AI;
using EntityStates.AI.Walker;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RoR2_Roomba
{
    public class WanderFar : Wander
    {
        public static float refreshTime = 15f;
        private float refreshTimer;


        public override void OnEnter()
        {
            // copy-past of EntityStates.AI.BaseAiState.OnEnter()
            characterMaster = GetComponent<CharacterMaster>();
            ai = GetComponent<BaseAI>();
            if ((bool)ai)
            {
                body = ai.body;
                bodyTransform = ai.bodyTransform;
                bodyInputBank = ai.bodyInputBank;
                bodyCharacterMotor = ai.bodyCharacterMotor;
                bodySkillLocator = ai.bodySkillLocator;
            }
            bodyInputs = default(BaseAI.BodyInputs);
            // end copy-paste

            if ((bool)base.ai && (bool)base.body)
            {
                BroadNavigationSystem.Agent broadNavigationAgent = base.ai.broadNavigationAgent;
                SetNewRandomTarget(broadNavigationAgent);
                aiUpdateTimer = 0.16f;
            }
        }

        private void SetNewRandomTarget(BroadNavigationSystem.Agent broadNavigationAgent)
        {
            targetPosition = PickRandomReachablePosition();
            if (targetPosition.HasValue)
            {
                broadNavigationAgent.goalPosition = targetPosition.Value;
                broadNavigationAgent.InvalidatePath();
            }
            PickNewTargetLookPosition();
        }

        public override void FixedUpdate()
        {
            fixedAge += Time.fixedDeltaTime;
            aiUpdateTimer -= Time.fixedDeltaTime;
            refreshTimer -= Time.fixedDeltaTime;
            if (!ai || !body)
            {
                return;
            }
            ai.SetGoalPosition(targetPosition);
            BroadNavigationSystem.Agent broadNavigationAgent = ai.broadNavigationAgent;
            if (aiUpdateTimer <= 0f)
            {
                aiUpdateTimer = cvAIUpdateInterval.value;
                ai.localNavigator.targetPosition = broadNavigationAgent.output.nextPosition ?? ai.localNavigator.targetPosition;
                ai.localNavigator.Update(cvAIUpdateInterval.value);
                if (bodyInputBank)
                {
                    bodyInputs.moveVector = ai.localNavigator.moveVector;
                    bodyInputs.desiredAimDirection = (targetLookPosition - bodyInputBank.aimOrigin).normalized;
                }
                lookTimer -= Time.fixedDeltaTime;
                if (lookTimer <= 0f)
                {
                    PickNewTargetLookPosition();
                }
                bool flag = false;
                if (targetPosition.HasValue)
                {
                    float sqrMagnitude = (body.footPosition - targetPosition.Value).sqrMagnitude;
                    float num = body.radius * body.radius * 4f;
                    flag = sqrMagnitude > num;
                }
                if(refreshTimer <= 0f || !flag)
                {
                    refreshTimer = refreshTime;
                    SetNewRandomTarget(broadNavigationAgent);
                }
            }
        }

        private Vector3? PickRandomReachablePosition()
        {
            if (!ai || !body)
            {
                return null;
            }
            NodeGraph nodeGraph = SceneInfo.instance.GetNodeGraph(body.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
            var nodeList = nodeGraph.FindNodesInRange(bodyTransform.position, 0f, float.PositiveInfinity, (HullMask)(1 << (int)body.hullClassification));

            NodeGraph.NodeIndex node = nodeList[UnityEngine.Random.Range(0, nodeList.Count)];
            if(nodeGraph.GetNodePosition(node, out var position))
            {
                return position;
            }
            return null;


            //NodeGraphSpider nodeGraphSpider = new NodeGraphSpider(nodeGraph, (HullMask)(1 << (int)body.hullClassification));
            //NodeGraph.NodeIndex nodeIndex = nodeGraph.FindClosestNode(bodyTransform.position, body.hullClassification, 50f);
            //nodeGraphSpider.AddNodeForNextStep(nodeIndex);
            //for (int i = 0; i < 6; i++)
            //{
            //    nodeGraphSpider.PerformStep();
            //}
            //List<NodeGraphSpider.StepInfo> collectedSteps = nodeGraphSpider.collectedSteps;
            //if (collectedSteps.Count == 0)
            //{
            //    return null;
            //}
            //int index = Random.Range(0, collectedSteps.Count);
            //NodeGraph.NodeIndex node = collectedSteps[index].node;
            //if (nodeGraph.GetNodePosition(node, out var position))
            //{
            //    return position;
            //}
            //return null;


            //return new Vector3();
        }
    }
}
