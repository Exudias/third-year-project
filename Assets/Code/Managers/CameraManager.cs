using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject bulbCamera;
    [SerializeField] private GameObject spiritCamera;

    private PlayerFormSwitcher.PlayerForm currentFormCamera = PlayerFormSwitcher.PlayerForm.Bulb;

    private CinemachineVirtualCamera virtualBulbCam;
    private CinemachineVirtualCamera virtualSpiritCam;

    private Vector2 baseBulbOffset;
    private Vector2 baseSpiritOffset;

    private bool xLocked;
    private bool yLocked;

    private float xLockValue;
    private float yLockValue;

    private void Start()
    {
        if (bulbCamera != null)
        {
            virtualBulbCam = bulbCamera.GetComponent<CinemachineVirtualCamera>();
            baseBulbOffset = virtualBulbCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
            bulbTargetOffset = baseBulbOffset;
        }
        
        if (spiritCamera != null)
        {
            virtualSpiritCam = spiritCamera.GetComponent<CinemachineVirtualCamera>();
            baseSpiritOffset = virtualSpiritCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
            spiritTargetOffset = baseSpiritOffset;
        }

        StartCoroutine(ResetVcamConfines());
    }

    private void Update()
    {
        if (!GameManager.IsPlayerDead())
        {
            MoveTowardsLockedValues(bulbCamera);
            MoveTowardsLockedValues(spiritCamera);
            MoveTowardsOffsetValues();
        }
        else
        {
            ApplyDeadPosition(bulbCamera);
            ApplyDeadPosition(spiritCamera);
        }
    }

    private void ApplyDeadPosition(GameObject camera)
    {
        if (camera == null) return;

        camera.GetComponent<LockCameraXY>().m_LockX = true;
        camera.GetComponent<LockCameraXY>().m_LockY = true;

        camera.GetComponent<LockCameraXY>().m_XPosition = xLockValue;
        camera.GetComponent<LockCameraXY>().m_YPosition = yLockValue;
    }

    private const float MAX_LOCK_STEP_PER_SEC = 5;

    private void MoveTowardsLockedValues(GameObject camera)
    {
        if (camera == null) return;

        camera.GetComponent<LockCameraXY>().m_LockX = xLocked;
        camera.GetComponent<LockCameraXY>().m_LockY = yLocked;

        float xPos = camera.GetComponent<LockCameraXY>().m_XPosition;
        float yPos = camera.GetComponent<LockCameraXY>().m_YPosition;

        camera.GetComponent<LockCameraXY>().m_XPosition = Mathf.MoveTowards(xPos, xLockValue, MAX_LOCK_STEP_PER_SEC * Time.unscaledDeltaTime);
        camera.GetComponent<LockCameraXY>().m_YPosition = Mathf.MoveTowards(yPos, yLockValue, MAX_LOCK_STEP_PER_SEC * Time.unscaledDeltaTime);
    }

    private void MoveTowardsOffsetValues()
    {
        if (bulbCamera != null)
        {
            CinemachineVirtualCamera bulbVcam = bulbCamera.GetComponent<CinemachineVirtualCamera>();
            Vector2 currentBulbOffset = bulbVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
            bulbVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = Vector2.MoveTowards(currentBulbOffset, bulbTargetOffset, MAX_LOCK_STEP_PER_SEC * Time.unscaledDeltaTime);
        }
        if (spiritCamera != null)
        {
            CinemachineVirtualCamera spiritVcam = spiritCamera.GetComponent<CinemachineVirtualCamera>();
            Vector2 currentSpiritOffset = spiritVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
            spiritVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = Vector2.MoveTowards(currentSpiritOffset, spiritTargetOffset, MAX_LOCK_STEP_PER_SEC * Time.unscaledDeltaTime);
        }
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

    private Vector2 bulbTargetOffset;
    private Vector2 spiritTargetOffset;

    public void SetCurrentCameraOffset(Vector2 offset, bool bulb, bool spirit)
    {
        CinemachineVirtualCamera bulbVcam = bulbCamera.GetComponent<CinemachineVirtualCamera>();
        CinemachineVirtualCamera spiritVcam = spiritCamera.GetComponent<CinemachineVirtualCamera>();
        if (bulb)
        {
            bulbVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
            bulbTargetOffset = offset;
        }
        if (spirit)
        {
            spiritVcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
            spiritTargetOffset = offset;
        }
    }

    public void SetForcedCameraPosition(Vector2 pos, bool forceX, bool forceY, bool instant)
    {
        xLocked = forceX;
        yLocked = forceY;
        xLockValue = pos.x;
        yLockValue = pos.y;

        if (instant)
        {
            if (bulbCamera != null)
            {
                bulbCamera.GetComponent<LockCameraXY>().m_XPosition = xLockValue;
                bulbCamera.GetComponent<LockCameraXY>().m_YPosition = yLockValue;
            }
            if (spiritCamera != null)
            {
                spiritCamera.GetComponent<LockCameraXY>().m_XPosition = xLockValue;
                spiritCamera.GetComponent<LockCameraXY>().m_YPosition = yLockValue;
            }
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

    public void SetOrthoSize(float size)
    {
        float correctSize = GetComponent<PixelPerfectCamera>().CorrectCinemachineOrthoSize(size);
        if (bulbCamera != null)
        {
            bulbCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = correctSize;
            bulbCamera.GetComponent<CinemachineConfiner2D>().InvalidateCache();
        }
        if (spiritCamera != null)
        {
            spiritCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = correctSize;
            spiritCamera.GetComponent<CinemachineConfiner2D>().InvalidateCache();
        }
    }

    public PlayerFormSwitcher.PlayerForm GetCurrentCameraForm() => currentFormCamera;

    public void SetIgnoreTimeScale(bool ignore)
    {
        GetComponent<CinemachineBrain>().m_IgnoreTimeScale = ignore;
    }
}
