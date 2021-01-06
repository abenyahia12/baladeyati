using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class Helper : MonoBehaviour
{
	public static byte[] Encrypt(byte[] input)
	{
		using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
		{
			byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes("gaghearhrahmt3ta" + SystemInfo.deviceUniqueIdentifier + "jkop;kmiopjvra"));
			using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
			{
				ICryptoTransform tr = trip.CreateEncryptor();
				return tr.TransformFinalBlock(input, 0, input.Length);
			}
		}
	}

	public static byte[] Decrypt(byte[] input)
	{
		using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
		{
			byte[] key = md5.ComputeHash(Encoding.UTF8.GetBytes("gaghearhrahmt3ta" + SystemInfo.deviceUniqueIdentifier + "jkop;kmiopjvra"));
			using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
			{
				ICryptoTransform tr = trip.CreateDecryptor();
				return tr.TransformFinalBlock(input, 0, input.Length);
			}
		}
	}
}
