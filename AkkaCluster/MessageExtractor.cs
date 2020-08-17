using System;
using Akka.Cluster.Sharding;

namespace AkkaCluster
{
    public class MessageExtractor : HashCodeMessageExtractor
    {
        public MessageExtractor() : base(3)
        {
        }

        public override string EntityId(object message) =>
            message switch
            {
                ICmd c => c.Id.ToString(),
                _ => throw new InvalidOperationException()
            };
    }
}