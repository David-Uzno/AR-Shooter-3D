using UnityEditor;
using UnityEngine;
using System.IO;

public class SetPhotoCounterEditor : EditorWindow
{
    #region Fields
    private int newCounterValue = 0;
    private bool confirmReset = false;
    private bool confirmDelete = false;
    private bool confirmDeleteAll = false;
    private string searchPattern = "SavedPhoto_*.png";
    private string customFileName = "SavedPhoto_CUSTOM.png";
    private int testImageWidth = 256;
    private int testImageHeight = 256;
    private Color testImageColor = Color.green;
    private Vector2 scrollPos;
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
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

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
        GUIStyle leftHelpBox = new GUIStyle(EditorStyles.helpBox);
        leftHelpBox.alignment = TextAnchor.MiddleLeft;
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
        newCounterValue = EditorGUILayout.IntField(newCounterValue, GUILayout.Width(80));
        if (GUILayout.Button("Guardar", GUILayout.Width(80)))
        {
            PhotoCounterManager.SetPhotoCounter(newCounterValue);
            Debug.Log($"PhotoCounter establecido en {newCounterValue}");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawQuickActionsSection()
    {
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.LabelField("Acciones rápidas", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (!confirmReset)
        {
            if (GUILayout.Button("Resetear contador", GUILayout.Width(150)))
                confirmReset = true;
        }
        else
        {
            EditorGUILayout.HelpBox("¿Seguro que quieres resetear el contador?", MessageType.Warning);
            if (GUILayout.Button("Sí", GUILayout.Width(50)))
            {
                PhotoCounterManager.ResetPhotoCounter();
                Debug.Log("PhotoCounter reseteado a 0");
                confirmReset = false;
            }
            if (GUILayout.Button("No", GUILayout.Width(50)))
                confirmReset = false;
        }
        GUILayout.FlexibleSpace();
        if (!confirmDelete)
        {
            if (GUILayout.Button("Eliminar todas las fotos", GUILayout.Width(180)))
                confirmDelete = true;
        }
        else
        {
            EditorGUILayout.HelpBox("¿Seguro que quieres eliminar TODAS las fotos?", MessageType.Error);
            if (GUILayout.Button("Sí", GUILayout.Width(50)))
            {
                PhotoFileManager.DeleteAllPhotos();
                Debug.Log("Todas las fotos eliminadas.");
                confirmDelete = false;
            }
            if (GUILayout.Button("No", GUILayout.Width(50)))
                confirmDelete = false;
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
        searchPattern = EditorGUILayout.TextField(searchPattern, GUILayout.Width(180));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Mostrar archivos encontrados", GUILayout.Width(200)))
        {
            string directory = PhotoFileManager.GetSavedPhotosPath();
            if (Directory.Exists(directory))
            {
                string[] files = System.IO.Directory.GetFiles(directory, searchPattern, System.IO.SearchOption.AllDirectories);
                Debug.Log($"Archivos encontrados ({files.Length}):\n" + string.Join("\n", files));
                EditorUtility.DisplayDialog("Archivos encontrados", $"Se encontraron {files.Length} archivos.\nRevisa la consola para detalles.", "OK");
            }
        }
        GUILayout.FlexibleSpace();
        if (!confirmDeleteAll)
        {
            if (GUILayout.Button("Eliminar archivos por patrón", GUILayout.Width(200)))
                confirmDeleteAll = true;
        }
        else
        {
            EditorGUILayout.HelpBox($"¿Seguro que quieres eliminar todos los archivos que coincidan con '{searchPattern}'?", MessageType.Warning);
            if (GUILayout.Button("Sí", GUILayout.Width(50)))
            {
                PhotoFileManager.DeleteFilesByPattern(searchPattern);
                Debug.Log("Archivos eliminados por patrón.");
                confirmDeleteAll = false;
            }
            if (GUILayout.Button("No", GUILayout.Width(50)))
                confirmDeleteAll = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Crear archivo de prueba PNG", EditorStyles.miniBoldLabel);

        customFileName = EditorGUILayout.TextField("Nombre de archivo", customFileName);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ancho (px):", GUILayout.Width(80));
        testImageWidth = EditorGUILayout.IntField(testImageWidth, GUILayout.Width(60));
        EditorGUILayout.LabelField("Alto (px):", GUILayout.Width(70));
        testImageHeight = EditorGUILayout.IntField(testImageHeight, GUILayout.Width(60));
        EditorGUILayout.LabelField("Color:", GUILayout.Width(45));
        testImageColor = EditorGUILayout.ColorField(testImageColor, GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Crear archivo de prueba", GUILayout.Width(200)))
        {
            PhotoFileManager.CreateTestPng(customFileName, testImageWidth, testImageHeight, testImageColor);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
    #endregion
}
