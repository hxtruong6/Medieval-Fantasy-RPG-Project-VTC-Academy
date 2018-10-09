using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(WeaponSystem))]

public class Goblin : Enemy
{  
    void Update()
    {
        this.PlayerOrEnemyAliveToContinue();
        this.UpdateDistanceToPlayer();
        this.Patroling();
        this.Attacking();
        this.Chasing();
        this.Fleeing();
    }
}
