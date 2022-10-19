using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EProjectileType
{
    Shock, //{Pointer finger} On hit, eletricutes enemy -- When hitting a Slime, slime becomes conductive. This means that the slime will eletricute any water, and do electric damage -- Also makes the slime resistant to electric attacks..., also activates electronic stuff
    Electric, //{Wavey Hands} Same as shock but, nearest enemy is electricuted up to X times.
    Punch, //{Fist} Pushes back an enemy 
    Explosive, //{Boxing Glove} Same as punch, but explode too.    
    Pull, // {Finger wagling} Pulls an enemy towards you
    Fling // {Like throwing a BBall 1 hand} // Throws self in desired direction 
}

public class Weapon : MonoBehaviour
{
    [Header("Handling")]
    //--------------------------------- HANDLING ---------------------------------//
    [SerializeField]
    private int bulletsPerMag = 10;
    //public int BulletsPerMag => bulletsPerMag;

    [SerializeField] private int maxMags;
    //public int MaxMags => maxMags;

    [SerializeField] private float timeBetweenShots;

    //public float TimeBetweenShots => timeBetweenShots;



    private int curBullets;
    private int curMag;
    private float curShotTime;

    
    

    //--------------------------------- HANDLING ---------------------------------//

    [Header("Shooting")]
    //--------------------------------- SHOOTING ---------------------------------//

    [SerializeField]
    private Projectile projectile; // TODO --> Convert to custom projectile type.
    
    [SerializeField]
    private float baseDamage;

    [SerializeField] private float forcePerShot = 1000;

    [SerializeField] private float spread = 0.4f;
    //public float BaseDamage => baseDamage;

    [Tooltip("This is how much ammo is used per shot")] [SerializeField]
    private int ammoPerShot;
    //public int AmmoPerShot => ammoPerShot;

    [Tooltip("This is how many bullets are shot")] [SerializeField]
    private int projectilesFired = 13;
    //public int ProjectilesFired => projectilesFired;

    public enum ESprayPattern
    {
        Random,
        Line,
        Circle,
        Star
    }

    [SerializeField] private ESprayPattern sprayPattern;
    [SerializeField] private LayerMask effectiveLayers; // USED FOR DOUBLE DAMAGE AGAINST ENEMY TYPES
    [SerializeField] private EProjectileType projectileType;

    private Transform camTrans;
    private Character owner;
    
        //--------------------------------- SHOOTING ---------------------------------//

    private void Awake()
    {
        curMag = bulletsPerMag;
        camTrans = Camera.main.transform;
    }

    public void Init(Character owner)
    {
        this.owner = owner;
    }

    private void Update()
    {
        curShotTime += Time.deltaTime;
    }

    public void TryShoot()
    {
        if(IsMagEmpty() && CanReload())
        {
            Reload();
        }
        else if (CanShoot())
        {
            Shoot();
            curShotTime = 0;
        }
        else
        {
            print("Weapon is out of ammo!");
        }
    }

    public void TryReload()
    {
        if (CanReload())
        {
            Reload();
        }
    }

    //Need a co-route somewhere inbetween come back here...
    private void Reload()
    {
        int ammoGiven = Mathf.Min(bulletsPerMag - curMag, curBullets); // Ammo to give || Say we need 5 bullelts // but we have 3 left. (5, 3)
        curBullets -= ammoGiven;
        curMag += ammoGiven;
    }

    public void StopReload()
    {
        
    }


    private bool CanReload()
    {
        return curBullets > 0;
    }
    

    private bool CanShoot()
    {
        return (curShotTime > timeBetweenShots && projectilesFired > 0);
    }

    private bool IsMagEmpty()
    {
        return (curMag - ammoPerShot < 1);
    }

    private void Shoot()
    {

        

        Vector2[] bulletPositions = new Vector2[projectilesFired];
        bulletPositions[0] = Vector2.zero;
        
        if(projectilesFired > 1)
        {
            switch (sprayPattern)
            {
                case ESprayPattern.Circle:
                    // Imagine drawing a circle on the players screen.
                    // In this case, the spread is the radius of the circle.
                    // Then, we calculate the distance between each point, by dividing the sum of projectiles to fire by 360
                    float angle = 360f / (projectilesFired-1) * Mathf.Deg2Rad; // 360 / 12 --> 30
                    //bulletPositions[1] = new Vector2(spread, 0);
                    float curAngle = 0;
                    //Now take a step forward from the player, (Distance of 0.01)
                    for (int i = 1; i < projectilesFired; i++)
                    {
                        
                        //SOH --> Sin(Ang) = Opp/Hyp, CAH --> Cos(Ang) = Adj / Hyp
                        bulletPositions[i] = new Vector2(spread * Mathf.Cos(curAngle), spread * Mathf.Sin(curAngle));
                        curAngle += angle;
                        
                    }

                    break;
                case ESprayPattern.Line:

                    //The first shot should be 100% accurate
                    float dist = 0;
                    for (int i = 1; i < projectilesFired; i++)
                    {
                        if ((i & 1) == 1) // Let's say I is 6 110 & 001 ==> 0 EVEN but, if I is 7, 111 & 001 ==> 1
                        {
                            dist += spread; // Only add ever other time.
                        }
                        dist = -dist; // Neg then pos then neg then ... 
                        bulletPositions[i] = new Vector2(dist, 0);
                    }

                    break;
                case ESprayPattern.Random:
                    //Start at one, the first shot should be 100% accurate
                    for (int i = 1; i < projectilesFired; i++)
                    {
                        bulletPositions[i] = new Vector2(Random.Range(-spread, spread), Random.Range(-spread, spread));
                    }
                    break;
                case ESprayPattern.Star:

                    //Each star should evenly distribute...
                    //PROBLEMATIC
                    //2 bullets BREAKS
                    //3 bullets (Line)
                    //REP
                    //4 triangle (Mozambique)
                    //5 Diamond / Square
                    //6 Star / PK
                    // -- REP --
                    //7 2x Mozambique
                    //8 .... ? 
                    //9 Square 2x 
                    
                    bulletPositions[1] = new Vector2(0, 1);
                    if(projectilesFired == 3)
                        bulletPositions[2] = new Vector2(0, -1);

                    else if (projectilesFired > 3)
                    {
                        float ang;
                        int step;
                        float startRot;

                        if ((projectilesFired - 1)%3 == 0) // 3, 6, 9
                        {
                            ang = 120 * Mathf.Deg2Rad;
                            step = 3;
                            startRot = 90 * Mathf.Deg2Rad;
                        }
                        else if ((projectilesFired - 1) % 2 == 0) //0, 2, 4, 8
                        {
                            ang = 90 * Mathf.Deg2Rad;
                            step = 4;
                            startRot = 45 * Mathf.Deg2Rad;
                        }
                        else if ( (projectilesFired - 1) % 5 == 0 ) // 5
                        {
                            ang = 72 * Mathf.Deg2Rad;
                            step = 5;
                            startRot = 12 * Mathf.Deg2Rad;
                        }
                        else // 1, 7
                        {
                            ang = 360/7f * Mathf.Deg2Rad;
                            step = 7;
                            startRot = 0;
                        }

                        int stepCount = step;
                        int curStep = 1;
                        float curAng = startRot;

                        //Start at one, the first shot should be 100% accurate
                        for (int i = 1; i < projectilesFired; i++)
                        {
                            //SOH --> Sin(Ang) = Opp/Hyp, CAH --> Cos(Ang) = Adj / Hyp
                            bulletPositions[i] = new Vector2(curStep * spread * Mathf.Cos(curAng), curStep * spread * Mathf.Sin(curAng));
                            curAng += ang;
                            print(curStep);
                            if (--stepCount == 0) //
                            {
                                curAng = startRot;
                                ++curStep;
                                stepCount = step;
                            }
                        }
                    }
                    break;
            }
        }
        
        //Imagine now that we have our forward vector. Let's transform this vector into a plane using the cross product;

        //If we have the rotation of the weapon, Just apply the same rotation to each point
        
        Quaternion vec = camTrans.rotation;

        foreach (Vector2 bulletPos in bulletPositions)
        {
            print("Bullet: " + bulletPos);

            Vector3 thisDir = vec * bulletPos;//Vector3.RotateTowards(bulletPos, transform.forward, 1, 1);
            //Vector3 BulletDir = Vector3.Cross(bulletPos, Vector3.forward);

            Projectile go = Instantiate(projectile, transform.position + thisDir, vec, GameManager.Instance.BulletParent);
            go.Init(owner, projectileType);
            
            go.GetComponent<Rigidbody>().AddForce(forcePerShot * camTrans.forward, ForceMode.Impulse);
            #if UNITY_EDITOR
            Debug.DrawRay(go.transform.position, 5 * (camTrans.forward), Color.blue, 2f, false);
            #endif
            Destroy(go.gameObject, 7);
        }
    }

    //Because I can
    public static Weapon operator +(Weapon weapon, UpgradeScriptableObject upgrade)
    {
        //Do upgrade stuff.
        return weapon;
    }



}
