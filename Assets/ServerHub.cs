using Assets.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
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
            Connection.On("GetMoney", () => GetMoney());
            Connection.On<Dictionary<string, KillDeathAmount>>("BroadcastKDTable", kDTable => ReceiveKDTable(kDTable));
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
                UserFactory.CreateUser(player.Name, player.X, player.Z);
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
            GameObject.Destroy(player);
        }

        public static void Shoot(float x, float z, Direction direction, string shooterName)
        {
            var projectileGameObject = (GameObject)GameObject.Instantiate(SceneObjects.ProjectileModel);
            var rigidbody = projectileGameObject.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;
            var collider = projectileGameObject.AddComponent<BoxCollider>();
            switch (direction)
            {
                case Direction.Top:
                    z += 1.5f;
                    projectileGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Bot:
                    z -= 1.5f;
                    projectileGameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case Direction.Left:
                    x -= 1.5f;
                    projectileGameObject.transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
                case Direction.Right:
                    x += 1.5f;
                    projectileGameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
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

        public static void Die()
        {
            Connection.InvokeAsync("IncrementDeath", SceneObjects.UserModel.Name);
        }

        public static void Kill(string killerName)
        {
            Connection.InvokeAsync("IncrementKill", killerName);
        }

        public static void GetMoney()
        {
            SceneObjects.UserModel.MoneyAmount++;
        }

        public static void ReceiveKDTable(Dictionary<string, KillDeathAmount> kDTable)
        {
            Debug.Log("KD table:");
            foreach (var item in kDTable)
            {
                Debug.Log($"User {item.Key}, Kills: {item.Value.Kills}, Deathes: {item.Value.Deathes}");
            }
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
