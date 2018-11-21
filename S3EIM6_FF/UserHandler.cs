using System;
using System.IO;

namespace S3EIM6_FF
{
    class UserHandler
    {
        public MetroLane[] ReadFile()
        {
            MetroLane[] Lanes = new MetroLane[3];
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

            return Lanes;
        }

        public void HandleUserInputs(MetroLane[] lanes, ref string startingStation, ref string destination)
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
    }
}