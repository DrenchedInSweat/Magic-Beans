using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class temp : MonoBehaviour
{
    [SerializeField] private GameObject repeatObj;
    [SerializeField] private int repeatTimes;
    [SerializeField] private bool start;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            Vector3 pos = transform.position;
            float dist = 0;
            for (int i = 1; i < repeatTimes; i++)
            {
                if ((i & 1) == 1) // Let's say I is 6 110 & 001 ==> 0 EVEN but, if I is 7, 111 & 001 ==> 1
                {
                    dist += 1; // Only add ever other time.
                }
                dist = -dist; // Neg then pos then neg then ... 
                Instantiate(repeatObj, pos + new Vector3(dist, dist, 0), Quaternion.identity, transform);
            }
            
            start = false;
        }
    }
}
