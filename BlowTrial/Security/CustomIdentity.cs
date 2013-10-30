using System;
using System.Security.Principal;

namespace BlowTrial.Security
{
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(Guid id, string name, string[] roles)
        {
            Name = name;
            Roles = roles;
            Id = id;
        }

        public string Name { get; private set; }
        public Guid Id { get; private set; }
        public string[] Roles { get; private set; }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }
}
