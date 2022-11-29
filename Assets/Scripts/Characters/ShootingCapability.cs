using Characters.BaseStats;
using UnityEngine;
using Weapons;

namespace Characters
{
    public class ShootingCapability : MonoBehaviour
    {
        [Header("Shooting")] [SerializeField] private Transform hand;
        public Weapon[] Weapons { get; private set; }
        public int[] Hashes { get; private set; }
        public int Len { get; private set; }

        public void Init(Character owner)
        {
            Len= hand.childCount;
            print("Changing len: " + Len);
            Weapons = new Weapon[Len];
            Hashes = new int[Len];
            for (int i = 0; i < Len; ++i)
            {
                print("adding weapon: " + i + " --> " + hand.GetChild(i).gameObject.name);
                Weapons[i] = hand.GetChild(i).GetComponent<Weapon>();
                Weapons[i].Init(owner);
                Hashes[i] = Animator.StringToHash(Weapons[i].GetStats<WeaponStatsSo>().AnimatorHash);
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
