using Assets.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class UserScript : MonoBehaviour
    {
        private float movementSpeed = 2;
        private float synchronizationTime = 1.0f;
        private float fireCoolDownTime = 0.3f;

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
            if (Input.GetButton("Horizontal"))
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput > 0)
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.right * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    if (SceneObjects.UserModel.Direction != Direction.Right)
                    {
                        if (SceneObjects.UserModel.Type == ShipType.Cruiser)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 90, -90);
                        }
                        else if (SceneObjects.UserModel.Type == ShipType.Fighter || SceneObjects.UserModel.Type == ShipType.Lincore)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 90, 0);
                        }
                        SceneObjects.UserModel.Direction = Direction.Right;
                    }
                }
                else
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.left * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    if (SceneObjects.UserModel.Direction != Direction.Left)
                    {
                        if (SceneObjects.UserModel.Type == ShipType.Cruiser)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 270, -90);
                        }
                        else if (SceneObjects.UserModel.Type == ShipType.Fighter || SceneObjects.UserModel.Type == ShipType.Lincore)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 270, 0);
                        }
                        SceneObjects.UserModel.Direction = Direction.Left;
                    }
                }
            }
            else if (Input.GetButton("Vertical"))
            {
                var varticalInput = Input.GetAxis("Vertical");
                if (varticalInput > 0)
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.forward * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    if (SceneObjects.UserModel.Direction != Direction.Top)
                    {
                        if (SceneObjects.UserModel.Type == ShipType.Cruiser)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 0, -90);
                        }
                        else if (SceneObjects.UserModel.Type == ShipType.Fighter || SceneObjects.UserModel.Type == ShipType.Lincore)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        }
                        SceneObjects.UserModel.Direction = Direction.Top;
                    }
                }
                else
                {
                    var targetPosition = SceneObjects.Player.transform.position + Vector3.back * movementSpeed;
                    SceneObjects.Player.transform.position = Vector3.MoveTowards(SceneObjects.Player.transform.position, targetPosition, step);
                    if (SceneObjects.UserModel.Direction != Direction.Bot)
                    {
                        if (SceneObjects.UserModel.Type == ShipType.Cruiser)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 180, -90);
                        }
                        else if (SceneObjects.UserModel.Type == ShipType.Fighter || SceneObjects.UserModel.Type == ShipType.Lincore)
                        {
                            SceneObjects.Player.transform.rotation = Quaternion.Euler(-90, 180, 0);
                        }
                        SceneObjects.UserModel.Direction = Direction.Bot;
                    }
                }
            }

            if (Input.GetButton("Fire1") && !SceneObjects.UserModel.IsOnCoolDown)
            {
                ServerHub.CreateProjectile(SceneObjects.UserModel.X,
                    SceneObjects.UserModel.Z,
                    SceneObjects.UserModel.Direction,
                    SceneObjects.UserModel.Name);
                fireCoolDownTime = 0.3f;
                SceneObjects.UserModel.IsOnCoolDown = true;
            }
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
