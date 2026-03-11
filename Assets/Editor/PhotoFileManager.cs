using UnityEngine;
using UnityEditor;
using System.IO;

public static class PhotoFileManager
{
    public static string GetSavedPhotosPath()
    {
        System.Type type = System.Type.GetType("FilePaths");
        if (type != null)
        {
            System.Reflection.PropertyInfo photographsPathProperty = type.GetProperty("SavedPhotographsPath");
            if (photographsPathProperty != null)
                return photographsPathProperty.GetValue(null, null)?.ToString();
        }
        return Application.persistentDataPath;
    }

    public static void DeleteAllPhotos()
    {
        string directory = GetSavedPhotosPath();
        if (Directory.Exists(directory))
        {
            string[] patterns = { "SavedPhoto_*.png", "SavedPhoto_*.jpg", "SavedPhoto_*.jpeg", "SavedPhoto_*.json" };
            foreach (string pattern in patterns)
            {
                foreach (string file in Directory.GetFiles(directory, pattern, SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException exception)
                    {
                        Debug.LogWarning($"No se pudo eliminar {file}: {exception.Message}");
                    }
                }
            }
        }
    }

    public static void DeleteFilesByPattern(string pattern)
    {
        string directory = GetSavedPhotosPath();
        if (Directory.Exists(directory))
        {
            string[] files = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (IOException exception)
                {
                    Debug.LogWarning($"No se pudo eliminar {file}: {exception.Message}");
                }
            }
        }
    }

    public static void DeletePhotoByIndex(int index)
    {
        PhotoDeletionHelper.DeletePhotoAndMetadata(index);
    }

    public static void OpenFolderInExplorer(string path)
    {
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path.Replace("/", "\\"));
        }
        else
        {
            EditorUtility.DisplayDialog("Directorio no encontrado", $"No se encontró el directorio:\n{path}", "OK");
        }
    }

    public static void CreateTestPng(string fileName, int width, int height, Color color)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogWarning("El nombre del archivo no puede estar vacío.");
            return;
        }
        if (!fileName.EndsWith(".png"))
        {
            fileName += ".png";
        }

        string photographsDir = Path.Combine(Application.persistentDataPath, "Photographs");
        if (!Directory.Exists(photographsDir))
        {
            Directory.CreateDirectory(photographsDir);
        }
        string path = Path.Combine(photographsDir, fileName);

        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        byte[] pngData = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        File.WriteAllBytes(path, pngData);
        Debug.Log($"Archivo de prueba PNG creado: {path}");
        EditorUtility.DisplayDialog("Archivo creado", $"Archivo de prueba PNG creado:\n{path}", "OK");
    }
}
