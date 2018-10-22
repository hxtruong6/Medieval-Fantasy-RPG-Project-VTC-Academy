using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    public Flock flock;
    public int index;
    //[HideInInspector] public bool alignment = true;
    //[HideInInspector] public bool cohesion = true;
    //[HideInInspector] public bool separation = true;
    public bool flocking;


    private Enemy enemy;

    public Vector3 Heading()
    {
        return enemy.GetHeading();
    }

    void Start()
    {   
        //if (flocking)
        //{
        //    steering += flock.Calculate(index);
        //}
    }

    public Vector3 Seek(Vector3 pos)
    {
        return enemy.Seek(pos);
    }










}
