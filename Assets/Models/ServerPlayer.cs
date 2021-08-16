namespace Assets.Models
{
    public class ServerPlayer
    {
        public string Name { get; set; }
        public float X { get; set; } = 20;
        public float Z { get; set; } = 20;
        public string ConnectionId { get; set; }
        public Direction Direction { get; set; } = Direction.Top;

        public ClientPlayer ConverToClientPlayer()
        {
            return new ClientPlayer
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
