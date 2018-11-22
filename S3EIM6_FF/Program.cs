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

            MetroLane[] lanes = GetLanesFromFile("METRO.DAT");
            UserHandler userHandler = new UserHandler();
            string[] stations = userHandler.HandleUserInputs(lanes);

            string startingStation = stations[0];
            string destination = stations[1];

            UserHandler.Seperator();

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
                userHandler.Verbose(route);
            else
                userHandler.NonVerBose(route);
        }

        private static MetroLane[] GetLanesFromFile(string fileName)
        {
            MetroLane[] lanes = new MetroLane[3];
            StreamReader reader = new StreamReader("./" + fileName);
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

                lanes[i++] = new MetroLane(lane);
            }
            reader.Close();
            reader.Dispose();

            return lanes;
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
            bool success = false;
            if (currentRoute.Lane.ContainsStation(destination))
            {
                currentRoute.IsEnd = true;
                currentRoute.To = currentRoute.Lane.GetFormalName(destination);
                success = true;
            }
            else
            {
                Transfer[] transferStations = currentRoute.GetTransfers(lanes);

                if (transferStations.Length > 0)
                {
                    int i = 0;
                    while (i < transferStations.Length && success == false)
                    {
                        currentRoute.To = transferStations[i].Station;
                        if (currentRoute.Distance != 0)
                        {
                            Route nextRoute = currentRoute.CreateNext(transferStations[i]);
                            currentRoute.NextRoute = nextRoute;
                            success = GetPath(nextRoute, destination, lanes);
                        }
                        i++;
                    }
                }
            }

            return success;
        }
    }
}
