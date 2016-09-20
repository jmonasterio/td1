using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// When an entity dies it becomes a carcas until:
    /// (1) It decomposes
    /// (2) Or is repaired.
    /// </summary>
    public class Carcas : MonoBehaviour
    {
        /// <summary>
        /// What type of entity is this a carcas of?
        /// </summary>
        public Entity.EntityClasses CarcasClass;

        public event EventHandler Decomposed;
        public float DecomposeTimeInterval = 7.0f;
        public float IncomeValue = 10; // TBD: Should be set from thing that died.


        private float _decomposeTimeRemainingAtStartOfDrag;
        private float _decomposeStartTime;
        private Entity _entity;
        private DragSource _dragSourceOrNull;
        private Animator _animator;

        public enum AnimStates
        {
            Decomposing = 5,
        }


        public void Start()
        {
            _entity = GetComponent<Entity>();
            _dragSourceOrNull = GetComponent<DragSource>();
            _animator = GetComponent<Animator>();
            SetAnimState(AnimStates.Decomposing);
        }

        public void SetAnimState(AnimStates animState)
        {
            _animator.SetInteger("AnimState", (int)animState);
            //string stateName = animState.ToString();
            //_animator.Play(stateName);
        }

        public void Update()
        {
            if (!IsDragging())
            {
                // As entity moves around, we want to update the CellMap to know where entity is.
                if (IsDecomposingDone()) // Needs to be a constant
                {
                    // We're done decomposing.
                    // TBD: Sound and graphics here?
                    EndDecomposing();
                }
            }
        }

        private bool IsDragging()
        {
            return ((_dragSourceOrNull != null) && _dragSourceOrNull.Dragging);

        }


        public void StartDecomposing()
        {
            Debug.Assert(_decomposeStartTime == 0.0f);
            _decomposeStartTime = Time.time;
            _decomposeTimeRemainingAtStartOfDrag = 0.0f;
        }

        public void ResumeDecompose()
        {
            if (_decomposeTimeRemainingAtStartOfDrag > 0.0f)
            {
                _decomposeStartTime = Time.time - _decomposeTimeRemainingAtStartOfDrag;
                _decomposeTimeRemainingAtStartOfDrag = 0.0f;
            }
        }

        public void PauseDecomposing()
        {
            if (_decomposeStartTime > 0.0f)
            {
                _decomposeTimeRemainingAtStartOfDrag = (Time.time - _decomposeStartTime);
            }
        }

        private void EndDecomposing()
        {
            Debug.Assert(!IsDragging());
            Debug.Assert(_decomposeStartTime > 0.0f);
            if (Decomposed != null)
            {
                Decomposed(this, new EventArgs());
            }
            _decomposeStartTime = 0.0f;
        }

        private bool IsDecomposingDone()
        {
            return (_decomposeStartTime > 0.0f) && (Time.time > _decomposeStartTime + DecomposeTimeInterval);
        }


    }
}