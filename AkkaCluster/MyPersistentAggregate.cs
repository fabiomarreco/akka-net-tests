using System;
using Akka.Actor;
using Akka.Persistence;
using Akka.Streams.Implementation.Fusing;

namespace AkkaCluster
{
    public class GetResponse
    {
        public GetResponse(ActorPath path, string value)
        {
            Path = path;
            Value = value;
        }

        public ActorPath Path { get; }
        public string Value { get; }
    }

    public class Ping : ICmd
    {
        public Ping(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
    public class MyPersistentAggregate : ReceivePersistentActor
    {
        private string _currentValue = string.Empty;
        public override string PersistenceId => Self.Path.Name;

        public MyPersistentAggregate()
        {
            Console.WriteLine($"Starting actor {Self.Path}");
            Command<Get>(_ => Sender.Tell(new GetResponse(Self.Path, _currentValue)));
            Command<Set>(s =>
                 Persist(s, _ => _currentValue = s.Msg));

            Recover<Set>(s => _currentValue = s.Msg);

            Command<Ping>(_ => 
                Console.WriteLine($"Received ping on {Self.Path}"));
        }

        protected override void PostStop()
        {
            Console.WriteLine($"Actor {Self.Path} Stopped");
        }
    }
}