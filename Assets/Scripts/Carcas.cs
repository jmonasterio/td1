using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// When an entity dies it becomes a carcas until:
    /// (1) It decomposes
    /// (2) Or is repaired.
    /// </summary>
    public class Carcas : EntityBehavior
    {
        /// <summary>
        /// What type of entity is this a carcas of?
        /// </summary>
        public Entity.EntityClasses CarcasClass;

        public float DecomposeTimeInterval = 7.0f; 

        private float _decomposeTimeRemainingAtStartOfDrag;
        private float _decomposeStartTime;
        private DragSource _dragSourceOrNull;
        private Animator _animator;

        public enum AnimStates
        {
            Normal = 1,
        }

        public new void Start()
        {
            base.Start();
            Debug.Assert( _entity != null);
            _dragSourceOrNull = GetComponent<DragSource>();
            _animator = GetComponent<Animator>();
            SetAnimState(AnimStates.Normal);
            StartDecomposing();
        }

        public void SetAnimState(AnimStates animState)
        {
            _animator.SetInteger("AnimState", (int)animState);
            string stateName = animState.ToString();
            _animator.Play(stateName);
        }

        public void Update()
        {
            if (!IsDragging())
            {
                // As entity moves around, we want to update the CellMap to know where entity is.
                if (IsDecomposingDone()) // Needs to be a constant
                {
                    // We're done decomposing.
                    PlayDecomposingSoundAndGraphics();
                    EndDecomposing();
                }
                else
                {
                    ResumeDecompose();
                }
            }
            else
            {
                PauseDecomposing();
            }
        }

        private void PlayDecomposingSoundAndGraphics()
        {
            // TBD: Sound and graphics here?
        }

        private bool IsDragging()
        {
            return ((_dragSourceOrNull != null) && _dragSourceOrNull.Dragging);

        }


        private void StartDecomposing()
        {
            Debug.Assert(_decomposeStartTime == 0.0f);
            _decomposeStartTime = Time.time;
            _decomposeTimeRemainingAtStartOfDrag = 0.0f;
        }

        private void ResumeDecompose()
        {
            if (_decomposeTimeRemainingAtStartOfDrag > 0.0f)
            {
                _decomposeStartTime = Time.time - _decomposeTimeRemainingAtStartOfDrag;
                _decomposeTimeRemainingAtStartOfDrag = 0.0f;
            }
        }

        private void PauseDecomposing()
        {
            if (_decomposeTimeRemainingAtStartOfDrag == 0.0f)
            {

                if (_decomposeStartTime > 0.0f)
                {
                    _decomposeTimeRemainingAtStartOfDrag = (Time.time - _decomposeStartTime);
                }
            }
        }

        private void EndDecomposing()
        {
            Debug.Assert(!IsDragging());
            Debug.Assert(_decomposeStartTime > 0.0f);
            UnityEngine.Object.Destroy(this.transform.gameObject);
            _decomposeStartTime = 0.0f;
            _decomposeTimeRemainingAtStartOfDrag = 0.0f;
        }

        private bool IsDecomposingDone()
        {
            return (_decomposeStartTime > 0.0f) && (Time.time > _decomposeStartTime + DecomposeTimeInterval);
        }


    }
}