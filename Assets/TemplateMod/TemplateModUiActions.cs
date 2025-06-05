using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TemplateMod
{
    // example component to bind UI from mod panel to some script actions
    public class TemplateModUiActions : MonoBehaviour
    {
        // don't forget to attach those to UI elements in Unity inspector!
        [SerializeField] private Toggle templateToggle1;
        [SerializeField] private Image image1;
        [SerializeField] private Button button1;
        [SerializeField] private TMP_InputField input1;
        [SerializeField] private TMP_Text text1;
        [Space] // just for inspector formatting
        [Header("Second Set of UI")] // just for inspector formatting
        [SerializeField] private Toggle templateToggle2;
        [SerializeField] private GameObject imageFrame2;
        [SerializeField] private Button button2;
        [SerializeField] private TMP_InputField input2;
        [SerializeField] private TMP_Text text2;

        
        // Unity method that runs at the initialization of the object - only once
        private void Awake()
        {
            // attaching actions on ui elements click or modifying text
            templateToggle1.onValueChanged.AddListener(SwitchImage);
            button1.onClick.AddListener(SetRandomColor);
            input1.onValueChanged.AddListener(SetTextValueOnType);

            templateToggle2.onValueChanged.AddListener(SwitchFrameImage);
            button2.onClick.AddListener(CleanAllForms);
            input2.onDeselect.AddListener(SetTextValueOnDeselect);  // notice it is OnDeselect event, not onValueChanged
        }

        // Unity method that runs each time the object becomes enabled, good for restoring UI content from settings
        private void OnEnable()
        {
            // Calling the same method that one of the buttons use
            CleanAllForms();
        }

        // This method runs when the object becomes deactivated, in this case once we close the settings panel
        // good for saving data after edits
        private void OnDisable()
        {
            
        }

        private void SwitchImage(bool value)
        {
            image1.enabled = value;
        }

        private void SetRandomColor()
        {
            image1.color = Random.ColorHSV();
        }

        private void SetTextValueOnType(string value)
        {
            text1.text = value;
        }

        private void SwitchFrameImage(bool value)
        {
            imageFrame2.SetActive(value);
        }

        private void CleanAllForms()
        {
            // Refreshing UI values in case those were changed from other sources (SetWithoutNotify changes values without calling previously attached actions)
            templateToggle1.SetIsOnWithoutNotify(false);
            input1.SetTextWithoutNotify(string.Empty);
            text1.text = "I copy inputField above";
            
            templateToggle2.SetIsOnWithoutNotify(false);
            input2.SetTextWithoutNotify(string.Empty);
            text2.text = "I copy inputField above, but only on deselect";
        }

        private void SetTextValueOnDeselect(string value)
        {
            text2.text = value;
        }
    }
}
