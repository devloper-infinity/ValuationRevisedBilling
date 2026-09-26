using BillingOther.App_Code.DAL;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace BillingOther.App_Code.BLL
{
    public sealed class PendingLogin
    {
        public int EmployeeId { get; set; }
        public string Role { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class MfaService
    {
        private readonly DALLogin dalLogin = new DALLogin();
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public bool IsRequired
        {
            get
            {
                bool enabled;
                bool configured = bool.TryParse(ConfigurationManager.AppSettings["MfaEnabled"], out enabled) && enabled;
                return configured && !HttpContext.Current.IsDebuggingEnabled;
            }
        }

        public string GetSecret(int employeeId)
        {
            string protectedSecret = dalLogin.GetMfaSecret(employeeId);
            if (string.IsNullOrWhiteSpace(protectedSecret)) return null;
            byte[] clear = MachineKey.Unprotect(Convert.FromBase64String(protectedSecret), "BillingOther.MFA", employeeId.ToString());
            return clear == null ? null : Encoding.UTF8.GetString(clear);
        }

        public void Enroll(int employeeId, string secret)
        {
            byte[] protectedValue = MachineKey.Protect(Encoding.UTF8.GetBytes(secret), "BillingOther.MFA", employeeId.ToString());
            dalLogin.SaveMfaSecret(employeeId, Convert.ToBase64String(protectedValue));
        }

        public string CreateSecret()
        {
            byte[] bytes = new byte[20];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            return Base32Encode(bytes);
        }

        public bool Verify(string secret, string suppliedCode)
        {
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(suppliedCode)) return false;
            suppliedCode = suppliedCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            long step = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
            for (long offset = -1; offset <= 1; offset++)
                if (FixedTimeEquals(GenerateCode(secret, step + offset), suppliedCode)) return true;
            return false;
        }

        public string BuildOtpUri(string account, string secret)
        {
            string issuer = ConfigurationManager.AppSettings["MfaIssuer"] ?? "Infinity IPS Billing";
            return "otpauth://totp/" + Uri.EscapeDataString(issuer + ":" + account) + "?secret=" + secret + "&issuer=" + Uri.EscapeDataString(issuer) + "&digits=6&period=30";
        }

        private static string GenerateCode(string secret, long counter)
        {
            byte[] key = Base32Decode(secret); byte[] data = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            using (HMACSHA1 hmac = new HMACSHA1(key))
            {
                byte[] hash = hmac.ComputeHash(data); int offset = hash[hash.Length - 1] & 15;
                int binary = ((hash[offset] & 127) << 24) | ((hash[offset + 1] & 255) << 16) | ((hash[offset + 2] & 255) << 8) | (hash[offset + 3] & 255);
                return (binary % 1000000).ToString("D6");
            }
        }

        private static bool FixedTimeEquals(string expected, string actual)
        {
            if (expected.Length != actual.Length) return false; int result = 0;
            for (int i = 0; i < expected.Length; i++) result |= expected[i] ^ actual[i];
            return result == 0;
        }

        private static string Base32Encode(byte[] data)
        {
            StringBuilder result = new StringBuilder(); int buffer = data[0], next = 1, bitsLeft = 8;
            while (bitsLeft > 0 || next < data.Length) { if (bitsLeft < 5) { if (next < data.Length) { buffer <<= 8; buffer |= data[next++] & 255; bitsLeft += 8; } else { buffer <<= 5 - bitsLeft; bitsLeft = 5; } } result.Append(Alphabet[(buffer >> (bitsLeft - 5)) & 31]); bitsLeft -= 5; }
            return result.ToString();
        }

        private static byte[] Base32Decode(string value)
        {
            value = value.TrimEnd('=').ToUpperInvariant(); byte[] output = new byte[value.Length * 5 / 8]; int buffer = 0, bitsLeft = 0, index = 0;
            foreach (char c in value) { int val = Alphabet.IndexOf(c); if (val < 0) throw new FormatException("Invalid MFA secret."); buffer = (buffer << 5) | val; bitsLeft += 5; if (bitsLeft >= 8) output[index++] = (byte)(buffer >> (bitsLeft -= 8)); }
            return output;
        }
    }
}
