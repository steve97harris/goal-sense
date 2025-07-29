using System;

namespace Framework.Services
{
    [Serializable]
    public class MiniLeague
    {
        public Guid Id { get; set; }
        public string InviteCode { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid[] Members { get; set; } = Array.Empty<Guid>();
    }
}