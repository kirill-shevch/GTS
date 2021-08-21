using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public static class UserInterfaceBehavior
    {
        public static GameObject ErrorButton;
        public static GameObject ErrorText;
        public static GameObject LoginButton;
        public static GameObject NameInput;
        public static GameObject UserNameText;
        public static GameObject HealthText;
        public static GameObject InvulnerableStatusText;

        public static void Initialize()
        {
            LoginButton = GameObject.Find("OkButton");
            ErrorButton = GameObject.Find("ErrorButton");
            ErrorText = GameObject.Find("ErrorText");
            NameInput = GameObject.Find("NameField");
            UserNameText = GameObject.Find("UserName");
            HealthText = GameObject.Find("Health");
            InvulnerableStatusText = GameObject.Find("InvulnerableStatus");


            LoginButton.GetComponentInChildren<Text>().text = "Ok";
            var loginButton = LoginButton.GetComponent<Button>();
            var nameInput = NameInput.GetComponent<InputField>();
            ErrorButton.GetComponentInChildren<Text>().text = "Ok";
            var errorButton = ErrorButton.GetComponent<Button>();
            var errorText = ErrorText.GetComponent<Text>();
            var healthText = HealthText.GetComponent<Text>();
            var userNameText = UserNameText.GetComponent<Text>();
            var invulnerableStatus = InvulnerableStatusText.GetComponent<Text>();
            loginButton.onClick.AddListener(OnLoginClick);
            errorButton.onClick.AddListener(OnErrorClick);
            errorText.enabled = false;
            healthText.enabled = false;
            userNameText.enabled = false;
            invulnerableStatus.enabled = false;
            ErrorButton.SetActive(false);
        }

        private static void OnLoginClick()
        {
            var userName = NameInput.GetComponent<InputField>().text;
            if (string.IsNullOrWhiteSpace(userName))
            {
                LoginButton.SetActive(false);
                NameInput.SetActive(false);
                ErrorText.GetComponent<Text>().text = "User name should not be empty!";
                ErrorText.GetComponent<Text>().enabled = true;
                ErrorButton.SetActive(true);
            }
            else if (SceneObjects.ScenePlayers.ContainsKey(userName))
            {
                LoginButton.SetActive(false);
                NameInput.SetActive(false);
                ErrorText.GetComponent<Text>().text = "User with this name is already exists!";
                ErrorText.GetComponent<Text>().enabled = true;
                ErrorButton.SetActive(true);
            }
            else
            {
                SceneObjects.Player = UserFactory.CreateUser(userName, -10, -5);
                SceneObjects.Player.AddComponent<UserScript>();
                SceneObjects.UserModel.Name = userName;
                SceneObjects.UserModel.ConnectionId = ServerHub.Connection.ConnectionId;

                LoginButton.SetActive(false);
                NameInput.SetActive(false);
                ServerHub.AddUserName(userName);
                var userNameText = UserNameText.GetComponent<Text>();
                userNameText.text = userName;
                userNameText.enabled = true;
                var healthText = HealthText.GetComponent<Text>();
                healthText.text = SceneObjects.UserModel.Health.ToString();
                healthText.enabled = true;
                var invulnerableStatus = InvulnerableStatusText.GetComponent<Text>();
                invulnerableStatus.text = SceneObjects.UserModel.IsInvulnerable ? "Invulnerable" : string.Empty;
                invulnerableStatus.enabled = true;
            }
        }

        private static void OnErrorClick()
        {
            LoginButton.SetActive(true);
            NameInput.SetActive(true);
            ErrorText.GetComponent<Text>().enabled = false;
            ErrorButton.SetActive(false);
        }
    }
}
