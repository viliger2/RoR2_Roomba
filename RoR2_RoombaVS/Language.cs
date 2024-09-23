using System.Collections.Generic;

namespace RoR2_Roomba
{
    public class Language
    {
        public const string LanguageFolder = "Language";

        public static void Language_onCurrentLanguageChanged()
        {
            RoR2.Language currentLanguage = RoR2.Language.currentLanguage;

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(RoombaPlugin).Assembly.Location), Language.LanguageFolder, RoR2.Language.currentLanguageName);
            if (System.IO.Directory.Exists(path))
            {
                List<KeyValuePair<string, string>> output = new List<KeyValuePair<string, string>>();
                RoR2.Language.LoadAllTokensFromFolder(path, output);
                foreach (KeyValuePair<string, string> item in output)
                {
                    switch (item.Key)
                    {
                        case "ROOMBA_ITEM_POSTER_DESCRIPTION":
                            var descriptionPoster = string.Format(item.Value,
                                (RoombaConfigs.PosterDamageAdd.Value / 100).ToString("###%"),
                                (RoombaConfigs.PosterDamageAddPerStack.Value / 100).ToString("###%"),
                                (RoombaConfigs.PosterShieldHealthPercent.Value / 100).ToString("###%"),
                                (RoombaConfigs.PosterShieldHealthPercentPerStack.Value / 100).ToString("###%")
                                );
                            currentLanguage.SetStringByToken("ROOMBA_ITEM_POSTER_DESCRIPTION", descriptionPoster);
                            break;
                        case "ROOMBA_ITEM_MAXWELL_DESCRIPTION":
                            var descriptionMaxwell = string.Format(item.Value,
                                (RoombaConfigs.MaxwellExplosionChance.Value / 100).ToString("###%"),
                                (RoombaConfigs.MaxwellExplosionDamage.Value / 100).ToString("###%"),
                                (RoombaConfigs.MaxwellExplosionDamagePerStack.Value / 100).ToString("###%"),
                                RoombaConfigs.MaxwellExplosionRadius.Value);
                            currentLanguage.SetStringByToken("ROOMBA_ITEM_MAXWELL_DESCRIPTION", descriptionMaxwell);
                            break;
                    }
                }
            }
        }

    }
}
