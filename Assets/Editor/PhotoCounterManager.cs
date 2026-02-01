using UnityEngine;

public static class PhotoCounterManager
{
    public static int GetPhotoCounter()
    {
        return PlayerPrefs.GetInt("PhotoCounter", 0);
    }

    public static void SetPhotoCounter(int value)
    {
        PlayerPrefs.SetInt("PhotoCounter", value);
        PlayerPrefs.Save();
    }

    public static void ResetPhotoCounter()
    {
        SetPhotoCounter(0);
    }
}
