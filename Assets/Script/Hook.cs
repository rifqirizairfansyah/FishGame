using StylizedWater2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public bool isTouchingWater = false;
    private float raycastDistance = 0.3f;
    public GameObject splashEffect;
    public GameObject rippleEffect;

    private GameObject currentRipple;
    private bool hasInstantiated = false;

    public AudioClip fishEatSound;
    public AudioClip touchWaterSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (!isTouchingWater)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
            {
                WaterObject waterObject = hit.collider.GetComponent<WaterObject>();
                if (waterObject != null)
                {
                    Fishing fishing = FindObjectOfType<Fishing>();
                    fishing.textGuidePull();
                    if (fishing != null)
                    {
                        fishing.StopReelSound();
                    }

                    isTouchingWater = true;
                    audioSource.PlayOneShot(touchWaterSound);
                    FloatingTransform floatingTransform = GetComponent<FloatingTransform>();
                    if (floatingTransform != null)
                    {
                        floatingTransform.autoFind = true;
                    }
                }
            }
        }

        if (currentRipple != null)
        {
            currentRipple.transform.position = transform.position;
        }

        if (isTouchingWater && !hasInstantiated)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            if (splashEffect != null)
            {
                GameObject splash = Instantiate(splashEffect, transform.position, Quaternion.identity);
                Destroy(splash, 0.75f);
                hasInstantiated = true;
                currentRipple = Instantiate(rippleEffect, transform.position, Quaternion.identity);
            }
        }
    }

    public void DestroyRippleEffect()
    {
        if (currentRipple != null)
        {
            Destroy(currentRipple);
            currentRipple = null;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isTouchingWater = true;
        }
    }
}
