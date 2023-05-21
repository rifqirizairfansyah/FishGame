using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public float weight = 1f;
    public string fishName = "Fish";
    public string description = "A fish";

    public float speed = 1f;
    public float eatSpeed = 0.5f;
    public Vector3 minBounds;
    public Vector3 maxBounds;
    public float eatTime = 3f;
    public float eatRadius = 1f;
    public float eatChance = 0.5f;

    private GameObject hook;
    private Vector3 direction;
    private float currentEatTime;

    private bool isEating = false;

    void Start()
    {
        direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;
        currentEatTime = eatTime;
    }

    void Update()
    {
        if (!isEating)
        {
            hook = GameObject.FindGameObjectWithTag("Hook");
            if (hook != null && Vector3.Distance(transform.position, hook.transform.position) < eatRadius)
            {
                Fishing fishing = FindObjectOfType<Fishing>();
                if (fishing != null && fishing.GetIsHooked())
                {
                    transform.position += direction * speed * Time.deltaTime;
                    if (transform.position.x < minBounds.x || transform.position.x > maxBounds.x || transform.position.z < minBounds.z || transform.position.z > maxBounds.z)
                    {
                        direction = new Vector3(-direction.x, 0f, -direction.z);
                    }
                    currentEatTime = eatTime;
                }
                else
                {
                    currentEatTime -= Time.deltaTime;
                    if (currentEatTime <= 0f && UnityEngine.Random.value < eatChance)
                    {
                        Fish[] otherFish = FindObjectsOfType<Fish>();
                        bool canEat = true;
                        foreach (Fish fish in otherFish)
                        {
                            if (fish != this && fish.isEating)
                            {
                                canEat = false; break;
                            }
                        }
                        if (canEat)
                        {
                            isEating = true;
                            if (fishing != null)
                            {
                                fishing.SetIsHooked(true);
                                ResetEatTime();
                            }
                        }
                    }
                }
            }
            else
            {
                transform.position += direction * speed * Time.deltaTime;
                if (transform.position.x < minBounds.x || transform.position.x > maxBounds.x || transform.position.z < minBounds.z || transform.position.z > maxBounds.z)
                {
                    direction = new Vector3(-direction.x, 0f, -direction.z);
                }
                currentEatTime = eatTime;
            }
        }
        else
        {
            if (hook != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, hook.transform.position, eatSpeed * Time.deltaTime);
            }
            else
            {
                isEating = false;
                ResetEatTime();
            }
        }
    }

    void ResetEatTime()
    {
        Fish[] otherFish = FindObjectsOfType<Fish>();

        foreach (Fish fish in otherFish)
        {
            if (fish != this && !fish.isEating && hook != null && Vector3.Distance(fish.transform.position, hook.transform.position) < eatRadius)
            {
                fish.currentEatTime = eatTime;
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, eatRadius);
        Gizmos.DrawWireCube((minBounds + maxBounds) / 2f, maxBounds - minBounds);
    }
}
