using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Akka;
using Akka.Remote;
using AkkaCluster.FSharp;
using ColorConsole;
using Console = System.Console;

namespace AkkaCluster
{
    using static ConsoleColor;
    class Program
    {
        static void Main(string[] args)
        {
            var nodes = CreateNodes();
            var parser = new NodeCommandParser();

            do
            {
                var console = new ConsoleWriter();
                console.SetForeGroundColor(Magenta);
                console.Write("Cmd: ");
                var txt = Console.ReadLine();
                if (txt.ToLower() == "q")
                    break;

                try
                {
                    parser.Run(nodes, txt);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            } while (true);
        }

        private static Dictionary<int, INode> CreateNodes()
        {
            var nodes = new Dictionary<int, INode>();
            for (int i = 1; i <= 3; i++)
            {
                var node = new Node(i);
                node.Start();
                nodes[i] = node;
            }

            return nodes;
        }

    }
}
