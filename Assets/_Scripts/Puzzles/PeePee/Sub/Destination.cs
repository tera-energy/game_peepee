using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    static Destination _instance;
    public static Destination xInstance { get { return _instance; } }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("RemoveObject"))
        {
            TrPuzzlePeePee.xInstance.yRemoveObjectList();
        }
    }




    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

    }
}
