using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RoR2_Roomba.Items
{
    public class PileOfDirt
    {
        public static ItemDef CreateItemDef(GameObject pilePickup, Material glassMaterial)
        {
            pilePickup.transform.Find("mdlPileOfDirt/Glass1").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;
            pilePickup.transform.Find("mdlPileOfDirt/Glass2").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;
            pilePickup.transform.Find("mdlPileOfDirt/Glass3").gameObject.GetComponent<MeshRenderer>().material = glassMaterial;

            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.tier = ItemTier.NoTier;
            itemDef.deprecatedTier = ItemTier.NoTier;
            itemDef.name = "PileOfDirt";
            itemDef.nameToken = "ROOMBA_ITEM_PILE_OF_DIRT_NAME";
            itemDef.pickupToken = "ROOMBA_ITEM_PILE_OF_DIRT_PICKUP";
            itemDef.descriptionToken = "ROOMBA_ITEM_PILE_OF_DIRT_DESCRIPTION";
            itemDef.loreToken = "ROOMBA_ITEM_PILE_OF_DIRT_LORE";
            itemDef.pickupModelPrefab = pilePickup;
            itemDef.canRemove = true;
            //itemDef.pickupIconSprite = ;

            return itemDef;
        }
    }
}
