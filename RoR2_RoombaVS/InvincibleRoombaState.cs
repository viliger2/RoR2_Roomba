using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2_Roomba
{
    public class InvincibleRoombaState : EntityStates.Idle
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
            }
        }
    }
}
