using System.Security.Cryptography;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity
{
    public class EncryptionService
    {
        public static byte[] GenerateSalt()
        {
            // Random產生 salt 值
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        public static byte[] CombineBytes(byte[] first, byte[] second)
        {
            // 将两个字节数组连接起来
            byte[] combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }

        public static byte[] ComputeHash(byte[] inputBytes)
        {
            // 使用 SHA-256 計算 hash 值
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(inputBytes);
            }
        }
    }
}
