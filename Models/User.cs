using System;

namespace Mdoels
{
    using BCrypt.Net;
    class User
    {
        string Username {get;set;} = string.Empty;
        string Email {get;set;} = string.Empty;
        UserRoles Role {get;set;}
        string PasswordHash {get;set;} = string.Empty;
        string HashPassword(string password)
        {
            string hashedPassword = string.Empty;

            hashedPassword = BCrypt.HashPassword(password);

            return hashedPassword;
        }
        bool VerifyPassword(string password)
        {
            return BCrypt.Verify(password, this.PasswordHash);
        }
    }
    enum UserRoles
    {
        Respondent,
        Author,
        Admin
    }
}