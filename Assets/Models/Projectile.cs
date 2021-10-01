using UnityEngine;

namespace Assets.Models
{
    public class Projectile
    {
        public string Uid { get; set; }
        public float Direction { get; set; }
        public float speed { get; set; } = 10;
        public float lifetime { get; set; } = 1.0f;
        public GameObject ProjectileGameObject { get; set; }
        public string ShooterName { get; set; }
        public bool IsOver { get; set; } = false;
        public int Damage = 1;

        public void Move(float deltaTime)
        {
            var step = speed * deltaTime;
            var angle = Direction * Mathf.Deg2Rad;
            lifetime -= deltaTime;
            if (lifetime < 0.0f || IsOver)
            {
                IsOver = true;
                return;
            }
            var xcordVertical = Mathf.Sin(angle);
            var zcordVertical = Mathf.Cos(angle);
            var targetPosition = new Vector3(ProjectileGameObject.transform.position.x + xcordVertical, 
                1, 
                ProjectileGameObject.transform.position.z + zcordVertical);
            ProjectileGameObject.transform.position = Vector3.MoveTowards(ProjectileGameObject.transform.position, targetPosition, step);

        }
    }
}
