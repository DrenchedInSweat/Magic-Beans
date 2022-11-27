using Characters;
using UnityEngine;
using UnityEngine.VFX;

//This code actively makes start up take longer
namespace Weapons
{
    public class LaserWeaponVFX : MonoBehaviour
    {
        private Color col;
        private VisualEffect effect;
    
        private readonly int lengthID = Shader.PropertyToID("Len");
        private readonly int startID = Shader.PropertyToID("StartFire");
        private readonly int stopID = Shader.PropertyToID("StopFire");
        private readonly int colorID = Shader.PropertyToID("Color");

        private LaserWeaponVFX child;
        private GameObject childObj;

        
        LaserWeapon.ApplyToCharacter del;

        // Start is called before the first frame update
        private void Awake()
        {
            effect = GetComponent<VisualEffect>();
            //lightTransform = transform.GetChild(1);
            //myLight = lightTransform.GetComponent<Light>();
            effect.SetVector4(colorID, col);
            //myLight.color = col;
        
            //effect.SetFloat(lengthID , 100);
        
            //myLight.intensity = intensityMin;
            if (transform.childCount != 0)
            { 
                childObj = transform.GetChild(0).gameObject;
                child = childObj.GetComponent<LaserWeaponVFX>();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            //Start beam Color
            Vector3 direction = transform.forward;
            if (!Physics.Raycast(transform.position, direction, out RaycastHit hit, 400)) return;
            //print("Hit Target");
            float dist = hit.distance / 2;
            effect.SetFloat(lengthID, dist);
            //lightTransform.localPosition = new Vector3(0,0,dist);
            //myLight.
            if (childObj && hit.transform.CompareTag("Reflective"))
            {
                if (!childObj.activeSelf)
                {
                    childObj.SetActive(true);
                    child.Activate(col, del);
                }
                childObj.transform.position = hit.point;
                childObj.transform.forward = Vector3.Reflect(direction, hit.normal);
                return;
            }
        
            if (childObj)
            {
                childObj.SetActive(false);
            }

            if (hit.transform.TryGetComponent(out Character c))
            {
                del.Invoke(c);
            }
        }

        public void Activate(Color c, LaserWeapon.ApplyToCharacter myDel)
        {
            del = myDel;
            col = c;
            effect.SendEvent(startID);
            effect.SetVector4(colorID, col);
        }

        public void DeActivate()
        {
            effect.SendEvent(stopID);
            effect.Stop();
        }
    }
}
