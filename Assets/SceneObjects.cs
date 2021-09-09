using Assets.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public static class SceneObjects
    {
        public static Dictionary<string, ClientPlayer> ScenePlayers = new Dictionary<string, ClientPlayer>();
        public static Dictionary<string, Projectile> Projectiles = new Dictionary<string, Projectile>();
        public static ClientPlayer UserModel = new ClientPlayer();
        public static GameObject Player;

        public static Object ProjectileModel;
        public static Object FighterGreenModel;
        public static Object FighterRedModel;        
        public static Object LincoreGreenModel;
        public static Object LincoreRedModel;        
        public static Object CruiserGreenModel;
        public static Object CruiserRedModel;

        public static void Initialize()
        {
            FighterGreenModel = Resources.Load("PlayerModels/Fighter_Green");
            FighterRedModel = Resources.Load("PlayerModels/Fighter_Red");
            LincoreGreenModel = Resources.Load("PlayerModels/Lincore_Green");
            LincoreRedModel = Resources.Load("PlayerModels/Lincore_Red");
            CruiserGreenModel = Resources.Load("PlayerModels/Cruiser_Green");
            CruiserRedModel = Resources.Load("PlayerModels/Cruiser_Red");
            ProjectileModel = Resources.Load("Cartoon_Tank_Free/CTF_Prefabs/СTF_Missile_Red");
        }
    }
}
