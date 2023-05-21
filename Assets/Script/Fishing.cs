using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : MonoBehaviour
{
    public GameObject hookPrefab;
    public Transform hookSpawner;
    public LineRenderer line;
    public Slider powerSlider;
    public float hookSpeed = 10f;
    public float maxHookSpeed = 20f;
    public float hookSpeedIncreaseRate = 1f;
    public float maxRodRotation = 45f;
    public float rodRotationSpeed = 45f;

    private Animator animator;

    private bool isChargingThrow = false;
    private bool isHookThrown = false;
    private bool isHooked = false;
    private float currentHookSpeed;
    private Quaternion initialRodRotation;
    private GameObject currentHook;
    private float hookedTime = 0f;
    private Hook hookScript;


    void Start()
    {
        animator = GetComponent<Animator>();
        powerSlider = GameObject.FindGameObjectWithTag("PowerSlider").GetComponent<Slider>();
        currentHookSpeed = hookSpeed;
        initialRodRotation = hookSpawner.rotation;
        line.positionCount = 2;
        line.enabled = false;
        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxHookSpeed - hookSpeed;
        powerSlider.value = 0f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHookThrown)
            {
                animator.Play("CastingHold");
                isChargingThrow = true;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            // ResetHook();
        }

        if (isChargingThrow)
        {
            currentHookSpeed += hookSpeedIncreaseRate * Time.deltaTime;
            currentHookSpeed = Mathf.Clamp(currentHookSpeed, hookSpeed, maxHookSpeed);
            float rodRotationAmount = Mathf.Lerp(0f, maxRodRotation, (currentHookSpeed - hookSpeed) / (maxHookSpeed - hookSpeed));
            hookSpawner.rotation = initialRodRotation * Quaternion.Euler(-rodRotationAmount, 0f, 0f);
            powerSlider.value = currentHookSpeed - hookSpeed;


            if (currentHookSpeed == maxHookSpeed)
            {
               // ThrowHook();
                animator.SetTrigger("Release");
            }
        }

        if (Input.GetMouseButtonUp(0) && isChargingThrow)
        {
            //ThrowHook();

            animator.SetTrigger("Release");
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
                        currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, hookSpawner.position, hookSpeed * Time.deltaTime);
                        hookScript.DestroyRippleEffect();
                    }
                }
                else
                {
                    hookedTime = 0f;
                }
            }
            else
            {
                if (Input.GetMouseButton(1))
                {
                    currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, hookSpawner.position, hookSpeed * Time.deltaTime);
                }
            }
        }
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
        currentHookSpeed = hookSpeed;
        StartCoroutine(RotateRodBack());
        isHookThrown = true;
        line.enabled = true;
        powerSlider.value = 0f;
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
