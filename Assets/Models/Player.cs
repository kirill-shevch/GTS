namespace Assets.Models
{
    class Player
    {
        public string Name { get; set; }
        public float X { get; set; } = 20;
        public float Z { get; set; } = 20;
        public bool IsMovingForward { get; set; } = false;
        public bool IsMovingBack { get; set; } = false;
        public bool IsMovingLeft { get; set; } = false;
        public bool IsMovingRight { get; set; } = false;
    }
}
