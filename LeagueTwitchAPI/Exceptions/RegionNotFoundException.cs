using System;
namespace LeagueAPI.Exceptions
{
    public class RegionNotFoundException : Exception
    {
        public RegionNotFoundException()
        {
        }

        public RegionNotFoundException(string message) : base(message)
        {
        }

        public RegionNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
