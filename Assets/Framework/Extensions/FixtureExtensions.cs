using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Services;
using UnityEngine;

namespace Framework.Extensions
{
    public static class FixtureExtensions
    {
        public static string GetStatusShort(this Fixture fixture)
        {
            //SCHEDULED, TIMED, IN_PLAY, PAUSED, FINISHED, SUSPENDED, POSTPONED, CANCELLED, AWARDED
            return fixture.Status switch
            {
                "FINISHED" => "FT",
                "IN_PLAY" => "IP",
                "SCHEDULED" => "SC",
                "TIMED" => "TM",
                "POSTPONED" => "POSTPONED",
                "SUSPENDED" => "SUSPENDED",
                "CANCELLED" => "CANCELLED",
                _ => ""
            };
        }
        
        public static List<Fixture> GetFirstFixturePerGameweek(this List<Fixture> fixtures)
        {
            if (fixtures == null || fixtures.Count == 0)
                return new List<Fixture>();

            return fixtures
                .Where(f => !string.IsNullOrEmpty(f.Matchweek))
                .GroupBy(f => f.Matchweek)
                .Select(group => group.OrderBy(f => f.Kickoff).First())
                .OrderBy(f => int.Parse(f.Matchweek))
                .ToList();
        }

        public static string GetCurrentGameweek(List<Fixture> fixtures, DateTime dateTimeNowGmt)
        {
            var fixturesByGw = fixtures
                .GroupBy(x => x.Matchweek)
                .ToDictionary(x => int.Parse(x.Key), x => 
                    x.OrderBy(f => f.Kickoff).ToList())
                .OrderByDescending(x => x.Key);

            foreach (var pair in fixturesByGw)
            {
                var gwFixtures = pair.Value;
                foreach (var fixture in gwFixtures)
                {
                    if (fixture.Kickoff.AddHours(2) < dateTimeNowGmt)
                        return pair.Key.ToString();
                }
            }

            return "1";
        }
    }
}