using EntityStates.AI.Walker;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RoR2_Roomba
{
    public class LookBusyFar : LookBusy
    {
        public override void FixedUpdate()
        {
            fixedAge += Time.fixedDeltaTime;
            if (!base.ai || !base.body)
            {
                return;
            }
            if (base.ai.hasAimConfirmation)
            {
                lookTimer -= Time.fixedDeltaTime;
                if (lookTimer <= 0f)
                {
                    PickNewTargetLookDirection();
                }
            }
            if (base.fixedAge >= duration)
            {
                outer.SetNextState(new WanderFar());
            }
        }


    }
}
