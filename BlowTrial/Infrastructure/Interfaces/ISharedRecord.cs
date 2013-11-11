using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    interface ISharedRecord
    {
        int Id { get; set; }
        DateTime RecordLastModified { get; set; }
    }
}
