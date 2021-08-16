namespace GtsServer.Models
{
    public class Player
    {
        public string Name { get; set; }
        public float X { get; set; } = 20;
        public float Z { get; set; } = 20;
        public string ConnectionId { get; set; }
        public Direction Direction { get; set; } = Direction.Top;
    }
}
