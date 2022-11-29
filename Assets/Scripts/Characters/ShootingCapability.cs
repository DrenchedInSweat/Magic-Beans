using Characters.BaseStats;
using UnityEngine;
using Weapons;

namespace Characters
{
    public class ShootingCapability : MonoBehaviour
    {
        [Header("Shooting")] [SerializeField] private Transform hand;
        private Weapon[] weapons;
        private int[] hashes;
        public int Len { get; private set; }

        public Weapon GetWep(int idx, out int hash)
        {
            hash = hashes[idx];
            return weapons[idx];
        }
        
        public Weapon GetWep(int idx)
        {
            return weapons[idx];
        }

        public void Init(Character owner)
        {
            Len= hand.childCount;
            print("Changing len: " + Len);
            weapons = new Weapon[Len];
            hashes = new int[Len];
            for (int i = 0; i < Len; ++i)
            {
                print("adding weapon: " + i + " --> " + hand.GetChild(i).gameObject.name);
                weapons[i] = hand.GetChild(i).GetComponent<Weapon>();
                weapons[i].Init(owner);
                hashes[i] = Animator.StringToHash(weapons[i].GetStats<WeaponStatsSo>().AnimatorHash);
            }
        }




#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 h = hand.position;
            Vector3 forward = hand.forward;
            Gizmos.DrawRay(h, forward);
            h.x = -h.x;
            Gizmos.DrawRay(h, forward);
        }
#endif
    }
}
