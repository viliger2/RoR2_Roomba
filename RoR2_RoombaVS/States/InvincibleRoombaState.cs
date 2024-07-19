using RoR2;
using UnityEngine.Networking;

namespace RoR2_Roomba.States
{
    public class InvincibleRoombaState : EntityStates.Idle
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }
    }
}
