using System.IO.Compression;
using System.Security.Cryptography;
using System.Security;
using System.Text;
using System.Text.Json;
using Youxel.Check.LicensesGenerator.Models;
using System.Text.Json.Serialization;

namespace Youxel.Check.LicensesGenerator.Helpers;

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

    public static License GetLicense(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Message and key cannot be null or empty.", nameof(data));

        try
        {
            var decodedLicenseRequest = DecodeRSA(data);
            var license = JsonSerializer.Deserialize<License>(decodedLicenseRequest, new JsonSerializerOptions
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

public static class Constants
{
    public const string PublicKey = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEApMRMIxOxK0pPUwyEX0lyDUDlXNoVAxV6bXoi09eOzvuH4hDRt8XGoDwP2U4WVzVXMZlSVLJUbgS2jqxIduooZfbC1m1Yw2rlCkK9EMgaxlXvUpApQmGmnKzIKSF/ZGkMUHEeCI9XYey4/Qba0L+nUUhD0t0tESzg1eAn9H8kYFUCpBrRbjPRJ+uzWRBAA4IzZ1H216V9IaZ+2JRpvY20xPnBbN3+T84ejMNURw7L/PXqeE9mCiy9YWrTfDAu1a1aNZT8yuZZIm/MkAj1S5SGcK/8IqIxtsqQfew8rMqIPBEpu14QjIZz5e5uGnhbjfIrOxDU9YTK6WiHhCJnjYlsUA9BcrFtIMJUnr2oPyA+NxpokmHu0t09YuCNzm35KX+T1gAN0E78EFTHAZoRvgJpcNcE2EUYsdBOPMbJA30vrtJWfrQWvt3slqKzBVVz3UQfQsCDQtutt/YH+MuYtUcYVfAm3Yplvpof/tLH8TXjTMQOeXKQpYQcWY8H382ldofOvj9iWZMEjzSZXBcbYxvpQOnjHavy2dFnkTkl6ZcdO1KI8WtQBf3iO3gRj+HH+WP/WcPNBv7jNemaBMp+jsxlCo/kxklvHzKc0OIf4GgrZ9PbtM1YMjcLg2eu6AjufznkGbqQ0RdCCgKx3nZYJh+5a9qDYFYrdrH6+8yIRx9aZO0CAwEAAQ==";

    public const string PrivateKey = "MIIJJwIBAAKCAgEApMRMIxOxK0pPUwyEX0lyDUDlXNoVAxV6bXoi09eOzvuH4hDRt8XGoDwP2U4WVzVXMZlSVLJUbgS2jqxIduooZfbC1m1Yw2rlCkK9EMgaxlXvUpApQmGmnKzIKSF/ZGkMUHEeCI9XYey4/Qba0L+nUUhD0t0tESzg1eAn9H8kYFUCpBrRbjPRJ+uzWRBAA4IzZ1H216V9IaZ+2JRpvY20xPnBbN3+T84ejMNURw7L/PXqeE9mCiy9YWrTfDAu1a1aNZT8yuZZIm/MkAj1S5SGcK/8IqIxtsqQfew8rMqIPBEpu14QjIZz5e5uGnhbjfIrOxDU9YTK6WiHhCJnjYlsUA9BcrFtIMJUnr2oPyA+NxpokmHu0t09YuCNzm35KX+T1gAN0E78EFTHAZoRvgJpcNcE2EUYsdBOPMbJA30vrtJWfrQWvt3slqKzBVVz3UQfQsCDQtutt/YH+MuYtUcYVfAm3Yplvpof/tLH8TXjTMQOeXKQpYQcWY8H382ldofOvj9iWZMEjzSZXBcbYxvpQOnjHavy2dFnkTkl6ZcdO1KI8WtQBf3iO3gRj+HH+WP/WcPNBv7jNemaBMp+jsxlCo/kxklvHzKc0OIf4GgrZ9PbtM1YMjcLg2eu6AjufznkGbqQ0RdCCgKx3nZYJh+5a9qDYFYrdrH6+8yIRx9aZO0CAwEAAQKCAgADqRjH7rgQ7iW1AqIuO8N85+Qwm+e7mc6OgBgl/LP/hTcPnrAKFSrEhJEVktaMOXMmdvGNUCjwCMrFgK5bdfhPy5uoh+Vg8g8Em1b3+HofzejzPKCuCN1avbv0rfnWkT7kyp6QMh8O3eN+O5jA6oEdHS3kkWVe7FBScBhzJh/4w+q7Gp1shVowONxTLjk92rappPiG8fZFuR04UpPG0k8h0Sqq8IOl4j5rgittKLb/d7ct1RW/RB8w+t6X87oWpTNL3XOATn3Mdg9lY82PUE1sU3yWiGPyW626HnV6oUVFkFwM/ZoAhTwt744iqGngTXEIbudMTRb0+I2KMfl6+4Kce5h8/MLyYp4A55zIZvfqtjkGkcRD/X034MZbfw41sdeZhIyzBkJItdp6P2X5MA2ih7kLyQg3fRIGXLCKQkZeAPK6bXyul9jXRDv3LrU7+zqwR9ZfghQYs+p0gcxD3Xd4XefigrgvcCDQpfEWKFfjDKexQynnhnuvkxmkV196CxSzVClNbd71bol8MKGHE8OUxxdUlE3PM+WiB6JyEiKTkDy2eHSxrSGqLc9IPBats2XhU77M8pHJpXjClBTloX1JGxGTOndQMRerIf5O6d+uGY0wtMxOs4WFjMvm0IBwrZdPHxPwTwRvWFVTLsA/NbFWk96Rug/EV3bqCx1WVVaLQQKCAQEA1Skhfr1tFTJMTyxhyLmOQ5/63vFp9jRfjcPUbUhC/dU9EhJgi4gDUrhpPnpHp+xHGnnZqUUX5V+PsVD1oiQMXK9c/D0Nx8smHb76uM1bhA82Ziq8VfIfbrfZZmuFrXz8I4MTrOz6Fk4728WkyZo5PDpIcDhMrfvQGv5YujgTBHO7F2L8W0G9faMSvEtTFkSWSbUMdIFGZ3UXld75amkM2ZUpfL4kzHbif5rcWjsolwMI5sNaC76rgkq7SLiAbDMbOpsQog/6dXXuKkTmx/Ct+6ghxxjndDQt6JJIqz87R5NmAarqDyp9e2V++YRWdA7IC+EFAqiuoE3i563VcD4d3wKCAQEAxeFbhxfwlo9qw0k5hHkcgbsXBU364TJEYSpH0fk1RFSDGeZpnfLTUEQHJhosJqboF1e9ij5pKfaDHLHoyg5mGLBiYjRnJbPhj6NDz+r1AmgL6KYA0Q4YhsH7e4vWrh6LfdOKHEtgl3eaZGsSgFJTb29Ia0hm1Y//0hCop9ran7VQXgpWVIcpXSFpVZZxjZ2JzeAPK8Va2Z/ZxD+a+T/WxNwYmwZAhTo87lxR4cNbsDlgDNbqmVfX5Xrh+W0jwiTRu391pYSnexELN5ocwjwjH1v38xR6nYb4QPdqjDu3oiRE4o7NMXSsCrJvpmwU9LaMYtQCmZPw/tFnTnTqLkC+swKCAQA9iNUhiD+AOffrdy13S5G1Fe7PAzQKng3jl9+v+IdWTYOGvwVd4lLLQ28mWRhscnoIO5rJEagXUCHkFzIBr5ReAUW/j0R2I8AKIEKyrJ25nsaeccHSscW/KZW5ylpZvdXvznwUlIiRfd4r0H+Er1/McirNMoN4SlRGJpyojV14EIy28J1XHsA8D6Jt9vSyXfRgrMI+s78GSLgZTLgFrvPFsBi86Qsodz1lhugLqD5McdTVARnygAV05GvyT0jLc2lt3qvLjP9Jf1TUoPVaTLF9D4lIt3jBg/qow+n0fZl4TSXKX3OYEzUMNT2VfE2UzVuUik3u+ZEHt9RD+1u9PkFTAoIBABmar10rm9XfeNEMCIU7ppSiwLfO/0dkWbS449InjNzqkownZEMryGv6YXJssInPzvg2QjBBIxpq60wrORPsCQnoyNIsNayLOgR7+6pLnKhTDjdsQ27JhCLrtr6luYGOMj51wMkod7choriboik4fdNNcvFuzN+VYmSsAya1CtTfgSap78HAO3nxeM+6R7crS0l4VH41ayv9ow/hqwvVVtTiJczpYi/a+UdearnrrVGAGw2OQYzV8PmgjNzZYmccnIbGV1Kzd3hVHD51koMPhPvpyouJetQnQrglS4QZtgkk6ETodWSQ1DIOJKREF/ISLNgc1rlIi0ZtkhB00FhryEECggEAPeN3vqyS9+Fa/RS4pTZoEc7irTswGeDE2Nn6qY4F/z63I9Ra6FVdP/4u9LpWA1CLufryyriaTa9rShcmxtTQaa3d4BTqAN9YqpiJTHg6OHTkIdEM0F22VEWqzGjVjMG2S3sQK1iarRygfKk/UZyzdaZ2UimfteLp/nNpMT7P0x5v5qc1obEZg5EuLpskUBbgFWRl11mQCsDWICC/FZ1qI6PlD6O1c9KSzNckEwf+zSWOQRCOJwx/ZwLvwuB0MzTTRQR7hVXqlRCms/3o3jesOudelO8htV+AiVsC3+Ob3/+lS9mX2lqeSFaaeZdPKVrAoMAwMTaasPIsZPLBekLR5g==";

}

public class LicenseValidationException : Exception
{
    public LicenseValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}