using System;
namespace BlowTrial.Security
{
    public class AnonymousIdentity : CustomIdentity
    {
        public AnonymousIdentity()
            : base(Guid.Empty, string.Empty, new string[] { })
        { }
    }
}