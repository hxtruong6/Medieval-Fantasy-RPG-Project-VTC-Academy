using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonDeathEffect : MonoBehaviour {
    public float timeEffectDeath = 2;
    public float pause = 1;
    public AnimationCurve fadeOut;
    public Material material1;
    public Material material2;
    float timer = 0;
    Renderer _renderer;
    Material[] mats;
    int shaderProperty;
    static DragonDeathEffect dde;
    private bool isDead = false;
    void Awake()
    {
        dde = this;
    }
    // Use this for initialization
    void Start () {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
		if (isDead)
        {
            if (timer < timeEffectDeath + pause)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
            }
            if (timer > timeEffectDeath)
            {
                DragonBoss.ClearBoss();
            }
            mats[0].SetFloat(shaderProperty, fadeOut.Evaluate(Mathf.InverseLerp(0, timeEffectDeath, timer)));
            mats[1].SetFloat(shaderProperty, fadeOut.Evaluate(Mathf.InverseLerp(0, timeEffectDeath, timer)));
        }
	}

    public static void DragonRegisterDeath()
    {
        dde.mats = dde._renderer.GetComponent<SkinnedMeshRenderer>().materials;
        dde.mats[0] = dde.material1;
        dde.mats[1] = dde.material2;
        dde._renderer.materials = dde.mats;
        dde.isDead = true;
    }


}
