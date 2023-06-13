using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(PlayerInput), typeof(PlayerManger))]
public class WeaponController : MonoBehaviour
{
    public float meleeAttackDamage = 10;
    public float attackCooldown = 2;
    public float minAttackDistance = 1;
    public bool canAttack = true;
    public GameObject head;
    public GameObject bulletPrefab;
    public VisualEffect swordEffect;

    private PlayerManger player;
    private PlayerInput playerInput;

    InputAction meleeAttackAction;
    InputAction rangedAttackAction;

    private IDamageable interactingWith;
    private float hitDistance;
    private Ray ray;

    private IEnumerator ProcessAttackCooldown(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private IEnumerator ProcessSwordDraw(float drawTime)
    {
        yield return new WaitForSeconds(drawTime);
        player.playerHUD.meleeWeapponHolster.gameObject.SetActive(true);
    }

    public void MeleeAttack()
    {
        if (GameManger.accpetPlayerInput && canAttack && player.playerHUD.equipmentInventory.inventoryManger.inventorySlots[5].itemData != null)
        {
            canAttack = false;
            player.playerHUD.meleeWeapponHolster.gameObject.SetActive(false);
            StartCoroutine(ProcessSwordDraw(attackCooldown));

            if (swordEffect != null) swordEffect.Play();
            if (AudioManger.Instance != null) AudioManger.Instance.PlaySFX("Sword Slash");

            if (interactingWith != null && minAttackDistance >= hitDistance)
            {
                interactingWith.Damage(meleeAttackDamage);
                interactingWith = null;
            }

            StartCoroutine(ProcessAttackCooldown(attackCooldown));
        }
    }

    public void RangeAttack()
    {
        if (player.playerHUD.GetAmmoCount() <= 0) return;

        if (GameManger.accpetPlayerInput && canAttack && player.playerHUD.equipmentInventory.inventoryManger.inventorySlots[4].itemData != null)
        {
            player.playerHUD.DecrementAmmoCount();
            GameObject bulletObj = GameObject.Instantiate(bulletPrefab, head.transform.position, Quaternion.identity);
            Projectile bullet = bulletObj.GetComponent<Projectile>();
            bullet.AddTagsToIgnore("Player");
            bullet.SetRay(ray);
        }
    }

    public void ProcessVision()
    {
        ray = new(head.transform.position, transform.forward.normalized);
        Debug.DrawRay(ray.origin, ray.origin + ray.direction * 100, Color.cyan);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform.TryGetComponent(out IDamageable damageable))
            {
                interactingWith = damageable;
                hitDistance = Vector3.Distance(head.transform.position, hit.point);
            }
            else interactingWith = null;
        }
        else interactingWith = null;
    }

    void Awake()
    {
        canAttack = true;

        player = GetComponent<PlayerManger>();
        playerInput = GetComponent<PlayerInput>();

        rangedAttackAction = playerInput.actions["PrimaryAttack"];
        meleeAttackAction = playerInput.actions["SecondaryAttack"];

        rangedAttackAction.performed += e => RangeAttack();
        meleeAttackAction.performed += e => MeleeAttack();

        playerInput.actions.Enable();
    }

    void Update()
    {
        ProcessVision();
    }
}
