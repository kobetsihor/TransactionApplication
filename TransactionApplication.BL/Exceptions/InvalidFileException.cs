using System;
using System.Collections.Generic;

namespace TransactionApplication.BL.Exceptions
{
    public class InvalidFileException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public InvalidFileException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}