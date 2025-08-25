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
            if (name.Equals("Wolverhampton Wanderers FC", StringComparison.OrdinalIgnoreCase))
                return "Wolves";
            if (name.Equals("Tottenham Hotspur FC", StringComparison.OrdinalIgnoreCase))
                return "Tottenham";
            if (name.Equals("Liverpool FC", StringComparison.OrdinalIgnoreCase))
                return "Liverpool";
            if (name.Equals("Burnley FC", StringComparison.OrdinalIgnoreCase))
                return "Burnley";
            if (name.Equals("Aston Villa FC", StringComparison.OrdinalIgnoreCase))
                return "Aston Villa";
            if (name.Equals("Sheffield United FC", StringComparison.OrdinalIgnoreCase))
                return "Sheffield United";
            if (name.Equals("West Bromwich Albion FC", StringComparison.OrdinalIgnoreCase))
                return "West Brom";
            if (name.Equals("Bournemouth FC", StringComparison.OrdinalIgnoreCase))
                return "Bournemouth";
            if (name.Equals("Fulham FC", StringComparison.OrdinalIgnoreCase))
                return "Fulham";
            if (name.Equals("Manchester City FC", StringComparison.OrdinalIgnoreCase))
                return "Man City";
            if (name.Equals("Manchester United FC", StringComparison.OrdinalIgnoreCase))
                return "Man Utd";
            if (name.Equals("Newcastle United FC", StringComparison.OrdinalIgnoreCase))
                return "Newcastle Utd";
            if (name.Equals("Nottingham Forest FC", StringComparison.OrdinalIgnoreCase))
                return "Nottingham Forest";
            if (name.Equals("Leeds United FC", StringComparison.OrdinalIgnoreCase))
                return "Leeds Utd";
            if (name.Equals("Brighton & Hove Albion FC", StringComparison.OrdinalIgnoreCase))
                return "Brighton & Hove";
            if (name.Equals("Chelsea FC", StringComparison.OrdinalIgnoreCase))
                return "Chelsea";
            if (name.Equals("Arsenal FC", StringComparison.OrdinalIgnoreCase))
                return "Arsenal";
            if (name.Equals("West Ham United FC", StringComparison.OrdinalIgnoreCase))
                return "West Ham";
            if (name.Equals("Crystal Palace FC", StringComparison.OrdinalIgnoreCase))
                return "Crystal Palace";
            if (name.Equals("Everton FC", StringComparison.OrdinalIgnoreCase))
                return "Everton";
            if (name.Equals("Southampton FC", StringComparison.OrdinalIgnoreCase))
                return "Southampton";
            return name;
        }
    }
}