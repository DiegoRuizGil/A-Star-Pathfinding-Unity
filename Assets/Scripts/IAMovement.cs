using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAMovement : MonoBehaviour
{
    [Header("Movement Config")]
    public Rigidbody2D rb2D;
    public float velocity;
    //[Range(0, 100)] public float maxVelocity;
    //[Range(0, 100)] public float aceleration;

    private Vector2 movement;
    private float movementDuration;
    private int pathNodeIndex;
    private float movementTimer;

    private bool inPath;

    void Start()
    {
        movement = Vector2.zero;
        movementDuration = 0f;
        pathNodeIndex = 0;
        movementTimer = 0f;
        inPath = false;
    }

    void Update()
    {
        if (inPath)
            movementTimer += Time.deltaTime;
        else
            movementTimer = 0f;
    }

    public void StopWalking(int number)
    {
        StopAllCoroutines();
        rb2D.velocity = Vector2.zero;
    }

    public void StartPath(Vector3[] path)
    {
        StopAllCoroutines();

        Debug.Log("Nuevo camino recibido");

        if (movementDuration == 0f)
            movementDuration = GetMovementDuration(path[1]);

        inPath = true;
        StartCoroutine(nameof(WalkPath), path);
    }

    private IEnumerator WalkPath(Vector3[] path)
    {
        pathNodeIndex = 0;

        while (pathNodeIndex < (path.Length - 1))
        {
            if (CheckTimer() || transform.position == path[0])
            {
                // Need to adjust position for the deviation due to decimals
                if (pathNodeIndex != 0)
                    transform.position = path[pathNodeIndex];

                ApplyMovement(path[pathNodeIndex + 1]);
                movementDuration = GetMovementDuration(path[pathNodeIndex + 1]);

                pathNodeIndex++;
                // Debug.Log("Index: " + pathNodeIndex);
            }
            else
            {
                yield return null;
            }
        }

        inPath = false;
        rb2D.velocity = Vector2.zero;
    }

    private float GetMovementDuration(Vector3 nextPoint)
    {
        float distance = Vector2.Distance(this.transform.position, nextPoint);

        return distance / velocity;
    }

    private bool CheckTimer()
    {
        if(movementTimer >= movementDuration)
        {
            movementTimer = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ApplyMovement(Vector3 nextPoint)
    {
        movement = GetDirection(nextPoint);
        rb2D.velocity = movement * velocity;
    }

    private Vector2 GetDirection(Vector3 nextPoint)
    {
        Vector2 currentPos = this.transform.position;
        Vector2 direction = new Vector2(nextPoint.x - currentPos.x, nextPoint.y - currentPos.y);

        return direction.normalized;
    }
}
