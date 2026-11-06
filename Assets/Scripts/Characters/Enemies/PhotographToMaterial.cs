using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotographToMaterial : MonoBehaviour
{
    #region Variables
    [SerializeField] private Material _baseMaterial;
    private const string PhotoPrefix = "SavedPhoto_";
    #endregion

    #region Static Generation
    public static bool TryGenerateJsonForPhoto(string photoPath, Material baseMaterial)
    {
        string resolvedPath = ResolvePhotoPathStatic(photoPath);
        if (string.IsNullOrEmpty(resolvedPath))
        {
            return false;
        }

        Texture2D photoTexture = LoadTextureStatic(resolvedPath);
        if (photoTexture == null)
        {
            return false;
        }

        Material sourceBaseMaterial = baseMaterial;
        bool includeReceiveShadows = true;

        if (sourceBaseMaterial == null)
        {
            sourceBaseMaterial = CreateFallbackBaseMaterialStatic();
            includeReceiveShadows = false;
        }

        if (sourceBaseMaterial == null)
        {
            string materialsDirectoryWithoutMaterial = EnsureMaterialsDirectoryStatic();
            string jsonFilePathWithoutMaterial = Path.Combine(materialsDirectoryWithoutMaterial, Path.GetFileNameWithoutExtension(resolvedPath) + ".json");
            SaveTextureOnlyJsonStatic(photoTexture, jsonFilePathWithoutMaterial, resolvedPath);
            Destroy(photoTexture);
            return true;
        }

        Material generatedMaterial = new(sourceBaseMaterial);

        if (baseMaterial == null)
        {
            Destroy(sourceBaseMaterial);
        }

        string textureProperty = GetTexturePropertyStatic(generatedMaterial);
        if (textureProperty == null)
        {
            Destroy(generatedMaterial);
            Destroy(photoTexture);
            return false;
        }

        generatedMaterial.SetTexture(textureProperty, photoTexture);

        string materialsDirectory = EnsureMaterialsDirectoryStatic();
        string jsonFilePath = Path.Combine(materialsDirectory, Path.GetFileNameWithoutExtension(resolvedPath) + ".json");

        SaveMaterialAsJsonStatic(generatedMaterial, jsonFilePath, resolvedPath, includeReceiveShadows);

        Destroy(generatedMaterial);
        Destroy(photoTexture);
        return true;
    }

    private static string EnsureMaterialsDirectoryStatic()
    {
        string materialsDirectory = FilePaths.GeneratedMaterialsPath;
        if (Directory.Exists(materialsDirectory) == false)
        {
            Directory.CreateDirectory(materialsDirectory);
        }

        return materialsDirectory;
    }

    private static Material CreateFallbackBaseMaterialStatic()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            return null;
        }

        return new Material(shader);
    }

    private static string ResolvePhotoPathStatic(string photoPath)
    {
        if (string.IsNullOrEmpty(photoPath) == false && File.Exists(photoPath))
        {
            return photoPath;
        }

        string fileName = Path.GetFileName(photoPath);
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        string prefixPath = FilePaths.SavedPhotographsPath;
        string parentDirectory = Path.GetDirectoryName(prefixPath);
        string withoutPrefix = fileName;

        if (fileName.StartsWith(PhotoPrefix, StringComparison.OrdinalIgnoreCase))
        {
            withoutPrefix = fileName.Substring(PhotoPrefix.Length);
        }

        if (parentDirectory != null)
        {
            string candidateInParent = Path.Combine(parentDirectory, fileName);
            if (File.Exists(candidateInParent))
            {
                return candidateInParent;
            }
        }

        string candidateInPrefix = prefixPath + withoutPrefix;
        if (File.Exists(candidateInPrefix))
        {
            return candidateInPrefix;
        }

        string candidateWithPrefix = prefixPath + fileName;
        if (File.Exists(candidateWithPrefix))
        {
            return candidateWithPrefix;
        }

        return null;
    }

    private static Texture2D LoadTextureStatic(string filePath)
    {
        if (File.Exists(filePath) == false)
        {
            return null;
        }

        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(fileData))
            {
                return texture;
            }

            Destroy(texture);
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string GetTexturePropertyStatic(Material material)
    {
        if (material.HasProperty("_BaseMap"))
        {
            return "_BaseMap";
        }

        if (material.HasProperty("_MainTex"))
        {
            return "_MainTex";
        }

        return null;
    }

    private static void SaveMaterialAsJsonStatic(Material material, string jsonFilePath, string sourcePhotoPath, bool includeReceiveShadows)
    {
        MaterialJsonData jsonData = BuildJsonDataStatic(material, sourcePhotoPath, includeReceiveShadows);
        string jsonContent = JsonUtility.ToJson(jsonData, true);
        File.WriteAllText(jsonFilePath, jsonContent);
    }

    private static void SaveTextureOnlyJsonStatic(Texture2D texture, string jsonFilePath, string sourcePhotoPath)
    {
        MaterialJsonData data = new();
        MaterialPropertiesData properties = new();
        BaseMapData baseMap = new()
        {
            Path = sourcePhotoPath,
            Width = texture.width,
            Height = texture.height,
            Format = texture.format.ToString()
        };

        properties.BaseMap = baseMap;
        data.Properties = properties;

        string jsonContent = JsonUtility.ToJson(data, true);
        File.WriteAllText(jsonFilePath, jsonContent);
    }

    private static MaterialJsonData BuildJsonDataStatic(Material material, string sourcePhotoPath, bool includeReceiveShadows)
    {
        MaterialJsonData data = new();
        MaterialPropertiesData properties = new();

        if (material.shader != null)
        {
            data.Shader = material.shader.name;
        }

        Texture baseMapTexture = material.GetTexture("_BaseMap");
        if (baseMapTexture == null && material.HasProperty("_MainTex"))
        {
            baseMapTexture = material.GetTexture("_MainTex");
        }

        if (baseMapTexture is Texture2D texture)
        {
            BaseMapData baseMap = new()
            {
                Path = sourcePhotoPath,
                Width = texture.width,
                Height = texture.height,
                Format = texture.format.ToString()
            };
            properties.BaseMap = baseMap;
        }

        if (material.HasProperty("_Color"))
        {
            Color color = material.GetColor("_Color");
            properties.HasColor = true;
            properties.ColorR = color.r;
            properties.ColorG = color.g;
            properties.ColorB = color.b;
            properties.ColorA = color.a;
        }

        if (includeReceiveShadows && material.HasProperty("_ReceiveShadows"))
        {
            properties.HasReceiveShadows = true;
            properties.ReceiveShadows = material.GetFloat("_ReceiveShadows");
        }

        data.Properties = properties;
        return data;
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        EnsureBaseMaterial();
        ProcessAllSavedPhotos();
    }

    private void OnEnable()
    {
        TakePhotograph.OnPhotoTaken += ProcessPhotoFromEvent;
    }

    private void OnDisable()
    {
        TakePhotograph.OnPhotoTaken -= ProcessPhotoFromEvent;
    }
    #endregion

    #region Initialization
    private void EnsureBaseMaterial()
    {
        if (_baseMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                Debug.LogWarning("No se encontró shader compatible para crear material base.");
                return;
            }

            _baseMaterial = new Material(shader);
        }
    }
    #endregion

    #region Event Processing
    private void ProcessPhotoFromEvent(string photoPath)
    {
        string resolvedPath = ResolvePhotoPath(photoPath);
        if (string.IsNullOrEmpty(resolvedPath))
        {
            Debug.LogWarning("No se pudo resolver la ruta de la fotografía para generar JSON: " + photoPath);
            return;
        }

        string materialsDirectory = CreateMaterialsDirectory();
        ProcessPhotoFile(resolvedPath, materialsDirectory);
    }
    #endregion

    #region File Management
    private void ProcessAllSavedPhotos()
    {
        string[] photoFiles = GetSavedPhotoFiles();
        string materialsDirectory = CreateMaterialsDirectory();

        for (int i = 0; i < photoFiles.Length; i++)
        {
            ProcessPhotoFile(photoFiles[i], materialsDirectory);
        }
    }

    private string[] GetSavedPhotoFiles()
    {
        int photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);
        Debug.Log("Contador de fotos: " + photoCounter);

        List<string> photoFiles = new();
        for (int i = 1; i <= photoCounter; i++)
        {
            string photoPath = FilePaths.SavedPhotographsPath + $"{i:D4}.png";

            if (File.Exists(photoPath))
            {
                photoFiles.Add(photoPath);
            }
            else
            {
                Debug.LogWarning("Archivo no encontrado: " + photoPath);
            }
        }

        return photoFiles.ToArray();
    }

    private string CreateMaterialsDirectory()
    {
        string materialsDirectory = FilePaths.GeneratedMaterialsPath;
        if (!Directory.Exists(materialsDirectory))
        {
            Directory.CreateDirectory(materialsDirectory);
        }
        return materialsDirectory;
    }

    private string ResolvePhotoPath(string photoPath)
    {
        if (string.IsNullOrEmpty(photoPath) == false && File.Exists(photoPath))
        {
            return photoPath;
        }

        string fileName = Path.GetFileName(photoPath);
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        string prefixPath = FilePaths.SavedPhotographsPath;
        string parentDirectory = Path.GetDirectoryName(prefixPath);
        string withoutPrefix = fileName;

        if (fileName.StartsWith(PhotoPrefix, StringComparison.OrdinalIgnoreCase))
        {
            withoutPrefix = fileName.Substring(PhotoPrefix.Length);
        }

        string[] candidates = new string[]
        {
            parentDirectory != null ? Path.Combine(parentDirectory, fileName) : null,
            prefixPath + withoutPrefix,
            prefixPath + fileName
        };

        for (int i = 0; i < candidates.Length; i++)
        {
            string candidate = candidates[i];
            if (candidate != null && File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }
    #endregion

    #region Photograph Processing
    private void ProcessPhotoFile(string photoFile, string materialsDirectory)
    {
        Texture2D photoTexture = LoadTexture(photoFile);
        if (photoTexture != null)
        {
            Material generatedMaterial = CreateMaterial(photoTexture);
            if (generatedMaterial == null)
            {
                Destroy(photoTexture);
                return;
            }

            string materialFilePath = Path.Combine(materialsDirectory, Path.GetFileNameWithoutExtension(photoFile) + ".json");
            SaveMaterialAsJson(generatedMaterial, materialFilePath, photoFile);
            Debug.Log("Material creado y guardado en: " + materialFilePath);

            Destroy(generatedMaterial);
            Destroy(photoTexture);
        }
        else
        {
            Debug.LogWarning("No se pudo cargar la textura desde: " + photoFile);
        }
    }

    private Material CreateMaterial(Texture2D photoTexture)
    {
        if (_baseMaterial == null)
        {
            return null;
        }

        Material generatedMaterial = new(_baseMaterial);
        string textureProperty = GetTextureProperty(generatedMaterial);
        if (textureProperty == null)
        {
            Destroy(generatedMaterial);
            return null;
        }

        generatedMaterial.SetTexture(textureProperty, photoTexture);
        return generatedMaterial;
    }
    #endregion

    #region Material Serialization
    private Texture2D LoadTexture(string filePath)
    {
        if (File.Exists(filePath) == false)
        {
            return null;
        }

        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(fileData))
            {
                return texture;
            }

            Destroy(texture);
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void SaveMaterialAsJson(Material material, string jsonFilePath, string sourcePhotoPath)
    {
        MaterialJsonData jsonData = BuildJsonData(material, sourcePhotoPath);
        string jsonContent = JsonUtility.ToJson(jsonData, true);
        File.WriteAllText(jsonFilePath, jsonContent);
    }

    private MaterialJsonData BuildJsonData(Material material, string sourcePhotoPath)
    {
        MaterialJsonData data = new();
        MaterialPropertiesData properties = new();

        if (material.shader != null)
        {
            data.Shader = material.shader.name;
        }

        Texture baseMapTexture = material.GetTexture("_BaseMap");
        if (baseMapTexture == null && material.HasProperty("_MainTex"))
        {
            baseMapTexture = material.GetTexture("_MainTex");
        }

        if (baseMapTexture is Texture2D texture)
        {
            BaseMapData baseMap = new()
            {
                Path = sourcePhotoPath,
                Width = texture.width,
                Height = texture.height,
                Format = texture.format.ToString()
            };
            properties.BaseMap = baseMap;
        }

        if (material.HasProperty("_Color"))
        {
            Color color = material.GetColor("_Color");
            properties.HasColor = true;
            properties.ColorR = color.r;
            properties.ColorG = color.g;
            properties.ColorB = color.b;
            properties.ColorA = color.a;
        }

        if (material.HasProperty("_ReceiveShadows"))
        {
            properties.HasReceiveShadows = true;
            properties.ReceiveShadows = material.GetFloat("_ReceiveShadows");
        }

        data.Properties = properties;
        return data;
    }
    #endregion

    #region Material Helpers
    private string GetTextureProperty(Material material)
    {
        if (material.HasProperty("_BaseMap"))
        {
            return "_BaseMap";
        }

        if (material.HasProperty("_MainTex"))
        {
            return "_MainTex";
        }

        return null;
    }
    #endregion

}
