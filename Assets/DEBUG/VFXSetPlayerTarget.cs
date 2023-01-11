using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
    
[VFXBinder("Transform/Position")]
public class VFXSetPlayerTarget : VFXBinderBase
{
    // VFXPropertyBinding attributes enables the use of a specific
    // property drawer that populates the VisualEffect properties of a
    // certain type.
    public ExposedProperty Target;


    // The IsValid method need to perform the checks and return if the binding
    // can be achieved.
    public override bool IsValid(VisualEffect component)
    {
        return component.HasFloat(Target);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        component.SetVector3(Target, GameManager.Instance.Player.transform.position);
    }
}
