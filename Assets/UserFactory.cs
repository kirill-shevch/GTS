using Assets.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public static class UserFactory
    {
        public static void CreateCurrentUser()
        {
            SceneObjects.Player = UserFactory.CreateUser(SceneObjects.UserModel.Name, -10, -8, SceneObjects.UserModel.Type, true);
            SceneObjects.Player.AddComponent<UserScript>();
            ServerHub.AddUserName(SceneObjects.UserModel.Name, SceneObjects.UserModel.Type);
            SceneObjects.UserModel.Health = 5;
            SceneObjects.UserModel.IsInvulnerable = true;
            SceneObjects.UserModel.InvulnerableTimer = 3;
            SceneObjects.UserModel.MoneyAmount = PlayerPrefs.GetInt("MoneyAmount", 0);

            var healthText = UserInterfaceBehavior.HealthText.GetComponent<Text>();
            healthText.text = SceneObjects.UserModel.Health.ToString();
            healthText.enabled = true;
        }

        public static GameObject CreateUser(string userName, float x, float z, ShipType type, bool isCurrentUser = false)
        {
            GameObject player = null;
            if (isCurrentUser)
            {
                switch (type)
                {
                    case ShipType.Fighter:
                        {
                            player = (GameObject)GameObject.Instantiate(SceneObjects.FighterGreenModel);
                            break;
                        }
                    case ShipType.Lincore:
                        {
                            player = (GameObject)GameObject.Instantiate(SceneObjects.LincoreGreenModel);
                            break;
                        }
                    case ShipType.Cruiser:
                        {
                            player = (GameObject)GameObject.Instantiate(SceneObjects.CruiserGreenModel);
                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case ShipType.Fighter:
                        {
                            player = (GameObject)GameObject.Instantiate(SceneObjects.FighterRedModel);
                            break;
                        }
                    case ShipType.Lincore:
                        {
                            player = (GameObject)GameObject.Instantiate(SceneObjects.LincoreRedModel);
                            break;
                        }
                    case ShipType.Cruiser:
                        {
                            player = (GameObject)GameObject.Instantiate(SceneObjects.CruiserRedModel);
                            break;
                        }
                    default:
                        break;
                }
            }
            player.transform.position = new Vector3(x, 1, z);
            player.name = userName;
            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;
            var collider = player.AddComponent<BoxCollider>();
            rigidbody.drag = 5;
            return player;
        }

        public static void DeleteCurrentUser()
        {
            ServerHub.Die();
            GameObject.Destroy(SceneObjects.Player);
        }
    }
}
