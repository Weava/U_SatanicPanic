using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Ammo Properties

    [SerializeField]
    protected bool infiniteAmmo;

    [SerializeField]
    protected int maxMachineGunAmmo;

    protected int currentMachineGunAmmo;

    [SerializeField]
    protected int maxShotgunAmmo;

    protected int currentShotgunAmmo;

    [SerializeField]
    protected int maxCrossbowAmmo;

    protected int currentCrossbowAmmo;

    #endregion Ammo Properties

    #region Weapon Slots

    //[SerializeField]
    //protected IWeapon item; //Throwable pickups

    [SerializeField]
    protected Weapon pistol;

    //[SerializeField]
    //protected IWeapon melee;

    //[SerializeField]
    //protected IWeapon machineGun;

    //[SerializeField]
    //protected IWeapon shotgun;

    //[SerializeField]
    //protected IWeapon crossbow;

    protected Weapon currentEquip;

    #endregion Weapon Slots

    #region Player Properties

    [SerializeField]
    protected GameObject player;

    [SerializeField]
    protected GameObject playerViewModel;

    #endregion Player Properties

    #region Meta

    private void Start()
    {
        currentMachineGunAmmo = maxMachineGunAmmo;
        currentShotgunAmmo = maxShotgunAmmo;
        currentCrossbowAmmo = maxCrossbowAmmo;
    }

    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetMouseButton(0) && currentEquip != null)
            Fire();

        //Pistol / Melee Equip
        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipPistolOrMelee();
    }

    #endregion Meta

    #region Fire

    public void Fire()
    {
        if (currentEquip.WeaponReady())
        {
            playerViewModel.GetComponent<ViewModelMotion>().Punch(-10);

            switch (currentEquip.AmmoType())
            {
                case AmmoType.SingleUse:
                    break;

                case AmmoType.Infinite:
                    FireInfinite();
                    break;

                case AmmoType.MachineGun:
                    break;

                case AmmoType.Shotgun:
                    break;

                case AmmoType.Crossbow:
                    break;

                default:
                    break;
            }
        }
    }

    protected void FireInfinite()
    {
        currentEquip.Fire(player.GetComponent<FPSController.FPSController>().GetPointOfFocus());
    }

    #endregion Fire

    #region Equip

    protected void EquipPistolOrMelee()
    {
        //if(currentEquip.WeaponType() == WeaponType.Pistol)
        //{
        //    //Equip Melee

        //    currentEquip.Unequip();
        //}
        //else
        //{
        var newEquip = pistol.Equip(playerViewModel);

        if (currentEquip != null)
            currentEquip.Unequip();

        currentEquip = newEquip;
        SetEquipSpawnInPlayerPrefab();
        //}
    }

    protected void SetEquipSpawnInPlayerPrefab()
    {
        currentEquip.SetName(currentEquip.GetName().Replace("(Clone)", ""));
        currentEquip.transform.parent = playerViewModel.transform;
    }

    #endregion Equip
}