using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBack;


    public RawImage backGround;
    public AspectRatioFitter fit;
    public GameObject cameraPanel;

    private void Start()
    {
        defaultBack = backGround.texture;
        camAvailable = false; // Камера изначально выключена
    }

    public void ActivateCamera()
    {
        if (!camAvailable) // Если камера еще не активирована
        {
            cameraPanel.SetActive(true);
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length == 0)
            {
                Debug.Log("No camera detected");
                return;
            }

            // Поиск задней камеры
            for (int i = 0; i < devices.Length; i++)
            {
                if (!devices[i].isFrontFacing)
                {
                    backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                    break;
                }
            }

            if (backCam == null)
            {
                Debug.Log("Unable to find the back camera");
                return;
            }

            // Активация камеры и отображение на RawImage
            backCam.Play();
            backGround.texture = backCam;
            camAvailable = true; // Помечаем камеру как активированную
        }
    }

    public void DeactivateCamera()
    {
        cameraPanel.SetActive(false);
        if (camAvailable) // Если камера активирована, деактивируем её
        {
            
            backCam.Stop();
            backGround.texture = defaultBack;
            camAvailable = false;
        }
    }

    private void Update()
    {
        if (!camAvailable)
            return;

        // Пропорциональное изменение размера отображаемого изображения
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        // Учёт вертикального отражения изображения
        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        backGround.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        // Учёт поворота изображения в зависимости от ориентации устройства
        int orient = -backCam.videoRotationAngle;
        backGround.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
}
