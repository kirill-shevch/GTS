namespace Assets.Models
{
    public class ServerPlayer
    {
        public string Name { get; set; }
        public float X { get; set; } = 20;
        public float Z { get; set; } = 20;
        public string ConnectionId { get; set; }
        public Direction Direction { get; set; } = Direction.Top;
        public int Health { get; set; } = 5;
        public bool IsInvulnerable { get; set; } = true;
        public float InvulnerableTimer { get; set; } = 3;

        public ClientPlayer ConverToClientPlayer()
        {
            return new ClientPlayer
            {
                Name = Name,
                X = X,
                Z = Z,
                ConnectionId = ConnectionId,
                Direction = Direction,
                Health = Health,
                IsInvulnerable = IsInvulnerable,
                InvulnerableTimer = InvulnerableTimer
            };
        }
    }
}
