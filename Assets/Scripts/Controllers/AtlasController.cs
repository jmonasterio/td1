using System;
using UnityEditor;
using UnityEngine;

public class AtlasController : MonoBehaviour
{

    public void Awake()
    {
        // Load atlas from disk folders.
        var assets = AssetDatabase.LoadAllAssetsAtPath(@"C:\td1\Assets\Animations\HumanGathererWalking.anim");
        StandardMaleWalking = assets[0] as AnimationClip;
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

