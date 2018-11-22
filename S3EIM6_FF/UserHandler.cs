﻿using System;
using System.IO;

namespace S3EIM6_FF
{
    class UserHandler
    {

        public string[] HandleUserInputs(MetroLane[] lanes)
        {
            string [] stations = new string[2];
            string startingStation;
            string destination;
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

            stations[0] = startingStation;
            stations[1] = destination;

            return stations;
        }

        public void Verbose(Route route)
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

        public void NonVerBose(Route route)
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
                    "{0} - {1} :: átszállás -->> {2} : {3} megállót utazik."
                    , i, route.From, route.MovingTowards, route.Distance);

                route = route.NextRoute;
                i++;
            }

            Console.WriteLine(
                "{0} - {1} :: átszállás -->> {2} : {3} megállót utazik.\n"
                , i, route.From, route.MovingTowards, route.Distance);

            Console.WriteLine("----------- VÉGE ------------");
        }

        public static void Seperator()
        {
            Console.WriteLine("------------------------");
        }

        private bool ValidateInput(string stationName, MetroLane[] lanes)
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
    }
}