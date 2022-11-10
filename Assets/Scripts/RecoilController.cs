using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour
{
    private Vector3 targetRotation;
    private Vector3 currentRotation;
    private Coroutine myRoute;

    [Header("Settings")]
    [SerializeField] private float returnSpeed;
    [SerializeField] private float snap;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    //Need a vector storing the players intended rotation
    
    

    // Update is called once per frame
    void Update()
    {
        //Make this a co-routine?
        //Reset to prv
        
        
        //targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        //currentRotation = Vector3.Slerp(currentRotation, targetRotation, snap * Time.fixedDeltaTime);
        //transform.localEulerAngles = currentRotation;
    }

    public void AddRecoil(Vector3 dir)
    {
        targetRotation += dir;
    }
}
