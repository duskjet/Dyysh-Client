using System;
using System.Security.Cryptography;

namespace Dyysh.Security
{   
    /// <summary>
    /// Performs asymmetric encryption and decryption using the implementation of
    ///     the System.Security.Cryptography.RSA algorithm provided by the cryptographic
    ///     service provider (CSP).
    /// </summary>
    class RsaCsp : IDisposable
    {
        private RSACryptoServiceProvider _csp = null;
        private bool _useOAEP = true;
        
        /// <summary>
        /// Initializes a new instance of the RSA Crypto Service Provider class
        ///     with existing key which is located in the specified container
        ///     in machine keystore.
        /// </summary>
        /// <param name="containerName">Name of the Key Container to use.</param>
        public RsaCsp(string containerName)
        {
            var cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey
            };

            _csp = new RSACryptoServiceProvider(cspParams);
        }

        /// <summary>
        /// Encrypts data with the RSA algorithm.
        /// </summary>
        /// <param name="plainText">The data to be encrypted.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(byte[] plainText)
        {
            return _csp.Encrypt(plainText, _useOAEP);
        }

        /// <summary>
        /// Decrypts data with the RSA algorithm.
        /// </summary>
        /// <param name="cypherText">The data to be decrypted.</param>
        /// <returns>The decrypted data, which is the original plain text before encryption.</returns>
        public byte[] Decrypt(byte[] cypherText)
        {
            if (_csp.PublicOnly)
                return _csp.Decrypt(cypherText, _useOAEP);
            else
                throw new CryptographicException("Key container does not have private key for decryption.");
        }

        public void Dispose()
        {
            if (_csp != null)
                _csp.Dispose();
        }
    }
}
