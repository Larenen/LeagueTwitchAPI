using System;
namespace LeagueAPI.Exceptions
{
    public class ChampionNotFoundException : Exception
    {
        public ChampionNotFoundException()
        {
        }

        public ChampionNotFoundException(string message) : base(message)
        {
        }

        public ChampionNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
