namespace Assets.Models
{
    public class ClientPlayer
    {
        public string Name { get; set; }
        public float X { get; set; } = 20;
        public float Z { get; set; } = 20;
        public string ConnectionId { get; set; }
        public Direction Direction { get; set; } = Direction.Top;
        public float FireCoolDown { get; set; } = 0.3f;

        public ServerPlayer ConvertToServerPlayer()
        {
            return new ServerPlayer
            {
                Name = Name,
                X = X,
                Z = Z,
                ConnectionId = ConnectionId,
                Direction = Direction
            };
        }
    }
}
