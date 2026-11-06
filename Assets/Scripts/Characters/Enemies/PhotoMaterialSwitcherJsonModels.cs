using System;

[Serializable]
public class PhotoMaterialJsonData
{
    public string Shader;
    public PhotoMaterialPropertiesJsonData Properties;
}

[Serializable]
public class PhotoMaterialPropertiesJsonData
{
    public PhotoMaterialBaseMapJsonData BaseMap;
    public bool HasColor;
    public float ColorR;
    public float ColorG;
    public float ColorB;
    public float ColorA;
    public bool HasReceiveShadows;
    public float ReceiveShadows;
}

[Serializable]
public class PhotoMaterialBaseMapJsonData
{
    public string Path;
    public int Width;
    public int Height;
    public string Format;
}
