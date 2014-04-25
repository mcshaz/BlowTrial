using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Exceptions
{
    [Serializable]
    public class DataKeyException :Exception
    {
        public DataKeyException(string message) : base(message)
        {
        }
    }
    [Serializable]
    public class DataKeyOutOfRangeException : DataKeyException
    {
        public DataKeyOutOfRangeException(string message)
            : base(message)
        {
        }
    }
    [Serializable]
    public class DuplicateDataKeyException : Exception
    {
        public DuplicateDataKeyException(string message)
            : base(message)
        {
        }
    }
    [Serializable]
    public class OverlappingDataKeyRangeException : Exception
    {
        public OverlappingDataKeyRangeException(string message)
            : base(message)
        {
        }
    }
    [Serializable]
    public class InvalidFileTypeException : Exception
    {
        public InvalidFileTypeException(string fileTypeGiven, string fileTypeExpected)
            : base(string.Format("filetype {0} given where {1} expected", fileTypeGiven, fileTypeExpected))
        {
        }
    }
    [Serializable]
    public class InvalidForeignKeyException : Exception
    {
        public InvalidForeignKeyException(string message)
            : base(message)
        {
        }
    }
}
