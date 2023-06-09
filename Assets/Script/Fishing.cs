using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : MonoBehaviour
{
    public GameObject hookPrefab;
    public Transform hookSpawner;
    public LineRenderer line;
    public Slider powerSlider;
    public float hookSpeed = 10f;
    public float pullingSpeed = 8f;
    public float maxHookSpeed = 20f;
    public float hookSpeedIncreaseRate = 1f;
    public float maxRodRotation = 45f;
    public float rodRotationSpeed = 45f;
    public float parabolicEffect = 5f;

    private Animator animator;

    private bool isChargingThrow = false;
    private bool isHookThrown = false;
    private bool isHooked = false;
    private float currentHookSpeed;
    private Quaternion initialRodRotation;
    private GameObject currentHook;
    private float hookedTime = 0f;
    private Hook hookScript;
    public TextMeshProUGUI textUIGuide;

    public AudioClip reelSound;
    public AudioClip touchWaterSound;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        powerSlider = GameObject.FindGameObjectWithTag("PowerSlider").GetComponent<Slider>();
        textUIGuide = GameObject.Find("TextGuide").GetComponent<TextMeshProUGUI>();
        currentHookSpeed = hookSpeed;
        initialRodRotation = hookSpawner.rotation;
        line.positionCount = 2;
        line.enabled = false;
        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxHookSpeed - hookSpeed;
        powerSlider.value = 0f;
        audioSource = gameObject.AddComponent<AudioSource>();
        textGuideThrow();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHookThrown && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                animator.Play("CastingHold");
                isChargingThrow = true;
            }
            else if (isHookThrown)
            {
                textGuidePull();
                if (isHooked)
                {
                    hookedTime += Time.deltaTime;

                    if (hookedTime > 1f)
                    {
                        currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, hookSpawner.position, pullingSpeed * Time.deltaTime);
                        PlayReelSound();
                        pullingAnimatoin(true);
                    }
                }
                else
                {
                    PlayReelSound();
                    pullingAnimatoin(true);
                    currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, hookSpawner.position, pullingSpeed * Time.deltaTime);
                }
            }
        }

        if (isChargingThrow)
        {
            currentHookSpeed += hookSpeedIncreaseRate * Time.deltaTime;
            currentHookSpeed = Mathf.Clamp(currentHookSpeed, hookSpeed, maxHookSpeed);
            float rodRotationAmount = Mathf.Lerp(0f, maxRodRotation, (currentHookSpeed - hookSpeed) / (maxHookSpeed - hookSpeed));
            hookSpawner.rotation = initialRodRotation * Quaternion.Euler(-rodRotationAmount, 0f, 0f);
            powerSlider.value = currentHookSpeed - hookSpeed;
        }

        if (Input.GetMouseButtonUp(0) && isChargingThrow)
        {
            StopReelSound();

            animator.SetTrigger("Release");
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopReelSound();
        }

        if (isHookThrown)
        {
            line.SetPosition(0, hookSpawner.position);
            line.SetPosition(1, currentHook.transform.position);

            if (isHooked)
            {
                if (Input.GetMouseButton(0))
                {
                    hookedTime += Time.deltaTime;

                    if (hookedTime > 1f)
                    {
                        currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, hookSpawner.position, pullingSpeed * Time.deltaTime);
                        PlayReelSound();
                        pullingAnimatoin(true);
                    }
                }
                else
                {
                    hookedTime = 0f;
                    pullingAnimatoin(false);
                    StopReelSound();
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {

                    PlayReelSound();
                    pullingAnimatoin(true);
                    currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, hookSpawner.position, pullingSpeed * Time.deltaTime);
                }
                else
                {
                    pullingAnimatoin(false);

                }

            }
        }
    }

    public void textGuideThrow() {
        textUIGuide.text = "Throw (Hold)";
    }

    public void textGuidePull()
    {
        textUIGuide.text = "Pull (Hold)";
    }

    public void pullingAnimatoin (Boolean value)
    {
        animator.SetBool("PullingHook", value);
    }

    public void PlayReelSound()
    {
        audioSource.clip = reelSound;
        audioSource.Play();
    }

    public void StopReelSound()
    {
        audioSource.Stop();
    }

    public bool ResetHook()
    {
        if (isHookThrown)
        {
            Destroy(currentHook);
            isHookThrown = false;
            line.enabled = false;
            isHooked = false;
            powerSlider.value = 0f;

            hookScript.DestroyRippleEffect();
            StopReelSound();

            return true;
        }
        return false;
    }

    void ThrowHook()
    {
        isChargingThrow = false;
        currentHook = Instantiate(hookPrefab, hookSpawner.position, hookSpawner.rotation);
        Rigidbody rb = currentHook.GetComponent<Rigidbody>();
        hookScript = currentHook.gameObject.GetComponent<Hook>();
        rb.velocity = transform.forward * currentHookSpeed;
        rb.AddForce(Vector3.up * parabolicEffect, ForceMode.Impulse);
        currentHookSpeed = hookSpeed;
        StartCoroutine(RotateRodBack());
        isHookThrown = true;
        line.enabled = true;
        powerSlider.value = 0f;
        PlayReelSound();
    }

    IEnumerator RotateRodBack()
    {
        while (Quaternion.Angle(hookSpawner.rotation, initialRodRotation) > 0.1f)
        {
            hookSpawner.rotation = Quaternion.RotateTowards(hookSpawner.rotation, initialRodRotation, rodRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetIsHooked(bool value)
    {
        isHooked = value;
    }

    public bool GetIsHooked()
    {
        return isHooked;
    }
}
