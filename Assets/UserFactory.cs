using UnityEngine;

namespace Assets
{
    public static class UserFactory
    {
        public static GameObject CreateUser(string userName, float x, float z)
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Cube);
            player.transform.position = new Vector3(x, 1, z);
            player.name = userName;
            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;
            var playerRenderer = player.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV());
            return player;
        }
    }
}
