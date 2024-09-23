namespace DirectorioElectricistas.Services

{
    using System.Security.Cryptography;
    using System.Text;
    public class PasswordService
    {
        public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = Convert.ToBase64String(hmac.Key);
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                passwordHash = Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            using (var hmac = new HMACSHA256(Convert.FromBase64String(storedSalt)))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(computedHash) == storedHash;
            }
        }
    }
}
