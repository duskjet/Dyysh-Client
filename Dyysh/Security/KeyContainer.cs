using System.Security.Cryptography;

namespace Dyysh.Security
{
    /// <summary>
    /// Provides methods to manage keys and their containers 
    ///     for cryptographic service providers.
    /// </summary>
    class KeyContainer
    {
        private static int _keySize = 2048;

        /// <summary>
        /// Creates new key to use with CryptoServiceProvider
        /// </summary>
        /// <param name="containerName">name of the key container</param>
        public static void Create(string containerName)
        {
            var cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            var csp = new RSACryptoServiceProvider(_keySize, cspParams);
        }

        /// <summary>
        /// Creates or updates the key container with existing key
        /// </summary>
        /// <param name="containerName">name of the key container</param>
        /// <param name="cspBlob">CryptoServiceProvier key to import</param>
        public static void Import(string containerName, byte[] cspBlob)
        {
            var cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            var csp = new RSACryptoServiceProvider(cspParams);
            csp.ImportCspBlob(cspBlob);
        }

        /// <summary>
        /// Extracts key as bytes from key container.
        /// </summary>
        /// <param name="containerName">name of the key container.</param>
        /// <param name="includePrivateParameters">true to include the private key; otherwise, false.</param>
        /// <returns></returns>
        public static byte[] Export(string containerName, bool includePrivateParameters)
        {
            var cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            var csp = new RSACryptoServiceProvider(_keySize, cspParams);
            byte[] key;

            key = csp.ExportCspBlob(includePrivateParameters);

            return key;
        }

        public static string ExportXml(string containerName, bool includePrivateParameters)
        {
            var cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            var csp = new RSACryptoServiceProvider(_keySize, cspParams);

            var key = csp.ToXmlString(includePrivateParameters);

            return key;
        }        

        /// <summary>
        /// Removes a key container from machine keystore.
        /// </summary>
        /// <param name="containerName">container name to remove.</param>
        public static void Delete(string containerName)
        {
            var cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey
            };

            var csp = new RSACryptoServiceProvider(cspParams) { PersistKeyInCsp = false };

            csp.Clear();
        }

        /// <summary>
        /// Defines the size of key in bytes to use with the CSP.
        /// </summary>
        public static int KeySize
        {
            get { return _keySize; }
            set { _keySize = value; }
        }

    }

}
