using UnityEditor;
using UnityEngine;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField] private Vector3 _worldOrigin = Vector3.zero;
        
        [field: SerializeField, Space(10)] public float cellSize { get; private set; } = 1f;
        [SerializeField] private LayerMask layerMask;
        
        public static GridManager Instance { get; private set; }
        public Grid Grid { get; private set; }

        private Vector3 _positionA;
        private Vector3 _positionB;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        private void Start()
        { 
            CreateGrid();
            LockArea();
        }

        #region GRID INITIALIZATION
        private void CreateGrid()
        {
            Vector2Int gridSize = Vector2Int.zero;
            gridSize += GetNumberOfCellsFromWorldOrigin(_pointA.position);
            gridSize += GetNumberOfCellsFromWorldOrigin(_pointB.position);
            
            Vector3 originPosition = GetGridOriginPosition();
            
            Grid = new Grid(gridSize.x, gridSize.y, cellSize, originPosition, layerMask);
            Grid.DrawGrid();
        }

        private void LockArea()
        {
            _positionA = _pointA.position;
            _positionB = _pointB.position;
            Destroy(_pointA.gameObject);
            Destroy(_pointB.gameObject);
        }
        
        private Vector2Int GetNumberOfCellsFromWorldOrigin(Vector3 position)
        {
            Vector2Int cells = new Vector2Int(
                Mathf.FloorToInt(Mathf.Abs(position.x - _worldOrigin.x) / cellSize),
                Mathf.FloorToInt(Mathf.Abs(position.y - _worldOrigin.y) / cellSize));
            
            return cells;
        }

        private Vector3 GetGridOriginPosition()
        {
            if (!_pointA || !_pointB) return Vector3.zero;
            
            Vector3 bottomLeftPosition = new Vector3(
                Mathf.Min(_pointA.position.x, _pointB.position.x),
                Mathf.Min(_pointA.position.y, _pointB.position.y),
                Mathf.Min(_pointA.position.z, _pointB.position.z));
            Vector2Int cells = GetNumberOfCellsFromWorldOrigin(bottomLeftPosition);
            Vector3 originPosition = new Vector3(
                cells.x * cellSize * Mathf.Sign(bottomLeftPosition.x),
                cells.y * cellSize * Mathf.Sign(bottomLeftPosition.y));
            
            return originPosition;
        }
        #endregion
        
        #region GIZMOS
        private void OnDrawGizmos()
        {
            if (IsSelectedOrChild())
            {
                Vector3 posA = Vector3.zero;
                Vector3 posB = Vector3.zero;
                if (Application.isPlaying)
                {
                    posA = _positionA;
                    posB = _positionB;
                }
                else if (_pointA && _pointB)
                {
                    posA = _pointA.position;
                    posB = _pointB.position;
                }
                
                Vector3 center = GetAreaCenter();
                Vector3 size = new Vector3(
                    Mathf.Abs(posA.x - posB.x),
                    Mathf.Abs(posA.y - posB.y),
                    0f);
                
                Gizmos.color = new Color(0, 1, 0, 0.2f);
                Gizmos.DrawCube(center, size);
                
                if (!Application.isPlaying)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(posA, 0.15f);
                    Gizmos.DrawSphere(posB, 0.15f);
                }
            }
        }

        private bool IsSelectedOrChild()
        {
            if (!Selection.activeGameObject)
                return false;

            if (Selection.activeGameObject == gameObject)
                return true;

            if (Selection.activeGameObject.transform.IsChildOf(gameObject.transform))
                return true;

            return false;
        }

        private Vector3 GetAreaCenter()
        {
            Vector3 posA = Vector3.zero;
            Vector3 posB = Vector3.zero;
            if (Application.isPlaying)
            {
                posA = _positionA;
                posB = _positionB;
            }
            else if (_pointA && _pointB)
            {
                posA = _pointA.position;
                posB = _pointB.position;
            }
            
            Vector3 center = Vector3.zero;
            center.x = posA.x > posB.x ? (posA.x - posB.x) / 2 + posB.x : (posB.x - posA.x) / 2 + posA.x;
            center.y = posA.y > posB.y ? (posA.y - posB.y) / 2 + posB.y : (posB.y - posA.y) / 2 + posA.y;
            
            return center;
        }
        #endregion
    }
}