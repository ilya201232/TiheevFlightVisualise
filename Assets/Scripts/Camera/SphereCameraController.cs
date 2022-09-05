using System;
using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class SphereCameraController : RotationAroundPointBaseClass
    {
        private CinemachineTransposer _transposer;

        private CinemachineVirtualCamera _virtualCamera;

        public override void Start()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            base.Start();
        }

        protected override Vector3 GetCurrentPosition()
        {
            return _transposer.EffectiveOffset;
        }

        protected override void SetPosition(Vector3 position)
        {
            _transposer.m_FollowOffset = position;
        }

        protected override bool IsActive()
        {
            return CinemachineCore.Instance.IsLive(_virtualCamera);
        }
    }
}