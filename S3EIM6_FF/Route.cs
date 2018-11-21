namespace S3EIM6_FF
{
    class Route
    {
        public Route(string startingStation, MetroLane lane)
        {
            from = lane.GetFormalName(startingStation);
            this.lane = lane;
            isFirst = true;
        }

        public Route(MetroLane lane, string transferStation, MetroLane currentLane)
        {
            this.lane = lane;
            from = transferStation;
            previousLane = currentLane;
        }

        public string From
        {
            get { return from; }
            set { from = value; }
        }

        public string To
        {
            get => to;
            set
            {
                to = value;
                movingTowards = lane.RouteDirection(From, to);
                distance = lane.CalculateDistance(From, to);
            }
        }

        public string MovingTowards
        {
            get { return movingTowards; }
        }

        public MetroLane Lane
        {
            get { return lane; }
        }

        public Route NextRoute
        {
            get { return nextRoute; }
            set { nextRoute = value; }
        }

        public MetroLane PreviousLane
        {
            get { return previousLane; }
        }

        public bool IsEnd
        {
            get { return isEnd; }
            set { isEnd = value; }
        }

        public bool IsFirst
        {
            get { return isFirst; }
        }

        public int Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        private string from;
        private string to;
        private string movingTowards;
        private readonly MetroLane lane;
        private readonly MetroLane previousLane;
        private Route nextRoute;
        private bool isEnd = false;
        private readonly bool isFirst;
        private int distance;
    }
}