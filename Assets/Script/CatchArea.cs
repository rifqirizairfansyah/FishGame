using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class CatchArea : MonoBehaviour
{
    public GameObject UI;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI fishWeightText;
    public TextMeshProUGUI fishLengthText;
    public TextMeshProUGUI fishDescriptionText;

    private void Start()
    {
        UIPanel(false);
    }

    void OnTriggerEnter(Collider other)
    {
        Fish fish = other.gameObject.GetComponent<Fish>();

        if (fish != null)
        {
            Fishing fishing = FindObjectOfType<Fishing>();
            fishing.textGuideThrow();

            if (fishing != null && fishing.GetIsHooked())
            {
                if (fishing.ResetHook())
                {
                    fishNameText.text = fish.fishName;
                    fishWeightText.text = fish.weight.ToString();
                    fishLengthText.text = fish.length.ToString();
                    fishDescriptionText.text = fish.description;
                    Destroy(fish.gameObject);
                    fishing.pullingAnimatoin(false);
                    UIPanel(true);
                   }
            }
        } else
        {
            Fishing fishing = FindObjectOfType<Fishing>();
            fishing.pullingAnimatoin(false);
            fishing.textGuideThrow();
            fishing.ResetHook();
        }
    }

    public void UIPanel (Boolean show)
    {
        UI.SetActive(show);

    }
}
