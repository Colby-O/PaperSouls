using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.UI.Minimap
{
    internal sealed class MinimapFollow : MonoBehaviour
    {
        [SerializeField] private MinimapSettings _settings;
        [SerializeField] private float _cameraHeight;

        private void Awake()
        {
            _settings = (_settings == null) ? GetComponentInParent<MinimapSettings>() : _settings;
            _cameraHeight = _settings.CameraHeight;
        }

        private void Update()
        {
            Vector3 targetPosition = _settings.TargetToFollow.transform.position;
            transform.position = new(targetPosition.x, targetPosition.y + _cameraHeight, targetPosition.z);
            if (_settings.RotateWithTarget)
            {
                Quaternion targetRotation = _settings.TargetToFollow.transform.rotation;
                transform.rotation = Quaternion.Euler(90, targetRotation.eulerAngles.y, 0);
            }
        }
    }
}
