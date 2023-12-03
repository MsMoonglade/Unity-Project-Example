using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineVirtualCameraSwitcher : MonoBehaviour
{
    public static CinemachineVirtualCameraSwitcher instance;

    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera editCamera;

    private float shakeTime;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        //if shaking remove shake time
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            if (shakeTime <= 0)
            {
                playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            }
        }
    }
    public void PlayerCameraPrio()
    {
        playerCamera.Priority = 11;
    }

    public void NoPlayerCameraPrio()
    {
        playerCamera.Priority = 0;
    }

    public void EditCameraPrio()
    {
        editCamera.Priority = 12;
    }

    public void NoEditCameraPrio()
    {
        editCamera.Priority = 0;
    }

    public void ShakeGameCamera(float i_time , float i_amount)
    {
        playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = i_amount;
        shakeTime = i_time;
    }
}