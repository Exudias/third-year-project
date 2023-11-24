using UnityEngine;
using Cinemachine;

// Taken from https://forum.unity.com/threads/follow-only-along-a-certain-axis.544511/#post-3591751
// Modified to suit my case

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's X or/and Y co-ordinate
/// </summary>
[AddComponentMenu("")] // Hide in menu
public class LockCameraXY : CinemachineExtension
{
    [Tooltip("If should lock the camera's X position to set value")]
    public bool m_LockX = false;
    [Tooltip("If should lock the camera's Y position to set value")]
    public bool m_LockY = false;
    [Tooltip("The camera's X position to lock to")]
    public float m_XPosition = 0;
    [Tooltip("The camera's y position to lock to")]
    public float m_YPosition = 0;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            Vector3 pos = state.RawPosition;
            if (m_LockX)
            {
                pos.x = m_XPosition;
            }
            if (m_LockY)
            {
                pos.y = m_YPosition;
            }
            state.RawPosition = pos;
        }
    }
}