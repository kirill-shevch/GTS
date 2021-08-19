﻿namespace Assets.Models
{
    public class ClientPlayer
    {
        public string Name { get; set; }
        public float X { get; set; } = 20;
        public float Z { get; set; } = 20;
        public string ConnectionId { get; set; }
        public Direction Direction { get; set; } = Direction.Top;
        public bool IsOnCoolDown { get; set; } = false;
        public int Health { get; set; } = 5;
        public bool IsInvulnerable { get; set; } = true;
        public float InvulnerableTimer { get; set; } = 3;


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
