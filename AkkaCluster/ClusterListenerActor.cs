using System;
using Akka.Actor;
using Akka.Cluster;
using ColorConsole;

namespace AkkaCluster
{
    public class ClusterListenerActor : UntypedActor
    {
        protected override void PreStart()
            =>Context.System.EventStream.Subscribe(Self, typeof(ClusterEvent.IMemberEvent));

        protected override void PostStop()
            => Context.System.EventStream.Unsubscribe(Self);

                
        protected override void OnReceive(object message)
        {
            var log = new ConsoleWriter();
            log.SetForeGroundColor(ConsoleColor.Cyan);
            switch (message)
            {
                case ClusterEvent.MemberUp up:
                {
                    var mem = up;
                    log.WriteLine(string.Format("Member is Up: {0}", mem.Member));
                    break;
                }
                case ClusterEvent.UnreachableMember unreachable:
                    log.WriteLine(string.Format("Member detected as unreachable: {0}", unreachable.Member));
                    break;
                case ClusterEvent.MemberRemoved removed:
                    log.WriteLine(string.Format("Member is Removed: {0}", removed.Member));
                    break;
                case ClusterEvent.IMemberEvent _:
                    //IGNORE                
                    break;
                case ClusterEvent.CurrentClusterState _:
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }
    }
}