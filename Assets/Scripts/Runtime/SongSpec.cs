using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SongSpec", menuName = "Scriptable Objects/SongSpec")]
public class SongSpec : ScriptableObject
{
    [field : SerializeField] public string title {  get; private set; }
    [field : SerializeField] public float bpm { get; private set; }
    [field: SerializeField] public AudioClip audioClip { get; private set; }
    [field: SerializeField] public List<float> peaks { get; private set; }

    public void BakePeaks(AudioClip audioClip, List<float> peaks)
    {
        this.audioClip = audioClip;
        this.peaks = peaks;
    }
}
