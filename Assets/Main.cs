using Assets;
using Assets.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    

    private float movementSpeed = 2;
    private float synchronizationTime = 1.0f;
    private float fireCoolDownTime = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
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
        if (SceneObjects.Player != null)
        {
            synchronizationTime -= Time.deltaTime;

            if (synchronizationTime <= 0.0f)
            {
                TimerEnded();
            }
            if (SceneObjects.UserModel.IsInvulnerable)
            {
                SceneObjects.UserModel.InvulnerableTimer -= Time.deltaTime;
                if (SceneObjects.UserModel.InvulnerableTimer <= 0.0f)
                {
                    SceneObjects.UserModel.IsInvulnerable = false;
                    var invulnerableStatus = GameObject.Find("InvulnerableStatus").GetComponent<Text>();
                    invulnerableStatus.text = string.Empty;
                }
            }
            if (SceneObjects.UserModel.IsOnCoolDown)
            {
                fireCoolDownTime -= Time.deltaTime;
                if (fireCoolDownTime <= 0.0f)
                {
                    SceneObjects.UserModel.IsOnCoolDown = false;
                }
            }
            if (Input.GetButton("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.right * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    SceneObjects.UserModel.Direction = Direction.Right;
                }
                else
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.left * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    SceneObjects.UserModel.Direction = Direction.Left;
                }
            }

            if (Input.GetButton("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.forward * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    SceneObjects.UserModel.Direction = Direction.Top;
                }
                else
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.back * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    SceneObjects.UserModel.Direction = Direction.Bot;
                }
            }

            if (Input.GetButton("Fire1") && !SceneObjects.UserModel.IsOnCoolDown)
            {
                ServerHub.CreateProjectile(SceneObjects.UserModel.X,
                    SceneObjects.UserModel.Z,
                    SceneObjects.UserModel.Direction,
                    SceneObjects.UserModel.Name);
                fireCoolDownTime = 0.3f;
                SceneObjects.UserModel.IsOnCoolDown = true;
            }
        }
    }

    private void TimerEnded()
    {
        synchronizationTime = 0.05f;
        SceneObjects.UserModel.X = SceneObjects.Player.transform.position.x;
        SceneObjects.UserModel.Z = SceneObjects.Player.transform.position.z;
        ServerHub.Synchronize(SceneObjects.UserModel.ConvertToServerPlayer());
    }

    void OnDestroy()
    {
        ServerHub.CloseConnection();
    }
}
