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

            UserHandler userHandler = new UserHandler();
            MetroLane[] lanes = userHandler.ReadFile();

            string startingStation = "";
            string destination = "";

            userHandler.HandleUserInputs(lanes, ref startingStation, ref destination);

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
