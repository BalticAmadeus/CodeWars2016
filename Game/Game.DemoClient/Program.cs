using System;
using System.Reflection;
using Game.ClientCommon.Infrastructure;
using Game.ClientCommon.Utilites;
using System.Text;

namespace Game.DemoClient
{
    public class Program
    {
        public static IServiceCallInvoker ServiceCallInvoker { get; private set; }

        static void Main(string[] args)
        {
            Console.Title = $"Game Demo Bot - ver. {Assembly.GetExecutingAssembly().GetName().Version}";
            ServiceCallInvoker = new ServiceCallInvoker(new Logger(), new JsonWebServiceClient());
            ReadParams(args);

            var gameFlowWrapper = new GameFlowWrapper();

            if (gameFlowWrapper.CreatePlayer())
            {
                while (true)
                {

                    Settings.Turn = 0;

                    while (true)
                    {
                        if (!gameFlowWrapper.WaitNextTurn())
                            break;

                        if (!gameFlowWrapper.GetPlayerView())
                            break;

                        //DumpBoard();

                        if (!gameFlowWrapper.PerformMove())
                            break;

                        //Console.ReadKey();
                    }
                }
            }

            Console.WriteLine("DemoClient Terminated");
            Console.ReadKey(true);
        }

        private static void DumpBoard()
        {
            for (var row = 0; row < Settings.Map.Length; row++)
            {
                char[] line = Settings.Map[row].ToCharArray();
                if (Settings.TecmanPosition.Row == row)
                    line[Settings.TecmanPosition.Col] = 'C';

                for (int i = 0; i < Settings.GhostPosition.Length; i++)
                {
                    if (Settings.GhostPosition[i].Row == row)
                        line[Settings.GhostPosition[i].Col] = (char)('1' + i);
                }
                Console.WriteLine(line);
            }
        }

        private static void ReadParams(string[] args)
        {
            Console.WriteLine(@"Enter server url:");
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine(args[0]);
                Settings.ServerUrl = args[0];
            }
            else
            {
                Settings.ServerUrl = Console.ReadLine();
            }


            Console.WriteLine(@"Enter Team name:");
            if (args.Length > 1 && !string.IsNullOrWhiteSpace(args?[1]))
            {
                Console.WriteLine(args[1]);
                Settings.TeamName = args[1];
            }
            else
            {
                Settings.TeamName = Console.ReadLine();
            }

            Console.WriteLine(@"Enter UserName:");
            if (args.Length > 2 && !string.IsNullOrWhiteSpace(args?[2]))
            {
                if (args[2] == "[random]") args[2] = "random" + new Random().Next(100);
                Console.WriteLine(args[2]);
                Settings.UserName = args[2];
            }
            else
            {
                Settings.UserName = Console.ReadLine();
            }

            Console.WriteLine(@"Enter Password:");
            if (args.Length > 3 && !string.IsNullOrWhiteSpace(args?[3]))
            {
                Console.WriteLine("******");
                Settings.Password = args[3];
            }
            else
            {
                Settings.Password = Console.ReadLine();
            }

        }
    }
}
