using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RoR2_Roomba.States
{
    public class RoombaSpawnState : GenericCharacterSpawnState
    {
        //private static PickupDropTable dropTable = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Chest1/dtChest1.asset").WaitForCompletion();

        //private PickupIndex dropPickup = PickupIndex.none;

        public override void OnEnter()
        {
            base.OnEnter();
            //if (NetworkServer.active)
            //{
            //    if (characterBody && characterBody.master)
            //    {
            //        characterBody.master.onBodyDeath.AddListener(OnBodyDeath);
            //        dropPickup = dropTable.GenerateDrop(new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong));
            //    }
            //}
        }

        //public override void OnExit()
        //{
        //    base.OnExit();
        //    if (NetworkServer.active)
        //    {
        //        if (characterBody && characterBody.master)
        //        {
        //            characterBody.master.onBodyDeath.RemoveListener(OnBodyDeath);
        //        }
        //    }
        //}

        //private void OnBodyDeath()
        //{
        //    if(NetworkServer.active) 
        //    {
        //        if(!(dropPickup == PickupIndex.none))
        //        {
        //            Vector3 vector = Vector3.up * 20 + transform.forward * 2; // numbers taken from chest1
        //            PickupDropletController.CreatePickupDroplet(dropPickup, transform.position + Vector3.up * 1.5f, vector);
        //        }
        //    }
        //}
    }
}
