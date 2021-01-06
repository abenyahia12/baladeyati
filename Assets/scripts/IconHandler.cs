using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class IconHandler
{
	public static IEnumerator GetIconFromURL(string iconURL, Action<Sprite> sprite)
	{
		string iconServerDirectory = Path.GetDirectoryName(iconURL);
		string iconFileName = Path.GetFileName(iconURL);

		if (iconFileName == null)
		{
			yield break;
		}

		string iconDir = Application.persistentDataPath + "/" + "DownloadedIcons" + iconServerDirectory;

		DirectoryInfo dirInf = new DirectoryInfo(iconDir);
		if (!dirInf.Exists)
		{
			dirInf.Create();
		}

		Texture2D spriteTexture = LoadTexture(iconDir + iconFileName);
		if (spriteTexture != null)
		{
			sprite(Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2()));
			yield break;
		}

		// Download it
		string serverURL = "https://ltms.cat-europe.com";
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(serverURL + iconURL);
		www.SetRequestHeader("Authorization", " Bearer " + PlayerPrefs.GetString("token"));
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			// If offline or something else has gone wrong, get unknown sprite
			yield break;
		}

		DownloadHandlerTexture textureHandler = (DownloadHandlerTexture)www.downloadHandler;
		File.WriteAllBytes(iconDir + iconFileName, textureHandler.data);
		spriteTexture = textureHandler.texture;
		sprite(Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2()));
	}

	public static Texture2D LoadTexture(string FilePath)
	{
		// Load a PNG or JPG file from disk to a Texture2D
		// Returns null if load fails

		if (File.Exists(FilePath))
		{
			byte[] FileData = File.ReadAllBytes(FilePath);
			Texture2D Tex2D = new Texture2D(2, 2);      // Create new "empty" texture
			if (Tex2D.LoadImage(FileData))      // Load the imagedata into the texture (size is set automatically)
			{
				return Tex2D;                   // If data = readable -> return texture
			}
		}

		return null;                            // Return null if load failed
	}
}