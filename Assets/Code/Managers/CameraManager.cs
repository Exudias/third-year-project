using UnityEngine;
using Cinemachine;
using System.Collections;

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
        if (bulbCamera != null)
        {
            virtualBulbCam = bulbCamera.GetComponent<CinemachineVirtualCamera>();
            baseBulbOffset = virtualBulbCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
        }
        
        if (spiritCamera != null)
        {
            virtualSpiritCam = spiritCamera.GetComponent<CinemachineVirtualCamera>();
            baseSpiritOffset = virtualSpiritCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
        }

        StartCoroutine(ResetVcamConfines());
    }

    IEnumerator ResetVcamConfines()
    {
        yield return null; // wait a frame

        if (bulbCamera != null)
        {
            bulbCamera.GetComponent<CinemachineConfiner2D>().InvalidateCache();
        }

        if (spiritCamera != null)
        {
            spiritCamera.GetComponent<CinemachineConfiner2D>().InvalidateCache();
        }
    }

    public void ActivateBulbCamera()
    {
        currentFormCamera = PlayerFormSwitcher.PlayerForm.Bulb;

        bulbCamera.SetActive(true);
        spiritCamera.SetActive(false);
    }

    public void ActivateSpiritCamera()
    {
        currentFormCamera = PlayerFormSwitcher.PlayerForm.Spirit;

        bulbCamera.SetActive(false);
        spiritCamera.SetActive(true);
    }

    public void SetCurrentCameraOffset(Vector2 offset, bool bulb, bool spirit)
    {
        CinemachineVirtualCamera bulbVcam = bulbCamera.GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera spiritVcam = spiritCamera.GetComponent<CinemachineVirtualCamera>();
        if (bulb)
        {
            bulbVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
        }
        if (spirit)
        {
            spiritVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
        }
    }

    public void SetForcedCameraPosition(Vector2 pos, bool forceX, bool forceY)
    {
        if (bulbCamera != null)
        {
            bulbCamera.GetComponent<LockCameraXY>().m_LockX = forceX;
            bulbCamera.GetComponent<LockCameraXY>().m_LockY = forceY;
            bulbCamera.GetComponent<LockCameraXY>().m_XPosition = pos.x;
            bulbCamera.GetComponent<LockCameraXY>().m_YPosition = pos.y;
        }
        if (spiritCamera != null)
        {
            spiritCamera.GetComponent<LockCameraXY>().m_LockX = forceX;
            spiritCamera.GetComponent<LockCameraXY>().m_LockY = forceY;
            spiritCamera.GetComponent<LockCameraXY>().m_XPosition = pos.x;
            spiritCamera.GetComponent<LockCameraXY>().m_YPosition = pos.y;
        }
    }

    public void DisableForcedCameraPosition()
    {
        if (bulbCamera != null)
        {
            bulbCamera.GetComponent<LockCameraXY>().m_LockX = false;
            bulbCamera.GetComponent<LockCameraXY>().m_LockY = false;
        }
        if (spiritCamera != null)
        {
            spiritCamera.GetComponent<LockCameraXY>().m_LockX = false;
            spiritCamera.GetComponent<LockCameraXY>().m_LockY = false;
        }
    }

    public PlayerFormSwitcher.PlayerForm GetCurrentCameraForm() => currentFormCamera;

    public void SetIgnoreTimeScale(bool ignore)
    {
        GetComponent<CinemachineBrain>().m_IgnoreTimeScale = ignore;
    }
}
