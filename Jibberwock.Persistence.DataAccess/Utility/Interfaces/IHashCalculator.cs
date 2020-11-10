using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.Utility.Interfaces
{
    /// <summary>
    /// Provides an interface enabling the creation of hashes for an input string.
    /// </summary>
    public interface IHashCalculator
    {
        /// <summary>
        /// Calculates the hash of an input string, returning the output as a base64 string.
        /// </summary>
        /// <param name="inputString">The string to be hashed.</param>
        /// <returns>The calculated hash as a base64 string.</returns>
        string CalculateHash(string inputString);

        /// <summary>
        /// Calculates the hash of an input string, returning the output as a base64 string.
        /// </summary>
        /// <param name="inputString">The string to be hashed.</param>
        /// <param name="salt">The bytes to be used as the salt for the hash.</param>
        /// <returns>The calculated hash as a base64 string.</returns>
        string CalculateHash(string inputString, byte[] salt);
    }
}
