using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Framework.Services
{
    public class ImageLoaderService
    {
        private static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        public static async void LoadImageToRawImage(string url, RawImage targetImage)
        {
            if (string.IsNullOrEmpty(url)) 
                return;

            try
            {
                if (TextureCache.TryGetValue(url, out var cachedTexture))
                {
                    targetImage.texture = cachedTexture;
                    return;
                }

                using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
                {
                    var operation = webRequest.SendWebRequest();

                    while (!operation.isDone)
                        await System.Threading.Tasks.Task.Yield();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                        TextureCache[url] = texture;
                        targetImage.texture = texture;
                    }
                    else
                    {
                        Debug.LogError($"Failed to load image: {webRequest.error}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading image: {e.Message}");
            }

        }

        public static void ClearCache()
        {
            foreach (var texture in TextureCache.Values
                         .Where(texture => 
                             texture != null))
            {
                UnityEngine.Object.Destroy(texture);
            }

            TextureCache.Clear();
        }

    }
}