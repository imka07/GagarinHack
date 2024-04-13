using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBack;


    public RawImage backGround;
    public AspectRatioFitter fit;
    public GameObject cameraPanel, menuPanel;
    public GameObject DocumentConteinerPrefab;
    public Transform[] DocumentSpawnParent;
    public int DocumentIndex;

    private void Start()
    {
        defaultBack = backGround.texture;
        camAvailable = false; 
    }

    public void ActivateCamera()
    {
        if (!camAvailable) 
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length == 0)
            {
                Debug.Log("No camera detected");
                return;
            }

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

       
            backCam.Play();
            backGround.texture = backCam;
            camAvailable = true; 
        }
    }

    public void DeactivateCamera()
    {
        cameraPanel.SetActive(false);
        if (camAvailable) 
        {
            backCam.Stop();
            backGround.texture = defaultBack;
            camAvailable = false;
        }
    }

    public void TakePhoto()
    {
        StartCoroutine(TakeAPhoto());
    }

    public void ShareImage()
    {
        if (backGround.texture != null)
        {
            // Преобразуем текстуру в byte array
            byte[] imageBytes = ((Texture2D)backGround.texture).EncodeToPNG();

            // Сохраняем изображение во временный файл
            string filePath = Application.persistentDataPath + "/tempImage.png";
            System.IO.File.WriteAllBytes(filePath, imageBytes);

            // Вызываем нативный метод для открытия диалога "поделиться" на iOS
            ShareImageiOS(filePath);
        }
        else
        {
            Debug.LogError("No image to share");
        }
    }

    // Объявляем нативный метод для iOS
    [DllImport("__Internal")]
    private static extern void ShareImageiOS(string imagePath);


    IEnumerator TakeAPhoto()
    {
        cameraPanel.SetActive(false);
        yield return new WaitForEndOfFrame();
        Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        photo.Apply();

        byte[] bytes = photo.EncodeToPNG();
        Destroy(photo);

        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllBytes(filePath, bytes);
        cameraPanel.SetActive(true);
        // Save to gallery
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, "MyGallery", fileName, (success, path) =>
        {
            if (success)
            {
                Debug.Log("Photo saved to gallery: " + path);
            }
            else
            {
                Debug.Log("Failed to save photo to gallery");
            }
        });

        if (permission != NativeGallery.Permission.Granted)
        {
            Debug.LogError("Permission not granted for saving photo to gallery");
        }


                GameObject container = Instantiate(DocumentConteinerPrefab, DocumentSpawnParent[DocumentIndex].transform);
                 

                // Получаем компонент RawImage из объекта DocumentConteinerPrefab
                RawImage rawImageComponent = container.GetComponent<RawImage>();

                // Проверяем, найден ли компонент RawImage
                if (rawImageComponent != null)
                {
                    // Устанавливаем текстуру снимка с камеры в компонент RawImage
                    rawImageComponent.texture = backCam;
                }
                else
                {
                    Debug.LogError("RawImage component not found in DocumentConteinerPrefab.");
                }

        Text setPhotoTime = container.GetComponentInChildren<Text>();

        DateTime currentTime = DateTime.Now;

        // Создаем текст для отображения времени сделанной фотографии
        string photoTimeText = currentTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Отображаем текст в UIText
        setPhotoTime.text = photoTimeText;



    }



    public void ChangeDocIndex(int index)
    {
        DocumentIndex = index; 
    }

    public void OpenGallery()
    {
        // Вызываем метод открытия галереи из NativeGallery
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null) // Если выбрано изображение
            {
                // Здесь вы можете использовать путь к изображению (path) для дальнейшей обработки
                Debug.Log("Image selected: " + path);
            }
            else // Если выбор изображения отменен
            {
                Debug.Log("Selection cancelled");
            }
        });

        // Проверяем разрешение доступа к галерее
        if (permission != NativeGallery.Permission.Granted)
        {
            Debug.LogError("Permission not granted for accessing gallery");
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



