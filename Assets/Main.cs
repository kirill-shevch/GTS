using Assets;
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
        Screen.SetResolution(640, 480, false);
    }

    void Update()
    {
        var step = movementSpeed * Time.deltaTime;
        foreach (var scenePlayer in SceneObjects.ScenePlayers)
        {
            var oldPlayer = GameObject.Find(scenePlayer.Key);
            var destination = new Vector3(scenePlayer.Value.X, 1, scenePlayer.Value.Z);
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
    }
}
