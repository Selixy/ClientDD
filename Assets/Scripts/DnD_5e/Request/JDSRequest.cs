namespace DnD.DnD_5e
{
    public class JDSRequest : Request<int>
    {
        public Entity_DnD_5e Target { get; }
        public DnDRollType   StatType { get; }
        public ContextRoll   Context { get; }

        public JDSRequest(Entity_DnD_5e target, DnDRollType statType, ContextRoll context)
        {
            Target   = target;
            StatType = statType;
            Context  = context;
        }

        public override void Execute()
        {
            int result = Target.RequestJDS(StatType, Context).total;
            Resolve(result);
        }
    }
}
