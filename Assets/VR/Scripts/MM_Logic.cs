using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MM_Logic : MonoBehaviour {

    public AudioClip clickSound, lightWind; // clickSound - for clicking on level button, lightWind - play sound when playing move camera animation
    public AudioClip[] ScenemanagerSounds;                    //SceneManagerSound - plays the sound when level loaded or reloaded for example wind ambient sound | Changed to array new in v.1.3 - 2.0
    private bool clicked = false;                               //if player select the level then can't click on it again
    public GameObject CamHolder, FadePanel, Scenemanager;       //CamHolder - plays moving animation when player choosed the level,  FadePanel - smoothly fade out panel at start
                                                                //SceneManager control values about gamemode or can contain any values of game features like user points and info about levels


    // Use this for initialization
    void Start () {

        Scenemanager = GameObject.Find("SCENEMANAGER"); //Find SceneManager at start of the game and never delete this gameobject throughout a gaming session
        Scenemanager.GetComponent<AudioSource>().Stop(); //stop wind background sound
    }

    public void GameModeSection(int GM) //where GM - Gamemode number new in v1.3 - 2.0
    {
        int levelnum = 0;

        if (GM == 0 || GM == 1 || GM == 2)
            levelnum = 1;

        LevelSection(levelnum, GM); //first build level index and game mode number

    }
	

    void LevelSection(int levelnum,int gamemode) //levelNum set from GameModeSection new in v.1.3 - 2.0
    {
        if (!clicked)
        {
            StartCoroutine(LevelLoad(5, levelnum));
            GetComponent<AudioSource>().PlayOneShot(clickSound, .5f);
            GetComponent<AudioSource>().PlayOneShot(lightWind, 3);

            if (levelnum == 1)
            {
                Scenemanager.GetComponent<SceneGM>().GameMode = gamemode; //reseting GameMode to default
                CamHolder.GetComponent<Animation>().Play("MM_Way01"); //Changed to legacy animation new in v.1.3 - 2.0
            }

            FadePanel.GetComponent<Animator>().Play("MM_FadePanelIn", -1, 0);
            //Scenemanager.GetComponent<SceneGM>().GameMode = 0; //0 for loading default game mode
            clicked = true;
        }
    }

    public IEnumerator LevelLoad(float WaitTime, int LV_idx)
    {
        yield return new WaitForSeconds(WaitTime);
        Scenemanager.GetComponent<AudioSource>().clip = ScenemanagerSounds[LV_idx-1];
        Scenemanager.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(LV_idx);
    }
}
