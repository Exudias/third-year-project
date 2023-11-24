using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;

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
    }

    public void ActivateBulbCamera()
    {
        // Save info from old cam to transfer to new one, resetting old one
        CinemachineVirtualCamera activeVcam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.
            VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        bool oldForceX = activeVcam.GetComponent<LockCameraXY>().m_LockX;
        bool oldForceY = activeVcam.GetComponent<LockCameraXY>().m_LockY;
        float oldPosX = activeVcam.GetComponent<LockCameraXY>().m_XPosition;
        float oldPosY = activeVcam.GetComponent<LockCameraXY>().m_YPosition;
        // Reset old one
        DisableForcedCameraPosition();

        currentFormCamera = PlayerFormSwitcher.PlayerForm.Bulb;

        virtualBulbCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = baseBulbOffset;

        bulbCamera.SetActive(true);
        spiritCamera.SetActive(false);

        // Transfer old cam's forced pos info to new one
        SetForcedCameraPosition(new Vector2(oldPosX, oldPosY), oldForceX, oldForceY);
    }

    public void ActivateSpiritCamera()
    {
        // Save info from old cam to transfer to new one, resetting old one
        CinemachineVirtualCamera activeVcam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.
             VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        bool oldForceX = activeVcam.GetComponent<LockCameraXY>().m_LockX;
        bool oldForceY = activeVcam.GetComponent<LockCameraXY>().m_LockY;
        float oldPosX = activeVcam.GetComponent<LockCameraXY>().m_XPosition;
        float oldPosY = activeVcam.GetComponent<LockCameraXY>().m_YPosition;
        // Reset old one
        DisableForcedCameraPosition();

        currentFormCamera = PlayerFormSwitcher.PlayerForm.Spirit;

        virtualSpiritCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = baseSpiritOffset;

        bulbCamera.SetActive(false);
        spiritCamera.SetActive(true);

        // Transfer old cam's forced pos info to new one
        SetForcedCameraPosition(new Vector2(oldPosX, oldPosY), oldForceX, oldForceY);
    }

    public void SetCurrentCameraOffset(Vector2 offset)
    {
        CinemachineVirtualCamera activeVcam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.
            VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        activeVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
    }

    public void SetForcedCameraPosition(Vector2 pos, bool forceX, bool forceY)
    {
        CinemachineVirtualCamera activeVcam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.
            VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        activeVcam.GetComponent<LockCameraXY>().m_LockX = forceX;
        activeVcam.GetComponent<LockCameraXY>().m_LockY = forceY;
        activeVcam.GetComponent<LockCameraXY>().m_XPosition = pos.x;
        activeVcam.GetComponent<LockCameraXY>().m_YPosition = pos.y;
    }

    public void DisableForcedCameraPosition()
    {
        CinemachineVirtualCamera activeVcam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.
            VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        activeVcam.GetComponent<LockCameraXY>().m_LockX = false;
        activeVcam.GetComponent<LockCameraXY>().m_LockY = false;
    }

    public PlayerFormSwitcher.PlayerForm GetCurrentCameraForm() => currentFormCamera;
}
