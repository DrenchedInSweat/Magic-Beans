using UnityEngine;

namespace Characters
{
    public class Ant : Enemy
    {
        private const int CheckLines = 8;
        [SerializeField] private float lineLength;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void CheckAroundAnt()
        {
            float rads = 360f / CheckLines * Mathf.Deg2Rad;
            Vector3 transPos = transform.position;
            Vector2 direction = Vector2.up;
            for (int i = 0; i < CheckLines; i++)
            {
                Physics.Raycast(transPos, direction, out RaycastHit hit, lineLength );
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            
        }
    
#endif
    
    }
}
