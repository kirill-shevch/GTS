using Assets.Models;
using System.Collections.Generic;

namespace Assets
{
    public static class SceneObjects
    {
        public static Dictionary<string, ClientPlayer> ScenePlayers = new Dictionary<string, ClientPlayer>();
        public static Dictionary<string, Projectile> Projectiles = new Dictionary<string, Projectile>();
        public static ClientPlayer UserModel = new ClientPlayer();
    }
}
