using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Akka.Routing;
using AkkaCluster.FSharp;
using ColorConsole;

namespace AkkaCluster
{
    public class Node : INode
    {
        private ActorSystem? _system;
        private IActorRef? _clusterRegion;
        public int NodeId { get; }

        public Node(int nodeId)
        {
            NodeId = nodeId;
            
        }

        public void Start()
        {
            var port = 8080 + NodeId;
            var console = new ConsoleWriter();
            console.SetForeGroundColor(ConsoleColor.Green);

            var baseConfig = ConfigurationFactory
                .FromResource("AkkaCluster.akka.conf", typeof(Program).Assembly)
                .WithFallback(ConfigurationFactory.Default());
            var config = ConfigurationFactory
                .ParseString($@"akka.remote.dot-netty.tcp.port={port}")
                .WithFallback(baseConfig);

            _system = ActorSystem.Create("ClusterSystem", config);
            console.WriteLine($"Starting seed node {port} on port {port}");
            _system.ActorOf<ClusterListenerActor>();



            _clusterRegion = ClusterSharding.Get(_system)
                .StartAsync(
                    typeName: "my-aggregate-type",
                    entityProps: Props.Create<MyPersistentAggregate>(),
                    settings: ClusterShardingSettings.Create(_system),
                    messageExtractor: new MessageExtractor())
                .Result;

        }

        public void Stop()
        {
            var shutdown = CoordinatedShutdown.Get(_system).Run(CoordinatedShutdown.ClrExitReason.Instance);
            shutdown.Wait();
            _clusterRegion = null;
            _system = null;
        }

        public void Set(int entityId, string message)
             => _clusterRegion?.Tell(new Set(entityId, message));

        public string Get(int entityId)
        {
            if (_clusterRegion is null)
                return "Err: region not found";

            var response = _clusterRegion?.Ask<GetResponse>(new Get(entityId)).Result;
            return $"{response.Value} (from {response.Path.ToString()})";
        }

        public void Ping(int entity)
        {
            _clusterRegion?.Tell(new Ping(entity));
        }

        public void Broadcast()
        {
            _clusterRegion?.Tell(new Broadcast(new Ping(0)));
        }
    }
}