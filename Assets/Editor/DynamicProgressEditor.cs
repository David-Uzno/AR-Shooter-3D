using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class DynamicProgressEditor : Editor
{
    #region Inspector
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        List<Type> typeHierarchy = new();
        Type type = target.GetType();

        while (type != null && type != typeof(MonoBehaviour) && type != typeof(UnityEngine.Object))
        {
            typeHierarchy.Add(type);
            type = type.BaseType;
        }

        List<FieldInfo> fields = new();
        for (int i = typeHierarchy.Count - 1; i >= 0; i--)
        {
            fields.AddRange(typeHierarchy[i].GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).OrderBy(f => f.MetadataToken));
        }

        HashSet<string> drawn = new();

        foreach (FieldInfo field in fields)
        {
            ProgressBarAttribute progressBarAttribute = field.GetCustomAttribute<ProgressBarAttribute>();
            if (progressBarAttribute != null)
            {
                DrawProgressBar(field, progressBarAttribute);
                continue;
            }

            SerializedProperty targetFieldProperty = serializedObject.FindProperty(field.Name);
            if (targetFieldProperty != null)
            {
                EditorGUILayout.PropertyField(targetFieldProperty, true);
                drawn.Add(targetFieldProperty.name);
            }
        }

        DrawRemainingProperties(drawn);

        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #region Helpers
    private void DrawProgressBar(FieldInfo field, ProgressBarAttribute progressBarAttribute)
    {
        string label = GetLabelFromAttribute(progressBarAttribute);
        if (label == null)
        {
            label = field.Name;
        }

        string maxMemberName = FindStringMemberThatIsASerializedProperty(progressBarAttribute);
        SerializedProperty maxSerializedProperty = null;
        if (!string.IsNullOrEmpty(maxMemberName))
        {
            maxSerializedProperty = serializedObject.FindProperty(maxMemberName);
        }

        SerializedProperty currentSerializedProperty = FindCurrentProperty(serializedObject, field.Name, maxMemberName);

        (float currentValue, float maxValue) = GetProgressValues(currentSerializedProperty, maxSerializedProperty);

        float fraction = 0f;
        if (maxValue > 0f)
        {
            fraction = Mathf.Clamp01(currentValue / maxValue);
        }

        EColor colorEnum = GetEColorFromAttribute(progressBarAttribute);
        Color fillColor = MapEColorToColor(colorEnum);

        Rect progressBarRect = GUILayoutUtility.GetRect(18, 18);
        EditorGUI.DrawRect(progressBarRect, new Color(0.18f, 0.18f, 0.18f));
        Rect fillRect = new(progressBarRect.x, progressBarRect.y, progressBarRect.width * fraction, progressBarRect.height);
        EditorGUI.DrawRect(fillRect, fillColor);

        string labelText = label + " " + Mathf.RoundToInt(currentValue) + "/" + Mathf.RoundToInt(maxValue);
        GUIStyle centeredLabelStyle = new(EditorStyles.whiteLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };
        EditorGUI.LabelField(progressBarRect, labelText, centeredLabelStyle);
    }

    private (float currentValue, float maxValue) GetProgressValues(SerializedProperty current, SerializedProperty max)
    {
        float currentValue = 0f;
        float maxValue = 1f;

        if (current != null)
        {
            if (current.propertyType == SerializedPropertyType.Integer)
            {
                currentValue = current.intValue;
            }
            else if (current.propertyType == SerializedPropertyType.Float)
            {
                currentValue = current.floatValue;
            }
        }

        if (max != null)
        {
            if (max.propertyType == SerializedPropertyType.Integer)
            {
                maxValue = Mathf.Max(1, max.intValue);
            }
            else if (max.propertyType == SerializedPropertyType.Float)
            {
                maxValue = Mathf.Max(1f, max.floatValue);
            }
        }

        return (currentValue, maxValue);
    }

    private void DrawRemainingProperties(HashSet<string> drawn)
    {
        SerializedProperty propertyIterator = serializedObject.GetIterator();
        if (propertyIterator.NextVisible(true))
        {
            do
            {
                if (propertyIterator.name == "m_Script" || drawn.Contains(propertyIterator.name))
                {
                    continue;
                }
                EditorGUILayout.PropertyField(propertyIterator, true);
            }
            while (propertyIterator.NextVisible(false));
        }
    }
    #endregion

    #region FindMax
    // Heurística: buscar dentro del atributo cadenas que sean nombres de propiedad serializada válidos
    private string FindStringMemberThatIsASerializedProperty(ProgressBarAttribute progressBarAttribute)
    {
        if (progressBarAttribute == null)
        {
            return null;
        }
        Type progressAttributeType = progressBarAttribute.GetType();

        // Buscar campos string
        foreach (FieldInfo fieldInfo in progressAttributeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (fieldInfo.FieldType == typeof(string))
            {
                string progressValue = fieldInfo.GetValue(progressBarAttribute) as string;
                if (!string.IsNullOrEmpty(progressValue) && serializedObject.FindProperty(progressValue) != null)
                {
                    return progressValue;
                }
            }
        }

        // Buscar propiedades string
        foreach (PropertyInfo progressStringProperty in progressAttributeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (progressStringProperty.PropertyType == typeof(string))
            {
                string progressValue = progressStringProperty.GetValue(progressBarAttribute) as string;
                if (!string.IsNullOrEmpty(progressValue) && serializedObject.FindProperty(progressValue) != null)
                {
                    return progressValue;
                }
            }
        }

        return null;
    }
    #endregion

    #region GetLabel
    // Obtener label/texto desde el atributo (primer string que no sea nombre de propiedad)
    private string GetLabelFromAttribute(ProgressBarAttribute progressBarAttribute)
    {
        if (progressBarAttribute == null)
        {
            return null;
        }
        Type progressAttributeType = progressBarAttribute.GetType();

        // Primer campo string que no apunta a una propiedad serializada
        foreach (FieldInfo fieldInfo in progressAttributeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (fieldInfo.FieldType == typeof(string))
            {
                string fieldValue = fieldInfo.GetValue(progressBarAttribute) as string;
                if (!string.IsNullOrEmpty(fieldValue) && serializedObject.FindProperty(fieldValue) == null)
                {
                    return fieldValue;
                }
            }
        }
        foreach (PropertyInfo propertyInfo in progressAttributeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (propertyInfo.PropertyType == typeof(string))
            {
                string progressValue = propertyInfo.GetValue(progressBarAttribute) as string;
                if (!string.IsNullOrEmpty(progressValue) && serializedObject.FindProperty(progressValue) == null)
                {
                    return progressValue;
                }
            }
        }
        return null;
    }
    #endregion

    #region FindCurrent
    // Intentar encontrar la propiedad "current" con varias heurísticas
    private SerializedProperty FindCurrentProperty(SerializedObject serializedObject, string progressFieldName, string maxMemberName)
    {
        // 1) La propiedad con el mismo nombre que el campo donde está el atributo
        if (!string.IsNullOrEmpty(progressFieldName))
        {
            SerializedProperty progressProperty = serializedObject.FindProperty(progressFieldName);
            if (progressProperty != null && (progressProperty.propertyType == SerializedPropertyType.Integer || progressProperty.propertyType == SerializedPropertyType.Float))
            {
                return progressProperty;
            }
        }

        // 2) Heurísticas basadas en maxMemberName (reemplazos comunes)
        if (!string.IsNullOrEmpty(maxMemberName))
        {
            List<string> candidateNames = new();
            string lowerCaseMaxMemberName = maxMemberName.ToLowerInvariant();
            if (lowerCaseMaxMemberName.Contains("max"))
            {
                candidateNames.Add(maxMemberName.ToLowerInvariant().Replace("max", "current"));
                candidateNames.Add(maxMemberName.ToLowerInvariant().Replace("max", "value"));
            }
            // prefijos/sufijos comunes
            candidateNames.Add("_current" + TrimLeadingUnderscore(maxMemberName));
            candidateNames.Add("current" + TrimLeadingUnderscore(maxMemberName));
            candidateNames.Add(TrimLeadingUnderscore(maxMemberName) + "Current");
            candidateNames.Add(TrimLeadingUnderscore(maxMemberName) + "Value");

            foreach (string candidateName in candidateNames.Distinct())
            {
                SerializedProperty candidateProperty = serializedObject.FindProperty(candidateName);
                if (candidateProperty != null && (candidateProperty.propertyType == SerializedPropertyType.Integer || candidateProperty.propertyType == SerializedPropertyType.Float))
                {
                    return candidateProperty;
                }
            }
        }

        // 3) Buscar cualquier propiedad numérica (int/float) que no sea el max exacto
        SerializedProperty iteratorProperty = serializedObject.GetIterator();
        if (iteratorProperty.NextVisible(true))
        {
            do
            {
                if (iteratorProperty.name == maxMemberName)
                {
                    continue;
                }
                if (iteratorProperty.propertyType == SerializedPropertyType.Integer || iteratorProperty.propertyType == SerializedPropertyType.Float)
                {
                    return serializedObject.FindProperty(iteratorProperty.name);
                }
            }
            while (iteratorProperty.NextVisible(false));
        }

        return null;
    }
    #endregion

    #region TrimUnderscore
    private string TrimLeadingUnderscore(string inputString)
    {
        if (string.IsNullOrEmpty(inputString))
        {
            return inputString;
        }
        return inputString.TrimStart('_');
    }
    #endregion

    #region GetColor
    private static EColor GetEColorFromAttribute(ProgressBarAttribute progressBarAttribute)
    {
        if (progressBarAttribute == null)
        {
            return EColor.Green;
        }

        Type barType = progressBarAttribute.GetType();
        FieldInfo colorFieldInfo = barType.GetField("color", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? barType.GetField("Color", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (colorFieldInfo != null && colorFieldInfo.FieldType == typeof(EColor))
        {
            return (EColor)colorFieldInfo.GetValue(progressBarAttribute);
        }

        PropertyInfo colorProperty = barType.GetProperty("color", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? barType.GetProperty("Color", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (colorProperty != null && colorProperty.PropertyType == typeof(EColor))
        {
            return (EColor)colorProperty.GetValue(progressBarAttribute);
        }

        foreach (FieldInfo fieldInfo in barType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (fieldInfo.FieldType == typeof(EColor))
            {
                return (EColor)fieldInfo.GetValue(progressBarAttribute);
            }
        }

        foreach (PropertyInfo colorPropertyInfo in barType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (colorPropertyInfo.PropertyType == typeof(EColor))
            {
                return (EColor)colorPropertyInfo.GetValue(progressBarAttribute);
            }
        }

        return EColor.Green;
    }
    #endregion

    #region MapColor
    private static Color MapEColorToColor(EColor colorEnum)
    {
        // Intentar obtener el array de colores usado por NaughtyAttributes por reflexión
        try
        {
            Assembly assembliesForEColor = typeof(EColor).Assembly;
            foreach (Type type in assembliesForEColor.GetTypes())
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (field.FieldType == typeof(Color[]))
                    {
                        object colorArrayValue = field.GetValue(null);
                        if (colorArrayValue != null && colorArrayValue is Color[])
                        {
                            Color[] colorArray = colorArrayValue as Color[];
                            if ((int)colorEnum >= 0 && (int)colorEnum < colorArray.Length)
                            {
                                return colorArray[(int)colorEnum];
                            }
                        }
                    }
                }
                // también comprobar propiedades estáticas que devuelvan Color[]
                foreach (PropertyInfo colorArrayProperty in type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (colorArrayProperty.PropertyType == typeof(Color[]))
                    {
                        object colorArrayValue = colorArrayProperty.GetValue(null, null);
                        if (colorArrayValue != null && colorArrayValue is Color[])
                        {
                            Color[] colorArray = colorArrayValue as Color[];
                            if ((int)colorEnum >= 0 && (int)colorEnum < colorArray.Length)
                            {
                                return colorArray[(int)colorEnum];
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // fallthrough a fallback
        }

        // Fallback: mapeo aproximado si no se encuentra el array interno
        return colorEnum switch
        {
            EColor.Red => new Color(0.86f, 0.22f, 0.22f),
            EColor.Green => new Color(0.26f, 0.80f, 0.28f),
            EColor.Blue => new Color(0.24f, 0.50f, 0.85f),
            EColor.Orange => new Color(1f, 0.6f, 0.2f),
            EColor.Yellow => new Color(1f, 0.83f, 0.0f),
            EColor.Gray => new Color(0.6f, 0.6f, 0.6f),
            _ => Color.blue,
        };
    }
    #endregion
}
