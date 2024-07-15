using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using static RoR2.DotController;
using UnityEngine;
using RoR2;

namespace RoR2_Roomba.States
{
    public class PileOfDirtDeath : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            Vector3 vector = (Vector3.up * 20f + Vector3.forward * 5f);
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ContentProvider.Items.PileOfDirt.itemIndex), transform.position + Vector3.up * 3f, vector);

            EntityState.Destroy(gameObject);
        }
    }
}
