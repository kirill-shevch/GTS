namespace Assets.Models
{
    public class ClientPlayer
    {
        public string Name { get; set; }
        public float X { get; set; } = -10;
        public float Z { get; set; } = -8f;
        public string ConnectionId { get; set; }
        public float Direction { get; set; } = 0;
        public bool IsOnCoolDown { get; set; } = false;
        public int Health { get; set; } = 5;
        public bool IsInvulnerable { get; set; } = true;
        public float InvulnerableTimer { get; set; } = 3;
        public int MoneyAmount { get; set; }
        public ShipType Type { get; set; }


        public ServerPlayer ConvertToServerPlayer()
        {
            return new ServerPlayer
            {
                Name = Name,
                X = X,
                Z = Z,
                ConnectionId = ConnectionId,
                Direction = Direction,
                IsInvulnerable = IsInvulnerable,
                Health = Health,
                Type = Type
            };
        }
    }
}
