using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Infrastructure.Security.PasswordHashing
{
    public interface IPasswordHasher
    {
        void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
    }
}
