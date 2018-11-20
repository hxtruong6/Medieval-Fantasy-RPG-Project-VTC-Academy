using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleBehaviour : AbilityBehaviour
{
    Vector3 startPosition;
    float speed = 1;
    float moveto = 2;
    bool start = false;
    public override void Use(AbilityUseParams useParamsToSet)
    {
        LookAtMousePosition();
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private void Update()
    {
        
        if (start)
        {
            //    gameObject.GetComponent<Rigidbody>().transform.position = Vector3.MoveTowards(gameObject.GetComponent<Rigidbody>().transform.position, hit.transform.position, speed * Time.deltaTime);
            //}
            //Vector3 targetforward = transform.forward * moveto;
            //Vector3 target = new Vector3(gameObject.GetComponent<Animator>().transform.position.x + moveto, gameObject.GetComponent<Animator>().transform.position.y, gameObject.GetComponent<Animator>().transform.position.z + moveto);
            gameObject.transform.position += Vector3.forward * Time.deltaTime * speed;
        }
        
    }

    private void LookAtMousePosition()
    {
        Vector3 mouse = Input.mousePosition;
        Camera camera = Camera.main;
        Ray castPoint = camera.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, camera.GetComponent<CameraRaycaster>().maxRaycastDepth))
        {
            gameObject.transform.LookAt(hit.point);
        }
    }

    private void StartRush()
    {
        GetComponent<EnergySystem>().ConsumeEnergy(GetEnergyCost());
        start = true;
    }
    private void StopRush()
    {
        start = false;
        //gameObject.GetComponent<Animator>().transform.parent.position = gameObject.GetComponent<Animator>().transform.position;
    }
}
