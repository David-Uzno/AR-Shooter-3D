using UnityEngine;

public class PhotographToMaterial : MonoBehaviour
{
    [SerializeField] private Material _baseMaterial;

    private void Start()
    {
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, "SavedPhoto_*.png");

        foreach (string file in files)
        {
            Texture2D texture = LoadTexture(file);
            if (texture != null)
            {
                Material newMaterial = new Material(_baseMaterial);
                newMaterial.mainTexture = texture;
                Debug.Log("Material creado con la textura: " + file);
            }
            else
            {
                Debug.LogWarning("No se pudo cargar la textura desde: " + file);
            }
        }
    }

    private Texture2D LoadTexture(string path)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }
}
