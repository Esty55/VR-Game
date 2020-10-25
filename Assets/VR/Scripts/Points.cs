using UnityEngine;
using System.Collections;


public class Points : MonoBehaviour {

    public float Equ_Points = 0; //Player points
    public bool gold_bonus = false; //is gold bonus?
    public bool green_bonus = false;//is green bonus?
    public bool radial_target = false; //is radial target (checkpoint)
    public AudioClip TB_sound; //turbo sound
    
    public GameObject MainCamera;

	void Start () {

        MainCamera = GameObject.Find("Main Camera");

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gold_bonus) //if player gets gold feather
            {
                MainCamera.GetComponent<AudioSource>().PlayOneShot(MainCamera.GetComponent<GameLogic>().GetPoints, .4f);
                Equ_Points = 500;
                MainCamera.GetComponent<GameLogic>().GoldBonus = true;
                MainCamera.GetComponent<GameLogic>().TimeBonus = 5;
                MainCamera.GetComponent<GameLogic>().Equ_Points += Equ_Points;

                MainCamera.GetComponent<GameLogic>().Checkpoint_spawn = true;
                if (MainCamera.GetComponent<GameLogic>().ch_idx == 14) //if player reached last checkpoint then start next game mode
                {
                    MainCamera.GetComponent<GameLogic>().EndedExploring = true;
                }
                Destroy(transform.parent.gameObject);
            }

            if (!gold_bonus && !green_bonus && !radial_target) //if player gets blue feather
            {
                Equ_Points = 300;
                MainCamera.GetComponent<AudioSource>().PlayOneShot(MainCamera.GetComponent<GameLogic>().GotFeather, .3f);
                MainCamera.GetComponent<GameLogic>().Equ_Points += Equ_Points;
                MainCamera.GetComponent<GameLogic>().Feather_left -= 1;
                Destroy(gameObject);
            }

            if (green_bonus) //if player gets green feather
            {
                Equ_Points = 100;
                MainCamera.GetComponent<GameLogic>().Equ_Points += Equ_Points;
                MainCamera.GetComponent<GameLogic>().TimeBonus = 5;
                //MainCamera.GetComponent<Animation>().Play("Turbo"); //See documentation
                MainCamera.GetComponent<GameLogic>().turbo = true;
                MainCamera.GetComponent<AudioSource>().PlayOneShot(TB_sound, 1);
                Destroy(gameObject);
            }

            if (radial_target) // for 2 game mode ending
            {
                MainCamera.GetComponent<GameLogic>().Checkpoint_time_spawn = true;
                MainCamera.GetComponent<AudioSource>().PlayOneShot(MainCamera.GetComponent<GameLogic>().GetPoints, .3f);
                if (MainCamera.GetComponent<GameLogic>().ch_idx == 15)
                {
                    MainCamera.GetComponent<GameLogic>().ShowEnding = true;
                }
                transform.parent.gameObject.SetActive(false);
            }

        }
    }
}
