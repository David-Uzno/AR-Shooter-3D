using UnityEditor;
using UnityEngine;
using System.IO;

public class SetPhotoCounterEditor : EditorWindow
{
    #region Fields
    private int _newCounterValue = 0;
    private bool _confirmReset = false;
    private bool _confirmDelete = false;
    private bool _confirmDeleteAll = false;
    private string _searchPattern = "SavedPhoto_*.png";
    private string _customFileName = "SavedPhoto_CUSTOM.png";
    private int _testImageWidth = 256;
    private int _testImageHeight = 256;
    private Color _testImageColor = Color.green;
    private Vector2 _scrollPosition;
    #endregion

    #region Unity Menu
    [MenuItem("Herramientas/Establecer PhotoCounter")]
    public static void ShowWindow()
    {
        SetPhotoCounterEditor window = GetWindow<SetPhotoCounterEditor>("Establecer PhotoCounter");
        window.minSize = new Vector2(600, 700);
        window.maxSize = new Vector2(900, 900);
    }
    #endregion

    #region Unity Methods
    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        GUILayout.Space(8);
        EditorGUILayout.LabelField("Gestión de PhotoCounter", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Herramienta para gestionar el contador y archivos de fotografías de la aplicación.", MessageType.Info);

        GUILayout.Space(8);
        DrawInfoSection();

        GUILayout.Space(12);
        DrawModifySection();

        GUILayout.Space(12);
        DrawQuickActionsSection();

        GUILayout.Space(12);
        DrawExtraOptionsSection();

        EditorGUILayout.EndScrollView();
    }
    #endregion

    #region UI Sections
    private void DrawInfoSection()
    {
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Información actual", EditorStyles.boldLabel);

        int currentValue = PhotoCounterManager.GetPhotoCounter();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Valor actual de PhotoCounter:", GUILayout.Width(180));
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical(GUILayout.Width(50));
        GUIStyle leftHelpBox = new(EditorStyles.helpBox)
        {
            alignment = TextAnchor.MiddleLeft
        };
        EditorGUILayout.LabelField(currentValue.ToString(), leftHelpBox, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        string photosPath = PhotoFileManager.GetSavedPhotosPath();
        EditorGUILayout.LabelField("Ruta de guardado de fotos:", EditorStyles.miniBoldLabel);
        EditorGUILayout.TextField(photosPath);

        GUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Abrir carpeta", GUILayout.Width(150)))
        {
            PhotoFileManager.OpenFolderInExplorer(photosPath);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawModifySection()
    {
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Modificar valor de PhotoCounter", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Nuevo valor:", GUILayout.Width(90));
        _newCounterValue = EditorGUILayout.IntField(_newCounterValue, GUILayout.Width(80));
        if (GUILayout.Button("Guardar", GUILayout.Width(80)))
        {
            PhotoCounterManager.SetPhotoCounter(_newCounterValue);
            Debug.Log($"PhotoCounter establecido en {_newCounterValue}");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawQuickActionsSection()
    {
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Acciones rápidas", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (!_confirmReset)
        {
            if (GUILayout.Button("Resetear contador", GUILayout.Width(150)))
                _confirmReset = true;
        }
        else
        {
            EditorGUILayout.HelpBox("¿Seguro que quieres resetear el contador?", MessageType.Warning);
            if (GUILayout.Button("Sí", GUILayout.Width(50)))
            {
                PhotoCounterManager.ResetPhotoCounter();
                Debug.Log("PhotoCounter reseteado a 0");
                _confirmReset = false;
            }
            if (GUILayout.Button("No", GUILayout.Width(50)))
                _confirmReset = false;
        }
        GUILayout.FlexibleSpace();
        if (!_confirmDelete)
        {
            if (GUILayout.Button("Eliminar todas las fotos", GUILayout.Width(180)))
                _confirmDelete = true;
        }
        else
        {
            EditorGUILayout.HelpBox("¿Seguro que quieres eliminar TODAS las fotos?", MessageType.Error);
            if (GUILayout.Button("Sí", GUILayout.Width(50)))
            {
                PhotoFileManager.DeleteAllPhotos();
                Debug.Log("Todas las fotos eliminadas.");
                _confirmDelete = false;
            }
            if (GUILayout.Button("No", GUILayout.Width(50)))
                _confirmDelete = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawExtraOptionsSection()
    {
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Opciones avanzadas", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Buscar y eliminar archivos", EditorStyles.miniBoldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Patrón de búsqueda:", GUILayout.Width(120));
        _searchPattern = EditorGUILayout.TextField(_searchPattern, GUILayout.Width(180));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Mostrar archivos encontrados", GUILayout.Width(200)))
        {
            string directory = PhotoFileManager.GetSavedPhotosPath();
            if (Directory.Exists(directory))
            {
                string[] files = Directory.GetFiles(directory, _searchPattern, SearchOption.AllDirectories);
                Debug.Log($"Archivos encontrados ({files.Length}):\n" + string.Join("\n", files));
                EditorUtility.DisplayDialog("Archivos encontrados", $"Se encontraron {files.Length} archivos.\nRevisa la consola para detalles.", "OK");
            }
        }
        GUILayout.FlexibleSpace();
        if (!_confirmDeleteAll)
        {
            if (GUILayout.Button("Eliminar archivos por patrón", GUILayout.Width(200)))
                _confirmDeleteAll = true;
        }
        else
        {
            EditorGUILayout.HelpBox($"¿Seguro que quieres eliminar todos los archivos que coincidan con '{_searchPattern}'?", MessageType.Warning);
            if (GUILayout.Button("Sí", GUILayout.Width(50)))
            {
                PhotoFileManager.DeleteFilesByPattern(_searchPattern);
                Debug.Log("Archivos eliminados por patrón.");
                _confirmDeleteAll = false;
            }
            if (GUILayout.Button("No", GUILayout.Width(50)))
                _confirmDeleteAll = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Crear archivo de prueba PNG", EditorStyles.miniBoldLabel);

        _customFileName = EditorGUILayout.TextField("Nombre de archivo", _customFileName);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ancho (px):", GUILayout.Width(80));
        _testImageWidth = EditorGUILayout.IntField(_testImageWidth, GUILayout.Width(60));
        EditorGUILayout.LabelField("Alto (px):", GUILayout.Width(70));
        _testImageHeight = EditorGUILayout.IntField(_testImageHeight, GUILayout.Width(60));
        EditorGUILayout.LabelField("Color:", GUILayout.Width(45));
        _testImageColor = EditorGUILayout.ColorField(_testImageColor, GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Crear archivo de prueba", GUILayout.Width(200)))
        {
            PhotoFileManager.CreateTestPng(_customFileName, _testImageWidth, _testImageHeight, _testImageColor);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
    #endregion
}
