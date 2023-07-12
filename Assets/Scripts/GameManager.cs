using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject opendWindow;
    [SerializeField] private GameObject escWindow;
    [SerializeField] private Slider BGM_SLIDER;
    [SerializeField] private Slider EFFECT_SLIDER;
    [SerializeField] private bool timeAble;
    [SerializeField] private GameObject clearWindow;
    [SerializeField] private AudioClip clearBGM;
    private bool clear;
    private float timeP;
    public static bool gameStop = false;

    private AudioSource adsBGM;
    private AudioSource adsEFFECT;

    private void Awake()
    {
        Time.timeScale = 1;
        gameStop = false;
        timeP = 0;
        adsBGM = GetComponent<AudioSource>();
        adsEFFECT = transform.GetChild(0).GetComponent<AudioSource>();
        if(PlayerPrefs.HasKey("BGM_V") || PlayerPrefs.HasKey("EFFECT_V"))
        {
            adsBGM.volume = PlayerPrefs.GetFloat("BGM_V");
            adsEFFECT.volume = PlayerPrefs.GetFloat("EFFECT_V");
            if (BGM_SLIDER != null && EFFECT_SLIDER != null)
            {
                BGM_SLIDER.value = PlayerPrefs.GetFloat("BGM_V");
                EFFECT_SLIDER.value = PlayerPrefs.GetFloat("EFFECT_V");
            }
        }
        else
        {
            PlayerPrefs.SetFloat("BGM_V", 1);
            PlayerPrefs.SetFloat("EFFECT_V", 1);
            BGM_SLIDER.value = PlayerPrefs.GetFloat("BGM_V");
            EFFECT_SLIDER.value = PlayerPrefs.GetFloat("EFFECT_V");
            adsBGM.volume = PlayerPrefs.GetFloat("BGM_V");
            adsEFFECT.volume = PlayerPrefs.GetFloat("EFFECT_V");
        }
    }

    public void SaveVariables()
    {
        PlayerPrefs.SetFloat("BGM_V", BGM_SLIDER.value);
        PlayerPrefs.SetFloat("EFFECT_V", EFFECT_SLIDER.value);
        adsBGM.volume = PlayerPrefs.GetFloat("BGM_V");
        adsEFFECT.volume = PlayerPrefs.GetFloat("EFFECT_V");
    }

    public void GameClear()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        adsBGM.clip = clearBGM;
        adsBGM.Stop();
        adsBGM.Play();
        gameStop = true;
        clear = true;
        clearWindow.SetActive(true);
        clearWindow.transform.GetChild(2).GetComponent<Text>().text = ("클리어 시간: " + timeP);
    }

    private void Update()
    {
        if(timeAble && !clear)
        {
            timeP += Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(escWindow != null && !clear)
            {
                if(!escWindow.activeSelf)
                {
                    escWindow.SetActive(true);
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    gameStop = true;
                }
                else
                {
                    escWindow.SetActive(false);
                    Time.timeScale = 1;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    gameStop = false;
                }
            }
            else if (opendWindow != null)
            {
                opendWindow.SetActive(false);
                opendWindow = null;
            }
        }
    }

    public void SceneMove(int moveSceneNum)
    {
        SceneManager.LoadScene(moveSceneNum);
        Time.timeScale = 1;
    }

    public void OpenWindow(GameObject openWindow)
    {
        if (BGM_SLIDER != null && EFFECT_SLIDER != null)
        {
            BGM_SLIDER.value = PlayerPrefs.GetFloat("BGM_V");
            EFFECT_SLIDER.value = PlayerPrefs.GetFloat("EFFECT_V");
        }
        opendWindow = openWindow;
        openWindow.SetActive(true);
        transform.GetChild(0).GetComponent<AudioSource>().Play();
    }

    public void ckickSound()
    {
        transform.GetChild(0).GetComponent<AudioSource>().Play();
    }

    public void StopGame()
    {
        Application.Quit();
    }

}
