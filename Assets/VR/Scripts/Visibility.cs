using UnityEngine;
using System.Collections;

public class Visibility : MonoBehaviour {

    public bool visible = true;

    // Use this for initialization
    void Start () {

        if (!visible) GetComponent< MeshRenderer > ().enabled = false;
        if (visible) GetComponent< MeshRenderer > ().enabled = true;

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
