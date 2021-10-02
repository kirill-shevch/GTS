using Assets.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets
{
    public class UserScript : MonoBehaviour
    {
        private float movementSpeed = 2;
        private float synchronizationTime = 1.0f;
        private float fireCoolDownTime = 0.3f;

        private InputAction moveAction;
        private InputAction rotateAction;
        private InputAction fireAction;

        private void Start()
        {
            moveAction = GetComponent<PlayerInput>().actions["Move"];
            rotateAction = GetComponent<PlayerInput>().actions["Rotate"];
            fireAction = GetComponent<PlayerInput>().actions["Shoot"];
        }

        private void Update()
        {
            var step = movementSpeed * Time.deltaTime;

            synchronizationTime -= Time.deltaTime;

            if (synchronizationTime <= 0.0f)
            {
                TimerEnded();
            }
            if (SceneObjects.UserModel.IsInvulnerable)
            {
                SceneObjects.UserModel.InvulnerableTimer -= Time.deltaTime;
                if (SceneObjects.UserModel.InvulnerableTimer <= 0.0f && SceneObjects.UserModel.IsInvulnerable)
                {
                    SceneObjects.UserModel.IsInvulnerable = false;
                }
            }
            if (SceneObjects.UserModel.IsOnCoolDown)
            {
                fireCoolDownTime -= Time.deltaTime;
                if (fireCoolDownTime <= 0.0f)
                {
                    SceneObjects.UserModel.IsOnCoolDown = false;
                }
            }
            Move();
            if ((fireAction.ReadValue<float>() == 1 || Input.GetButton("Fire1")) && !SceneObjects.UserModel.IsOnCoolDown)
            {
                ServerHub.CreateProjectile(SceneObjects.UserModel.X,
                    SceneObjects.UserModel.Z,
                    SceneObjects.UserModel.Direction,
                    SceneObjects.UserModel.Name);
                fireCoolDownTime = 0.3f;
                SceneObjects.UserModel.IsOnCoolDown = true;
            }
        }

        private void Move()
        {
            var move = moveAction.ReadValue<Vector2>();
            var angle = transform.eulerAngles.y * Mathf.Deg2Rad;

            //Horizontal
            var xcordHorizontal = Mathf.Cos(angle) * move.x;
            var zcordHorizontal = -Mathf.Sin(angle) * move.x;
            GetComponent<Rigidbody>().AddForce(new Vector3(xcordHorizontal, 0, zcordHorizontal) * 250 * Time.deltaTime);

            //Vertical
            var xcordVertical = Mathf.Sin(angle) * move.y;
            var zcordVertical = Mathf.Cos(angle) * move.y;
            GetComponent<Rigidbody>().AddForce(new Vector3(xcordVertical, 0, zcordVertical) * 500 * Time.deltaTime);

            var look = rotateAction.ReadValue<Vector2>();
            transform.RotateAround(transform.position, Vector3.up, look.x * 30 * Time.deltaTime);

            SceneObjects.UserModel.Direction = transform.eulerAngles.y;
            SceneObjects.UserModel.X = transform.position.x;
            SceneObjects.UserModel.Z = transform.position.z;
        }

        private void TimerEnded()
        {
            synchronizationTime = 0.05f;
            SceneObjects.UserModel.X = SceneObjects.Player.transform.position.x;
            SceneObjects.UserModel.Z = SceneObjects.Player.transform.position.z;
            ServerHub.Synchronize(SceneObjects.UserModel.ConvertToServerPlayer());
        }
    }
}
