using R2API;
using RoR2;
using UnityEngine;

namespace RoR2_Roomba.Items
{
    public class Poster
    {
        public static ItemDef CreateItemDef(GameObject posterPickup, Sprite icon)
        {
            Transform focusPoint = posterPickup.transform.Find("FocusPoint");
            Transform cameraPosition = posterPickup.transform.Find("CameraPosition");
            if (focusPoint && cameraPosition)
            {
                var modelPanel = posterPickup.AddComponent<ModelPanelParameters>();
                modelPanel.focusPointTransform = focusPoint;
                modelPanel.cameraPositionTransform = cameraPosition;
                modelPanel.minDistance = 1.5f;
                modelPanel.maxDistance = 3f;
            }

            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.tier = ItemTier.Tier2;
            itemDef.deprecatedTier = ItemTier.Tier2;
            itemDef.name = "Poster";
            itemDef.nameToken = "ROOMBA_ITEM_POSTER_NAME";
            itemDef.pickupToken = "ROOMBA_ITEM_POSTER_PICKUP";
            itemDef.descriptionToken = "ROOMBA_ITEM_POSTER_DESCRIPTION";
            itemDef.loreToken = "ROOMBA_ITEM_POSTER_LORE";
            itemDef.pickupModelPrefab = posterPickup;
            itemDef.canRemove = true;
            itemDef.pickupIconSprite = icon;
            itemDef.tags = new ItemTag[] { ItemTag.WorldUnique };

            return itemDef;
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var count = sender.inventory.GetItemCount(ContentProvider.Items.Poster);
                if (count > 0)
                {
                    args.damageMultAdd += RoombaConfigs.PosterDamageAdd.Value / 100 + RoombaConfigs.PosterDamageAddPerStack.Value / 100 * (count - 1);
                    args.baseShieldAdd += sender.maxHealth * (RoombaConfigs.PosterShieldHealthPercent.Value / 100) + sender.maxHealth * (RoombaConfigs.PosterShieldHealthPercentPerStack.Value / 100) * (count - 1);
                }
            }
        }
    }
}
