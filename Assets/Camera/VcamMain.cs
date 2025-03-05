using System;
using Cinemachine;
using UnityEngine;

public class VcamMain : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    Player player;
    
    public float minFOV = 20f;
    public float maxFOV = 60f;

    private void OnEnable()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        player = FindObjectOfType<Player>();

        vcam.Follow = player.CameraPivot;
        vcam.LookAt = player.CameraPivot;
        
        player.Input.MouseScroll += OnMouseScroll;
    }

    private void OnDisable()
    {
        if (player == null) return;
        if (player.Input == null) return;
        
        player.Input.MouseScroll -= OnMouseScroll;
    }

    private void OnMouseScroll(int scrollDelta)
    {
        var targetFOV = Mathf.Clamp(vcam.m_Lens.FieldOfView - scrollDelta, minFOV, maxFOV);
        vcam.m_Lens.FieldOfView = targetFOV;
    }
}
