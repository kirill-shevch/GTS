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
            }
        }
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
            loginButtonObject.SetActive(false);
            nameFieldObject.SetActive(false);
        }
    }

    void OnErrorClick()
    {
        loginButtonObject.SetActive(true);
        nameFieldObject.SetActive(true);
        errorText.enabled = false;
        errorButtonObject.SetActive(false);
    }
}
