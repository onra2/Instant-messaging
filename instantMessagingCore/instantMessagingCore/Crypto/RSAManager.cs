using System;
using System.Security.Cryptography;
using System.Text;

namespace instantMessagingCore.Crypto
{
    public class RSAManager
    {
        private readonly RSAEncryptionPadding EncryptionPadding = RSAEncryptionPadding.Pkcs1;
        private readonly RSASignaturePadding SignaturPadding = RSASignaturePadding.Pkcs1;
        private readonly HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA512;

        private RSA Rsa { get; set; }

        /// <summary>
        /// Return the current key(s) in xml format
        /// </summary>
        /// <param name="withPrivateKey">include private key</param>
        /// <returns>The key(s) in xml format</returns>
        public string GetKey(bool withPrivateKey)
        {
            return Rsa.ToXmlString(withPrivateKey);
        }

        /// <summary>
        /// Instance a RSA with a new keys
        /// </summary>
        public RSAManager()
        {
            Rsa = RSA.Create();
        }

        /// <summary>
        /// Instance a RSA with this key
        /// </summary>
        /// <param name="key">The xml fromated key to use in RSA</param>
        public RSAManager(string key)
        {
            Rsa = RSA.Create();
            Rsa.FromXmlString(key);
        }

        /// <summary>
        /// Encrypt a data byte array
        /// </summary>
        /// <param name="data">The byte array of data to encrypt</param>
        /// <returns>A byte array of data encrypted</returns>
        public byte[] Encrypt(byte[] data)
        {
            return Rsa.Encrypt(data, EncryptionPadding);
        }

        /// <summary>
        /// Decrypt a data byte array
        /// </summary>
        /// <param name="data">The byte array of data to decrypt</param>
        /// <returns>A byte array of data decrypted</returns>
        public byte[] Decrypt(byte[] data)
        {
            return Rsa.Decrypt(data, EncryptionPadding);
        }

        /// <summary>
        /// Sign the data with the private key
        /// </summary>
        /// <param name="data">The data to sign</param>
        /// <returns>The signed data</returns>
        public byte[] Sign(byte[] data)
        {
            return Rsa.SignData(data, hashAlgorithmName, SignaturPadding);
        }

        /// <summary>
        /// Verify the signed data with the public key
        /// </summary>
        /// <param name="data">the data to sign</param>
        /// <param name="signature">The signature to use</param>
        /// <returns>True if the signature is correct</returns>
        public bool Verify(byte[] data, byte[] signature)
        {
            return Rsa.VerifyData(data, signature, hashAlgorithmName, SignaturPadding);
        }

    }
}
