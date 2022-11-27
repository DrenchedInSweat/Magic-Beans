using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    [SerializeField] private float timeBetweenShots;

    //public float TimeBetweenShots => timeBetweenShots;
    [SerializeField] private float recoilMultiplier = 1;
    [SerializeField] private Vector2 recoil;

    private int curBullets;
    private float curShotTime;

    
    //--------------------------------- HANDLING ---------------------------------//

    [Header("Shooting")]
    //--------------------------------- SHOOTING ---------------------------------//

    [SerializeField]
    private Projectile projectile; // TODO --> Convert to custom projectile type.
    
    [SerializeField]
    private float baseDamage;

    [SerializeField] private float bulletSpeed = 1000;

    [SerializeField] private float randomSpread = 0.4f;
    
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
    [SerializeField] private EProjectileType projectileType;

    private Transform camTrans;
    protected Character owner;

    public bool tryingToShoot;
    private bool isShooting;
    
        //--------------------------------- SHOOTING ---------------------------------//

    #region Getters
    public int ProjectilesFired => projectilesFired;
    public float TimeBetweenShots => timeBetweenShots;
    public float RecoilMultiplier => recoilMultiplier;
    public float BaseDamage => baseDamage;
    public float BulletSpeed => bulletSpeed;
    public float RandomRandomSpread => randomSpread;
    public Projectile Projectile => projectile;
    public ESprayPattern SprayPattern => sprayPattern;
    
    #endregion
        

    private void Awake()
    {
        camTrans = Camera.main.transform;
    }

    public void Init(Character owner)
    {
        this.owner = owner;
    }

    private void Update()
    {
        curShotTime += Time.deltaTime;

        if (tryingToShoot)
        {
            if (!isShooting)
            {
                isShooting = true;
                StartFire();
            }
            TryShoot();
        }
        else if (isShooting)
        {
            isShooting = false;
            StopFire();
        }
    }


    protected virtual void StartFire()
    {
    }

    protected virtual void StopFire()
    {
    }

    protected virtual void TryShoot()
    { 
        
        if (CanShoot())
        {
            Shoot();
            curShotTime = 0;
        }
    }

    protected virtual bool CanShoot()
    {
        //TODO add check, cannot shoot in safe zones, or if hand is being used to wall run.
        return (curShotTime > timeBetweenShots && projectilesFired > 0);
    }

    protected virtual void Shoot()
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
                        bulletPositions[i] = new Vector2(randomSpread * Mathf.Cos(curAngle), randomSpread * Mathf.Sin(curAngle));
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
                            dist += randomSpread; // Only add ever other time.
                        }
                        dist = -dist; // Neg then pos then neg then ... 
                        bulletPositions[i] = new Vector2(dist, 0);
                    }

                    break;
                case ESprayPattern.Random:
                    //Start at one, the first shot should be 100% accurate
                    for (int i = 1; i < projectilesFired; i++)
                    {
                        bulletPositions[i] = new Vector2(Random.Range(-randomSpread, randomSpread), Random.Range(-randomSpread, randomSpread));
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
                            bulletPositions[i] = new Vector2(curStep * randomSpread * Mathf.Cos(curAng), curStep * randomSpread * Mathf.Sin(curAng));
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

            Projectile go = Instantiate(projectile, transform.position + thisDir, vec * projectile.transform.rotation, GameManager.Instance.BulletParent);
            go.Init(owner, projectileType);
            
            go.GetComponent<Rigidbody>().AddForce(bulletSpeed * camTrans.forward, ForceMode.Impulse);
            #if UNITY_EDITOR
            Debug.DrawRay(go.transform.position, 5 * (camTrans.forward), Color.blue, 2f, false);
            #endif
            print("Reached");
            Destroy(go.gameObject, 4);
        }
        //TODO: move to more appropriate spot
       if(owner is Player player)
            player.AddRecoil(new Vector2(recoil.x, recoil.y));
    }

    //Because I can
    public static Weapon operator +(Weapon weapon, UpgradeScriptableObject upgrade)
    {
        //Do upgrade stuff.
        return weapon;
    }



}
