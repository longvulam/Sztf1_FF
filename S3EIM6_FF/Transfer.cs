namespace S3EIM6_FF
{
    class Transfer
    {
        public Transfer(string trStation, MetroLane lane)
        {
            Station = trStation;
            To = lane;
        }

        public string Station { get; set; }
        public MetroLane To { get; set; }
    }
}