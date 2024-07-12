using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

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


        public static void PopulateConfigs(ConfigFile Config)
        {
            RoombaSpawnChance = Config.Bind("Roomba", "Chance To Spawn", 30f, "Chance of Roomba to spawn on stage start.");
            MakeVoidExplosionBypassArmor = Config.Bind("Void Death", "Make Void Explosion Bypass Armor", true, "Add BypassArmor flag to all void explosions that result in instant death. Used so you can kill Roomba with it.");
            RoombaNothingWeight = Config.Bind("Roomba", "Roomba with Nothing Weight", 2f, "Weight chance of Roomba with nothing on it. Set to 0 to disable.");
            RoombaMaxwellWeight = Config.Bind("Roomba", "Roomba with Maxwell Weight", 4f, "Weight chance of Roomba with Maxwell on it. Set to 0 to disable.");
            RoombaTVWeight = Config.Bind("Roomba", "Roomba with TV Weight", 4f, "Weight chance of Roomba with TV on it. Set to 0 to disable.");
            CustomItems = Config.Bind("Items", "Enable Custom Items", true, "Enable custom items.");
            RoombaCanDropItems = Config.Bind("Items", "Roomba Drops Normal Chest Items", true, "Roomba will drop items. It will drop item equal to normal chest, if you want to disable custom drops, uncheck Enable Custom Items.");
        }

    }
}
