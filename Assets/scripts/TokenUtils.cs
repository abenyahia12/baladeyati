using System;

public static class TokenUtils
{
	public static long GetExpiryTimeOfToken(string token)
	{
		string[] parts = token.Split('.');

		if (parts.Length <= 2)
		{
			return 0;
		}

		string decode = parts[1];
		int padLength = 4 - decode.Length % 4;

		if (padLength < 4)
		{
			decode += new string('=', padLength);
		}

		byte[] bytes = Convert.FromBase64String(decode);
		string userInfo = System.Text.Encoding.ASCII.GetString(bytes);
		JSONObject tokenObject = new JSONObject(userInfo);

		long exp = 0;
		tokenObject.GetField(ref exp, "exp");

		//DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		//DateTime expiryDateTime = expiryDateTime = epoch.AddSeconds(exp);
		//Debug.Log("Expiry Time: " + expiryDateTime);
		return exp;
	}

	public static bool HasTokenExpired(string token)
	{
		return TimeToExpiry(token) < 0;
	}

	public static long TimeToExpiry(string token)
	{
		long tokenExpiry = GetExpiryTimeOfToken(token);
		long currentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
		return tokenExpiry - currentTime;
	}
}
