using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Interfaces
{
    public interface IUser
    {
        Guid Id { get; set; }
        string Username { get; set; }
        ICollection<BlowTrial.Domain.Tables.Role> Roles { get; set; }
    }
}
