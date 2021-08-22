using UnityEngine;

namespace Assets
{
    public static class UserFactory
    {
        public static GameObject CreateUser(string userName, float x, float z, bool isCurrentUser = false)
        {
            var player = (GameObject)(isCurrentUser ? GameObject.Instantiate(SceneObjects.TankBlueModel) : GameObject.Instantiate(SceneObjects.TankRedModel));
            player.transform.position = new Vector3(x, 1, z);
            player.name = userName;
            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ;
            return player;
        }
    }
}
