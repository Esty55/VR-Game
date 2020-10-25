using UnityEngine;
using System.Collections;

public class SimpleRotation : MonoBehaviour {

    public float speedRot = 0;
    public bool VecY = false;
    public bool VecX = true;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (VecX)
        transform.Rotate(Vector3.forward * speedRot* Time.deltaTime);
        if (VecY)
            transform.Rotate(Vector3.up * speedRot * Time.deltaTime);
    }
}
