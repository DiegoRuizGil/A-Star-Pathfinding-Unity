using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pruebas : MonoBehaviour
{
    private int[] a;
    private int[] b;

    void Start()
    {
        a = new int[5] {1, 2, 6, 9, 10};
        b = new int[3] {6, 9, 10};
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (a.Intersect(b).Count() == b.Length)
            {
                Debug.Log("Todos los elementos de b estan en a");
            }
            else
            {
                Debug.Log("No todos los elementos de b estan en a");
            }
        }
    }
}
