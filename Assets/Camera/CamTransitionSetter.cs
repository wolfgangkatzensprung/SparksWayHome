using Cinemachine;
using UnityEngine;

public class CamTransitionSetter : MonoBehaviour
{
    public void SetMainCamHardCut()
    {
        Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
    }
}
