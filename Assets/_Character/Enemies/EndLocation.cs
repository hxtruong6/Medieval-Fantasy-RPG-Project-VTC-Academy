using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLocation : MonoBehaviour {
    static EndLocation endLocation;
    public GameObject END;
    int enemyCount;
    void Awake()
    {
        endLocation = this;
    }
    public static void  AddToEnemyCount()
    {
        endLocation.enemyCount++;
        endLocation.END.SetActive(false);
    }
    public static void RemoveEnemy()
    {
        endLocation.enemyCount--;
        if (endLocation.enemyCount <= 0) endLocation.END.SetActive(true);
    }
}
