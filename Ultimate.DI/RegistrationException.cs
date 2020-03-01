using System;

namespace Ultimate.DI
{
    public class RegistrationException : UltimateDiException
    {
        public RegistrationException(string message) : base(message)
        {
        }

        public RegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}