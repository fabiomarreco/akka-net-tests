namespace AkkaCluster
{
    public class Set : ICmd
    {
        public int Id { get; }
        public string Msg { get; }

        public Set(int id, string msg)
        {
            Id = id;
            Msg = msg;
        }
    }
}