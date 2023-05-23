using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    public float length = 1f;
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

    public AudioClip fishEatSound;
    private AudioSource audioSource;

    public GameObject hook;
    private Vector3 direction;
    private float currentEatTime;

    private bool isEating = false;

    void Start()
    {
        direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;
        currentEatTime = eatTime;
        length = Mathf.Round(UnityEngine.Random.Range(18f, 24f) * 100f) / 100f;
        weight = Mathf.Round(((length - 18f) * (0.5f - 0.3f) / (24f - 18f) + 0.3f) * 100f) / 100f;
        audioSource = gameObject.AddComponent<AudioSource>();
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
                    MoveFish();
                }
                else
                {
                    currentEatTime -= Time.deltaTime;
                    if (currentEatTime <= 0f && UnityEngine.Random.value < eatChance && CanEat())
                    {
                        isEating = true;
                        audioSource.PlayOneShot(fishEatSound);
                        fishing.SetIsHooked(true);
                        ResetEatTime();
                    }
                }
            }
            else
            {
                MoveFish();
            }
            transform.LookAt(transform.position + direction);
        }
        else
        {
            if (hook != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, hook.transform.position, eatSpeed * Time.deltaTime);
                transform.LookAt(hook.transform);
            }
            else
            {
                isEating = false;
                ResetEatTime();
            }
        }
    }

    void MoveFish()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (transform.position.x < minBounds.x || transform.position.x > maxBounds.x || transform.position.z < minBounds.z || transform.position.z > maxBounds.z)
        {
            direction = new Vector3(-direction.x, 0f, -direction.z);
        }
        currentEatTime = eatTime;
    }

    bool CanEat()
    {
        Fish[] otherFish = FindObjectsOfType<Fish>();
        foreach (Fish fish in otherFish)
        {
            if (fish != this && fish.isEating) return false;
        }
        return true;
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
