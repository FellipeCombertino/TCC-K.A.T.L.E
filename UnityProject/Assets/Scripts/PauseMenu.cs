using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public bool isMenu;
    public bool paused;
    public Animator anim;
    PauseMenu Instance;
    public Slider sensibility;
    public Slider music;
    public Slider sfx;
    public Dropdown resolutionDrop;
    public Dropdown qualityDrop;
    public Toggle fullScreenToggle;
    public bool fullscreen;
    public List<AudioSource> Musics;
    public List<AudioSource> Sfxs;
    public Resolution[] Resolutions;
    public List<string> ResolutionString;
    public List<string> QualityList;
    public GameObject Controll, Graphic, Volume,Screens,FirstButtoms,Background;
    
    
    
    public void Start()
    {

        if (Sfxs.Count == 0)
        {
            for (int i = 0; i < FindObjectsOfType<AudioSource>().Length; i++)
            {

                if (FindObjectsOfType<AudioSource>()[i].CompareTag("music"))
                {
                    Musics.Add(FindObjectsOfType<AudioSource>()[i]);
                   

                }
                else
                {

                    Sfxs.Add(FindObjectsOfType<AudioSource>()[i]);

                }

            }
        }
        fullscreen = fullScreenToggle.isOn = (PlayerPrefs.GetInt("full", 1) == 0) ? false : true;

        resolutionDrop.ClearOptions();
        ResolutionString = new List<string>();
        Resolutions = Screen.resolutions;

        qualityDrop.ClearOptions();
        QualityList = QualitySettings.names.ToList();

        qualityDrop.AddOptions(QualityList);

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality", 5));

        qualityDrop.value = PlayerPrefs.GetInt("quality", 5);




        foreach (var item in Resolutions)
        {

            ResolutionString.Add(item.width + "x" + item.height);


        }

        resolutionDrop.AddOptions(ResolutionString);

        if(PlayerPrefs.GetInt("res",-1) == -1)
        {

            for (int i = 0; i < ResolutionString.Count; i++)
            {

                if (ResolutionString[i].Split(new char[] { 'x' })[0] == Screen.width.ToString() && ResolutionString[i].Split(new char[] { 'x' })[1] == Screen.height.ToString())
                {




                    resolutionDrop.value = i;




                }


            }



        }
        else
        {

            Screen.SetResolution(Resolutions[PlayerPrefs.GetInt("res", -1)].width, Resolutions[PlayerPrefs.GetInt("res", -1)].width,fullscreen);
            Screen.fullScreen = fullscreen;


        }



        Instance = this;
        sensibility.value = PlayerPrefs.GetFloat("Sensi", 50);
        try
        {
            CameraController.Instance.sensitivityX = sensibility.value / 4;
            CameraController.Instance.sensitivityY = -sensibility.value / 4;
        }
        catch
        {


        }
        music.value = PlayerPrefs.GetFloat("Music", 50);
        sfx.value = PlayerPrefs.GetFloat("Sfx", 50);
 

        foreach (AudioSource mus in Musics)
        {
            mus.volume = music.value / 700;
            if(!mus.isPlaying)
            mus.Play();
            mus.loop = true;
        }

        foreach (AudioSource Sfx in Sfxs)
        {
            if (Sfx)
            {
                Sfx.volume = sfx.value / 700;

            }
            else
            {
                Sfxs.Remove(Sfx);

            }
            

        }

        
    }
    private void Update()
    {
        if (!isMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Start();
                Pause();

            }
        }


    }
    public void BackToMenu()
    {

        SceneManager.LoadScene("MenuNew");

    }
    public void setQuality()
    {



        QualitySettings.SetQualityLevel(qualityDrop.value);
        PlayerPrefs.SetInt("quality", qualityDrop.value);

    }
    public void ChangeFullScreen()
    {

        fullscreen = fullScreenToggle.isOn;
        PlayerPrefs.SetInt("full", 1);
        Screen.fullScreen = fullscreen;

    }
    public void ChangeRes()
    {
        PlayerPrefs.SetInt("res", resolutionDrop.value);
        print(Resolutions[resolutionDrop.value].width + "   " + Resolutions[resolutionDrop.value].height);
        Screen.SetResolution(Resolutions[resolutionDrop.value].width, Resolutions[resolutionDrop.value].height, fullscreen);
        
        


    }
    public void ChangeSense()
    {


        PlayerPrefs.SetFloat("Sensi", sensibility.value);
        try
        {
            CameraController.Instance.sensitivityX = sensibility.value / 4;
            CameraController.Instance.sensitivityY = -sensibility.value / 4;
        }
        catch
        {


        }
     
    }
    public void ChangeMusic()
    {

       
        PlayerPrefs.SetFloat("Music", music.value);
        foreach (var mus in Musics)
        {
            mus.volume = music.value / 700;

        }

    }

    public void ChangeSfx()
    {

        PlayerPrefs.SetFloat("Sfx", sfx.value);

        foreach (var _Sfx in Sfxs)
        {
            if (_Sfx)
            {
                _Sfx.volume = sfx.value / 700;
            }
        }
        
    }

    public void Pause()
    {
        if(Time.timeScale == 0)
        {

            Time.timeScale = 1;
            anim.Play("Hide");
            CameraController.Instance.lockCamera = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Screens.SetActive(false);
            FirstButtoms.SetActive(true);
           
        }
        else
        {
            Time.timeScale = 0;
            anim.Play("Show");
            CameraController.Instance.lockCamera = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        

    }
    public void changeToControll()
    {

        Controll.transform.SetSiblingIndex(2);

    }

    public void changeToVolume()
    {

        Volume.transform.SetSiblingIndex(2);

    }
    public void changeToGraphics()
    {
        Graphic.transform.SetSiblingIndex(2);


    }

}
