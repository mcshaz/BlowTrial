using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IBirthday
    {
        int Id { get; }
        DateTime DateTimeBirth { get; }
        int AgeDays { get; set; }
    }
}
