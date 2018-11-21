using System;

namespace S3EIM6_FF
{
    public class MetroLane
    {
        private readonly string[] stations;
        private readonly string A_EndStation;
        private readonly string B_EndStation;

        public MetroLane(string[] stations)
        {
            this.stations = stations;
            A_EndStation = stations[0];
            B_EndStation = stations[stations.Length - 1];
        }

        public int StationsLength
        {
            get { return stations.Length; }
        }

        public void ListStations()
        {
            Console.WriteLine(string.Join(", ",stations));
        }

        public bool ContainsStation(string stationName)
        {
            bool contains = false;
            int i = 0;
            while (i < stations.Length && stations[i].ToLowerInvariant() != stationName.ToLowerInvariant())
            {
                i++;
            }

            if (i < stations.Length)
            {
                contains = true;
            }

            return contains;
        }

        public int CalculateDistance(string firstStation, string secondStation)
        {
            if (!ContainsStation(firstStation) || !ContainsStation(secondStation))
            {
                return 0;
            }

            int[] indexes = GetIndexes(firstStation, secondStation);

            return Math.Abs(indexes[0] - indexes[1]);
        }

        public string RouteDirection(string firstStation, string secondStation)
        {
            if (!ContainsStation(firstStation) || !ContainsStation(secondStation))
            {
                return "";
            }

            int[] indexes = GetIndexes(firstStation, secondStation);

            string endPoint;
            if (stations.Length - indexes[0] > stations.Length - indexes[1])
            {
                endPoint = B_EndStation;
            }
            else
            {
                endPoint = A_EndStation;
            }

            return endPoint;
        }

        public string GetTransferStation(MetroLane otherLane)
        {
            int i = 0;
            while (i < stations.Length && !otherLane.ContainsStation(stations[i]))
            {
                i++;
            }

            string transferStation = null;
            if (i < stations.Length)
            {
                transferStation = stations[i];
            }
            return transferStation;
        }

        public string GetFormalName(string startingStation)
        {
            int i = 0;
            while (i < stations.Length && stations[i].ToLowerInvariant() != startingStation.ToLowerInvariant())
            {
                i++;
            }

            string formalName = "";
            if (i < stations.Length)
            {
                formalName = stations[i];
            }

            return formalName;
        }

        private int[] GetIndexes(string firstStation, string secondStation)
        {
            int[] indexes = new int[2];
            int i = 0;
            string first = firstStation.ToLowerInvariant();
            string second = secondStation.ToLowerInvariant();

            while (i < stations.Length && (indexes[0] == 0 || indexes[1] == 0))
            {
                string currentStationToCheck = stations[i].ToLowerInvariant();
                if (currentStationToCheck == first && currentStationToCheck == second)
                {
                    indexes[0] = i;
                    indexes[1] = i;
                }
                else
                {
                    if (currentStationToCheck == first)
                    {
                        indexes[0] = i;
                    }
                    else
                    {
                        if (currentStationToCheck == second)
                        {
                            indexes[1] = i;
                        }
                    }
                }
                i++;
            }
            return indexes;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return ((MetroLane)obj).A_EndStation == A_EndStation && ((MetroLane)obj).B_EndStation == B_EndStation;
        }
    }
}