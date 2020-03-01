using System;

namespace Ultimate.DI
{
    public abstract class UltimateDiException : Exception
    {
        protected UltimateDiException(string message) : base(message)
        {
        }

        protected UltimateDiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}