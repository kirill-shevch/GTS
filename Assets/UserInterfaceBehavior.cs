using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public static class UserInterfaceBehavior
    {
        public static void Initialize()
        {
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

        private static void OnLoginClick()
        {
            var userName = GameObject.Find("NameField").GetComponent<InputField>().text;
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
                SceneObjects.Player = GameObject.CreatePrimitive(PrimitiveType.Cube);
                SceneObjects.Player.transform.position = new Vector3(20, 1, 20);
                SceneObjects.Player.name = userName;
                var rigidbody = SceneObjects.Player.AddComponent<Rigidbody>();
                rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                    RigidbodyConstraints.FreezeRotationX |
                    RigidbodyConstraints.FreezeRotationY |
                    RigidbodyConstraints.FreezeRotationZ;
                var playerRenderer = SceneObjects.Player.GetComponent<Renderer>();
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

        private static void OnErrorClick()
        {
            GameObject.Find("OkButton").SetActive(true);
            GameObject.Find("NameField").SetActive(true);
            GameObject.Find("ErrorText").GetComponent<Text>().enabled = false;
            GameObject.Find("ErrorButton").SetActive(false);
        }
    }
}
