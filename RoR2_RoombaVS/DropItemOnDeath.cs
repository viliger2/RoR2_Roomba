using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RoR2_Roomba
{
    public class DropItemOnDeath : MonoBehaviour, IOnKilledServerReceiver
    {
        //public CharacterBody body;

        public ItemDef itemToDrop;

        public bool dropRandomChest1Item = false;

        public float dropAngle;

        private static PickupDropTable dropTable = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Chest1/dtChest1.asset").WaitForCompletion();

        public void OnKilledServer(DamageReport damageReport)
        {
            if (NetworkServer.active)
            {
                PickupIndex dropIndex = PickupIndex.none;
                if (dropRandomChest1Item)
                {
                    dropIndex = dropTable.GenerateDrop(new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong));
                }
                else if (itemToDrop)
                {
                    dropIndex = PickupCatalog.FindPickupIndex(itemToDrop.itemIndex);
                }
                if (dropIndex != PickupIndex.none)
                {
                    Vector3 vector = Quaternion.AngleAxis(dropAngle, Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                    //Quaternion quaternion = Quaternion.AngleAxis(dropAngle, Vector3.up);
                    //vector = quaternion * vector;
                    //Vector3 vector = Quaternion.AngleAxis(dropAngle, Vector3.up) * Vector3.up * 20 + transform.forward * 2; // numbers taken from chest1
                    PickupDropletController.CreatePickupDroplet(dropIndex, transform.position + Vector3.up * 3f, vector);
                }
            }
        }
    }
}
