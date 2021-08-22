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
        public static Object TankBlueModel;
        public static Object TankRedModel;

        public static void Initialize()
        {
            TankBlueModel = Resources.Load("Cartoon_Tank_Free/CTF_Prefabs/TankFree_Blue");
            TankRedModel = Resources.Load("Cartoon_Tank_Free/CTF_Prefabs/TankFree_Red");
            ProjectileModel = Resources.Load("Cartoon_Tank_Free/CTF_Prefabs/СTF_Missile_Red");
        }
    }
}
