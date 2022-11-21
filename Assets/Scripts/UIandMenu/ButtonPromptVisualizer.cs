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
    

    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        textBox.text = InputControlPath.ToHumanReadableString(actionToReference.action.
            bindings[actionToReference.action.GetBindingIndexForControl(actionToReference.action.controls[0])].
            effectivePath,InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}
