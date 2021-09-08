using Assets;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float movementSpeed = 2;

    // Update is called once per frame
    void Update()
    {
        if (SceneObjects.Player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(SceneObjects.Player.transform.position.x, transform.position.y, SceneObjects.Player.transform.position.z-3.5f),
                movementSpeed);
        }
    }
}
