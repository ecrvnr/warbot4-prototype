using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public GameObject PrefabExplosion;
    public GameObject PrefabFire;
    public GameObject PrefabMoving;

    private static SoundManager actual;

    public static SoundManager Actual { get { return actual; } }

	// Use this for initialization
	void Start () {
        actual = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Play(GameObject obj, GameObject sound)
    {
        Instantiate(sound, obj.transform);
    }

    public void PlayExplosion(GameObject obj)
    {
        this.Play(obj, PrefabExplosion);
    }

    public void PlayFire(GameObject obj)
    {
        this.Play(obj, PrefabFire);
    }

    public void PlayMoving(GameObject obj)
    {
        this.Play(obj, PrefabMoving);
    }

}
