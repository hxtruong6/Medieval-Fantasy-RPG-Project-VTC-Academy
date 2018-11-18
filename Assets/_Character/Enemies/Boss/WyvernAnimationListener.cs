using UnityEngine;

public class WyvernAnimationListener : MonoBehaviour
{
    private WyvernAttacking wyvernAttacking;
    // Use this for initialization
    void Start()
    {
        wyvernAttacking = GetComponentInParent<WyvernAttacking>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void switchState()
    {
        Debug.Log("Switch");
        wyvernAttacking.switchState();
    }
}
