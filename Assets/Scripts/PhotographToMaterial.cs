using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PhotographToMaterial : MonoBehaviour
{
    #region Variables
    [SerializeField] private Material _baseMaterial;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (_baseMaterial == null)
        {
            Debug.LogWarning("La variable _baseMaterial no está asignada. Se utilizará un material por defecto.");
            _baseMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        string[] photoFiles = GetSavedPhotoFiles();
        string materialsDirectory = CreateMaterialsDirectory();

        foreach (string photoFile in photoFiles)
        {
            ProcessPhotoFile(photoFile, materialsDirectory);
        }
    }
    #endregion

    #region File Management
    private string[] GetSavedPhotoFiles()
    {
        int photoCounter = PlayerPrefs.GetInt("PhotoCounter", 0);
        Debug.Log("Contador de fotos: " + photoCounter);

        List<string> photoFiles = new List<string>();
        for (int i = 1; i <= photoCounter; i++)
        {
            string photoPath = FilePaths.SavedPhotosPath + $"{i:D4}.png";

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
    #endregion

    #region Photograph Processing
    private void ProcessPhotoFile(string photoFile, string materialsDirectory)
    {
        Texture2D photoTexture = LoadTexture(photoFile);
        if (photoTexture != null)
        {
            Material generatedMaterial = CreateMaterial(photoTexture);
            string materialFilePath = Path.Combine(materialsDirectory, Path.GetFileNameWithoutExtension(photoFile) + ".json");
            SaveMaterialAsJson(generatedMaterial, materialFilePath);
            Debug.Log("Material creado y guardado en: " + materialFilePath);
        }
        else
        {
            Debug.LogWarning("No se pudo cargar la textura desde: " + photoFile);
        }
    }

    private Material CreateMaterial(Texture2D photoTexture)
    {
        Material generatedMaterial = new Material(_baseMaterial);
        generatedMaterial.SetTexture("_BaseMap", photoTexture);
        return generatedMaterial;
    }
    #endregion

    #region Material Serialization
    private Texture2D LoadTexture(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }

    private void SaveMaterialAsJson(Material material, string jsonFilePath)
    {
        Texture baseMapTexture = material.GetTexture("_BaseMap");

        string jsonContent = SerializeMaterialToJson(material, baseMapTexture, jsonFilePath);
        File.WriteAllText(jsonFilePath, jsonContent);

        AssignMaterialToGameObject(material);
    }

    private string SerializeMaterialToJson(Material material, Texture baseMapTexture, string jsonFilePath)
    {
        string baseMapJson = "";
        if (baseMapTexture != null && baseMapTexture is Texture2D)
        {
            baseMapJson = "\"BaseMap\": {" +
                "\"Path\": \"Assets/Materials/Textures/" + Path.GetFileNameWithoutExtension(jsonFilePath) + ".png\"," +
                "\"Width\": " + ((Texture2D)baseMapTexture).width + "," +
                "\"Height\": " + ((Texture2D)baseMapTexture).height + "," +
                "\"Format\": \"" + ((Texture2D)baseMapTexture).format.ToString() + "\"},";
        }

        string colorJson = "";
        if (material.HasProperty("_Color"))
        {
            colorJson = "\"Color\": \"" + material.GetColor("_Color").ToString() + "\"";
        }

        return "{" +
            "\"Shader\": \"" + material.shader.name + "\"," +
            "\"Properties\": {" +
            baseMapJson +
            colorJson +
            "}}";
    }
    #endregion

    #region Debugging Logs
    private void AssignMaterialToGameObject(Material material)
    {
        GameObject sphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereObject.name = "TempMaterialSphere";
        sphereObject.GetComponent<MeshRenderer>().material = material;
    }
    #endregion
}
