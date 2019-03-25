using System;
using System.Globalization;

namespace WebApi.Helpers
{
    // Clase de ayuda para excepciones personalizadas
    public class AppException : Exception
    {
        public AppException() : base() {}

        public AppException(string mensaje) : base(mensaje) { }

        public AppException(string mensaje, params object[] args) 
            : base(String.Format(CultureInfo.CurrentCulture, mensaje, args))
        {
        }
    }
}