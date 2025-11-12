using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings _instance;
    public float _volume;

    void Awake() //Ensures that this object presists during scene changes
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetVolume(float volume)
    {
        _volume = volume;
    }
}
