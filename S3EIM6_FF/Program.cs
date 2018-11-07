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

        static void Main(string[] args)
        {
            ReadFile();

            Console.WriteLine("Kérem a kezdőállomást");
            string startingStation = Console.ReadLine().Trim(' ');

            Console.WriteLine();

            Console.WriteLine("Kérem a célállomást");
            string destination = Console.ReadLine().Trim(' ');

            Console.WriteLine();

            Route route = new Route(startingStation, GetStartingLane(startingStation, destination));

            GetPath(route, destination);
            if (args.Length > 0 && args[0] == "-v")
                Verbose(route);
            else
                NonVerBose(route);
        }

        private static MetroLane GetStartingLane(string startingStation, string destination)
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

            //while (i < Lanes.Length && !Lanes[i].ContainsStation(station))
            //{
            //    i++;
            //}
            MetroLane startingLane;
            if (foundLanes.Length > 1)
            {
                int i = 0;
                while (i < k && !foundLanes[i].ContainsStation(destination))
                {
                    i++;
                }

                if (i < k)
                {
                    startingLane = foundLanes[i];
                }
                else
                {
                    startingLane = foundLanes[0];
                }

            }
            else
            {
                startingLane = foundLanes[0];
            }

            return startingLane;
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
                    "{0} - {1} -->> {2} : {3} megállót utazik."
                    , i, route.From, route.MovingTowards, route.Distance);

                route = route.Next;
                i++;
            }

            Console.WriteLine(
                "{0} - {1} -->> {2} : {3} megállót utazik.\n"
                , i, route.From, route.MovingTowards, route.Distance);
            Console.WriteLine("----------- VÉGE ------------");
        }

        private static void GetPath(Route currentRoute, string destination, int i = 0)
        {
            MetroLane currentLane = currentRoute.Lane;
            if (currentLane.ContainsStation(destination))
            {
                currentRoute.IsEnd = true;
                currentRoute.To = currentLane.GetFormalName(destination);
                return;
            }

            string transferStation = null;
            int j = 0;
            MetroLane nextLane = null;
            while (j < Lanes.Length && transferStation == null)
            {
                nextLane = Lanes[j];
                if (!nextLane.Equals(currentLane) && !nextLane.Equals(currentRoute.Previous))
                {
                    transferStation = currentLane.GetTransferStation(nextLane);
                }
                j++;
            }

            var nextRoute = new Route(nextLane, transferStation, currentLane);

            currentRoute.Next = nextRoute;
            currentRoute.To = transferStation;

            GetPath(nextRoute, destination, ++i);
        }

        private static MetroLane GetLane(string station)
        {
            int i = 0;
            while (i < Lanes.Length && !Lanes[i].ContainsStation(station))
            {
                i++;
            }

            return Lanes[i];
        }

        private static void ReadFile()
        {
            StreamReader reader = new StreamReader("./METRO_2transfer.DAT");
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
