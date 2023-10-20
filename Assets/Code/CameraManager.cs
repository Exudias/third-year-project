using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject bulbCamera;
    [SerializeField] private GameObject spiritCamera;

    private PlayerFormSwitcher.PlayerForm currentFormCamera = PlayerFormSwitcher.PlayerForm.Bulb;

    private CinemachineVirtualCamera virtualBulbCam;
    private CinemachineVirtualCamera virtualSpiritCam;

    private Vector2 baseBulbOffset;
    private Vector2 baseSpiritOffset;

    private void Start()
    {
        virtualBulbCam = bulbCamera.GetComponent<CinemachineVirtualCamera>();
        virtualSpiritCam = spiritCamera.GetComponent<CinemachineVirtualCamera>();

        baseBulbOffset = virtualBulbCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
        baseSpiritOffset = virtualSpiritCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
    }

    public void ActivateBulbCamera()
    {
        currentFormCamera = PlayerFormSwitcher.PlayerForm.Bulb;

        virtualBulbCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = baseBulbOffset;

        bulbCamera.SetActive(true);
        spiritCamera.SetActive(false);
    }

    public void ActivateSpiritCamera()
    {
        currentFormCamera = PlayerFormSwitcher.PlayerForm.Spirit;

        virtualSpiritCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = baseSpiritOffset;

        bulbCamera.SetActive(false);
        spiritCamera.SetActive(true);
    }

    public void SetCurrentCameraOffset(Vector2 offset)
    {
        if (currentFormCamera == PlayerFormSwitcher.PlayerForm.Bulb)
        {
            virtualBulbCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
        }
        else if (currentFormCamera == PlayerFormSwitcher.PlayerForm.Spirit)
        {
            virtualSpiritCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
        }
        else
        {
            throw new System.Exception("Current form camera state invalid!");
        }
    }

    public PlayerFormSwitcher.PlayerForm GetCurrentCameraForm() => currentFormCamera;
}
