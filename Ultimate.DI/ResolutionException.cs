using System;
using System.Collections.Generic;
using System.Text;

namespace Ultimate.DI
{
    public class ResolutionException : UltimateDiException
    {
        public ResolutionException(string message) : base(message)
        {
        }

        public ResolutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
