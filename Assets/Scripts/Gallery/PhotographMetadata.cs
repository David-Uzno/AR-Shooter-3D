using System;
using UnityEngine;

public class PhotographMetadata : MonoBehaviour
{
    public Texture2D InitialTexture { get; set; }
    public DateTime CaptureDate { get; set; }

    public static void SaveTexture(Texture2D texture, int photoCounter)
    {
        string savePath = FilePaths.SavedPhotographsPath + $"{photoCounter:D4}.png";
        System.IO.File.WriteAllBytes(savePath, texture.EncodeToPNG());
    }

    public static void SaveMetadata(Texture2D texture, int photoCounter)
    {
        string metadataPath = FilePaths.SavedPhotographsPath + $"{photoCounter:D4}_metadata.json";

        if (texture != null)
        {
            string metadataJson = "{" +
                "\"CaptureDate\": \"" + DateTime.Now.ToString("o") + "\"," +
                "}";
            System.IO.File.WriteAllText(metadataPath, metadataJson);
        }
    }
}