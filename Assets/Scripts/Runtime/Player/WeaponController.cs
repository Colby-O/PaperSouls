using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using PaperSouls.Core;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.Weapons;

namespace PaperSouls.Runtime.Player
{
    [RequireComponent(typeof(PlayerInput), typeof(PlayerManger))]
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private float _meleeAttackDamage = 10;
        [SerializeField] private float _attackCooldown = 2;
        [SerializeField] private float _minAttackDistance = 1;
        [SerializeField] private bool _canAttack = true;
        [SerializeField] private GameObject _head;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private VisualEffect _swordEffect;

        private PlayerManger _player;
        private PlayerInput _playerInput;

        private InputAction _meleeAttackAction;
        private InputAction _rangedAttackAction;

        private IDamageable _interactingWith;
        private float _hitDistance;
        private Ray _ray;

        /// <summary>
        /// Coroutine to process attack cooldown
        /// </summary>
        private IEnumerator ProcessAttackCooldown(float _attackCooldown)
        {
            yield return new WaitForSeconds(_attackCooldown);
            _canAttack = true;
        }

        /// <summary>
        /// Coroutine to process sword draw effect
        /// </summary>
        private IEnumerator ProcessSwordDraw(float drawTime)
        {
            yield return new WaitForSeconds(drawTime);
            _player.PlayerHUD.MeleeWeapponHolster.gameObject.SetActive(true);
        }

        /// <summary>
        /// Process a Melee Attack
        /// </summary>
        public void MeleeAttack()
        {
            if (GameManger.AccpetPlayerInput && _canAttack && _player.PlayerHUD.EquipmentInventory.InventoryManger.InventorySlots[5].ItemData != null)
            {
                _canAttack = false;
                _player.PlayerHUD.MeleeWeapponHolster.gameObject.SetActive(false);
                StartCoroutine(ProcessSwordDraw(_attackCooldown));

                if (_swordEffect != null) _swordEffect.Play();
                if (AudioManger.Instance != null) AudioManger.Instance.PlaySFX("Sword Slash");

                if (_interactingWith != null && _minAttackDistance >= _hitDistance)
                {
                    _interactingWith.Damage(_meleeAttackDamage);
                    _interactingWith = null;
                }

                StartCoroutine(ProcessAttackCooldown(_attackCooldown));
            }
        }

        /// <summary>
        /// Process a  Ranged Attack
        /// </summary>
        public void RangeAttack()
        {
            if (_player.PlayerHUD.GetAmmoCount() <= 0) return;

            if (GameManger.AccpetPlayerInput && _canAttack && _player.PlayerHUD.EquipmentInventory.InventoryManger.InventorySlots[4].ItemData != null)
            {
                _player.PlayerHUD.DecrementAmmoCount();
                GameObject bulletObj = GameObject.Instantiate(_bulletPrefab, _head.transform.position, Quaternion.identity);
                Projectile bullet = bulletObj.GetComponent<Projectile>();
                bullet.AddTagsToIgnore("Player");
                bullet.SetRay(_ray);
            }
        }

        /// <summary>
        /// Get any objects in the player's line-of-sight
        /// </summary>
        public void ProcessVision()
        {
            _ray = new(_head.transform.position, transform.forward.normalized);
            Debug.DrawRay(_ray.origin, _ray.origin + _ray.direction * 100, Color.cyan);
            if (Physics.Raycast(_ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.TryGetComponent(out IDamageable damageable))
                {
                    _interactingWith = damageable;
                    _hitDistance = Vector3.Distance(_head.transform.position, hit.point);
                }
                else _interactingWith = null;
            }
            else _interactingWith = null;
        }

        void Awake()
        {
            _canAttack = true;

            _player = GetComponent<PlayerManger>();
            _playerInput = GetComponent<PlayerInput>();

            _rangedAttackAction = _playerInput.actions["PrimaryAttack"];
            _meleeAttackAction = _playerInput.actions["SecondaryAttack"];

            _rangedAttackAction.performed += e => RangeAttack();
            _meleeAttackAction.performed += e => MeleeAttack();

            _playerInput.actions.Enable();
        }

        void Update()
        {
            ProcessVision();
        }
    }
}
