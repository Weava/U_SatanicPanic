using Assets.Scripts.Projectiles;
using UnityEngine;

/// <summary>
/// Class for handling weapons or items that are equipable
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected AmmoType ammoType;
    [SerializeField]
    protected WeaponType weaponType;

    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected bool automatic;
    /// <summary>
    /// Rounds per minute
    /// </summary>
    [SerializeField]
    protected float fireRate;
    protected float fireRateCooldown;
    protected bool weaponReady;

    [SerializeField]
    protected AudioSource fireSound;

    [SerializeField]
    protected Animator animator;

    protected GameObject muzzlePointInstance;

    #region Property Getters

    public virtual bool WeaponReady()
    {
        return weaponReady;
    }

    public AmmoType AmmoType()
    {
        return ammoType;
    }

    public WeaponType WeaponType()
    {
        return weaponType;
    }

    #endregion

    public void Fire(Vector3 aimPoint)
    {
        if(weaponReady)
        {
            weaponReady = false;
            fireRateCooldown = 1;
            PlayFireAnimation();
            SpawnProjectile(aimPoint);
            fireSound.Play();
        }
    }

    protected void SpawnProjectile(Vector3 aimPoint)
    {
        var projectileInstance = Instantiate(projectile, muzzlePointInstance.transform.position, muzzlePointInstance.transform.rotation);
        projectileInstance.SetTarget(aimPoint);
    }

    public Weapon Equip(GameObject target)
    {
        var instance = Instantiate(this, target.transform.position, target.transform.rotation);   
        return instance;
    }

    #region Animations

    protected void PlayFireAnimation()
    {
        animator.SetFloat("FireRate", fireRate);
        animator.Play("Fire");
    }

    protected void PlayCycleAnimation()
    {
        animator.SetFloat("CycleRate", fireRate);
        animator.Play("Cycle");
    }

    #endregion

    #region Meta

    public void SetName(string name)
    {
        gameObject.name = name;
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void Unequip()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        //Have equip delay later
        weaponReady = true;

        //Set absolute muzzle point for accuracy, accounts for View Model Motion
        muzzlePointInstance = new GameObject("Muzzle");
        muzzlePointInstance.transform.position = transform.Find("MuzzlePoint").transform.position;
        muzzlePointInstance.transform.rotation = transform.Find("MuzzlePoint").transform.rotation;
        muzzlePointInstance.transform.parent = transform.parent.parent; //Camera

        //Animation setup
        animator.SetFloat("FireRate", fireRate * 2.0f);
        animator.SetFloat("CycleRate", fireRate * 2.0f);
    }

    private void Update()
    {
        if (fireRateCooldown > 0)
        {
            fireRateCooldown -= (Time.deltaTime * fireRate / 60.0f);
        }

        if(!weaponReady && fireRateCooldown <= 0)
        {
            //PlayCycleAnimation();
            if (automatic || Input.GetMouseButtonUp(0))
            {
                weaponReady = true;
            }
        }
    }

    #endregion
}

public enum WeaponType
{
    Pistol,
    Melee,
    Shotgun,
    MachineGun,
    Crossbow
}

public enum AmmoType
{
    SingleUse,
    Infinite,
    MachineGun,
    Shotgun,
    Crossbow
}
