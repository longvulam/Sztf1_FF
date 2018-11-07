namespace S3EIM6_FF
{
    class Route
    {
        public Route()
        {

        }
        public Route(string startingStation, MetroLane lane)
        {
            From = lane.GetFormalName(startingStation);
            Lane = lane;
            IsFirst = true;
        }

        public Route(MetroLane lane, string transferStation, MetroLane currentLane)
        {
            Lane = lane;
            From = transferStation;
            Previous = currentLane;
        }

        public string From { get; set; }

        public string To
        {
            get => to;
            set
            {
                to = value;
                MovingTowards = Lane.RouteDirection(From, to);
                Distance = Lane.CalculateDistance(From, to);
            }
        }

        public string MovingTowards { get; private set; }
        public MetroLane Lane { get; }
        public Route Next { get; set; }
        public MetroLane Previous { get; }
        public bool IsEnd { get; set; } = false;
        public bool IsFirst { get; }
        public int Distance { get; set; }

        private string to;
    }
}