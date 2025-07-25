using UnityEngine;

public static class FilePaths
{
    public static readonly string BaseFilePath = Application.persistentDataPath;
    public static readonly string GeneratedMaterialsPath = BaseFilePath + "/Materials";
    public static readonly string SavedPhotographsPath = BaseFilePath + "/Photographs/SavedPhoto_";
}
