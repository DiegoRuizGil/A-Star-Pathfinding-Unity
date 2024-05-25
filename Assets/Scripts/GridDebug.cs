using GridSystem;
using UnityEngine;

public class GridDebug : MonoBehaviour
{
    private Camera _mainCamera;

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
