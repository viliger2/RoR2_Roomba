using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RoR2_Roomba
{
    public class DropItemOnDeath : MonoBehaviour, IOnKilledServerReceiver
    {
        public ItemDef itemToDrop;

        public bool dropRandomChest1Item = false;

        public float dropAngle;

        private static PickupDropTable dropTable = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Chest1/dtChest1.asset").WaitForCompletion();

        private void OnEnable()
        {
            gameObject.layer = LayerIndex.defaultLayer.intVal;
        }

        private void FixedUpdate()
        {
            // I LOVE IT
            if(gameObject.layer != LayerIndex.defaultLayer.intVal)
            {
                gameObject.layer = LayerIndex.defaultLayer.intVal;
            }
        }

        public void OnKilledServer(DamageReport damageReport)
        {
            if (NetworkServer.active)
            {
                UniquePickup dropIndex = new UniquePickup(PickupIndex.none);
                if (dropRandomChest1Item)
                {
                    dropIndex = dropTable.GeneratePickup(new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong));
                }
                else if (itemToDrop)
                {
                    dropIndex = new UniquePickup(PickupCatalog.FindPickupIndex(itemToDrop.itemIndex));
                }
                if (dropIndex.pickupIndex != PickupIndex.none)
                {
                    Vector3 vector = Quaternion.AngleAxis(dropAngle, Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                    PickupDropletController.CreatePickupDroplet(dropIndex, transform.position + Vector3.up * 3f, vector, false);
                }
            }
        }
    }
}
