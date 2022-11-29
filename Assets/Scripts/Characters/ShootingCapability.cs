using Characters.BaseStats;
using UnityEngine;
using Weapons;

namespace Characters
{
    public class ShootingCapability : MonoBehaviour
    {
        [Header("Shooting")] [SerializeField] private Transform hand;
        public Weapon[] Weapons { get; private set; }

        public void Init(Character owner)
        {
            int num = hand.childCount;
            Weapons = new Weapon[num];
            for (int i = 0; i < num; ++i)
            {
                print("adding weapon: " + i + " --> " + hand.GetChild(i).gameObject.name);
                Weapons[i] = hand.GetChild(i).GetComponent<Weapon>();
                Weapons[i].Init(owner);
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
