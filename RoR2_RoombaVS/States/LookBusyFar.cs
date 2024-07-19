using EntityStates.AI.Walker;
using UnityEngine;

namespace RoR2_Roomba.States
{
    public class LookBusyFar : LookBusy
    {
        public override void FixedUpdate()
        {
            fixedAge += Time.fixedDeltaTime;
            if (!ai || !body)
            {
                return;
            }
            if (ai.hasAimConfirmation)
            {
                lookTimer -= Time.fixedDeltaTime;
                if (lookTimer <= 0f)
                {
                    PickNewTargetLookDirection();
                }
            }
            if (fixedAge >= duration)
            {
                outer.SetNextState(new WanderFar());
            }
        }
    }
}
