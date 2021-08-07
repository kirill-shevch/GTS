using Assets.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private GameObject player;

    private GameObject loginButtonObject;
    private Button loginButton;

    private GameObject errorButtonObject;
    private Button errorButton;

    private Text errorText;

    private GameObject nameFieldObject;
    private InputField nameField;

    private float movementSpeed = 1;

    private string userName = "";

    private HubConnection connection;

    private HashSet<string> scenePlayers = new HashSet<string>();

    // Start is called before the first frame update
    void Start()
    {
        loginButtonObject = GameObject.Find("OkButton");
        loginButtonObject.GetComponentInChildren<Text>().text = "Ok";
        loginButton = loginButtonObject.GetComponent<Button>();
        nameFieldObject = GameObject.Find("NameField");
        nameField = nameFieldObject.GetComponent<InputField>();
        errorButtonObject = GameObject.Find("ErrorButton");
        errorButtonObject.GetComponentInChildren<Text>().text = "Ok";
        errorButton = errorButtonObject.GetComponent<Button>();
        errorText = GameObject.Find("ErrorText").GetComponent<Text>();
        loginButton.onClick.AddListener(OnLoginClick);
        errorButton.onClick.AddListener(OnErrorClick);
        errorText.enabled = false;
        errorButtonObject.SetActive(false);

        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/userHub")
            .Build();
        connection.StartAsync();
        connection.On<Dictionary<string, Player>> ("ReceiveUserList", x => ReceiveUserList(x));
        connection.On<string> ("RemoveUser", x => RemoveUser(x));
    }

    void Update()
    {
        if (player != null)
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    player.transform.Translate(movementSpeed, 0, 0);
                }
                else
                {
                    player.transform.Translate(-movementSpeed, 0, 0);
                }
                connection.InvokeAsync("SetCoordinate", userName, player.transform.position.x, player.transform.position.z);
            }
            if (Input.GetButtonDown("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    player.transform.Translate(0, 0, movementSpeed);
                }
                else
                {
                    player.transform.Translate(0, 0, -movementSpeed);
                }
                connection.InvokeAsync("SetCoordinate", userName, player.transform.position.x, player.transform.position.z);
            }
        }
    }

    void OnDestroy()
    {
        connection.InvokeAsync("RemoveUserName", userName);
        connection.StopAsync();
    }

    void OnLoginClick()
    {
        userName = nameField.text;
        if (string.IsNullOrWhiteSpace(userName))
        {
            loginButtonObject.SetActive(false);
            nameFieldObject.SetActive(false);
            errorText.text = "User name should not be empty!";
            errorText.enabled = true;
            errorButtonObject.SetActive(true);
        }
        else
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Cube);
            player.transform.position = new Vector3(20, 1, 20);
            var playerRenderer = player.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", Random.ColorHSV());

            loginButtonObject.SetActive(false);
            nameFieldObject.SetActive(false);
            connection.InvokeAsync("AddUserName", userName);
        }
    }

    void OnErrorClick()
    {
        loginButtonObject.SetActive(true);
        nameFieldObject.SetActive(true);
        errorText.enabled = false;
        errorButtonObject.SetActive(false);
    }

    void ReceiveUserList(Dictionary<string, Player> players)
    {
        foreach (var player in players)
        {
            if (player.Key == userName)
            {
                continue;
            }
            else if (!scenePlayers.Contains(player.Key))
            {
                scenePlayers.Add(player.Key);
                var newPlayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newPlayer.transform.position = new Vector3(player.Value.X, 1, player.Value.Z);
                newPlayer.name = player.Key;
                var newPlayerRenderer = newPlayer.GetComponent<Renderer>();
                newPlayerRenderer.material.SetColor("_Color", Random.ColorHSV());
            }
            else 
            {
                var oldPlayer = GameObject.Find(player.Key);
                oldPlayer.transform.position = new Vector3(player.Value.X, 1, player.Value.Z);
            }
        }
    }

    void RemoveUser(string name)
    {
        var player = GameObject.Find(name);
        scenePlayers.Remove(name);
        player.SetActive(false);
    }
}
