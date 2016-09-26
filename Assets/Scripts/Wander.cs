using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Goes to a random cell. When it gets there (or blocked), picks another random cell.
    /// </summary>
    public class Wander : MonoBehaviour
    {
        public float speed = 5;
        public GameGrid.GameCell _targetCell = null;
        private PathFollower _pf;
        private bool _stopped;

        public bool IsStopped {
            get { return _stopped; }
        }

        public enum WanderModes
        {
            Random,
            ToCity, // TBD: Needs work to be "other city".
            ToCarcas,
        }

        public WanderModes WanderMode = WanderModes.Random;

        void Start()
        {
            // Set random initial rotation
            _pf = this.GetComponent<PathFollower>();
            _pf.AtFinish -= Pf_AtFinishOrBlocked;
            _pf.Blocked -= Pf_AtFinishOrBlocked;
            _pf.AtFinish += Pf_AtFinishOrBlocked;
            _pf.Blocked += Pf_AtFinishOrBlocked;

        }

        private void Pf_AtFinishOrBlocked(object sender, System.EventArgs e)
        {
            // Will try to make a new path on next update.
            _targetCell = null;
        }

        void Update()
        {
            if (_targetCell == null)
            {
                _targetCell = MakePathOrNull();
            }
            if (_targetCell == null)
            {
                _stopped = true;
            }
        }

        private GameGrid.GameCell MakePathOrNull()
        {

            var worldPos = this.transform.position;
            worldPos.z = -10;

            var gameGrid = Toolbox.Instance.GameManager.GameGrid;

            var endCell = GetTargetCellForWanderModeOrNull(gameGrid);
            if (endCell == null)
            {
                return null;
            }

            _pf.CurrentGameCell = gameGrid.MapPositionToGameCellOrNull(worldPos);
            _pf.PrevGameCell = _pf.CurrentGameCell;
            _pf.TargetCell = endCell;
            _pf.OrderedWaypointCells = new List<GameGrid.GameCell>(); // Empty. No way points.
            _pf.FollowToTargetCell(gameGrid, transform.position);
            return _pf.CurrentGameCell;
        }

        private GameGrid.GameCell GetTargetCellForWanderModeOrNull(GameGrid gameGrid)
        {
            if (WanderMode == WanderModes.Random)
            {
                return gameGrid.RandomGameCellOrNull(GameGrid.GameCell.GroundTypes.Dirt);
            }
            if (WanderMode == WanderModes.ToCarcas)
            {
                return gameGrid.RandomCarcasOrNull();
            }
            if (WanderMode == WanderModes.ToCity)
            {
                return gameGrid.RandomTowerCellOrNull(Tower.TowerClasses.City);
            }
            Debug.LogError("Unknown wander mode.");
            return null;
        }

        public void RestartWandering()
        {
            _targetCell = null;
        }

        public void StopWandering()
        {
            _pf.TargetCell = null;
        }
    }
}