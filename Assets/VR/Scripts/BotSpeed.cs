using UnityEngine;
using System.Collections;

public class BotSpeed : MonoBehaviour {

    ////////////////Describes eaglebots following

    public Transform Target;
    public float speed=0;
    public float Dist;
    public float TimeSpeed = 0;
    private float speed_val = 2;
    public GameObject Eagle;
    public Animation EagleAnim;
    public bool randAnim=false;
    public float randTime = 4;

    void Start () {
        EagleAnim = Eagle.GetComponent<Animation>();
    }
	
	void FixedUpdate () {
        TimeSpeed += Time.deltaTime;
        
        if (TimeSpeed >= randTime)
        {
            randAnim = !randAnim;
            if (randAnim)
            {
                EagleAnim["Armature|Fly_main"].speed = speed_val;
                EagleAnim.CrossFade("Armature|Fly_main");
            }
            if (!randAnim)
            {
                EagleAnim["Armature|Free_fly"].speed = speed_val;
                EagleAnim.CrossFade("Armature|Free_fly");
            }
            speed_val = Random.Range(2,2.5f);
            randTime = Random.Range(1,5);
            TimeSpeed = 0;
        }
        Dist = Vector3.Distance(transform.position, Target.transform.position);

            speed = Dist / speed_val; 

        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
