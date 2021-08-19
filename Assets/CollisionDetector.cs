using UnityEngine;

namespace Assets
{
    public class CollisionDetector : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Collistion!");
        }
    }
}
