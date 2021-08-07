using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public GameObject player;
    private float movementSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.transform.position = new Vector3(20, 1, 20);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput > 0)
            {
                player.transform.position = player.transform.position + new Vector3(movementSpeed, 0, 0);
            }
            else
            {
                player.transform.position = player.transform.position + new Vector3(-movementSpeed, 0, 0);
            }
        }
        if (Input.GetButtonDown("Vertical"))
        {
            var varticalInput = Input.GetAxis("Vertical");
            if (varticalInput > 0)
            {
                player.transform.position = player.transform.position + new Vector3(0, 0, movementSpeed);
            }
            else
            {
                player.transform.position = player.transform.position + new Vector3(0, 0, -movementSpeed);
            }
        }

    }
}
