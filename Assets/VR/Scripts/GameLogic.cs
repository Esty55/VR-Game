using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour {

    public Transform BonusPref; //Bonus prefab
    public float thrust; //eagle moving speed
    public Transform SphereTransform; //Player object in hierarchy
    public float TimeGenerate = 0.0f; //Time for generating checkpoints
    public float Equ_Points; //player points
    public float Feather_left = 8; //count feathers for finding quest
    public int gameMode=0; //2 - chasing, 1-Feather Finding, 0-Exploring
    public GameObject BonusHolder; //bonus gameobject
    public TextMesh Points_Text; //for printing points
    public TextMesh Bonus_Text; //for printing bonus text
    public GameObject Eagle; //using for eagle animations
    public Animation EagleAnim; //current eagle animation
    private float TimeWings = 0; //play animated wings every 5 seconds

    public bool EagleFlight = false; //cap
    public float TimeBonus = 0; //time bonus left
    private float TimeLeft = 129; //time left for finding quest
    public bool GoldBonus=false; //receives from the Points script
    private float TimeFlupps = 0; //switching eagle animations
    public bool PlayFlupps = false; //play eagle fly animation

    public Transform[] Checkpoint_waypoints; //for free ride game mode
    public GameObject Checkpoint_prefab; //checkpoints for exploring
    public GameObject Feather_prefab; //feather prefab
    public float Spawn_time_checkpoint=0;
    public bool Checkpoint_spawn = true;
    public bool Checkpoint_time_spawn = false;
    public int ch_idx=-1; //for arrays
    public Text Global_Messages;
    public TextMesh LimitsAlert; //Out of map message
    public bool GL_Mes_View = false; //cap
    private bool Start_Mes_View = false; //play fadeout anim after last waypoint in 0 game mode
    private bool End_Mes_View = false; //play fadeout anim after last waypoint in 0 game mode
    private float GL_Mes_time = 0; //smooth fade out screen. After delay time for showing next message 
    public Transform[] CheckPoint_Finding_Feathers; //for finding game mode
    public AudioClip Notification; //blink sound
    public AudioClip[] SFX; //Drop sounds
    public AudioClip[] EagleSounds; //eagle sounds
    public GameObject MusicHolder; //Music holder on main camera for playing music as background
    public AudioClip[] BK_Music; //background music list array
    private float EagleSoundTime; //for random time sound
    public Transform[] SpawnZones; //for telepotation after quest ended
    private bool moving = true;
    public bool turbo = false; //turbo receives from the Points script
    public GameObject[] EagleBot; //Eagle bot
    public GameObject TargetBot; // Target follow for Eagle Bot
    public Transform[] Turbo_Feathers; //for racing game
    public Transform[] Speed_Targets; //for racing checkpoints
    public bool ShowEnding=false;
    public bool EndedExploring = false; //if explore mode (gamemode=0) ended then launch gamemode = 1 
    public Image LogoImage;
    public GameObject[] Lines;
    public Transform ParentObject; //PlayerObject
    public GameObject PanelFading; //using for mobile VR smoothly fade in or out screen

    ////////////////////////////////////////////////////////////Life and level control

    public bool impacted = false;
    public GameObject Scenemanager; //see documentation
    public AudioClip GetPoints,GotFeather; //checkpoint and got feather sounds

    ////////////////////////////////////////////////////////////PAUSE AND MAIN MENU

    private bool paused=false;
    public RectTransform PausePanel;
    public Image Cursor;
    private bool loadingState = false; //if user choosed option restart or go to main menu and eagle did not respond to a collision

    //////////////////////////////////////////////////////////Control type
    private bool clicked = false; //if mouse button down
    private float SpInc = 25; //increase speed by every intensity clicks, default value gets from thrust value
    private bool cooldown = false;
    private bool RandomizeFlups = true; //if user not click then activate time of autoflaps
    public bool gotItem = false; //if player has got item then eagle not react on user clicks for speed increases while player doesn't use item


    void Start () {

        EagleAnim = Eagle.GetComponent<Animation>();
        SpInc = thrust;
        Scenemanager = GameObject.Find("SCENEMANAGER");
        if (Scenemanager == null) return;
        gameMode = Scenemanager.GetComponent<SceneGM>().GameMode;

    }

    void Update()
    {

            if (gameMode == 0)
            {

            Global_Messages.text = "Explore the world with checkpoints";

            if (!Start_Mes_View)
            {
#if UNITY_ANDROID
                PanelFading.SetActive(true); //playing fading out animation of panel    //See documentation
                PanelFading.GetComponent<Animator>().Play("PanelFalling", -1, 0f);      //See documentation
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                //GetComponent<Animation>().Play(); //See documentation
#endif
                MusicHolder.GetComponent<AudioSource>().Play(); //see documentation
                MusicHolder.GetComponent<Animation>().Play("FadeInMusic"); //see documentation

                Global_Messages.gameObject.SetActive(false);
                if (Scenemanager != null)
                    Scenemanager.GetComponent<SceneGM>().GameMode = gameMode;
                Lines[0].GetComponent<TrailRenderer>().Clear();
                Lines[1].GetComponent<TrailRenderer>().Clear();
                Lines[0].SetActive(false);
                Lines[1].SetActive(false);
                StartCoroutine(SpawnOnPosition(2.0F, 0));
                Start_Mes_View = true;
            }

            if (GL_Mes_View)
                GL_Mes_time += Time.deltaTime;

            if (GL_Mes_time >= 3.5f && GL_Mes_View)
            {
                GL_Mes_time = 0;
                GL_Mes_View = false;
            }

            if (!GL_Mes_View && !End_Mes_View)
            {
                GetComponent<AudioSource>().PlayOneShot(Notification, 1.5f);
                Global_Messages.gameObject.SetActive(true);
                Global_Messages.GetComponent<Animator>().Play("FadeIn_Text", -1, 0f);
                Spawn_time_checkpoint = 0;
                Checkpoint_spawn = true;
                End_Mes_View = true;
            }

            //**********************************************************************

            if (Spawn_time_checkpoint < 5) //spawn explore checkpoints
                {
                Spawn_time_checkpoint += Time.deltaTime;
                } else

                if (Spawn_time_checkpoint >= 5 && Checkpoint_spawn && ch_idx < Checkpoint_waypoints.Length-1)
                {
#if UNITY_ANDROID
                PanelFading.SetActive(false); //See documentation
#endif
                Global_Messages.gameObject.SetActive(false);
                    ch_idx += 1;
                    Instantiate(Checkpoint_prefab, Checkpoint_waypoints[ch_idx].transform.position, Checkpoint_waypoints[ch_idx].transform.rotation);
                    Checkpoint_spawn = false; //stop timer for first checkpoint spawner
                }

                Points_Text.text = "Points: " + Equ_Points.ToString();

            if (EndedExploring)
            {
                Start_Mes_View = false;
                GL_Mes_View = true;
                End_Mes_View = false;
                Spawn_time_checkpoint = 0;
                gameMode = 1;
            }

            }

            if (gameMode == 1)
            {
                Global_Messages.text = "Let's find 8 blue feathers";

            if (!Start_Mes_View)
                {
#if UNITY_ANDROID
                PanelFading.SetActive(true);                                       //See documentation
                PanelFading.GetComponent<Animator>().Play("PanelFalling", -1, 0f); //See documentation
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                //GetComponent<Animation>().Play(); //See documentation
#endif

                MusicHolder.GetComponent<Animation>().Play("FadeOutMusic"); //see documentation
                Global_Messages.gameObject.SetActive(false);
                if (Scenemanager != null)
                    Scenemanager.GetComponent<SceneGM>().GameMode = gameMode;
                Lines[0].GetComponent<TrailRenderer>().Clear();
                    Lines[1].GetComponent<TrailRenderer>().Clear();
                    Lines[0].SetActive(false);
                    Lines[1].SetActive(false);
                    StartCoroutine(SpawnOnPosition(2.0F,1));
                    Start_Mes_View = true;
                }

                if (GL_Mes_View)
                GL_Mes_time += Time.deltaTime;

                if (GL_Mes_time>=3.5f && GL_Mes_View)
                {
                    GL_Mes_time = 0;
                    GL_Mes_View = false;
                }
                
                if (!GL_Mes_View && !End_Mes_View)
                {
                MusicHolder.GetComponent<AudioSource>().Stop();
                MusicHolder.GetComponent<AudioSource>().clip = BK_Music[1];
                MusicHolder.GetComponent<AudioSource>().Play();
                MusicHolder.GetComponent<Animation>().Play("FadeInMusic"); //see documentation

                GetComponent<AudioSource>().PlayOneShot(Notification, 1.5f);
                Global_Messages.gameObject.SetActive(true);
                    Global_Messages.GetComponent<Animator>().Play("FadeIn_Text", -1, 0f);
                    Spawn_time_checkpoint = 0;
                    Checkpoint_spawn = true;
                    End_Mes_View = true;
                }

                if (Spawn_time_checkpoint < 6)
                {
                    Spawn_time_checkpoint += Time.deltaTime;
                }
                else

               if (Spawn_time_checkpoint >= 6 && Checkpoint_spawn)
                {
                Global_Messages.gameObject.SetActive(false);
                    for (ch_idx = 0;ch_idx < CheckPoint_Finding_Feathers.Length; ch_idx++)
                    {
                        CheckPoint_Finding_Feathers[ch_idx].gameObject.SetActive(true);
                    }
                    BonusHolder.SetActive(true);
#if UNITY_ANDROID
                PanelFading.SetActive(false);     //See documentation
#endif
                Checkpoint_spawn = false; //stop timer for first checkpoint spawner
                }

                
                TimeLeft -= Time.deltaTime;

                float minutes = TimeLeft / 60;
                float seconds = TimeLeft % 60;
                float fraction = (TimeLeft * 100) % 100;

                Bonus_Text.text = "Time left: " + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
                Points_Text.text = "Feathers left: " + Feather_left.ToString();

                if (Feather_left <= 0)
                {
                    Start_Mes_View = false;
                    GL_Mes_View = true;
                    End_Mes_View = false;
                    Spawn_time_checkpoint = 0;
                    BonusHolder.SetActive(false);
                    gameMode = 2;
                }
            }

        if (gameMode == 2)
        {

            Global_Messages.text = "Get the first place";

            if (!Start_Mes_View)
            {
#if UNITY_ANDROID
                PanelFading.SetActive(true);                                      //See documentation
                PanelFading.GetComponent<Animator>().Play("PanelFalling", -1, 0f);//See documentation
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                //GetComponent<Animation>().Play(); //See documentation
#endif
                MusicHolder.GetComponent<Animation>().Play("FadeOutMusic"); //see documentation
                if (Scenemanager != null)
                    Scenemanager.GetComponent<SceneGM>().GameMode = gameMode;
                StartCoroutine(SpawnOnPosition(2.0F, 2));
                Lines[0].GetComponent<TrailRenderer>().Clear();
                Lines[1].GetComponent<TrailRenderer>().Clear();
                Lines[0].SetActive(false);
                Lines[1].SetActive(false);
                Speed_Targets[0].gameObject.SetActive(true);
                Start_Mes_View = true;
            }

            if (GL_Mes_View)
                GL_Mes_time += Time.deltaTime;

            if (GL_Mes_time >= 3.5f && GL_Mes_View)
            {
                GL_Mes_time = 0;
                GL_Mes_View = false;
            }

            if (!GL_Mes_View && !End_Mes_View)
            {
                MusicHolder.GetComponent<AudioSource>().Stop();
                MusicHolder.GetComponent<AudioSource>().clip = BK_Music[2];
                MusicHolder.GetComponent<AudioSource>().Play();
                MusicHolder.GetComponent<Animation>().Play("FadeInMusic"); //see documentation

                Global_Messages.gameObject.SetActive(true);
                GetComponent<AudioSource>().PlayOneShot(Notification, 1.5f);
                Global_Messages.GetComponent<Animator>().Play("FadeIn_Text", -1, 0f);
                Spawn_time_checkpoint = 0;
                Checkpoint_spawn = true;
                End_Mes_View = true;
            }

            if (Spawn_time_checkpoint >= 3 && Checkpoint_spawn)
            {
                EagleBot[0].SetActive(true);
                EagleBot[1].SetActive(true);
                TargetBot.SetActive(true);
            }

            if (Spawn_time_checkpoint < 5)
            {
                Spawn_time_checkpoint += Time.deltaTime;
            }
            else       
                if (Spawn_time_checkpoint >= 5 && Checkpoint_spawn)
            {
                Global_Messages.gameObject.SetActive(false);
                for (ch_idx = 0; ch_idx < Turbo_Feathers.Length; ch_idx++)
                    Turbo_Feathers[ch_idx].gameObject.SetActive(true);
                ch_idx = 0;
#if UNITY_ANDROID
                PanelFading.SetActive(false); //See documentation
#endif
                Checkpoint_spawn = false; //stop timer for first checkpoint spawner
            }
        

                if (Spawn_time_checkpoint >= 2 && Checkpoint_time_spawn && ch_idx < Speed_Targets.Length-1)
                {
                    ch_idx += 1;
                    Speed_Targets[ch_idx].gameObject.SetActive(true);
                    Checkpoint_time_spawn = false; //stop timer for first checkpoint spawner
                    
                }

                Points_Text.text = "Points: " + Equ_Points.ToString();

            }


        if (Input.GetKeyDown(KeyCode.Escape)) //Pause Menu
        {

            paused = !paused;
            if (paused) { Cursor.gameObject.SetActive(true); Time.timeScale = 0; }
            if (!paused) { Cursor.gameObject.SetActive(false); Time.timeScale = 1; }
            PausePanel.gameObject.SetActive(paused);

        }

        EagleSoundTime += Time.deltaTime;
        if (EagleSoundTime >= 12)            //randomize eagle sounds
        {
            GetComponent<AudioSource>().PlayOneShot(EagleSounds[Random.Range(0, EagleSounds.Length)], .5f);
            EagleSoundTime = 0;
        }

        if (GoldBonus)
            {
                BonusHolder.SetActive(true);
                TimeBonus -= Time.deltaTime;

                if (TimeBonus <= 0)
                {
                    TimeBonus = 0;
                    BonusHolder.SetActive(false);
                    GoldBonus = false;
                }

                float minutes = TimeBonus / 60;
                float seconds = TimeBonus % 60;
                float fraction = (TimeBonus * 100) % 100;

                Bonus_Text.text = "Bonus: "+string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
            }

            if (EagleFlight)
            {
                TimeWings += Time.deltaTime;
                if (TimeWings >= 5)
                {
                    TimeWings = 0;
                    EagleFlight = false;
                }
            }

            if (ShowEnding)
            {
                if (!GetComponent<Animation>().isPlaying)
                {
                    Lines[0].SetActive(false);
                    Lines[1].SetActive(false);
                    Points_Text.gameObject.SetActive(false);
                    Bonus_Text.gameObject.SetActive(false);
                    BonusHolder.gameObject.SetActive(false);
                    //GetComponent<Animation>().Play("Ending"); //See documentation
                    MainMenu(); //back to main menu
                    MusicHolder.GetComponent<Animation>().Play("FadeOutMusic"); //see documentation
                    LogoImage.gameObject.SetActive(true);
                    ShowEnding = false;
                }
            }

            if (impacted)
        {
            impacted = false;
        }

        if (clicked)
        {
            if (!cooldown)
            {
                thrust = SpInc * 1.5f; //convert to eagle speed
                TimeFlupps = 10;
            }
            if (cooldown)
            {
                thrust -= (SpInc / 2) * Time.deltaTime;
                if (thrust <= 25)
                {
                    thrust = 25;
                    SpInc = thrust;
                    RandomizeFlups = true;
                    TimeFlupps = 0;
                    clicked = false;
                }
            }

        }

        if (!gotItem) //if player doesn't hold item eagle will accelerate by pressing mouse button or tap new in v.1.3 - 2.0
        {
            if (Input.GetMouseButtonDown(0))
            {
                clicked = true;
                SpInc += 1; //increase clicks intensity
                cooldown = false;
                RandomizeFlups = false;
            }
            if (Input.GetMouseButtonUp(0))
            {
                cooldown = true;
            }
        }
    }



    void FixedUpdate()
    {
            if (turbo)
            {
                BonusHolder.SetActive(true);
                TimeBonus -= Time.deltaTime;
                thrust = SpInc + (TimeBonus*3);

                if (TimeBonus <= 0)
                {
                    TimeBonus = 0;
                    BonusHolder.SetActive(false);
                    turbo = false;
                }

                float minutes = TimeBonus / 60;
                float seconds = TimeBonus % 60;
                float fraction = (TimeBonus * 100) % 100;

                Bonus_Text.text = "Turbo: " + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);

            }

                if (moving)
            {
                    SphereTransform.transform.position += Camera.main.transform.forward * thrust * Time.deltaTime;
        }

                
                if (RandomizeFlups)
        TimeFlupps += Time.deltaTime;

            if (TimeFlupps>=10)
            {
                EagleAnim["Armature|Fly_main"].speed = 2;
                EagleAnim.CrossFade("Armature|Fly_main");
                EagleFlight = true;
            if (!Eagle.GetComponent<AudioSource>().isPlaying)
                Eagle.GetComponent<AudioSource>().Play();
                TimeFlupps = 0;
                }
            else
                    if (!EagleFlight)
            {
                EagleAnim["Armature|Free_fly"].speed = 2;
                EagleAnim.CrossFade("Armature|Free_fly");
                Eagle.GetComponent<AudioSource>().Stop();
            }
    }

   public IEnumerator SpawnOnPosition(float waitTime, int idxNum)
    {
        yield return new WaitForSeconds(waitTime);
        ParentObject.position = SpawnZones[idxNum].position;
        GetComponent<CameraRotation>().yaw = 0;
        GetComponent<CameraRotation>().pitch = 0;
        moving = false;
        UnityEngine.XR.InputTracking.Recenter();
        yield return new WaitForSeconds(.1f);
        moving = true;
        Lines[0].SetActive(true);
        Lines[1].SetActive(true);

    }

    public void Restart() //Restart the level
    {
#if UNITY_ANDROID
        PanelFading.gameObject.SetActive(true); //play fade out animation of panel  //See documentation
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //GetComponent<Animation>().Play(); //See documentation
#endif
        int currLV = SceneManager.GetActiveScene().buildIndex; //get current level number
        StartCoroutine(Option(1, currLV));
    }

    public void MainMenu() //Back to main menu
    {
#if UNITY_ANDROID
        PanelFading.gameObject.SetActive(true); //play fade out animation of panel  //See documentation
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //GetComponent<Animation>().Play(); //See documentation
#endif
        MusicHolder.GetComponent<Animation>().Play("FadeOutMusic"); //see documentation
        StartCoroutine(Option(1, 0));
    }

    public IEnumerator Option(float waitTime, int LV_idx) //restart or quit to main menu option where waitTime - waiting while panel smoothly fade and LV_idx - level index
    {
        Time.timeScale = 1;
        loadingState = true;
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(LV_idx);
    }

    public IEnumerator ResetByFall(float waitTime, int idxNum)
    {
        Lines[0].GetComponent<TrailRenderer>().Clear();
        Lines[1].GetComponent<TrailRenderer>().Clear();
        Lines[0].SetActive(false);
        Lines[1].SetActive(false);

#if UNITY_ANDROID
        PanelFading.gameObject.SetActive(true); //play fade out animation of panel  //See documentation
        PanelFading.GetComponent<Animator>().Play("PanelFalling", -1, 0f);//See documentation
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //GetComponent<Animation>().Play(); //See documentation
#endif

        ParentObject.position = SpawnZones[idxNum].position;
        GetComponent<CameraRotation>().yaw = 0;
        GetComponent<CameraRotation>().pitch = 0;
        moving = false;
        UnityEngine.XR.InputTracking.Recenter();
        yield return new WaitForSeconds(.3f);
        if (!loadingState)
        {
            int currLV = SceneManager.GetActiveScene().buildIndex; //get current level number
            SceneManager.LoadScene(currLV);
        }
    }
}
