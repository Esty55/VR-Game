using UnityEngine;
using System.Collections;

public class SceneGM : MonoBehaviour {

    public int GameMode = 0;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }


    }

}
