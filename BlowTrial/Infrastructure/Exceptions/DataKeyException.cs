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
}
