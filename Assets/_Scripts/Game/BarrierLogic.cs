using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierLogic : MonoBehaviour
{
    public int hitPoints;

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
        if(hitPoints <= 0)
        {
            DestroyBarrier();
        }
    }
    public void DestroyBarrier()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public void RepairBarrier()
    {
        if(hitPoints <= 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        hitPoints++;
        GameManager.Instance.AddPoints(10);
    }
}
