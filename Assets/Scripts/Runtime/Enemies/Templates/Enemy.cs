using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    public EnemyData enemyData;

    protected int currentLevel;
    protected float currentHealth;
    protected bool isDead;
    protected float timeSinceDead;

    protected Vector3 directionToPlayer;
    protected Ray visonRay;
    protected RaycastHit hit;
    protected float timeLastPlayerSeen;
    protected float timeSinceLastAttack;
    protected bool isObjectInLineOfSight;

    protected SphereCollider sphereCollider;
    protected OverheadHealthBar healthBar;
    protected SpriteRenderer spriteRenderer;
    protected GameObject target;
    protected GameObject body;

    protected abstract void ActiveMovement();

    protected abstract void PassiveMovement();

    protected abstract void Attack(IDamageable damageable, float dmg);

    protected virtual void ProcessAttack()
    {
        if (hit.transform == null) return;
        timeSinceLastAttack += Time.deltaTime;

        if (hit.transform.CompareTag(target.tag) && Vector3.Distance(target.transform.position, gameObject.transform.position) <= enemyData.minDistToObject + sphereCollider.radius)
        {
            Attack(hit.transform.GetComponent<IDamageable>(), enemyData.attackDamage);
        }
    }

    protected virtual void ProcessVison()
    {
        if (gameObject == null) return;

        directionToPlayer = target.transform.position - gameObject.transform.position;
        directionToPlayer = directionToPlayer.normalized;

        visonRay = new(gameObject.transform.position, directionToPlayer);

        #if UNITY_EDITOR
        Debug.DrawRay(visonRay.origin, visonRay.direction * 1000, Color.red);
        #endif

        isObjectInLineOfSight = Physics.Raycast(visonRay, out hit);

        if (hit.transform != null && hit.transform.CompareTag("Player"))
        {
            timeLastPlayerSeen = 0;
        }
    }

    protected virtual void ProcessRotation()
    {
        if (isObjectInLineOfSight && hit.transform.CompareTag("Player"))
        {
            Vector3 targetPosition = new(hit.point.x, gameObject.transform.position.y, hit.point.z);

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - gameObject.transform.position);

            body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, targetRotation, Time.deltaTime * enemyData.lookSpeed);
        }
    }

    protected virtual void ProcessMovement()
    {
        Vector3 nearestObjectPosition = new(hit.point.x, gameObject.transform.position.y, hit.point.z);

        if (Vector3.Distance(nearestObjectPosition, gameObject.transform.position) > enemyData.minDistToObject)
        {
            if (hit.transform != null && hit.transform.CompareTag("Player") || timeLastPlayerSeen < enemyData.memoryTime)
            {
                ActiveMovement();
            }
            else
            {
                PassiveMovement();
            }
        }
    }

    protected virtual void DeathProcess()
    {
        timeSinceDead = 0.0f;
        body.GetComponent<Animator>().enabled = false;
        spriteRenderer.sharedMaterial = enemyData.deathMat;
        spriteRenderer.sharedMaterial.SetFloat("_DissolveAmount", 0.0f);
    }

    protected virtual void SpawnDrops()
    {
        if (gameObject != null && enemyData.lootTable != null && enemyData.dropLoot)
        {
            List<Item> drops = enemyData.lootTable.GetListOfItem(Random.Range(0, enemyData.maxNumberOfDrops + 1));
            foreach (Item drop in drops)
            {
                GameObject.Instantiate(drop.itemPrefab, transform.position + new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f)), Quaternion.identity);
            }
        }
    }

    protected virtual void UpdateOnDeath()
    {
        timeSinceDead += Time.deltaTime;
        spriteRenderer.sharedMaterial.SetFloat("_DissolveAmount", timeSinceDead);
        if (timeSinceDead > 1)
        {
            SpawnDrops();
            target.GetComponent<PlayerManger>().AddXP(enemyData.xpGain);
            GameObject.Destroy(gameObject);
        }
    }

    protected virtual void InitializeCollider()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    protected virtual void InitializeHealthBar()
    {
        healthBar = gameObject.AddComponent<OverheadHealthBar>();
        healthBar.HealthBarInit(Vector3.up * sphereCollider.radius + gameObject.transform.position, Quaternion.Euler(90, 0, 0), enemyData.healthBarPrefab, gameObject);
    }

    protected virtual void InitializeStats()
    {
        currentHealth = enemyData.health;
        timeLastPlayerSeen = enemyData.memoryTime;
    }

    protected virtual void InitializeSpriteRenderer()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sharedMaterial = enemyData.defaultMat;
    }

    protected virtual void InitializeBody()
    {
       List<Transform> objs = new();
       gameObject.GetComponentsInChildren(objs);
       Transform body = objs.Find(e => e.name.CompareTo("Body") == 0);
       if (body != null) this.body = body.gameObject;
       else this.body = gameObject;
    }

    protected virtual void FindTarget()
    {
        target = GameObject.Find("Player");
    }

    public virtual void Damage(float dmg)
    {
        currentHealth -= dmg;

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth / enemyData.health);

        if (currentHealth <= 0) Destroy();
    }

    public virtual void Destroy()
    {
        if (isDead) return;
        isDead = true;
        if (gameObject != null) DeathProcess();
    }

    protected virtual void Awake()
    {
        isDead = false;
        InitializeCollider();
        InitializeHealthBar();
        InitializeStats();
        InitializeSpriteRenderer();
        FindTarget();
        InitializeBody();
    }

    protected virtual void Update()
    {
        if (target == null) return;

        ProcessVison();
        ProcessRotation();
        if (!isDead)
        {
            ProcessMovement();
            ProcessAttack();
        }
        else UpdateOnDeath();
    }
}
