using System;

namespace Framework.Services
{
    [Serializable]
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Logo { get; set; }
        public int Season { get; set; }
        public string Code { get; set; }
    }
}