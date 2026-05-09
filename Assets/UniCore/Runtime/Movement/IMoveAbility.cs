namespace KarenKrill.UniCore.Movement
{
    public interface IMoveAbility
    {
        public bool Enabled { get; set; }

        public void Update(MovementContext ctx);
    }
}
