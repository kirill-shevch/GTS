namespace GtsServer.Models
{
    public class ServerPlayer
    {
        public string Name { get; set; }
        public float X { get; set; } = -10;
        public float Z { get; set; } = -8;
        public string ConnectionId { get; set; }
        public Direction Direction { get; set; } = Direction.Top;
        public int Health { get; set; } = 5;
        public bool IsInvulnerable { get; set; } = true;
    }
}
