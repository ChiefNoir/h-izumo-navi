﻿namespace CollectorFake.Models
{
    public class Hit
    {
        public string Source { get; set; } = string.Empty;
        public string PersonId { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PlaceName { get; set; } = string.Empty;
        public DateTime? Date { get; set; } = null;
    }
}
