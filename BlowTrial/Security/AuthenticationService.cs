using System.Linq;
using System.Text;
using System.Security.Cryptography;
using BlowTrial.Domain.Tables;
using System.Collections.Generic;
using System;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Interfaces;
using BlowTrial.Infrastructure.Interfaces;

namespace BlowTrial.Security
{
    public interface IAuthenticationService
    {
        IUser AuthenticateUser(string username, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        public IUser AuthenticateUser(string username, string clearTextPassword)
        {
            using (var context = new MembershipContext())
            {
                return AuthenticateUser(username, clearTextPassword, context);
            }
        }
        public IUser AuthenticateUser(string username, string clearTextPassword, IMembershipContext context)
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

        private static string CalculateHash(string clearTextPassword, string salt)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            // Use the hash algorithm to calculate the hash
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // Return the hash as a base64 encoded string to be compared to the stored password
            return Convert.ToBase64String(hash);
        }
    }
}
