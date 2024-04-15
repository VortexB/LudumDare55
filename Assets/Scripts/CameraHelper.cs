using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraHelper : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera MainCam;
    [SerializeField] CinemachineVirtualCamera ZoomCam;
    private void Start() {
        // Cursor.lockState =CursorLockMode.Locked;
        // Cursor.visible = false;
    }
    private void Update() {
        ZoomCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = MainCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value;
    }
}
