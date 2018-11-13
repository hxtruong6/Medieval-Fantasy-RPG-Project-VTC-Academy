using System;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{

    private GameObject mainSkeleton;
   
    void Update()
    {
        if (mainSkeleton && mainSkeleton.GetComponent<HealthSystem>().HealthAsPercentage <= 0)
        {

            Destroy(this.gameObject);
        }
    }

    public void GetMainSkeleton()
    {
        mainSkeleton = gameObject.GetComponentInChildren<HealthSystem>().gameObject;
    }
}
