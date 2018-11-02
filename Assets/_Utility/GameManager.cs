using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HealthPotionConfig smallHealthPotion;
    public HealthPotionConfig largeHealthPotion;
    public ManaPotionConfig smallManaPotion;
    public ManaPotionConfig largeManaPotion;


    void Start ()
    {
        DontDestroyOnLoad(this);
	}
}
