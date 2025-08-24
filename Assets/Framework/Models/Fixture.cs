using System;
namespace Framework.Services
{
    [Serializable]
    public class Fixture
    {
        public Guid Id { get; set; }
        public int LeagueId { get; set; } 
        public int ExternalId { get; set; } 
        public string Matchweek { get; set; } 
        public string HomeTeam { get; set; } 
        public string AwayTeam { get; set; } 
        public string HomeTeamLogo { get; set; } 
        public string AwayTeamLogo { get; set; }
        public DateTime Kickoff { get; set; } 
        public string Status { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public string Location { get; set; }
    }
    
    public static class NameExtensions
    {
        public static string ToFriendlyName(this string name)
        {
            if (name.Equals("Wolverhampton Wanderers FC", StringComparison.InvariantCultureIgnoreCase))
                return "Wolves";
            if (name.Equals("Tottenham Hotspurs FC", StringComparison.InvariantCultureIgnoreCase))
                return "Tottenham";
            if (name.Equals("Manchester City FC", StringComparison.InvariantCultureIgnoreCase))
                return "Man City";
            return name;
        }
    }
}