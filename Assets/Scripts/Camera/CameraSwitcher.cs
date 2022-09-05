using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> camerasList;

        [SerializeField] private int basePriority;
        [SerializeField] private int activeCamPriority;
        
        private int _currentCamera;

        private void Start()
        {
            activeCamPriority = Math.Max(activeCamPriority, basePriority + 1);

            if (camerasList.Count == 0)
            {
                Debug.LogWarning("Camera Switcher doesn't have any cameras!");
            }
            else
            {
                _currentCamera = 0;
                foreach (var virtualCamera in camerasList)
                {
                    virtualCamera.Priority = basePriority;
                }

                camerasList[_currentCamera].Priority = activeCamPriority;
            }

            
        }

        public void SwitchCamera()
        {
            camerasList[_currentCamera].Priority = basePriority;
            _currentCamera = (_currentCamera + 1) % camerasList.Count;
            camerasList[_currentCamera].Priority = activeCamPriority;
        }
    }
}