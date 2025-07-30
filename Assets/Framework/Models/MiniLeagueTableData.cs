using System;
namespace Framework.Services
{
    [Serializable]
    public class MiniLeagueTableData
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int TotalPoints { get; set; }
    }
}