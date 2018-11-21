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
        static void Main(string[] args)
        {
            //////////////////////////////////
            // Preperations and user inputs //
            //////////////////////////////////

            MetroLane[] lanes = ReadFile();
            string startingStation = "";
            string destination = "";
            HandleUserInputs(lanes, ref startingStation, ref destination);

            Seperator();

            ////////////////////
            // Business Logic //
            ////////////////////

            MetroLane[] startingLanes = GetStartingLanes(startingStation, lanes);

            // Init a starting route;
            Route route = new Route(startingStation, startingLanes[0]);

            bool success = GetPath(route, destination, lanes);
            int j = 1;
            while (!success)
            {
                route = new Route(startingStation, startingLanes[j++]);
                success = GetPath(route, destination, lanes);
            }

            //////////////
            // Solution //
            //////////////

            if (args.Length > 0 && args[0] == "-v")
                Verbose(route);
            else
                NonVerBose(route);
        }

        private static void HandleUserInputs(MetroLane[] lanes, ref string startingStation, ref string destination)
        {
            bool isValid = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Jelenlegi állomások:");
                Seperator();
                for (int i = 0; i < lanes.Length; i++)
                {
                    Console.Write(i + 1 + ") ");
                    lanes[i].ListStations();
                }

                Seperator();

                Console.WriteLine("Kérem a kezdőállomást:");
                startingStation = Console.ReadLine();

                Seperator();

                Console.WriteLine("Kérem a célállomást");
                destination = Console.ReadLine().Trim(' ');
                isValid = ValidateInput(destination, lanes) && ValidateInput(startingStation, lanes);
                if (!isValid)
                {
                    Seperator();
                    Console.WriteLine("Valamelyik állomás nincs a megadott metróvonalakon!");
                    Console.WriteLine("Kérem próbálja meg újra az enter lenyomása után.");
                    Console.ReadLine();
                }
            } while (!isValid);
        }

        private static bool ValidateInput(string stationName, MetroLane[] lanes)
        {
            int i = 0;
            while (i < lanes.Length && !lanes[i].ContainsStation(stationName))
            {
                i++;
            }

            bool isValid = false;
            if (i < lanes.Length)
            {
                isValid = true;
            }

            return isValid;
        }

        private static MetroLane[] GetStartingLanes(string startingStation, MetroLane[] lanes)
        {
            MetroLane[] foundLanes = new MetroLane[lanes.Length];
            int k = 0;
            foreach (MetroLane lane in lanes)
            {
                if (lane.ContainsStation(startingStation))
                {
                    foundLanes[k++] = lane;
                }
            }

            return foundLanes;
        }

        private static bool GetPath(Route currentRoute, string destination, MetroLane[] lanes)
        {
            MetroLane currentLane = currentRoute.Lane;
            if (currentLane.ContainsStation(destination))
            {
                currentRoute.IsEnd = true;
                currentRoute.To = currentLane.GetFormalName(destination);
                return true;
            }

            Transfer[] transferStations = new Transfer[currentLane.StationsLength];
            MetroLane previous = currentRoute.PreviousLane;

            int numberOfTransfers = GetTransfers(currentLane, previous, transferStations, lanes);

            if (numberOfTransfers > 0)
            {
                currentRoute.To = transferStations[0].Station;
                if (currentRoute.Distance == 0)
                {
                    return false;
                }
                Route nextRoute = NextRoute(currentRoute, transferStations[0], currentLane);
                currentRoute.NextRoute = nextRoute;

                bool success = GetPath(nextRoute, destination, lanes);
                if (!success)
                {
                    currentRoute.To = transferStations[1].Station;
                    if (currentRoute.Distance == 0)
                    {
                        return false;
                    }
                    nextRoute = NextRoute(currentRoute, transferStations[1], currentLane);
                    currentRoute.NextRoute = nextRoute;
                    return GetPath(nextRoute, destination, lanes);
                }
                return true;
            }

            return false;
        }

        private static int GetTransfers(MetroLane currentLane, MetroLane previous, Transfer[] transferStations, MetroLane[] lanes)
        {
            int j = 0;
            for (int i = 0; i < lanes.Length; i++)
            {
                MetroLane lane = lanes[i];
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

        private static MetroLane[] ReadFile()
        {
            MetroLane[] Lanes = new MetroLane[3];
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

            return Lanes;
        }

        private static void Verbose(Route route)
        {
            Console.WriteLine(
                "A túristának '{0}' állomástól, '{1}' felé, {2} megállót utazik.\n"
                , route.From, route.MovingTowards, route.Distance);

            if (route.NextRoute == null)
            {
                return;
            }

            route = route.NextRoute;
            while (!route.IsEnd)
            {
                Console.WriteLine(
                    "Utána '{0}' állomáson átszállva '{1}' felé {2} megállót halad.\n"
                    , route.From, route.MovingTowards, route.Distance);

                route = route.NextRoute;
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

            if (route.NextRoute == null)
            {
                return;
            }

            route = route.NextRoute;
            i++;
            while (!route.IsEnd)
            {
                Console.WriteLine(
                    "{0} - {1} átszáll -->> {2} : {3} megállót utazik."
                    , i, route.From, route.MovingTowards, route.Distance);

                route = route.NextRoute;
                i++;
            }

            Console.WriteLine(
                "{0} - {1}: átszállás -->> {2} : {3} megállót utazik.\n"
                , i, route.From, route.MovingTowards, route.Distance);

            Console.WriteLine("----------- VÉGE ------------");
        }

        static void Seperator()
        {
            Console.WriteLine("------------------------");
        }
    }
}
