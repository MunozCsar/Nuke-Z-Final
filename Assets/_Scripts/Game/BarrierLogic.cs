using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierLogic : MonoBehaviour
{
    public int hitPoints;
    public GameObject[] planks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReduceHitPoints()
    {
        hitPoints--;
        DestroyBarrier();
    }
    public void DestroyBarrier()
    {
        planks[hitPoints].SetActive(false);
        if(hitPoints <= 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void RepairBarrier()
    {
        if(hitPoints <= 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        planks[hitPoints].SetActive(true);
        hitPoints++;
        GameManager.Instance.AddPoints(10);
    }
}
