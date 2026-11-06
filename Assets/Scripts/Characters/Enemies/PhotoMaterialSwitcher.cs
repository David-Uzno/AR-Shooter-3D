using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

[DisallowMultipleComponent]
public class PhotoMaterialSwitcher : MonoBehaviour
{
    #region Variables
    private const string PhotoPrefix = "SavedPhoto_";
    private const string JsonPattern = "*.json";

    private Renderer targetRenderer;
    private Material runtimeMaterial;
    private Texture2D runtimeTexture;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        targetRenderer = GetComponentInChildren<Renderer>(true);
    }

    private void OnEnable()
    {
        TakePhotograph.OnPhotoTaken += OnPhotoTaken;
    }

    private void Start()
    {
        ApplyRandomMaterialFromJson();
    }

    private void OnDisable()
    {
        TakePhotograph.OnPhotoTaken -= OnPhotoTaken;
    }

    private void OnDestroy()
    {
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
        }

        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
    #endregion

    #region Photo Application
    private void OnPhotoTaken(string _)
    {
        ApplyRandomMaterialFromJson();
    }

    private void ApplyRandomMaterialFromJson()
    {
        if (targetRenderer == null)
        {
            return;
        }

        string[] jsonFiles = GetMaterialJsonFiles();
        if (jsonFiles == null)
        {
            return;
        }

        if (jsonFiles.Length == 0)
        {
            return;
        }

        int startIndex = UnityEngine.Random.Range(0, jsonFiles.Length);

        for (int i = 0; i < jsonFiles.Length; i++)
        {
            int index = startIndex + i;
            if (index >= jsonFiles.Length)
            {
                index = index - jsonFiles.Length;
            }

            bool applied = TryApplyJsonMaterial(jsonFiles[index]);
            if (applied)
            {
                return;
            }
        }
    }

    private bool TryApplyJsonMaterial(string jsonPath)
    {
        PhotoMaterialJsonData materialData = ReadMaterialJson(jsonPath);
        if (materialData == null)
        {
            return false;
        }

        string resolvedTexturePath = ResolveTexturePath(materialData, jsonPath);
        if (resolvedTexturePath == null)
        {
            return false;
        }

        Texture2D texture = LoadTexture(resolvedTexturePath);
        if (texture == null)
        {
            return false;
        }

        Material material = CreateRuntimeMaterial(materialData);
        if (material == null)
        {
            Destroy(texture);
            return false;
        }

        ApplyJsonProperties(material, materialData);

        string textureProperty = GetTextureProperty(material);
        if (textureProperty == null)
        {
            Destroy(texture);
            Destroy(material);
            return false;
        }

        material.SetTexture(textureProperty, texture);
        targetRenderer.material = material;

        ReplaceRuntimeObjects(material, texture);
        return true;
    }

    private string[] GetMaterialJsonFiles()
    {
        string materialsDirectory = FilePaths.GeneratedMaterialsPath;
        if (Directory.Exists(materialsDirectory) == false)
        {
            return null;
        }

        string[] jsonFiles = Directory.GetFiles(materialsDirectory, JsonPattern);
        return jsonFiles;
    }
    #endregion

    #region JSON Loading
    private PhotoMaterialJsonData ReadMaterialJson(string jsonPath)
    {
        if (File.Exists(jsonPath) == false)
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(jsonPath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            string normalizedJson = RemoveTrailingCommas(json);
            PhotoMaterialJsonData data = JsonUtility.FromJson<PhotoMaterialJsonData>(normalizedJson);
            return data;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private string RemoveTrailingCommas(string json)
    {
        return Regex.Replace(json, ",\\s*([}\\]])", "$1");
    }
    #endregion

    #region Material Creation
    private Material CreateRuntimeMaterial(PhotoMaterialJsonData materialData)
    {
        Material baseMaterial = targetRenderer.sharedMaterial;
        Material createdMaterial = null;

        if (baseMaterial != null)
        {
            createdMaterial = new Material(baseMaterial);
        }

        Shader shaderFromJson = null;

        if (materialData != null && string.IsNullOrEmpty(materialData.Shader) == false)
        {
            shaderFromJson = Shader.Find(materialData.Shader);
        }

        if (createdMaterial == null)
        {
            Shader shader = shaderFromJson;
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }
            }

            if (shader == null)
            {
                return null;
            }

            return new Material(shader);
        }

        if (shaderFromJson != null && createdMaterial.shader != shaderFromJson)
        {
            createdMaterial.shader = shaderFromJson;
        }

        return createdMaterial;
    }

    private void ApplyJsonProperties(Material material, PhotoMaterialJsonData materialData)
    {
        if (materialData == null)
        {
            return;
        }

        if (materialData.Properties == null)
        {
            return;
        }

        PhotoMaterialPropertiesJsonData properties = materialData.Properties;

        if (properties.HasColor)
        {
            Color color = new(properties.ColorR, properties.ColorG, properties.ColorB, properties.ColorA);

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }
        }

        if (properties.HasReceiveShadows && material.HasProperty("_ReceiveShadows"))
        {
            ApplyReceiveShadows(material, properties.ReceiveShadows);
        }

        NormalizeMaterialKeywords(material);
    }
    #endregion

    #region Texture Resolution
    private string ResolveTexturePath(PhotoMaterialJsonData materialData, string jsonPath)
    {
        if (materialData != null && materialData.Properties != null && materialData.Properties.BaseMap != null)
        {
            string pathFromJson = materialData.Properties.BaseMap.Path;
            if (string.IsNullOrEmpty(pathFromJson) == false)
            {
                string resolvedByPath = ResolvePhotoPath(pathFromJson);
                if (resolvedByPath != null)
                {
                    return resolvedByPath;
                }

                string fileNameFromJson = Path.GetFileName(pathFromJson);
                string resolvedByFileName = ResolvePhotoPath(fileNameFromJson);
                if (resolvedByFileName != null)
                {
                    return resolvedByFileName;
                }
            }
        }

        string fallbackFileName = Path.GetFileNameWithoutExtension(jsonPath) + ".png";
        return ResolvePhotoPath(fallbackFileName);
    }
    #endregion

    #region Runtime Object Management
    private void ReplaceRuntimeObjects(Material material, Texture2D texture)
    {
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
        }

        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }

        runtimeMaterial = material;
        runtimeTexture = texture;
    }
    #endregion

    #region Texture Loading
    private Texture2D LoadTexture(string path)
    {
        if (File.Exists(path) == false)
        {
            return null;
        }

        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new(2, 2, TextureFormat.RGBA32, false);
            bool loaded = texture.LoadImage(bytes);
            if (loaded == false)
            {
                Destroy(texture);
                return null;
            }

            return texture;
        }
        catch (Exception)
        {
            return null;
        }
    }
    #endregion

    #region Path Resolution
    private string ResolvePhotoPath(string photoPath)
    {
        if (photoPath != null && File.Exists(photoPath))
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

    #region Material Helpers
    private void ApplyReceiveShadows(Material material, float receiveShadows)
    {
        if (receiveShadows > 0.5f)
        {
            material.SetFloat("_ReceiveShadows", 1f);
            material.DisableKeyword("_RECEIVE_SHADOWS_OFF");
            return;
        }

        material.SetFloat("_ReceiveShadows", 0f);
        material.EnableKeyword("_RECEIVE_SHADOWS_OFF");
    }

    private void NormalizeMaterialKeywords(Material material)
    {
        if (material.HasProperty("_ReceiveShadows"))
        {
            float receiveShadows = material.GetFloat("_ReceiveShadows");
            ApplyReceiveShadows(material, receiveShadows);
        }

        if (material.HasProperty("_EmissionColor"))
        {
            bool shouldEnableEmission = ShouldEnableEmissionKeyword(material);
            if (shouldEnableEmission)
            {
                material.EnableKeyword("_EMISSION");
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    private bool ShouldEnableEmissionKeyword(Material material)
    {
        Color emissionColor = material.GetColor("_EmissionColor");
        float maxChannel = emissionColor.r;

        if (emissionColor.g > maxChannel)
        {
            maxChannel = emissionColor.g;
        }

        if (emissionColor.b > maxChannel)
        {
            maxChannel = emissionColor.b;
        }

        if (maxChannel > 0.0001f)
        {
            return true;
        }

        if (material.HasProperty("_EmissionMap"))
        {
            Texture emissionMap = material.GetTexture("_EmissionMap");
            if (emissionMap != null)
            {
                return true;
            }
        }

        return false;
    }

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
