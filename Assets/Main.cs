using Assets;
using Assets.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private GameObject player;

    private float movementSpeed = 2;
    private float synchronizationTime = 1.0f;
    private float fireCoolDownTime = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(640, 480, false);
        var loginButtonObject = GameObject.Find("OkButton");
        loginButtonObject.GetComponentInChildren<Text>().text = "Ok";
        var loginButton = loginButtonObject.GetComponent<Button>();
        var nameFieldObject = GameObject.Find("NameField");
        var nameField = nameFieldObject.GetComponent<InputField>();
        var errorButtonObject = GameObject.Find("ErrorButton");
        errorButtonObject.GetComponentInChildren<Text>().text = "Ok";
        var errorButton = errorButtonObject.GetComponent<Button>();
        var errorText = GameObject.Find("ErrorText").GetComponent<Text>();
        var healthText = GameObject.Find("Health").GetComponent<Text>();
        var userNameText = GameObject.Find("UserName").GetComponent<Text>();
        var invulnerableStatus = GameObject.Find("InvulnerableStatus").GetComponent<Text>();
        loginButton.onClick.AddListener(OnLoginClick);
        errorButton.onClick.AddListener(OnErrorClick);
        errorText.enabled = false;
        healthText.enabled = false;
        userNameText.enabled = false;
        invulnerableStatus.enabled = false;
        errorButtonObject.SetActive(false);
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
        if (player != null)
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
                    var targetPosition = player.transform.position + Vector3.right * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    SceneObjects.UserModel.Direction = Direction.Right;
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.left * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    SceneObjects.UserModel.Direction = Direction.Left;
                }
            }

            if (Input.GetButton("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    var targetPosition = player.transform.position + Vector3.forward * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    SceneObjects.UserModel.Direction = Direction.Top;
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.back * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
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
        SceneObjects.UserModel.X = player.transform.position.x;
        SceneObjects.UserModel.Z = player.transform.position.z;
        ServerHub.Synchronize(SceneObjects.UserModel.ConvertToServerPlayer());
    }

    void OnDestroy()
    {
        ServerHub.CloseConnection();
    }

    void OnLoginClick()
    {
        userName = GameObject.Find("NameField").GetComponent<InputField>().text;
        if (string.IsNullOrWhiteSpace(userName))
        {
            GameObject.Find("OkButton").SetActive(false);
            GameObject.Find("NameField").SetActive(false);
            GameObject.Find("ErrorText").GetComponent<Text>().text = "User name should not be empty!";
            GameObject.Find("ErrorText").GetComponent<Text>().enabled = true;
            GameObject.Find("ErrorButton").SetActive(true);
        }
        else
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Cube);
            player.transform.position = new Vector3(20, 1, 20);
            player.name = userName;
            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;
            var playerRenderer = player.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV());
            SceneObjects.UserModel.Name = userName;
            SceneObjects.UserModel.ConnectionId = ServerHub.Connection.ConnectionId;

            GameObject.Find("OkButton").SetActive(false);
            GameObject.Find("NameField").SetActive(false);
            ServerHub.AddUserName(userName);
            var userNameText = GameObject.Find("UserName").GetComponent<Text>();
            userNameText.text = userName;
            userNameText.enabled = true;
            var healthText = GameObject.Find("Health").GetComponent<Text>();
            healthText.text = SceneObjects.UserModel.Health.ToString();
            healthText.enabled = true;
            var invulnerableStatus = GameObject.Find("InvulnerableStatus").GetComponent<Text>();
            invulnerableStatus.text = SceneObjects.UserModel.IsInvulnerable ? "Invulnerable" : string.Empty;
            invulnerableStatus.enabled = true;
        }
    }

    void OnErrorClick()
    {
        GameObject.Find("OkButton").SetActive(true);
        GameObject.Find("NameField").SetActive(true);
        GameObject.Find("ErrorText").GetComponent<Text>().enabled = false;
        GameObject.Find("ErrorButton").SetActive(false);
    }
}
