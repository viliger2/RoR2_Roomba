using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
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

        public static void RebuildString()
        {
            RoR2.Language currentLanguage = RoR2.Language.currentLanguage;
            string description = "";

            List<KeyValuePair<string, string>> output = new List<KeyValuePair<string, string>>();
            RoR2.Language.LoadAllTokensFromFolder(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(RoombaPlugin).Assembly.Location), Language.LanguageFolder, RoR2.Language.currentLanguageName), output);
            foreach (KeyValuePair<string, string> item in output)
            {
                if(item.Key.Equals("ROOMBA_ITEM_POSTER_DESCRIPTION"))
                {
                    description = item.Value;
                    break;
                }
            }

            description = string.Format(description, RoombaConfigs.PosterDamageAdd.Value.ToString("###%"), RoombaConfigs.PosterDamageAddPerStack.Value.ToString("###%"), RoombaConfigs.PosterShieldHealthPercent.Value.ToString("###%"), RoombaConfigs.PosterShieldHealthPercentPerStack.Value.ToString("###%"));

            currentLanguage.SetStringByToken("ROOMBA_ITEM_POSTER_DESCRIPTION", description);
        }
    }
}
