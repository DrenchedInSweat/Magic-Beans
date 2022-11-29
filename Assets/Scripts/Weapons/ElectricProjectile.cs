using Characters;
using UnityEngine;

namespace Weapons
{
    public class ElectricProjectile : Projectile
    {
        protected override void OnHit(Transform hitObject)
        {
            print("Doing Electric");
            ElectricRecurse(hitObject, hitObject, recursion);
        }

        private void ElectricRecurse(Transform prv, Transform hitObj, int remainingHits)
        {
            if(onHitSound) AudioSource.PlayClipAtPoint(onHitSound, hitObj.position, 0.2f);
            Electrocute(hitObj, remainingHits);
            
            //Base Case
            if (remainingHits == 0)
                return;

            RaycastHit[] results = new RaycastHit[10]; // This is how many it can hit...
            Transform t = transform;
            int num = Physics.SphereCastNonAlloc(t.position, remainingHits * 2.5f, t.forward, results, remainingHits * 2.5f, GameManager.Instance.ElectricLayers);

            //Search through and get the correct instance...
            float curMinDist = 100f;
            Transform curTarg = null;
            for (int i = 0; i < num; i++)
            {
                //The object his is the object OR it's the previous
                if (results[i].transform == hitObj || results[i].transform == prv)
                    continue;

                float dist = results[i].distance;
                if (dist < curMinDist)
                {
                    curTarg = results[i].transform;
                    curMinDist = dist;
                }
            }

            //Failed to riccochet
            if (curTarg == null)
                return;
            //Recurse
            ElectricRecurse(hitObj, curTarg, remainingHits-1);

        }

        //Should this be outside of this scope??
        private void Electrocute(Transform hitObj, int power)
        {
        
            if (hitObj.TryGetComponent(out Character character))
            {
                print("Electrocuted a character");
                if (character is Slime s) //Slime logic
                    s.Electrocute();  
                else //Take damage
                    character.TakeDamage(myOwner,  (power + 1f) / recursion * damage);
            }
            else if (hitObj.TryGetComponent(out PuzzleSwitch hitSwitch))
            {
                print("Electrocuted a lever");
                hitSwitch.Activate();
            }
        }
    }
}
