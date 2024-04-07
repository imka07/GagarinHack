using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public GameObject panelToActivate; // ссылка на панель, которую нужно активировать
    public MainUIController panelManager; // ссылка на менеджер панелей

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        panelManager.DisableAllPanelsExcept(panelToActivate);
        panelToActivate.SetActive(true);
    }
}
