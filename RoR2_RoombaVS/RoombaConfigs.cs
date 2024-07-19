using BepInEx.Configuration;

namespace RoR2_Roomba
{
    public static class RoombaConfigs
    {

        public static ConfigEntry<float> RoombaSpawnChance;
        public static ConfigEntry<bool> MakeVoidExplosionBypassArmor;
        public static ConfigEntry<float> RoombaNothingWeight;
        public static ConfigEntry<float> RoombaMaxwellWeight;
        public static ConfigEntry<float> RoombaTVWeight;
        public static ConfigEntry<bool> CustomItems;
        public static ConfigEntry<bool> RoombaCanDropItems;

        public static ConfigEntry<float> MaxwellExplosionChance;
        public static ConfigEntry<float> MaxwellExplosionDamage;
        public static ConfigEntry<float> MaxwellExplosionDamagePerStack;
        public static ConfigEntry<float> MaxwellExplosionRadius;

        public static ConfigEntry<float> PosterDamageAdd;
        public static ConfigEntry<float> PosterDamageAddPerStack;
        public static ConfigEntry<float> PosterShieldHealthPercent;
        public static ConfigEntry<float> PosterShieldHealthPercentPerStack;

        public static void PopulateConfigs(ConfigFile Config)
        {
            RoombaSpawnChance = Config.Bind("Roomba", "Chance To Spawn", 30f, "Chance of Roomba to spawn on stage start.");
            MakeVoidExplosionBypassArmor = Config.Bind("Void Death", "Make Void Explosion Bypass Armor", true, "Add BypassArmor flag to all void explosions that result in instant death. Used so you can kill Roomba with it.");
            RoombaNothingWeight = Config.Bind("Roomba", "Roomba with Nothing Weight", 8f, "Weight chance of Roomba with nothing on it. Set to 0 to disable.");
            RoombaMaxwellWeight = Config.Bind("Roomba", "Roomba with Maxwell Weight", 4f, "Weight chance of Roomba with Maxwell on it. Set to 0 to disable.");
            RoombaTVWeight = Config.Bind("Roomba", "Roomba with TV Weight", 4f, "Weight chance of Roomba with TV on it. Set to 0 to disable.");
            CustomItems = Config.Bind("Items", "Enable Custom Items", true, "Enable custom items.");
            RoombaCanDropItems = Config.Bind("Items", "Roomba Drops Normal Chest Items", true, "Roomba will drop items. It will drop item equal to normal chest, if you want to disable custom drops, uncheck Enable Custom Items.");

            MaxwellExplosionChance = Config.Bind("Maxwell", "Maxwell Proc Chance", 5f, "Chance to spawn Evil Maxwell.");
            MaxwellExplosionDamage = Config.Bind("Maxwell", "Maxwell Explosion Damage", 400f, "Damage of Evil Maxwell explosion, in percent of total damage.");
            MaxwellExplosionDamagePerStack = Config.Bind("Maxwell", "Maxwell Explosion Damage Per Stack", 400f, "Damage of Evil Maxwell explosion per stack, in percent of total damage.");
            MaxwellExplosionRadius = Config.Bind("Maxwell", "Maxwell Explosion Radius", 11f, "Explosion radius of Evil Maxwell. By default value is the same as explosion of Commando's grenade.");

            PosterDamageAdd = Config.Bind("Poster", "Poster Damage Increase", 5f, "Damage increase of Poster, in percent.");
            PosterDamageAddPerStack = Config.Bind("Poster", "Poster Damage Increase Per Stack", 5f, "Damage increase of Poster, in percent per stack.");
            PosterShieldHealthPercent = Config.Bind("Poster", "Poster Shield Value", 15f, "How much of max health as shield Poster gives.");
            PosterShieldHealthPercentPerStack = Config.Bind("Poster", "Poster Shield Value Per Stack", 15f, "How much of max health as shield Poster gives per stack.");
        }
    }
}
