using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //當按下Play鍵
    public void ButtonPlayOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);//載入下一個場景編號
    }
    //當按下Quit鍵
    public void ButtonQuitOnClick()
    {
        Application.Quit();//結束程式
    }
}
