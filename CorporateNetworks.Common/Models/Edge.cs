namespace CorporateNetworks.Common.Models
{
    public class Edge
    {
        public Edge()
        {
            this.Weight = double.PositiveInfinity;
        }

        public int Parent { get; set; }

        public int Child { get; set; }

        public double Weight { get; set; }
    }
}
