using UnityEngine;

public class WyvernAnimationListener : MonoBehaviour
{
    private WyvernAttacking wyvernAttacking;
    // Use this for initialization
    void Start()
    {
        wyvernAttacking = GetComponentInParent<WyvernAttacking>();
    }

  void switchState()
    {
        Debug.Log("Switch");
        wyvernAttacking.switchState();
    }

    void deleteAllCollision()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        Debug.Log("Collider "+ colliders.Length);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

}
