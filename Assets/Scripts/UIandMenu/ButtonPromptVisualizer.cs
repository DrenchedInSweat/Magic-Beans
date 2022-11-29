using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ButtonPromptVisualizer : MonoBehaviour
{
    [SerializeField] InputActionReference actionToReference;
    [SerializeField] TMP_Text textBox;
    private PlayerInput controls;


    // Start is called before the first frame update
    void Start()
    {
        controls = new PlayerInput();
        SetText();
        controls.onControlsChanged += UpdateText;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetText()
    {
        textBox.text = InputControlPath.ToHumanReadableString(actionToReference.action.
            bindings[actionToReference.action.GetBindingIndexForControl(actionToReference.action.controls[0])].
            effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

    }
    
    void UpdateText(PlayerInput input)
    {
        if (input.currentControlScheme.Equals("Controller"))
        {
            textBox.text = InputControlPath.ToHumanReadableString(actionToReference.action.
            bindings[actionToReference.action.GetBindingIndexForControl(actionToReference.action.controls[1])].
            effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        else
        {
            textBox.text = InputControlPath.ToHumanReadableString(actionToReference.action.
            bindings[actionToReference.action.GetBindingIndexForControl(actionToReference.action.controls[0])].
            effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        
    }


}
