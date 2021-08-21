using Assets.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using UnityEngine;

namespace Assets
{
    public static class ServerHub
    {
        public static HubConnection Connection;

        public static void Initialize()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/userHub")
                .Build();
            Connection.StartAsync();
            Connection.On<ServerPlayer>("SendUser", x => ReceiveUser(x));
            Connection.On<string>("RemoveUser", x => RemoveUser(x));
            Connection.On<float, float, Direction, string>("Shoot", (x, z, direction, shooterName) => Shoot(x, z, direction, shooterName));
        }

        public static void ReceiveUser(ServerPlayer player)
        {
            if (player.Name == SceneObjects.UserModel.Name)
            {
                return;
            }
            else if (!SceneObjects.ScenePlayers.ContainsKey(player.Name))
            {
                SceneObjects.ScenePlayers.Add(player.Name, player.ConverToClientPlayer());
                var newPlayer = UserFactory.CreateUser(player.Name, player.X, player.Z);
            }
            else
            {
                SceneObjects.ScenePlayers[player.Name].X = player.X;
                SceneObjects.ScenePlayers[player.Name].Z = player.Z;
                SceneObjects.ScenePlayers[player.Name].Direction = player.Direction;
                SceneObjects.ScenePlayers[player.Name].IsInvulnerable = player.IsInvulnerable;
                SceneObjects.ScenePlayers[player.Name].Health = player.Health;
            }
        }

        public static void RemoveUser(string name)
        {
            var player = GameObject.Find(name);
            SceneObjects.ScenePlayers.Remove(name);
            player.SetActive(false);
        }

        public static void Shoot(float x, float z, Direction direction, string shooterName)
        {
            var projectileGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            switch (direction)
            {
                case Direction.Top:
                    z++;
                    break;
                case Direction.Bot:
                    z--;
                    break;
                case Direction.Left:
                    x--;
                    break;
                case Direction.Right:
                    x++;
                    break;
                default:
                    break;
            }
            var position = new Vector3(x, 1, z);
            var uid = Guid.NewGuid().ToString();
            projectileGameObject.transform.position = position;
            projectileGameObject.tag = "Projectile";
            projectileGameObject.name = uid;
            projectileGameObject.AddComponent<ProjectileCollisionDetector>();


            SceneObjects.Projectiles.Add(uid, new Projectile
            {
                Uid = uid,
                Direction = direction,
                ProjectileGameObject = projectileGameObject,
                ShooterName = shooterName
            });
        }

        public static void CreateProjectile(float x, float z, Direction direction, string shooterName)
        {
            Connection.InvokeAsync("CreateProjectile",
                SceneObjects.UserModel.X,
                SceneObjects.UserModel.Z,
                SceneObjects.UserModel.Direction,
                SceneObjects.UserModel.Name);
        }

        public static void Synchronize(ServerPlayer player)
        {
            Connection.InvokeAsync("Synchronize", SceneObjects.UserModel.ConvertToServerPlayer());
        }

        public static void CloseConnection()
        {
            Connection.InvokeAsync("RemoveUserName", SceneObjects.UserModel.Name);
            Connection.StopAsync();
        }

        public static void AddUserName(string userName)
        {
            Connection.InvokeAsync("AddUserName", userName, Connection.ConnectionId);
        }
    }
}
