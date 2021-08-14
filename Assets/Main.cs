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
    private float synchronizationTime = 1.0f;
    private float step;

    private string userName = "";
    private Player userModel = new Player();

    private HubConnection connection;

    private Dictionary<string, Player> scenePlayers = new Dictionary<string, Player>();

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(640, 480, false);
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
        connection.On<Player> ("SendUser", x => ReceiveUser(x));
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
            synchronizationTime -= Time.deltaTime;

            if (synchronizationTime <= 0.0f)
            {
                timerEnded();
            }
            if (Input.GetButton("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    var targetPosition = player.transform.position + Vector3.right * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.IsMovingRight = true;
                    userModel.IsMovingLeft = false;
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.left * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.IsMovingLeft = true;
                    userModel.IsMovingRight = false;
                }
            }
            else if (userModel.IsMovingRight)
            {
                connection.InvokeAsync("SetMovingRightStatus", userName, false);
                userModel.IsMovingRight = false;
            }
            else if (userModel.IsMovingLeft)
            {
                connection.InvokeAsync("SetMovingLeftStatus", userName, false);
                userModel.IsMovingLeft = false;
            }
            if (Input.GetButtonDown("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    connection.InvokeAsync("SetMovingRightStatus", userName, true);
                    userModel.IsMovingRight = true;
                    Debug.Log("Start moving right!");
                }
                else
                {
                    connection.InvokeAsync("SetMovingLeftStatus", userName, true);
                    userModel.IsMovingLeft = true;
                    Debug.Log("Start moving left!");
                }
            }
            if (Input.GetButtonUp("Horizontal"))
            {
                connection.InvokeAsync("SetMovingRightStatus", userName, false);
                userModel.IsMovingRight = false;
                connection.InvokeAsync("SetMovingLeftStatus", userName, false);
                userModel.IsMovingLeft = false; 
            }

            if (Input.GetButton("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    var targetPosition = player.transform.position + Vector3.forward * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.IsMovingForward = true;
                    userModel.IsMovingBack = false;
                }
                else
                {
                    var targetPosition = player.transform.position + Vector3.back * movementSpeed;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, step);
                    userModel.IsMovingBack = true;
                    userModel.IsMovingForward = false;
                }
            }
            else if (userModel.IsMovingForward)
            {
                connection.InvokeAsync("SetMovingForwardStatus", userName, false);
                userModel.IsMovingForward = false;
            }
            else if (userModel.IsMovingBack)
            {
                connection.InvokeAsync("SetMovingBackStatus", userName, false);
                userModel.IsMovingBack = false;
            }
            if (Input.GetButtonDown("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    connection.InvokeAsync("SetMovingForwardStatus", userName, true);
                    userModel.IsMovingForward = true;
                }
                else
                {
                    connection.InvokeAsync("SetMovingBackStatus", userName, true);
                    userModel.IsMovingBack = true;
                }
            }
            if (Input.GetButtonUp("Vertical"))
            {
                connection.InvokeAsync("SetMovingForwardStatus", userName, false);
                userModel.IsMovingForward = false;
                connection.InvokeAsync("SetMovingBackStatus", userName, false);
                userModel.IsMovingBack = false;
            }
        }
    }

    private void timerEnded()
    {
        synchronizationTime = 0.5f;
        userModel.X = player.transform.position.x;
        userModel.Z = player.transform.position.z;
        connection.InvokeAsync("Synchronize", userModel);
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
            userModel.Name = userName;

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

    void ReceiveUser(Player player)
    {
        if (player.Name == userName)
        {
            return;
        }
        else if (!scenePlayers.ContainsKey(player.Name))
        {
            scenePlayers.Add(player.Name, new Player { Name = player.Name, X = player.X, Z = player.Z });
            var newPlayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newPlayer.transform.position = new Vector3(player.X, 1, player.Z);
            newPlayer.name = player.Name;
            var newPlayerRenderer = newPlayer.GetComponent<Renderer>();
            newPlayerRenderer.material.SetColor("_Color", Random.ColorHSV());
        }
        else
        {
            var oldPlayer = GameObject.Find(player.Name);
            scenePlayers[player.Name].IsMovingForward = player.IsMovingForward;
            scenePlayers[player.Name].IsMovingBack = player.IsMovingBack;
            scenePlayers[player.Name].IsMovingLeft = player.IsMovingLeft;
            scenePlayers[player.Name].IsMovingRight = player.IsMovingRight;
            if (Mathf.Abs(oldPlayer.transform.position.x - player.X) > 1)
            {
                oldPlayer.transform.position = new Vector3(player.X, 1, oldPlayer.transform.position.z);
            }
            if (Mathf.Abs(oldPlayer.transform.position.z - player.Z) > 1)
            {
                oldPlayer.transform.position = new Vector3(oldPlayer.transform.position.x, 1, player.Z);
            }
            //oldPlayer.transform.position = new Vector3(player.X, 1, player.Z);
        }
    }

    void RemoveUser(string name)
    {
        var player = GameObject.Find(name);
        scenePlayers.Remove(name);
        player.SetActive(false);
    }
}
