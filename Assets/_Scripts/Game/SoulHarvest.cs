using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class SoulHarvest : MonoBehaviour
{
    public int soulsRequired,actualSouls=0, boxID;
    public bool isFull = false;
    public Animator containerAnimator;

    public BoxCollider soulZone;

    private void Start()
    {
        soulZone = GetComponent<BoxCollider>();
        soulZone.enabled = true;
    }

    private void Update()
    {
        if(actualSouls >= soulsRequired)
        {
            isFull = true;
        }

        if (isFull)
        {
            containerAnimator.SetBool("isOpened", false);
            soulZone.enabled = false;
        }
    }

    public void OpenContainer()
    {
        containerAnimator.SetBool("isOpened", true);
    }
}
