using Assets.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class ProjectileCollisionDetector : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            Projectile projectile;
            if (SceneObjects.Projectiles.TryGetValue(transform.name, out projectile))
            {
                if (projectile.ShooterName != collision.gameObject.name)
                {
                    projectile.IsOver = true;
                }
                if (collision.gameObject.name == SceneObjects.UserModel.Name && 
                    projectile.ShooterName != SceneObjects.UserModel.Name)
                {
                    SceneObjects.UserModel.Health--;
                    SceneObjects.UserModel.InvulnerableTimer = 0.5f;
                    SceneObjects.UserModel.IsInvulnerable = true;
                    projectile.IsOver = true;
                    var healthText = GameObject.Find("Health").GetComponent<Text>();
                    healthText.text = SceneObjects.UserModel.Health.ToString();
                }
            }
        }
    }
}
