using System;

namespace Framework.Extensions
{
    public static class NameExtensions
    {
        public static string ToFriendlyLeagueName(this string name)
        {
            if (name.Equals("Primera Division", StringComparison.OrdinalIgnoreCase))
                return "La Liga";
            
            return name;
        }
        
        public static string ToFriendlyTeamName(this string name)
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