using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeapons : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject shotgun;
    [SerializeField] GameObject autogun;
    [SerializeField] GameObject rifle;
    [SerializeField] Transform leftHandTarget;

    Controls.PlayerActions controls;
    Rig rig;
    Animator animator;

    bool isBusy;

    void Awake()
    {
        controls = PlayerControl.Controls.Player;
        rig = GetComponentInChildren<Rig>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SwitchWeapon(pistol, 1);
    }

    // Update is called once per frame
    void Update()
    {
        ToggleWeapon();
        HandleWeapon();
    }

    void HandleWeapon()
    {
        if (controls.Attack.WasPerformedThisFrame())
        {
            animator.SetTrigger("Shoot");
        }

        float target = isBusy ? 0f : 1f;
        rig.weight = Mathf.MoveTowards(rig.weight, target, Time.deltaTime * 8f);

        if (controls.Reload.WasCompletedThisFrame())
        {
            animator.SetTrigger("Reload");
            isBusy = true;
        }
    }

    void ToggleWeapon()
    {
        if (controls.Weapon1.WasPerformedThisFrame()) SwitchWeapon(pistol, 1);
        else if (controls.Weapon2.WasPerformedThisFrame()) SwitchWeapon(shotgun, 2);
        else if (controls.Weapon3.WasPerformedThisFrame()) SwitchWeapon(autogun, 1);
        else if (controls.Weapon4.WasPerformedThisFrame()) SwitchWeapon(rifle, 3);
    }

    void SwitchWeapon(GameObject gun, int layerInt)
    {
        animator.SetTrigger("Equip");
        animator.SetBool("isBusy", true);
        isBusy = true;

        // Toggling gameobject
        pistol.SetActive(false);
        shotgun.SetActive(false);
        autogun.SetActive(false);
        rifle.SetActive(false);
        gun.SetActive(true);

        // Setting up IK target transform
        Transform targetTransform = gun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;

        for (int i = 1; i < animator.layerCount; i++)
        {
            if (i == layerInt) animator.SetLayerWeight(i, 1);
            else animator.SetLayerWeight(i, 0);
        }
    }

    public void RestoreRig()
    {
        isBusy = false;
        animator.SetBool("isBusy", false);
    }
}
