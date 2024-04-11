using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    public GameObject[] panels; // массив всех панелей
    public static MainUIController instance;

    public void DisableAllPanelsExcept(GameObject activePanel)
    {
        foreach (GameObject panel in panels)
        {
            if (panel != activePanel)
            {
                panel.SetActive(false);
            }
        }
    }

    private void Start()
    {
        instance = this;
    }

    public void MobileKeyBoard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
    }


}
