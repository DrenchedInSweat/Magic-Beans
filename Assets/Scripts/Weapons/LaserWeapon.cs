using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class LaserWeapon : Weapon
{
    private LaserWeaponVFX myWeapon;
    private VisualEffect flash;
    [SerializeField, ColorUsage(true, true)]
    private Color col;
    //TODO: move
    public delegate void ApplyToCharacter(Character victim);

    private ApplyToCharacter onHit;

    private readonly int delayID = Shader.PropertyToID("Delay");
    private readonly int startID = Shader.PropertyToID("StartFire");
    private readonly int stopID = Shader.PropertyToID("StopFire");
    private readonly int colorID = Shader.PropertyToID("Color");
    private void Start()
    {
        myWeapon = transform.GetChild(0).GetComponent<LaserWeaponVFX>();
        flash = GetComponent<VisualEffect>();
        flash.SetFloat(delayID, TimeBetweenShots);
        flash.SetVector4(colorID, col);
        onHit += victim =>
        {
            print("Killing Enemy");
            victim.TakeDamage(owner, BaseDamage);
        };
    }

    protected override void StartFire()
    {
        print("Start Beam");
        StartCoroutine(StartBeam());
    }


    protected override void StopFire()
    {
        print("Stop Beam");
        flash.SendEvent(stopID);
        flash.Stop();
        myWeapon.gameObject.SetActive(false);
        myWeapon.DeActivate();
    }

    private IEnumerator StartBeam()
    {
        flash.SendEvent(startID);
        yield return new WaitForSeconds(TimeBetweenShots);
        myWeapon.gameObject.SetActive(true);
        myWeapon.Activate(col, onHit);
    }

    //I'm lazy
    protected override void TryShoot()
    {
        
    }
}
