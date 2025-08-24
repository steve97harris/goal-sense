using System;

namespace Framework.Services
{
    [Serializable]
    public class UserData
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}