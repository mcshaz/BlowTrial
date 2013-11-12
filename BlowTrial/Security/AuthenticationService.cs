using System.Linq;
using System.Text;
using System.Security.Cryptography;
using BlowTrial.Domain.Tables;
using System.Collections.Generic;
using System;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Interfaces;
using BlowTrial.Infrastructure.Interfaces;
using System.Security;
using System.Runtime.InteropServices;

namespace BlowTrial.Security
{
    public interface IAuthenticationService
    {
        IUser AuthenticateUser(string username, SecureString password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        public IUser AuthenticateUser(string username, SecureString password)
        {
            using (var context = new MembershipContext())
            {
                return AuthenticateUser(username, password, context);
            }
        }
        public IUser AuthenticateUser(string username, SecureString clearTextPassword, IMembershipContext context)
        {
            string calculatedHash = CalculateHash(clearTextPassword, username);
            Investigator userData = (from i in context.Investigators.Include("Roles")
                                     where i.Password == calculatedHash
                                     select i).FirstOrDefault();
            if (userData == null)
            {
                throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");
            }
            userData.LastLoginAt = DateTime.Now;
            context.SaveChanges();
            return userData;
        }

        internal static string CalculateHash(SecureString securePassword, string salt)
        {
            IntPtr unmanagedStr = IntPtr.Zero;

            SHA256 hasher = new SHA256Managed();
            byte[] theHash = null;
            GCHandle hashHandle = GCHandle.Alloc(theHash, GCHandleType.Pinned);

            try
            {
                // Need to get an unmanaged reference to the contents of the SecureString.
                int pwLength = securePassword.Length;
                int saltLength = salt.Length;
                unmanagedStr = Marshal.SecureStringToGlobalAllocAnsi(securePassword);

                // We now need to create a byte array of the password in order to compute a SHA hash of for it.
                // --NOTE-- This is the weak point of using this method as we are bringing the contents of our
                // password into managed code. We will try to minimize any exposure risk by immediatly zeroing
                // out the array after the hash is created.
                byte[] pw = new byte[pwLength + saltLength];
                byte[] sb = new byte[saltLength];
                GCHandle pwHandle = GCHandle.Alloc(pw, GCHandleType.Pinned);
                try
                {
                    sb = Encoding.UTF8.GetBytes(salt);
                    Marshal.Copy(unmanagedStr, pw, 0, pwLength);
                    System.Buffer.BlockCopy(sb, 0, pw, pwLength, saltLength);

                    // make the hash and zero out the byte array.
                    theHash = hasher.ComputeHash(pw);
                    return Convert.ToBase64String(theHash, 0, theHash.Length);
                }
                finally
                {
                    for (int i = 0; i < pw.Length; i++)
                    {
                        pw[i] = 0;
                    }
                    for (int i = 0; i < sb.Length; i++)
                    {
                        sb[i] = 0;
                    }
                    if (pwHandle.IsAllocated)
                    {
                        pwHandle.Free();
                    }
                    hasher = null;
                }

            }
            finally
            {
                // Make sure the unmananged string is always zeroed out and released.
                if (unmanagedStr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocAnsi(unmanagedStr);
                }
                if (hashHandle.IsAllocated)
                {
                    hashHandle.Free();
                }
            }
        }
    }
}
