namespace AkkaCluster
{
    public class Get : ICmd
    {
        public int Id { get; }

        public Get(int id)
        {
            Id = id;
        }
    }
}