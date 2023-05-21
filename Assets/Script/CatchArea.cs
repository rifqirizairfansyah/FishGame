using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CatchArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Fish fish = other.gameObject.GetComponent<Fish>();

        if (fish != null)
        {
            Fishing fishing = FindObjectOfType<Fishing>();

            if (fishing != null && fishing.GetIsHooked())
            {
                if (fishing.ResetHook())
                {
                    Debug.Log("Caught a fish!");
                    Debug.Log("Weight: " + fish.weight);
                    Debug.Log("Name: " + fish.fishName);
                    Debug.Log("Description: " + fish.description);
                    Destroy(fish.gameObject);
                }
            }
        }
    }
}
