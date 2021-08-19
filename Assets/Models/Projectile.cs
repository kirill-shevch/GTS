using UnityEngine;

namespace Assets.Models
{
    public class Projectile
    {
        public Direction Direction { get; set; }
        public float speed { get; set; } = 10;
        public float lifetime { get; set; } = 1.0f;
        public GameObject ProjectileGameObject { get; set; }
        public bool IsOver { get; set; } = false;

        public void Move(float deltaTime)
        {
            var step = speed * deltaTime;
            lifetime -= deltaTime;
            if (lifetime < 0.0f)
            {
                IsOver = true;
                return;
            }
            var targetPosition = new Vector3();
            switch (Direction)
            {
                case Direction.Top:
                    targetPosition = ProjectileGameObject.transform.position + Vector3.forward * speed;
                    break;
                case Direction.Bot:
                    targetPosition = ProjectileGameObject.transform.position + Vector3.back * speed;
                    break;
                case Direction.Left:
                    targetPosition = ProjectileGameObject.transform.position + Vector3.left * speed;
                    break;
                case Direction.Right:
                    targetPosition = ProjectileGameObject.transform.position + Vector3.right * speed;
                    break;
                default:
                    break;
            }
            ProjectileGameObject.transform.position = Vector3.MoveTowards(ProjectileGameObject.transform.position, targetPosition, step);

        }
    }
}
