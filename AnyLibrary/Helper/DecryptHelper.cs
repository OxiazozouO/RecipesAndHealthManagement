using System.Security.Cryptography;
using System.Text;

namespace AnyLibrary.Helper;

public class DecryptHelper
{
    public static void GetHasPassword(string password, out string salt, out string hashpswd)
    {
        var sha256 = SHA256.Create();
        salt = StringHelper.GetRandomString();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
        hashpswd = System.Convert.ToBase64String(bytes);
    }
}