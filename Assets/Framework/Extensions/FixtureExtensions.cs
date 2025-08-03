using Framework.Services;

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
    }
}