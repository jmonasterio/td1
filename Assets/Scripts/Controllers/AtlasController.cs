using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class AtlasController : MonoBehaviour
{

    [Serializable]
    public class HumanSprites
    {
        public Sprite StandardMale;
        public Sprite StandardFemale;
        public Sprite GathererMale;
        public Sprite GathererFemale;

    }

    public AnimationClip StandardMaleWalking;

    [SerializeField] public HumanSprites Humans;
}

