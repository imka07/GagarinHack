using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Collections;

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
    int DocumentIndex;

    private void Start()
    {
        defaultBack = backGround.texture;
        camAvailable = false; // Камера изначально выключена
    }

    public void ActivateCamera()
    {
        if (!camAvailable) // Если камера еще не активирована
        {
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

    public void TakePhoto()
    {
        StartCoroutine(TakeAPhoto());
    }

    IEnumerator TakeAPhoto()
    {
        cameraPanel.SetActive(false);
        // Wait until rendering is complete, before taking the photo.
        yield return new WaitForEndOfFrame();

        // Create a new texture to store the photo
        Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Read the current screen contents into the texture
        photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        photo.Apply();

        // Encode the photo as PNG
        byte[] bytes = photo.EncodeToPNG();
        Destroy(photo);

        // Define the file path using the persistent data path
        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        // Write the PNG data to the file
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


        // If saving to gallery fails, log an error
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
       



    }

    public void ChangeDocIndex(int index)
    {
        DocumentIndex = index;
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



