using UnityEngine;
using System.IO;

public class PhotographToMaterial : MonoBehaviour
{
    [SerializeField] private Material _baseMaterial;

    private void Start()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "SavedPhoto_*.png");

        foreach (string file in files)
        {
            Texture2D texture = LoadTexture(file);
            if (texture != null)
            {
                Material newMaterial = new Material(_baseMaterial);
                newMaterial.SetTexture("_BaseMap", texture);

                Debug.Log("Material creado con la textura: " + file);

                // Asignar el material a un GameObject para verificar visualmente
                GameObject sphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphereObject.GetComponent<Renderer>().material = newMaterial;
            }
            else
            {
                Debug.LogWarning("No se pudo cargar la textura desde: " + file);
            }
        }
    }

    private Texture2D LoadTexture(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }
}
