using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject[] panelToActivate; // ссылка на панель, которую нужно активировать
    public MainUIController panelManager; // ссылка на менеджер панелей

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        for (int i = 0; i < panelToActivate.Length; i++)
        {
            panelManager.DisableAllPanelsExcept(panelToActivate[i]);
            panelToActivate[i].SetActive(true);
        }
    }
}
