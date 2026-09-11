using System;
using System.Security.Cryptography;
using System.Text;

namespace Darkages.Security
{
    /// <summary>
    /// How a character's password is kept. It used to go into the character file exactly as typed, so anyone
    /// who could read that folder could read every password — and people reuse passwords elsewhere. It is
    /// stored now as PBKDF2 over a random salt, in one string that carries everything needed to check it:
    /// the scheme, the work factor, the salt, and the hash.
    ///
    /// Accounts made before this change still hold the password as typed. <see cref="Verify" /> accepts those
    /// so that nobody is locked out, and says so through <c>needsRehash</c> — the login path stores the hash
    /// at that moment, so an account moves over the first time its owner signs in and never again.
    /// </summary>
    public static class Passwords
    {
        /// <summary>Marks a stored value as hashed. A password typed by a person will not begin with this.</summary>
        private const string Scheme = "pbkdf2-sha256";

        /// <summary>OWASP's 2023 figure for PBKDF2-SHA256. Raising it later does not invalidate old values —
        /// the count is stored with each one, and Verify reads it from there.</summary>
        private const int Iterations = 210_000;

        private const int SaltBytes = 16;
        private const int HashBytes = 32;
        private const char Separator = '$';

        /// <summary>Whether a stored value is already hashed rather than a password written as typed.</summary>
        public static bool IsHashed(string stored) =>
            stored != null && stored.StartsWith(Scheme + Separator, StringComparison.Ordinal);

        public static string Hash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] salt = RandomNumberGenerator.GetBytes(SaltBytes);
            byte[] hash = Derive(password, salt, Iterations);

            return string.Join(
                Separator.ToString(),
                Scheme,
                Iterations.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        /// <summary>
        /// Whether the typed password matches what is stored. <paramref name="needsRehash" /> is true when it
        /// matched but the stored form was a password written as typed, which the caller should replace with
        /// <see cref="Hash" />.
        /// </summary>
        public static bool Verify(string stored, string given, out bool needsRehash)
        {
            needsRehash = false;

            if (stored == null || given == null)
            {
                return false;
            }

            if (!IsHashed(stored))
            {
                // An account from before this change. Compare as it was compared then, and ask to be moved.
                bool same = FixedTimeEquals(stored, given);
                needsRehash = same;

                return same;
            }

            string[] parts = stored.Split(Separator);

            if (parts.Length != 4
                || !int.TryParse(parts[1], out int iterations)
                || iterations <= 0)
            {
                return false;
            }

            byte[] salt;
            byte[] expected;

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actual = Derive(given, salt, iterations, expected.Length);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int length = HashBytes) =>
            Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, length);

        /// <summary>
        /// Comparing the old plain values without leaking their length or first difference through timing.
        /// </summary>
        private static bool FixedTimeEquals(string left, string right) =>
            CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(left), Encoding.UTF8.GetBytes(right));
    }
}
