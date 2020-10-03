using System;
namespace LeagueAPI.Exceptions
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException()
        {
        }

        public PlayerNotFoundException(string message) : base(message)
        {
        }

        public PlayerNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
