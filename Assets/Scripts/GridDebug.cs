using System.Collections.Generic;
using GridSystem;
using Pathfinding;
using UnityEngine;

public class GridDebug : MonoBehaviour
{
    public Transform targetPoint;
    
    private Camera _mainCamera;

    private List<Node> _path = new List<Node>();

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Node node = GetMousePositionNode();
            Debug.Log(node.ToString());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // generate path
            Node startNode = GridManager.Instance.Grid.GetNode(transform.position);
            Node finalNode = GridManager.Instance.Grid.GetNode(targetPoint.position);
            _path = AStarPathfinding.GetPath(startNode, finalNode, GridManager.Instance.Grid);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            // clean path
            _path = new List<Node>();
        }
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

    private Vector3 GetMouseWorldPosition()
    {
        return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private Node GetMousePositionNode()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        return GridManager.Instance.Grid.GetNode(mousePos);
    }
}
