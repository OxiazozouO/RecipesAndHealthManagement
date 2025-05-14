using System.Security.Cryptography;
using System.Text;

namespace AnyLibrary.Helper;

/// <summary>
///     加密帮助
/// </summary>
public static class EncryptionHelper
{
    public static void HasPassword(string password, string salt, out string hashpswd)
    {
        var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
        hashpswd = Convert.ToBase64String(bytes);
    }
}