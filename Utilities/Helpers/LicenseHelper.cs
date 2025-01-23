using System.IO.Compression;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Youxel.Check.LicensesGenerator.Domain.Consts;
using Youxel.Check.LicensesGenerator.Domain.Exceptions;
using Youxel.Check.LicensesGenerator.Domain.Models;

namespace Youxel.Check.LicensesGenerator.Utilities.Helpers;

public static class LicenseHelper
{
    private const int MinRsaKeySize = 4096;
    private static readonly byte[] PrivateKey = Convert.FromBase64String(Constants.PrivateKey);

    public static string GenerateLicenseKey(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Message and key cannot be null or empty.");

        try
        {
            using var rsa = CreateRsaFromPrivateKey();

            // Compress the data
            var messageBytes = Encoding.UTF8.GetBytes(data);
            var compressedMessageBytes = CompressData(messageBytes);

            // Sign the compressed data
            var signatureBytes = rsa.SignData(compressedMessageBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Return Base64 strings of compressed data and signature
            return Convert.ToBase64String(compressedMessageBytes) + "." + Convert.ToBase64String(signatureBytes);
        }
        catch (Exception ex)
        {
            throw new LicenseValidationException("License signing failed.", ex);
        }
    }

    public static LicenseVM GetLicense(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Message and key cannot be null or empty.", nameof(data));

        try
        {
            var decodedLicenseRequest = DecodeRSA(data);
            var license = JsonSerializer.Deserialize<LicenseVM>(decodedLicenseRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });


            if (license == null)
            {
                throw new LicenseValidationException("License deserialization resulted in null.", null!);
            }

            return license;
        }
        catch (Exception ex)
        {
            throw new LicenseValidationException("License deserialization failed.", ex);
        }
    }

    private static string DecodeRSA(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message and key cannot be null or empty.");

        try
        {
            using var rsa = CreateRsaFromPrivateKey();

            if (rsa.KeySize < MinRsaKeySize)
                throw new SecurityException($"RSA key size must be at least {MinRsaKeySize} bits.");

            var messageBytes = Convert.FromBase64String(message);
            var decryptedBytes = rsa.Decrypt(messageBytes, RSAEncryptionPadding.OaepSHA1);
            var decodedMessage = Encoding.UTF8.GetString(DecompressData(decryptedBytes));

            return decodedMessage;
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid Base64 string.", ex);
        }
    }

    private static byte[] CompressData(byte[] data)
    {
        using var outputStream = new MemoryStream();
        using (var compressionStream = new GZipStream(outputStream, CompressionLevel.Optimal))
        {
            compressionStream.Write(data, 0, data.Length);
        }

        return outputStream.ToArray();
    }

    private static byte[] DecompressData(byte[] data)
    {
        using var inputStream = new MemoryStream(data);
        using var outputStream = new MemoryStream();
        using (var decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
        {
            decompressionStream.CopyTo(outputStream);
        }

        return outputStream.ToArray();
    }

    private static RSA CreateRsaFromPrivateKey()
    {
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(PrivateKey, out _);

        if (rsa.KeySize < MinRsaKeySize)
            throw new SecurityException($"RSA key size must be at least {MinRsaKeySize} bits.");

        return rsa;
    }
}