using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
class Background : MonoBehaviour
    {
        void Start()
        {
            // Force component onto background LAYER.
            this.gameObject.layer = 8;
        }
    }
