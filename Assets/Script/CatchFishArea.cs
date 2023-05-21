using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchFishArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
    }
}
