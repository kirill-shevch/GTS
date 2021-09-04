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
        public static GameObject MessageText;
        public static GameObject MessageButton;
        public static GameObject UserNamePanel;
        public static GameObject MoneyPanel;
        public static GameObject MoneyAmountText;        
        public static GameObject KillDeathPanel;
        public static GameObject KillDeathListText;

        public static void Initialize()
        {
            LoginButton = GameObject.Find("OkButton");
            ErrorButton = GameObject.Find("ErrorButton");
            ErrorText = GameObject.Find("ErrorText");
            NameInput = GameObject.Find("NameField");
            UserNameText = GameObject.Find("UserName");
            HealthText = GameObject.Find("Health");
            MessageText = GameObject.Find("MessageText");
            MessageButton = GameObject.Find("MessageButton");
            UserNamePanel = GameObject.Find("UserNamePanel");
            MoneyPanel = GameObject.Find("MoneyPanel");
            MoneyAmountText = GameObject.Find("MoneyAmountText");
            KillDeathPanel = GameObject.Find("KillDeathPanel");
            KillDeathListText = GameObject.Find("KillDeathListText");


            LoginButton.GetComponentInChildren<Text>().text = "Ok";
            var loginButton = LoginButton.GetComponent<Button>();
            var nameInput = NameInput.GetComponent<InputField>();
            ErrorButton.GetComponentInChildren<Text>().text = "Ok";
            MessageButton.GetComponentInChildren<Text>().text = "Ok";
            var errorButton = ErrorButton.GetComponent<Button>();
            var messageButton = MessageButton.GetComponent<Button>();
            var errorText = ErrorText.GetComponent<Text>();
            var healthText = HealthText.GetComponent<Text>();
            var moneyAmountText = MoneyAmountText.GetComponent<Text>();
            var userNameText = UserNameText.GetComponent<Text>();
            var messageText = MessageText.GetComponent<Text>();
            var killDeathListText = KillDeathListText.GetComponent<Text>();
            loginButton.onClick.AddListener(OnLoginClick);
            errorButton.onClick.AddListener(OnErrorClick);
            messageButton.onClick.AddListener(OnMessageClick);
            errorText.enabled = false;
            healthText.enabled = false;
            userNameText.enabled = false;
            messageText.enabled = false;
            killDeathListText.enabled = false;
            ErrorButton.SetActive(false);
            MessageButton.SetActive(false);
            UserNamePanel.SetActive(false);
            KillDeathPanel.SetActive(false);
            nameInput.text = PlayerPrefs.GetString("UserName", string.Empty);
            moneyAmountText.text = PlayerPrefs.GetInt("MoneyAmount", 0).ToString();
        }

        public static void ShowMessageText(string message)
        {
            var messageText = MessageText.GetComponent<Text>();
            messageText.text = message;
            messageText.enabled = true;
            MessageButton.SetActive(true);
        }

        public static void OnMessageClick()
        {
            UserFactory.CreateCurrentUser();
            var messageText = MessageText.GetComponent<Text>();
            messageText.enabled = false;
            MessageButton.SetActive(false);
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
                SceneObjects.UserModel.Name = userName;
                SceneObjects.UserModel.ConnectionId = ServerHub.Connection.ConnectionId;

                UserFactory.CreateCurrentUser();

                LoginButton.SetActive(false);
                NameInput.SetActive(false);
                UserNamePanel.SetActive(true);
                KillDeathPanel.SetActive(true);
                var killDeathListText = KillDeathListText.GetComponent<Text>();
                var userNameText = UserNameText.GetComponent<Text>();
                userNameText.text = userName;
                userNameText.enabled = true;
                killDeathListText.enabled = true;
                PlayerPrefs.SetString("UserName", userName);
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
