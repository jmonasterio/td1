using System;
using Assets.Scripts;

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
        public Block Block;

        public Path PathContainer;
        public Waypoint Start;
        public Waypoint Midpoint;
        public Waypoint End;

        public Tower Tower;
    }

    [Serializable]
    public class HealthSprites
    {
        public Sprite Health0;
        public Sprite Health1;
        public Sprite Health2;
        public Sprite Health3;
        public Sprite Health4;
        public Sprite Health5;
        public Sprite Health6;
        public Sprite Health7;
        public Sprite Health8;
        public Sprite Health9;
        public Sprite Health10;
    }

    public AnimationClip StandardMaleWalking;

    [SerializeField] public HumanSprites Humans;
    [SerializeField] public EmptyPrefabTypes EmptyPrefabs;
    [SerializeField] public HealthSprites DefaultHealthSprites;
}

