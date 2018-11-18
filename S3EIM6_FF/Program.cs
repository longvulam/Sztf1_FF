using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3EIM6_FF
{
    class Program
    {
        private static readonly MetroLane[] Lanes = new MetroLane[3];
        private static readonly string[] Transfers = new string[Lanes.Length - 1];

        static void Main(string[] args)
        {
            ReadFile();

            int j = 0;
            for (int i = 0; i < Lanes.Length; i++)
            {
                for (int l = i + 1; l < Lanes.Length - 1; l++)
                {
                    string transferStation = Lanes[i].GetTransferStation(Lanes[l]);
                    if (transferStation != null)
                    {
                        Transfers[j++] = transferStation;
                    }
                }
            }
            string[] tomb = new string[10];
            string[] newT = new string[tomb.Length + 1];

            Console.WriteLine("Kérem a kezdőállomást");
            string startingStation = Console.ReadLine().Trim(' ');

            Console.WriteLine();

            Console.WriteLine("Kérem a célállomást");
            string destination = Console.ReadLine().Trim(' ');

            Console.WriteLine();

            MetroLane[] startingLanes = GetStartingLanes(startingStation, destination);
            Route route = new Route(startingStation, startingLanes[0]);

            bool success = GetPath(route, destination);
            j = 1;
            while (!success)
            {
                route = new Route(startingStation, startingLanes[j++]);
                success = GetPath(route, destination);
            }

            if (args.Length > 0 && args[0] == "-v")
                Verbose(route);
            else
                NonVerBose(route);
        }

        private static MetroLane[] GetStartingLanes(string startingStation, string destination)
        {
            MetroLane[] foundLanes = new MetroLane[Lanes.Length];
            int k = 0;
            foreach (MetroLane lane in Lanes)
            {
                if (lane.ContainsStation(startingStation))
                {
                    foundLanes[k++] = lane;
                }
            }
            return foundLanes;
        }

        private static void Verbose(Route route)
        {
            Console.WriteLine(
                "A túristának '{0}' állomástól, '{1}' felé, {2} megállót utazik.\n"
                , route.From, route.MovingTowards, route.Distance);

            if (route.Next == null)
            {
                return;
            }

            route = route.Next;
            while (!route.IsEnd)
            {
                Console.WriteLine(
                    "Utána '{0}' állomáson átszállva '{1}' felé {2} megállót halad.\n"
                    , route.From, route.MovingTowards, route.Distance);

                route = route.Next;
            }

            Console.WriteLine(
                "Végül '{0}' állomáson átszáll és '{1}' felé haladva, {2} megállót utazik."
                , route.From, route.MovingTowards, route.Distance);
        }

        private static void NonVerBose(Route route)
        {
            int i = 1;
            Console.WriteLine("{0} - {1} -->> {2} : {3} megállót utazik."
                , i, route.From, route.MovingTowards, route.Distance);

            if (route.Next == null)
            {
                return;
            }

            route = route.Next;
            i++;
            while (!route.IsEnd)
            {
                Console.WriteLine(
                    "{0} - {1} átszáll -->> {2} : {3} megállót utazik."
                    , i, route.From, route.MovingTowards, route.Distance);

                route = route.Next;
                i++;
            }

            Console.WriteLine(
                "{0} - {1}: átszállás -->> {2} : {3} megállót utazik.\n"
                , i, route.From, route.MovingTowards, route.Distance);

            Console.WriteLine("----------- VÉGE ------------");
        }

        private static bool GetPath(Route currentRoute, string destination)
        {
            MetroLane currentLane = currentRoute.Lane;
            if (currentLane.ContainsStation(destination))
            {
                currentRoute.IsEnd = true;
                currentRoute.To = currentLane.GetFormalName(destination);
                return true;
            }

            Transfer[] transferStations = new Transfer[currentLane.StationsLength];
            MetroLane previous = currentRoute.Previous;

            int numberOfTransfers = GetTransfers(currentLane, previous, transferStations);

            if (numberOfTransfers > 0)
            {
                currentRoute.To = transferStations[0].Station;
                if (currentRoute.Distance == 0)
                {
                    return false;
                }
                Route nextRoute = NextRoute(currentRoute, transferStations[0], currentLane);
                currentRoute.Next = nextRoute;

                bool success = GetPath(nextRoute, destination);
                if (!success)
                {
                    currentRoute.To = transferStations[1].Station;
                    if (currentRoute.Distance == 0)
                    {
                        return false;
                    }
                    nextRoute = NextRoute(currentRoute, transferStations[1], currentLane);
                    currentRoute.Next = nextRoute;
                    return GetPath(nextRoute, destination);
                }
                return true;
            }

            return false;

        }

        private static int GetTransfers(MetroLane currentLane, MetroLane previous, Transfer[] transferStations)
        {
            int j = 0;
            for (int i = 0; i < Lanes.Length; i++)
            {
                MetroLane lane = Lanes[i];
                string trStation = currentLane.GetTransferStation(lane);
                if (trStation != null && !lane.Equals(currentLane) && !lane.Equals(previous))
                {
                    transferStations[j++] = new Transfer(trStation, lane);
                    ;
                }
            }
            return j;
        }

        private static Route NextRoute(Route currentRoute, Transfer transferStation, MetroLane currentLane)
        {
            var nextRoute = new Route(transferStation.To, transferStation.Station, currentLane);
            currentRoute.To = transferStation.Station;
            return nextRoute;
        }

        private static void ReadFile()
        {
            StreamReader reader = new StreamReader("./METRO_1transfer.DAT");
            int i = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] split = line.Split(';');
                int numberOfStations = int.Parse(split[0]);
                string[] lane = new string[numberOfStations];
                for (int j = 1; j < split.Length; j++)
                {
                    lane[j - 1] = split[j];
                }

                Lanes[i++] = new MetroLane(lane);
            }
            reader.Close();
            reader.Dispose();
        }
    }
}
