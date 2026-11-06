using System;

[Serializable]
public class MaterialJsonData
{
    public string Shader;
    public MaterialPropertiesData Properties;
}

[Serializable]
public class MaterialPropertiesData
{
    public BaseMapData BaseMap;
    public bool HasColor;
    public float ColorR;
    public float ColorG;
    public float ColorB;
    public float ColorA;
    public bool HasReceiveShadows;
    public float ReceiveShadows;
}

[Serializable]
public class BaseMapData
{
    public string Path;
    public int Width;
    public int Height;
    public string Format;
}
