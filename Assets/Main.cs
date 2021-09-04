using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private float movementSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        ServerHub.Initialize();
        UserInterfaceBehavior.Initialize();
        SceneObjects.Initialize();
        Screen.SetResolution(750, 600, false);
    }

    void Update()
    {
        var step = movementSpeed * Time.deltaTime;
        foreach (var scenePlayer in SceneObjects.ScenePlayers)
        {
            var oldPlayer = GameObject.Find(scenePlayer.Key);
            Vector3 destination;
            var xDiff = Math.Abs(oldPlayer.transform.position.x - scenePlayer.Value.X);
            var zDiff = Math.Abs(oldPlayer.transform.position.z - scenePlayer.Value.Z);
            if (xDiff > zDiff)
            {
                destination = new Vector3(scenePlayer.Value.X, 1, oldPlayer.transform.position.z);
            }
            else
            {
                destination = new Vector3(oldPlayer.transform.position.x, 1, scenePlayer.Value.Z);
            }
            switch (scenePlayer.Value.Direction)
            {
                case Assets.Models.Direction.Top:
                    oldPlayer.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Assets.Models.Direction.Bot:
                    oldPlayer.transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case Assets.Models.Direction.Left:
                    oldPlayer.transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
                case Assets.Models.Direction.Right:
                    oldPlayer.transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                default:
                    break;
            }
            oldPlayer.transform.position = Vector3.MoveTowards(oldPlayer.transform.position, destination, step);
        }
        var uidsToDelete = new List<string>();
        foreach (var projectile in SceneObjects.Projectiles.Values)
        {
            projectile.Move(Time.deltaTime);
            if (projectile.IsOver)
            {
                uidsToDelete.Add(projectile.Uid);
                GameObject.Destroy(projectile.ProjectileGameObject);
            }
        }
        uidsToDelete.ForEach(x => SceneObjects.Projectiles.Remove(x));
    }

    void OnDestroy()
    {
        ServerHub.CloseConnection();
        PlayerPrefs.SetInt("MoneyAmount", SceneObjects.UserModel.MoneyAmount);
    }
}
