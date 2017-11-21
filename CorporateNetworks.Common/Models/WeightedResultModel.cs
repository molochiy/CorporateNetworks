﻿using System.Text;

namespace CorporateNetworks.Common.Models
{
    public class WeightedResultModel
    {
        public int Node { get; set; }

        public StringBuilder Path { get; set; }

        public double Weight { get; set; }
    }
}
