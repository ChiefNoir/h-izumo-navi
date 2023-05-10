﻿using Abstractions.Models;
using Abstractions.IRepositories;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;
using Infrastructure.Models;

namespace Infrastructure.Repositories
{
    public class HeatZoneRepository : IHeatZoneRepository
    {
        private static readonly double LongitudeMin = 132.617758; // Long
        private static readonly double LongitudeMax = 132.922474;
        private static readonly double LatitudeMin = 35.281593; // Lat
        private static readonly double LatitudeMax = 35.514545; 

        private static readonly double size = 0.005765;

        private readonly DataContext _context;

        private static IEnumerable<HeatZone> GenerateMap()
        {
            var lst = new List<HeatZone>();

            for (var longt = LongitudeMin; longt < LongitudeMax; longt += size)
            {
                for (var lat = LatitudeMin; lat < LatitudeMax; lat += size)
                {
                    lst.Add(new HeatZone
                    {
                        Temperature = 0,
                        ZoneCoordinates = new()
                        {
                            new ZoneCoordinates { X = longt, Y =  lat},
                            new ZoneCoordinates { X = longt, Y =  lat + size},
                            new ZoneCoordinates { X = longt + size, Y =  lat + size},
                            new ZoneCoordinates { X = longt + size, Y =  lat },
                        }
                    });
                }
            }

            return lst;
        }

        public HeatZoneRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HeatZone>> GetAsync(IEnumerable<string> sourceCode)
        {
            if (sourceCode == null || !sourceCode.Any())
            {
                return Enumerable.Empty<HeatZone>();
            }

            var result = GenerateMap();

            if (sourceCode.Contains(SourcesNames.Example))
            {
                foreach (var item in _context.ExampleHits)
                {
                    var tmp = result.FirstOrDefault(x => x.IsIn(item.Longitude, item.Latitude));
                    if (tmp != null)
                    {
                        tmp.Temperature++;
                        if (tmp.HitStatistics.ContainsKey(SourcesNames.Example))
                        {
                            tmp.HitStatistics[SourcesNames.Example]++;
                        }
                        else
                        {
                            tmp.HitStatistics.Add(SourcesNames.Example, 1);
                        }
                    }
                }
            }
            if (sourceCode.Contains(SourcesNames.FakeTwitter))
            {
                foreach (var item in _context.FakeTwitterHit)
                {
                    var tmp = result.FirstOrDefault(x => x.IsIn(item.Longitude, item.Latitude));
                    if (tmp != null)
                    {
                        tmp.Temperature++;
                        if (tmp.HitStatistics.ContainsKey(SourcesNames.FakeTwitter))
                        {
                            tmp.HitStatistics[SourcesNames.FakeTwitter]++;
                        }
                        else
                        {
                            tmp.HitStatistics.Add(SourcesNames.FakeTwitter, 1);
                        }
                    }
                }
            }

            var maxTemp = result.Max(x => x.Temperature);

            // normalize heat
            foreach (var item in result)
            {
                item.Temperature = IntegerExtensions.RoundOff(item.Temperature * 100d / maxTemp);
            }

            return result;
        }
    }
}
