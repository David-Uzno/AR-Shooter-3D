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

        string photographsDirectory = Path.GetDirectoryName(FilePaths.SavedPhotographsPath);
        if (string.IsNullOrEmpty(photographsDirectory))
        {
            Debug.LogError("No se pudo determinar el directorio de fotografías.");
            return false;
        }

        string photoIndexString = index.ToString("D4");
        string[] photographPatterns = new string[]
        {
            $"SavedPhoto_{photoIndexString}.png",
            $"SavedPhoto_{photoIndexString}.jpg",
            $"SavedPhoto_{photoIndexString}.jpeg",
            $"SavedPhoto_{photoIndexString}_metadata.json"
        };

        string[] materialPatterns = new string[]
        {
            $"SavedPhoto_{photoIndexString}.json"
        };

        bool anyDeleted = false;

        bool photographsDeleted = DeleteMatchingFiles(photographsDirectory, photographPatterns);
        if (photographsDeleted)
        {
            anyDeleted = true;
        }

        bool materialDeleted = DeleteMatchingFiles(FilePaths.GeneratedMaterialsPath, materialPatterns);
        if (materialDeleted)
        {
            anyDeleted = true;
        }

        if (!anyDeleted)
        {
            Debug.LogWarning($"No se encontraron archivos para el índice {photoIndexString}.");
        }

        return anyDeleted;
    }

    private static bool DeleteMatchingFiles(string directory, string[] patterns)
    {
        if (string.IsNullOrEmpty(directory))
        {
            return false;
        }

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
                return anyDeleted;
            }
        }

        return anyDeleted;
    }
}
