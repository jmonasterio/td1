using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AtlasController : MonoBehaviour
{

    public void Awake()
    {
        // Load atlas from disk folders.
#if UNITY_EDITOR
        var assets = AssetDatabase.LoadAllAssetsAtPath(@"C:\td1\Assets\Animations\HumanGathererWalking.anim");
        StandardMaleWalking = assets[0] as AnimationClip;
#endif
    }

    [Serializable]
    public class HumanSprites
    {
        public Sprite StandardMale;
        public Sprite StandardFemale;
        public Sprite GathererMale;
        public Sprite GathererFemale;

    }

    public AnimationClip StandardMaleWalking;

    [SerializeField]
    public HumanSprites Humans;
}

