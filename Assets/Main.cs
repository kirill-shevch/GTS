using Assets.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private GameObject player;

    private float movementSpeed = 2;
    private float synchronizationTime = 1.0f;
    private float fireCoolDownTime = 0.3f;

    private string userName = "";
    private ClientPlayer userModel = new ClientPlayer();

    private HubConnection connection;

    private Dictionary<string, ClientPlayer> scenePlayers = new Dictionary<string, ClientPlayer>();
    private List<Projectile> projectiles = new List<Projectile>();

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

        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/userHub")
            .Build();
        connection.StartAsync();
        connection.On<ServerPlayer>("SendUser", x => ReceiveUser(x));
        connection.On<string>("RemoveUser", x => RemoveUser(x));
        connection.On<float, float, Direction>("Shoot", (x, z, direction) => Shoot(x, z, direction));
    }

    void Update()
    {
        var step = movementSpeed * Time.deltaTime;
        foreach (var scenePlayer in scenePlayers)
        {
            var oldPlayer = GameObject.Find(scenePlayer.Key);
            var destination = new Vector3(scenePlayer.Value.X, 1, scenePlayer.Value.Z);
            oldPlayer.transform.position = Vector3.MoveTowards(oldPlayer.transform.position, destination, step);
        }
        foreach (var projectile in projectiles)
        {
            projectile.Move(Time.deltaTime);
            if (projectile.IsOver)
            {
                GameObject.Destroy(projectile.ProjectileGameObject);
            }
        }
        projectiles.RemoveAll(x => x.IsOver);
        if (player != null)
        {
            synchronizationTime -= Time.deltaTime;

            if (synchronizationTime <= 0.0f)
            {
                TimerEnded();
            }
            if (userModel.IsInvulnerable)
            {
                userModel.InvulnerableTimer -= Time.deltaTime;
                if (userModel.InvulnerableTimer <= 0.0f)
                {
                    userModel.IsInvulnerable = false;
                    var invulnerableStatus = GameObject.Find("InvulnerableStatus").GetComponent<Text>();
                    invulnerableStatus.text = string.Empty;
                }
            }
            if (userModel.IsOnCoolDown)
            {
                fireCoolDownTime -= Time.deltaTime;
                if (fireCoolDownTime <= 0.0f)
                {
                    userModel.IsOnCoolDown = false;
                }
            }
            if (Input.GetButton("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    var targetPosition = player.transform.position + Vector3.right * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.Direction = Direction.Right;
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.left * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.Direction = Direction.Left;
                }
            }

            if (Input.GetButton("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    var targetPosition = player.transform.position + Vector3.forward * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.Direction = Direction.Top;
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.back * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.Direction = Direction.Bot;
                }
            }

            if (Input.GetButton("Fire1") && !userModel.IsOnCoolDown)
            {
                connection.InvokeAsync("CreateProjectile", userModel.X, userModel.Z, userModel.Direction, userModel.Name);
                fireCoolDownTime = 0.3f;
                userModel.IsOnCoolDown = true;
            }
        }
    }

    private void Shoot(float x, float z, Direction direction, string shooterName)
    {
        var projectileGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var position = new Vector3(x, 1, z);
        projectileGameObject.transform.position = position;

        projectiles.Add(new Projectile
        {
            Direction = direction,
            ProjectileGameObject = projectileGameObject,
            ShooterName = shooterName
        });
    }

    private void TimerEnded()
    {
        synchronizationTime = 0.05f;
        userModel.X = player.transform.position.x;
        userModel.Z = player.transform.position.z;
        connection.InvokeAsync("Synchronize", userModel.ConvertToServerPlayer());
    }

    void OnDestroy()
    {
        connection.InvokeAsync("RemoveUserName", userName);
        connection.StopAsync();
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
            var playerRenderer = player.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", Random.ColorHSV());
            userModel.Name = userName;
            userModel.ConnectionId = connection.ConnectionId;

            GameObject.Find("OkButton").SetActive(false);
            GameObject.Find("NameField").SetActive(false);
            connection.InvokeAsync("AddUserName", userName, connection.ConnectionId);
            var userNameText = GameObject.Find("UserName").GetComponent<Text>();
            userNameText.text = userName;
            userNameText.enabled = true;
            var healthText = GameObject.Find("Health").GetComponent<Text>();
            healthText.text = userModel.Health.ToString();
            healthText.enabled = true;
            var invulnerableStatus = GameObject.Find("InvulnerableStatus").GetComponent<Text>();
            invulnerableStatus.text = userModel.IsInvulnerable ? "Invulnerable" : string.Empty;
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

    void ReceiveUser(ServerPlayer player)
    {
        if (player.Name == userName)
        {
            return;
        }
        else if (!scenePlayers.ContainsKey(player.Name))
        {
            scenePlayers.Add(player.Name, player.ConverToClientPlayer());
            var newPlayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newPlayer.transform.position = new Vector3(player.X, 1, player.Z);
            newPlayer.name = player.Name;
            var newPlayerRenderer = newPlayer.GetComponent<Renderer>();
            newPlayerRenderer.material.SetColor("_Color", Random.ColorHSV());
        }
        else
        {
            scenePlayers[player.Name].X = player.X;
            scenePlayers[player.Name].Z = player.Z;
            scenePlayers[player.Name].Direction = player.Direction;
            scenePlayers[player.Name].IsInvulnerable = player.IsInvulnerable;
            scenePlayers[player.Name].Health = player.Health;
        }
    }

    void RemoveUser(string name)
    {
        var player = GameObject.Find(name);
        scenePlayers.Remove(name);
        player.SetActive(false);
    }
}
