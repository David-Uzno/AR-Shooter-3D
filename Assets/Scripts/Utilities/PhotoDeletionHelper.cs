using System.IO;
using UnityEngine;

public static class PhotoDeletionHelper
{
    public static bool DeletePhotoAndMetadata(int index)
    {
        if (index <= 0)
        {
            Debug.LogWarning("Índice de foto inválido: " + index);
            return false;
        }

        string directory = Path.GetDirectoryName(FilePaths.SavedPhotographsPath);
        if (string.IsNullOrEmpty(directory))
        {
            Debug.LogError("No se pudo determinar el directorio de fotografías.");
            return false;
        }

        string photoIndexString = index.ToString("D4");
        string[] patterns = new string[]
        {
            $"SavedPhoto_{photoIndexString}.png",
            $"SavedPhoto_{photoIndexString}.jpg",
            $"SavedPhoto_{photoIndexString}.jpeg",
            $"SavedPhoto_{photoIndexString}_metadata.json"
        };

        bool anyDeleted = false;
        foreach (string pattern in patterns)
        {
            try
            {
                string[] files = Directory.GetFiles(directory, pattern, SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        Debug.Log($"Eliminado: {file}");
                        anyDeleted = true;
                    }
                    catch (IOException exception)
                    {
                        Debug.LogWarning($"No se pudo eliminar {file}: {exception.Message}");
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Debug.LogWarning($"Directorio no encontrado al intentar borrar: {directory}");
                break;
            }
        }

        if (!anyDeleted)
        {
            Debug.LogWarning($"No se encontraron archivos para el índice {photoIndexString} en {directory}.");
        }

        return anyDeleted;
    }
}
