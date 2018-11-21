namespace S3EIM6_FF
{
    class Transfer
    {
        private string station;
        private MetroLane to;

        public Transfer(string trStation, MetroLane lane)
        {
            station = trStation;
            to = lane;
        }

        public string Station
        {
            get { return station; }
            set { station = value; }
        }

        public MetroLane To
        {
            get { return to; }
            set { to = value; }
        }
    }
}