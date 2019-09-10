using System.Collections.Generic;
using Cinemachine;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
  public class CameraShake : MonoBehaviour {
    [SerializeField] float shakeDuration = 0.3f;
    [SerializeField] float shakeAmplitude = 1f;
    [SerializeField] float shakeFrequency = 1f;

    private float timeSinceStartedShaking = Mathf.Infinity;
    private List<CinemachineVirtualCamera> cameraRigs = new List<CinemachineVirtualCamera>();
    private List<CinemachineBasicMultiChannelPerlin> cameraRigsNoise = new List<CinemachineBasicMultiChannelPerlin>();

    private void OnEnable() {
      foreach (Health health in FindObjectsOfType<Health>()) {
        health.takeDamageUnityEvent.AddListener(ShakeCamera);
      }
    }

    private void OnDisable() {
      foreach (Health health in FindObjectsOfType<Health>()) {
        health.takeDamageUnityEvent.RemoveListener(ShakeCamera);
      }
    }

    private void Start() {
      CinemachineFreeLook freeLookCamera = GetComponent<CinemachineFreeLook>();
      for (int i = 0; i < 3; i++) {
        cameraRigs.Add(freeLookCamera.GetRig(i));
        cameraRigsNoise.Add(cameraRigs[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
      }
    }

    private void ShakeCamera(float damageTaken) {
      timeSinceStartedShaking = 0f;
    }

    private void Update() {

      if (timeSinceStartedShaking < shakeDuration) {
        foreach (CinemachineBasicMultiChannelPerlin noise in cameraRigsNoise) {
          noise.m_AmplitudeGain = shakeAmplitude;
          noise.m_FrequencyGain = shakeFrequency;
        }
        timeSinceStartedShaking += Time.deltaTime;
      } else {
        foreach (CinemachineBasicMultiChannelPerlin noise in cameraRigsNoise) {
          noise.m_AmplitudeGain = 0f;
          noise.m_FrequencyGain = 0f;
        }
      }
    }
  }
}