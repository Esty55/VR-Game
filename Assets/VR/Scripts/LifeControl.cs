using UnityEngine;
using System.Collections;

public class LifeControl : MonoBehaviour {

    public AudioClip Fall; //fall sound of eagle
    public ParticleSystem SplashEffect;
    public AudioClip WaterSplash;
    private int cap;

    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain")) //all collided objects needs to be set Terrain tag
        {
            Camera.main.GetComponent<GameLogic>().impacted = true;
            Camera.main.GetComponent<AudioSource>().PlayOneShot(Fall, 0.7F);
            StartCoroutine(Camera.main.GetComponent<GameLogic>().ResetByFall(0.2F,0));
        }
    }

}
