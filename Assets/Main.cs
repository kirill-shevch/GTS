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

    private float movementSpeed = 2;
    private float timeModifier = 0.02f;
    private float step;

    private string userName = "";

    private HubConnection connection;

    private Dictionary<string, Player> scenePlayers = new Dictionary<string, Player>();

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
        step = movementSpeed * timeModifier;
    }

    void Update()
    {
        foreach (var scenePlayer in scenePlayers)
        {
            var oldPlayer = GameObject.Find(scenePlayer.Key);
            if (scenePlayer.Value.IsMovingRight)
            {
                var targetPosition = oldPlayer.transform.position + Vector3.right * movementSpeed;
                oldPlayer.transform.position = Vector3.MoveTowards(oldPlayer.transform.position, targetPosition, step);
            }
            if (scenePlayer.Value.IsMovingLeft)
            {
                var targetPosition = oldPlayer.transform.position + Vector3.left * movementSpeed;
                oldPlayer.transform.position = Vector3.MoveTowards(oldPlayer.transform.position, targetPosition, step);
            }
            if (scenePlayer.Value.IsMovingForward)
            {
                var targetPosition = oldPlayer.transform.position + Vector3.forward * movementSpeed;
                oldPlayer.transform.position = Vector3.MoveTowards(oldPlayer.transform.position, targetPosition, step);
            }            
            if (scenePlayer.Value.IsMovingBack)
            {
                var targetPosition = oldPlayer.transform.position + Vector3.back * movementSpeed;
                oldPlayer.transform.position = Vector3.MoveTowards(oldPlayer.transform.position, targetPosition, step);
            }
        }
        if (player != null)
        {
            if (Input.GetButton("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    var targetPosition = player.transform.position + Vector3.right * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.left * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                }
                //connection.InvokeAsync("SetCoordinate", userName, player.transform.position.x, player.transform.position.z);
            }
            if (Input.GetButtonDown("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    connection.InvokeAsync("SetMovingRightStatus", userName, true);
                    Debug.Log("Start moving right!");
                }
                else
                {
                    connection.InvokeAsync("SetMovingLeftStatus", userName, true);
                    Debug.Log("Start moving left!");
                }
            }
            if (Input.GetButtonUp("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    connection.InvokeAsync("SetMovingRightStatus", userName, false);
                    Debug.Log("Stop moving right!");
                }
                else
                {
                    connection.InvokeAsync("SetMovingLeftStatus", userName, false);
                    Debug.Log("Stop moving left!");
                }
            }

            if (Input.GetButton("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    var targetPosition = player.transform.position + Vector3.forward * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.back * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                }
            }
            if (Input.GetButtonDown("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    connection.InvokeAsync("SetMovingForwardStatus", userName, true);
                    Debug.Log("Start moving forward!");
                }
                else
                {
                    connection.InvokeAsync("SetMovingBackStatus", userName, true);
                    Debug.Log("Start moving back!");
                }
            }
            if (Input.GetButtonUp("Vertical"))
            {
                var horizontalInput = Input.GetAxis("Vertical");
                if (horizontalInput > 0)
                {
                    connection.InvokeAsync("SetMovingForwardStatus", userName, false);
                    Debug.Log("Stop moving forward!");
                }
                else
                {
                    connection.InvokeAsync("SetMovingBackStatus", userName, false);
                    Debug.Log("Stop moving back!");
                }
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
            else if (!scenePlayers.ContainsKey(player.Key))
            {
                scenePlayers.Add(player.Key, new Player { Name = player.Key, X = player.Value.X, Z = player.Value.Z });
                var newPlayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newPlayer.transform.position = new Vector3(player.Value.X, 1, player.Value.Z);
                newPlayer.name = player.Key;
                var newPlayerRenderer = newPlayer.GetComponent<Renderer>();
                newPlayerRenderer.material.SetColor("_Color", Random.ColorHSV());
            }
            else 
            {
                var oldPlayer = GameObject.Find(player.Key);
                scenePlayers[player.Key].IsMovingForward = player.Value.IsMovingForward;
                scenePlayers[player.Key].IsMovingBack = player.Value.IsMovingBack;
                scenePlayers[player.Key].IsMovingLeft = player.Value.IsMovingLeft;
                scenePlayers[player.Key].IsMovingRight = player.Value.IsMovingRight;
                //oldPlayer.transform.position = new Vector3(player.Value.X, 1, player.Value.Z);
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
