namespace Framework.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToOrdinal(this int number)
        {
            var lastDigit = number % 10;
            var lastTwoDigits = number % 100;
        
            var suffix = lastTwoDigits switch
            {
                11 or 12 or 13 => "th",
                _ => lastDigit switch
                {
                    1 => "st",
                    2 => "nd",
                    3 => "rd",
                    _ => "th"
                }
            };

            return $"{number}{suffix}";
        }
    }
}