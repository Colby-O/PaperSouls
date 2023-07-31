using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Interfaces;
using UnityEngine.Events;
using PaperSouls.Runtime.Player;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.DungeonBehaviour
{
    internal class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Item _itemRequiredToUnlock;
        [SerializeField] [Range(0, 1)] private float _lockProbability;
        [SerializeField] private float _rotationSpeed = 1.0f;
        [SerializeField] private float _rotationAmount = 90.0f;
        [SerializeField] [Range(-1, 1)] private float _forwardDirection = 0;

        private bool _isOpen;
        private bool _isLocked;
        private Vector3 _startRotation;
        private Vector3 _forward;
        private Coroutine _animationCoroutine;

        public UnityAction<IInteractable> OnInteractionComplete { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        private IEnumerator RotateOpen(float playerPos)
        {
            Quaternion start = transform.rotation;
            Quaternion end;

            if (playerPos >= _forwardDirection) end = Quaternion.Euler(0, _startRotation.y - _rotationAmount, 0);
            else end = Quaternion.Euler(0, _startRotation.y + _rotationAmount, 0);

            _isOpen = true;
            float time = 0;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(start, end, time);
                yield return null;
                time += Time.deltaTime * _rotationSpeed;
            }
        }

        private IEnumerator RotateClose()
        {
            Quaternion start = transform.rotation;
            Quaternion end = Quaternion.Euler(_startRotation);

            _isOpen = false;
            float time = 0;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(start, end, time);
                yield return null;
                time += Time.deltaTime * _rotationSpeed;
            }
        }

        public void Open(Vector3 playerPos)
        {
            if(!_isOpen)
            {
                if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);

                float dot = Vector3.Dot(_forward, (playerPos - transform.position).normalized);
                _animationCoroutine = StartCoroutine(RotateOpen(dot));
            }
        }

        public void Close()
        {
            if (_isOpen)
            {
                if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
                _animationCoroutine = StartCoroutine(RotateClose());
            }
        }

        private void OnFailedUnlock()
        {
            Debug.Log($"Cannot Unlocked Door. You Need a {_itemRequiredToUnlock.displayName}!");
        }

        public void Interact(Interactor interactor, out bool successful)
        {
            successful = true;
            if (!_isOpen)
            {
                if (!_isLocked) Open(interactor.transform.position);
                else
                {
                    InventoryManger inv = interactor.gameObject.GetComponentInChildren<InventoryHolder>().InventoryManger;
                    if (inv.HasItem(_itemRequiredToUnlock))
                    {
                        inv.TakeItem(_itemRequiredToUnlock, 1);
                        Open(interactor.transform.position);
                        _isLocked = false;
                    }
                    else OnFailedUnlock();
                }
            }
            else Close();
        }

        public void EndInteraction()
        {
        }

        private void Awake()
        {
            _startRotation = transform.rotation.eulerAngles;
            _forward = transform.forward;
            _isLocked = (_lockProbability >= Random.value) ? true : false;
        }
    }
}
