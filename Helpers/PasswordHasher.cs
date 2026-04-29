namespace onlineShop.Helpers
{
    public static class PasswordHasher
    {
        // Hasher un mot de passe
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Vérifier un mot de passe
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
