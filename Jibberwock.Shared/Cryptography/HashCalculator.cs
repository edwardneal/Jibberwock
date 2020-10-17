using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Cryptography
{
    /// <summary>
    /// Calculates the Argon2 hash of an input string.
    /// </summary>
    public sealed class HashCalculator : IHashCalculator
    {
        public HashCalculator()
        {
        }

        /// <summary>
        /// Calculates the hash of an input string, returning the output as a string.
        /// </summary>
        /// <param name="inputString">The string to be hashed.</param>
        /// <returns>The calculated hash as a string.</returns>
        public string CalculateHash(string inputString)
        {
            return Sodium.PasswordHash.ArgonHashString(inputString).Trim('\0');
        }

        /// <summary>
        /// Calculates the hash of an input string, returning the output as a base64 string.
        /// </summary>
        /// <param name="inputString">The string to be hashed.</param>
        /// <param name="salt">The bytes to be used as the salt for the hash.</param>
        /// <returns>The calculated hash as a base64 string.</returns>
        public string CalculateHash(string inputString, byte[] salt)
        {
            var inputBytes = System.Text.Encoding.Unicode.GetBytes(inputString);
            var hashBytes = Sodium.PasswordHash.ArgonHashBinary(inputBytes, salt);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
