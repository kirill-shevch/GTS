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
                    projectile.ShooterName != SceneObjects.UserModel.Name &&
                    !SceneObjects.UserModel.IsInvulnerable)
                {
                    SceneObjects.UserModel.Health--;
                    if (SceneObjects.UserModel.Health == 0)
                    {
                        UserFactory.DeleteCurrentUser();
                        UserInterfaceBehavior.ShowMessageText("You are dead! Not big surprise!");
                        return;
                    }
                    else
                    {
                        SceneObjects.UserModel.InvulnerableTimer = 0.5f;
                        SceneObjects.UserModel.IsInvulnerable = true;
                        var invulnerableStatus = UserInterfaceBehavior.InvulnerableStatusText.GetComponent<Text>();
                        invulnerableStatus.text = "Invulnerable";
                    }
                    projectile.IsOver = true;
                    var healthText = UserInterfaceBehavior.HealthText.GetComponent<Text>();
                    healthText.text = SceneObjects.UserModel.Health.ToString();
                }
            }
        }
    }
}
