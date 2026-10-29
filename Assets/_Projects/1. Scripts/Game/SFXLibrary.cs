using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SfxLibrary", menuName = "Audio/Sfx Library")]
public class SfxLibrary : ScriptableObject
{
    [System.Serializable]
    public class SfxEntry
    {
        public string key;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float minPitch = 0.95f;
        [Range(0.5f, 2f)] public float maxPitch = 1.05f;

        public float GetPitch() => Random.Range(minPitch, maxPitch);
    }

    [SerializeField] private SfxEntry[] entries;

    private Dictionary<string, SfxEntry> lookup;

    void BuildLookup(){
        lookup = new Dictionary<string, SfxEntry>(entries.Length);
        foreach(var e in entries){
            if(string.IsNullOrEmpty(e.key)) continue;
            lookup[e.key] = e;
        }
    }

    public SfxEntry Get(string key){
        if(lookup == null) BuildLookup();
        return lookup.TryGetValue(key, out var entry) ? entry : null;
    }
}