using TMPro;
using UnityEngine;
using System;

public class MetadataUI : MonoBehaviour
{
    [Header("Added Date")]
    [SerializeField] private string _captureDatePrefix;
    [SerializeField] private bool _showTime = false;

    [Header("Dependencies")]
    [SerializeField] private TextMeshProUGUI _captureDate;

    private void OnEnable()
    {
        InteractionPhotographs.PhotoNameSelected += LoadMetadata;
    }

    private void OnDisable()
    {
        InteractionPhotographs.PhotoNameSelected -= LoadMetadata;
    }

    public void LoadMetadata(string photoName)
    {
        UpdateMetadata(photoName);
    }

    [System.Serializable]
    private class Metadata
    {
        public string CaptureDate;
    }

    private void UpdateMetadata(string photoName)
    {
        string metadataPath = FilePaths.SavedPhotographsPath + photoName + "_metadata.json";

        if (System.IO.File.Exists(metadataPath))
        {
            try
            {
                string metadataJson = System.IO.File.ReadAllText(metadataPath);
                if (string.IsNullOrWhiteSpace(metadataJson))
                {
                    Debug.LogError("El archivo de metadatos está vacío: " + metadataPath);
                    return;
                }
                var metadata = JsonUtility.FromJson<Metadata>(metadataJson);
                if (string.IsNullOrEmpty(metadata.CaptureDate))
                {
                    Debug.LogError("La fecha de captura está vacía o no es válida en el archivo de metadatos: " + metadataPath);
                    return;
                }
                string dateFormat = "dd/MM/yyyy";
                if (_showTime)
                {
                    dateFormat = "dd/MM/yyyy HH:mm:ss";
                }
                _captureDate.text = _captureDatePrefix + DateTime.Parse(metadata.CaptureDate).ToString(dateFormat);
            }
            catch (Exception exception)
            {
                Debug.LogError("Error al procesar el archivo de metadatos: " + exception.Message);
            }
        }
        else
        {
            Debug.LogError("Archivo de metadatos no encontrado: " + metadataPath);
        }
    }
}
