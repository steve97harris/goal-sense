using System;
namespace Framework.Services
{
    [Serializable]
    public class Prediction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FixtureId { get; set; }
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }
        public int PointsAwarded { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsProcessed { get; set; }
    }
}