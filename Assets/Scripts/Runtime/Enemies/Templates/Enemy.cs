using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.UI;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    internal abstract class Enemy : MonoBehaviour, IDamageable
    {
        public EnemyData Data;

        protected int _currentLevel;
        protected float _currentHealth;
        protected bool _isDead;
        protected float _timeSinceDead;

        protected Vector3 _directionToPlayer;
        protected Ray _visonRay;
        protected RaycastHit _hit;
        protected float _timeLastPlayerSeen;
        protected float _timeSinceLastAttack;
        protected bool _isObjectInLineOfSight;

        protected SphereCollider _sphereCollider;
        protected OverheadHealthBar _healthBar;
        protected SpriteRenderer _spriteRenderer;
        protected GameObject _target;
        protected GameObject _body;

        /// <summary>
        /// Active movement (player in sight) beahviour of an Eemey
        /// </summary>
        protected abstract void ActiveMovement();

        /// <summary>
        /// Passive movement (player out of sihgt) beahviour of an Eemey
        /// </summary>
        protected abstract void PassiveMovement();

        /// <summary>
        /// Enemy's attack
        /// </summary>
        protected abstract void Attack(IDamageable damageable, float dmg);

        /// <summary>
        ///  Process an enemies attack move 
        /// </summary>
        protected virtual void ProcessAttack()
        {
            if (_hit.transform == null) return;
            _timeSinceLastAttack += Time.deltaTime;

            if (_hit.transform.CompareTag(_target.tag) && Vector3.Distance(_target.transform.position, gameObject.transform.position) <= Data.minDistToObject + _sphereCollider.radius)
            {
                Attack(_hit.transform.GetComponent<IDamageable>(), Data.attackDamage);
            }
        }

        /// <summary>
        ///  Process view and see if player is in the lne of sight
        /// </summary>
        protected virtual void ProcessVison()
        {
            if (gameObject == null) return;

            _directionToPlayer = _target.transform.position - gameObject.transform.position;
            _directionToPlayer = _directionToPlayer.normalized;

            _visonRay = new(gameObject.transform.position, _directionToPlayer);

#if UNITY_EDITOR
            Debug.DrawRay(_visonRay.origin, _visonRay.direction * 1000, Color.red);
#endif

            _isObjectInLineOfSight = Physics.Raycast(_visonRay, out _hit);

            if (_hit.transform != null && _hit.transform.CompareTag("Player"))
            {
                _timeLastPlayerSeen = 0;
            }
        }

        /// <summary>
        ///  Roate the sprite towards the player
        /// </summary>
        protected virtual void ProcessRotation()
        {
            if (_isObjectInLineOfSight && _hit.transform.CompareTag("Player"))
            {
                Vector3 targetPosition = new(_hit.point.x, gameObject.transform.position.y, _hit.point.z);

                Quaternion targetRotation = Quaternion.LookRotation(targetPosition - gameObject.transform.position);

                _body.transform.localRotation = Quaternion.Lerp(_body.transform.localRotation, targetRotation, Time.deltaTime * Data.lookSpeed);
            }
        }

        /// <summary>
        ///  Process the movement of the enemy
        /// </summary>
        protected virtual void ProcessMovement()
        {
            Vector3 nearestObjectPosition = new(_hit.point.x, gameObject.transform.position.y, _hit.point.z);

            if (Vector3.Distance(nearestObjectPosition, gameObject.transform.position) > Data.minDistToObject)
            {
                if (_hit.transform != null && _hit.transform.CompareTag("Player") || _timeLastPlayerSeen < Data.memoryTime)
                {
                    ActiveMovement();
                }
                else
                {
                    PassiveMovement();
                }
            }
        }

        /// <summary>
        ///  Death process
        /// </summary>
        protected virtual void DeathProcess()
        {
            _timeSinceDead = 0.0f;
            _body.GetComponent<Animator>().enabled = false;
            _spriteRenderer.sharedMaterial = Data.deathMat;
            _spriteRenderer.sharedMaterial.SetFloat("_DissolveAmount", 0.0f);
        }

        /// <summary>
        ///  Spawn drops
        /// </summary>
        protected virtual void SpawnDrops()
        {
            if (gameObject != null && Data.lootTable != null && Data.dropLoot)
            {
                List<Item> drops = Data.lootTable.GetListOfItem(Random.Range(0, Data.maxNumberOfDrops + 1));
                foreach (Item drop in drops)
                {
                    GameObject.Instantiate(drop.itemPrefab, transform.position + new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f)), Quaternion.identity);
                }
            }
        }

        /// <summary>
        ///  Method to be called after death
        /// </summary>
        protected virtual void UpdateOnDeath()
        {
            _timeSinceDead += Time.deltaTime;
            _spriteRenderer.sharedMaterial.SetFloat("_DissolveAmount", _timeSinceDead);
            if (_timeSinceDead > 1)
            {
                SpawnDrops();
                _target.GetComponent<PlayerManger>().AddXP(Data.xpGain);
                GameObject.Destroy(gameObject);
            }
        }

        /// <summary>
        ///  Initialize Sphere Collider
        /// </summary>
        protected virtual void InitializeCollider()
        {
            _sphereCollider = GetComponent<SphereCollider>();
        }

        /// <summary>
        ///  Initialize Heath bar
        /// </summary>
        protected virtual void InitializeHealthBar()
        {
            _healthBar = gameObject.AddComponent<OverheadHealthBar>();
            _healthBar.HealthBarInit(Vector3.up * _sphereCollider.radius + gameObject.transform.position, Quaternion.Euler(90, 0, 0), Data.healthBarPrefab, gameObject);
        }

        /// <summary>
        ///  Initialize Statas
        /// </summary>
        protected virtual void InitializeStats()
        {
            _currentHealth = Data.health;
            _timeLastPlayerSeen = Data.memoryTime;
        }

        /// <summary>
        ///  Initialize Sprite
        /// </summary>
        protected virtual void InitializeSpriteRenderer()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _spriteRenderer.sharedMaterial = Data.defaultMat;
        }

        /// <summary>
        ///  Initialize Body
        /// </summary>
        protected virtual void InitializeBody()
        {
            List<Transform> objs = new();
            gameObject.GetComponentsInChildren(objs);
            Transform body = objs.Find(e => e.name.CompareTo("Body") == 0);
            if (body != null) this._body = body.gameObject;
            else this._body = gameObject;
        }

        /// <summary>
        ///  Find the player
        /// </summary>
        protected virtual void FindTarget()
        {
            _target = GameObject.Find("Player");
        }

        /// <summary>
        ///  Called when enemy is damaged
        /// </summary>
        public virtual void Damage(float dmg)
        {
            _currentHealth -= dmg;

            if (_healthBar != null) _healthBar.UpdateHealthBar(_currentHealth / Data.health);

            if (_currentHealth <= 0) Destroy();
        }

        /// <summary>
        ///  Called When a death occurs
        /// </summary>
        public virtual void Destroy()
        {
            if (_isDead) return;
            _isDead = true;
            if (gameObject != null) DeathProcess();
        }

        protected virtual void Awake()
        {
            _isDead = false;
            InitializeCollider();
            InitializeHealthBar();
            InitializeStats();
            InitializeSpriteRenderer();
            FindTarget();
            InitializeBody();
        }

        protected virtual void Update()
        {
            if (_target == null) return;

            ProcessVison();
            ProcessRotation();
            if (!_isDead)
            {
                ProcessMovement();
                ProcessAttack();
            }
            else UpdateOnDeath();
        }
    }
}
