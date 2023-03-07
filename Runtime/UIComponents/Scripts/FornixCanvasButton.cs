using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class FornixCanvasButton : SerializedMonoBehaviour
{
    // This script is made to make it easier to style the buttons when in the Editor (can also be used in runtime if ever needed). The only functionality that is meant to be used in runtime is the invoking pressed event and toggle button //

    // Toggle button settings
    [OnValueChanged("IsToggleButtonChanged")]
    [Tooltip("Is this a Toggle Button/ checkbox?")][PropertyOrder(-15)][SerializeField] private bool ToggleButton = false;
    [OnValueChanged("UpdateToggleVisual")]
    [ShowIf("ToggleButton")][PropertyOrder(-15)][InfoBox("!This mode is meant for toggle button component prefab, and should not be enabled on regular buttons")][SerializeField] private bool isToggleActive = false;

    // Deactivate button settings
    [OnValueChanged("UpdateActiveState")]
    [PropertyOrder(-15)][SerializeField][Tooltip("When set to deactivated the button can not be pressed. This can also be set in code with the DeactivateButton/Activate button function")] private bool IsDeactivated = false; [Space]

    // Button settings
    [OnValueChanged("SetScale")][SerializeField] private float Width = 150;
    [OnValueChanged("SetScale")][SerializeField] private float Height = 50;
    [Tooltip("!! The Min Position on PhysicsKinematicButton needs to be set to Depth/2 manually")]
    [OnValueChanged("SetScale")][SerializeField] private float Depth = 20; [Space]

    [OnValueChanged("SetScale")][SerializeField][Range(0.0f, 20.0f)] private float OutlineThickness = 2.5f;
    [OnValueChanged("SetRadius")][SerializeField][Range(0.0f, 60.0f)] private float BorderRadius = 18;
    [OnValueChanged("SetRadius")][SerializeField][Range(-1f, 3.0f)] private float InnerRadiusOffset = 0.1f;

    [FoldoutGroup("Objects")][SerializeField] private RectTransform ButtonFrontVisual; // Rect transform that holds all visuals
    [FoldoutGroup("Objects")][SerializeField] private RectTransform BackImage, BorderImage, HighlightImage, FrontImage; // Images for button visuals
    [FoldoutGroup("Objects")][SerializeField] private RectTransform DisabledImage; // Images for when button is disabled
    [FoldoutGroup("Objects")][SerializeField] private BoxCollider ColliderBox;
    [FoldoutGroup("Objects")][SerializeField] private TMP_Text ButtonText;
    [ShowIf("ToggleButton")][FoldoutGroup("Objects")][SerializeField] private RectTransform ToggleCheckmarkImage;

    [PropertyOrder(10)][SerializeField] private UnityEvent _OnButtonPressed;
    [PropertyOrder(10)][SerializeField] private UnityEvent _OnButtonReleased;
    [PropertyOrder(10)][ShowIf("ToggleButton")][SerializeField] private ToggleButtonEvent _OnToggleChanged;
    //______________________________________________________________________//

    // Set custom size on the button 
    [PropertyOrder(5)]
    [Button(SdfIconType.TextareaResize, "Set size to width & height"), PropertyTooltip("Set button size (sprites and colliders) to width and height)")]
    public void SetScale()
    {
        UpdateScale(Width, Height);
    }


    // Match button scale with UI rules(rect transform size)
    [PropertyOrder(5)]
    [Button(SdfIconType.BoundingBoxCircles, "Set size to Rect Transform"), PropertyTooltip("Scale button to match the Rect Transform size. Usefull when the parent component should control the size of the button, such as canvas groups")]
    public void MatchScale()
    {
        float w = GetComponent<RectTransform>().rect.width;
        float h = GetComponent<RectTransform>().rect.height;
        UpdateScale(w, h);
    }


    // Set rect transform size to width and height 
    [PropertyOrder(5)]
    [Button(SdfIconType.ArrowsFullscreen, "Set Rect Transform to size"), PropertyTooltip("Set Rect Transform size to width and height")]
    public void SetRectTransform()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Width, Height);
    }


    // Set border radius on all sprites used in button
    public void SetRadius()
    {
        float radius = BorderRadius;
        FrontImage.GetComponent<Image>().pixelsPerUnitMultiplier = radius + (OutlineThickness / 2 * InnerRadiusOffset);
        BackImage.GetComponent<Image>().pixelsPerUnitMultiplier = radius;
        BorderImage.GetComponent<Image>().pixelsPerUnitMultiplier = radius;
        HighlightImage.GetComponent<Image>().pixelsPerUnitMultiplier = radius;
        DisabledImage.GetComponent<Image>().pixelsPerUnitMultiplier = radius;
    }


    // Shortcut for setting text on button
    [PropertyOrder(6)]
    [Button(SdfIconType.CardText, "Set Text"), PropertyTooltip("Change the button text, can also be done manually")]

    public void SetText(string text)
    {
        ButtonText.text = text;
#if (UNITY_EDITOR)
        EditorUtility.SetDirty(ButtonText);
#endif
        //EditorUtility.SetDirty(ButtonText);
    }


    // Set/Update new scale values //
    public void UpdateScale(float w, float h)
    {
        // set width and height on button visuals
        ButtonFrontVisual.sizeDelta = new Vector2(w, h);
        BackImage.sizeDelta = new Vector2(w, h);
        FrontImage.offsetMin = new Vector2(OutlineThickness, OutlineThickness);
        FrontImage.offsetMax = new Vector2(-OutlineThickness, -OutlineThickness);

        // Move visual pos to the begining/edge of the box
        ButtonFrontVisual.transform.localPosition = new Vector3(0, 0, -Depth / 2);

        // rescale the collider to match new size
        ColliderBox.size = new Vector3(w, h, Depth);
        // ColliderBox.GetComponent<PhysicKinematicButton>().Min
    }


    // -- Code used in runtime -- //
    // Set active state on the button
    private void UpdateActiveState()
    {
        DisabledImage.gameObject.SetActive(IsDeactivated);
        ColliderBox.gameObject.GetComponent<PhysicKinematicButton>().enabled = !IsDeactivated;
    }
    public void DeactivateButton()
    {
        IsDeactivated = true;
        UpdateActiveState();
    }
    public void ActiveButton()
    {
        IsDeactivated = true;
        UpdateActiveState();
    }


    // EVENTS // Gets excecuted from the HP fornix button child. This is only added to make the events appear on the top parent of the button, for easier access
    public void OnPressed()
    {
        _OnButtonPressed.Invoke();
        if (ToggleButton) TogglePressed();
    }
    public void OnReleased()
    {
        _OnButtonReleased.Invoke();
    }


    // THIS IS ONLY FOR TOGGLE BUTTON //
    public void TogglePressed()
    {
        isToggleActive = !isToggleActive;
        ToggleCheckmarkImage.gameObject.SetActive(isToggleActive);
        _OnToggleChanged.Invoke(isToggleActive);
    }

    // Update the checkmark when changed in Editor
    private void UpdateToggleVisual()
    {
        ToggleCheckmarkImage.gameObject.SetActive(isToggleActive);
    }

    // remove checkmark when ToggleMode is disabled
    private void IsToggleButtonChanged()
    {
        ToggleCheckmarkImage.gameObject.SetActive(false);
        if (ToggleButton) UpdateToggleVisual();

    }
}

// Toggle button event with isToggleActive bool
[System.Serializable]
public class ToggleButtonEvent : UnityEvent<bool> { }
