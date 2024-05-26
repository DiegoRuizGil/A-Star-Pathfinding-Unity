using System.Collections.Generic;
using GridSystem;
using Pathfinding;
using UnityEngine;

namespace Demo
{
    public class IAMovement : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private float _pathUpdateTime;
        [SerializeField] private float _speed;

        [Header("Dependencies")] [SerializeField]
        private Transform _target;

        private Vector3 _movDirection;
        private int _pathIndex;
        private Vector3 _nextPosition;

        private float _movDuration; // duration of the movement between the current and the next positions
        private float _movTimer;
        
        private List<Node> _path = new List<Node>();

        private void Start()
        {
            _pathIndex = 1;
            InvokeRepeating(nameof(UpdatePath), 0f, _pathUpdateTime);
        }

        private void Update()
        {
            ApplyMovement();
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            if (_path.Count > 1)
            {
                Gizmos.color = Color.green;
            
                for (int i = 1; i < _path.Count; i++)
                {
                    Vector3 currentPosition = _path[i - 1].CenterPosition;
                    Vector3 nextPosition = _path[i].CenterPosition;

                    Gizmos.DrawLine(currentPosition, nextPosition);
                }
            }
        }

        private void UpdatePath()
        {
            Node startNode = GridManager.Instance.Grid.GetNode(transform.position);
            Node finalNode = GridManager.Instance.Grid.GetNode(_target.position);
            
            if (finalNode == null)
                _path = new List<Node>();
            else
                _path = AStarPathfinding.GetPath(startNode, finalNode, GridManager.Instance.Grid);
            
            _pathIndex = 1;
            _movTimer = 0;
            if (_path.Count > 1)
            {
                _nextPosition = _path[_pathIndex].CenterPosition;
                _movDuration = GetMovementDuration(_nextPosition);
            }
        }

        private void ApplyMovement()
        {
            if (_path.Count == 0)
            {
                _movTimer = 0f;
                return;
            }
            
            _movDirection = (_nextPosition - transform.position).normalized;
            transform.Translate(_movDirection * (_speed * Time.deltaTime));
            
            _movTimer += Time.deltaTime;
            if (_movTimer >= _movDuration)
            {
                transform.position = _nextPosition;
                _pathIndex = (_pathIndex + 1) % _path.Count;
                _nextPosition = _path[_pathIndex].CenterPosition;
                
                _movDuration = GetMovementDuration(_path[_pathIndex].CenterPosition);
                _movTimer = 0f;
            }
        }

        private float GetMovementDuration(Vector3 nextPosition)
        {
            float distance = Vector3.Distance(transform.position, nextPosition);
            return distance / _speed;
        }
    }
}
