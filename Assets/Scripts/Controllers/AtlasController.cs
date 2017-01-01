using System;
using Assets.Scripts;
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

    [Serializable]
    public class EmptyPrefabTypes
    {
        public Level Level;
        public Human Human;
        public Robot Robot;

        public Path PathContainer;
        public Waypoint Start;
        public Waypoint Midpoint;
        public Waypoint End;

        public Tower Tower;
    }

    public AnimationClip StandardMaleWalking;

    [SerializeField] public HumanSprites Humans;
    [SerializeField] public EmptyPrefabTypes EmptyPrefabs;
}

