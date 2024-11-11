﻿namespace Shared
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class AllReadyExistsException : Exception 
    {
        public AllReadyExistsException() : base() { }

        public AllReadyExistsException(string message) : base(message) { }

        public AllReadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }

}