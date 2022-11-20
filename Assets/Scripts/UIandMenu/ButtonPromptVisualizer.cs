using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ButtonPromptVisualizer : MonoBehaviour
{
    [SerializeField] TMP_Text textBox;
    [SerializeField] InputActionReference actionToReference;

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateText()
    {
        textBox.text = InputControlPath.ToHumanReadableString(actionToReference.action.
            bindings[actionToReference.action.GetBindingIndexForControl(actionToReference.action.controls[0])].
            effectivePath,InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}
