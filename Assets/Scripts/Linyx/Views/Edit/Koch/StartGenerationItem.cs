using MaterialUI;
using System;
using TMPro;
using UnityEngine;

public class StartGenerationItem : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private MaterialButton _deleteButton;
    [SerializeField] private MaterialCheckbox _outwardsCheckbox;
    [SerializeField] private MaterialSlider _scaleSlider;

    #endregion

    #region Public Attributes
    public string Guid { get; set; }
    public bool IsOutwards { get; set; }
    public float Scale { get; set; }

    #endregion

    #region Private Attributes

    private Action<StartGenerationItem> _onDeleteCallback;
    private Action<StartGenerationItem> _onUpdateCallback;

    #endregion

    public void Initialize(string title, string guid, Action<StartGenerationItem> onDeleteCallback, Action<StartGenerationItem> onUpdateCallback)
    {
        Guid = guid;
        _title.SetText(title);
        _onDeleteCallback = onDeleteCallback;
        _onUpdateCallback = onUpdateCallback;

        _deleteButton.buttonObject.onClick.AddListener(OnDeleteClicked);
        _outwardsCheckbox.toggle.onValueChanged.AddListener(OnOutwardsValueChanged);
        _scaleSlider.slider.onValueChanged.AddListener(OnScaleValueChanged);
    }

    public void SetTitle(string title)
    {
        _title.SetText(title);
    }

    public void SetOutwards(bool value)
    {
        IsOutwards = value;
        _outwardsCheckbox.toggle.isOn = value;
    }
    public void SetScale(float value)
    {
        Scale = value;
        _scaleSlider.slider.value = value;
    }

    private void OnScaleValueChanged(float value)
    {
        Scale = value;
        _onUpdateCallback.Invoke(this);
    }

    private void OnOutwardsValueChanged(bool value)
    {
        IsOutwards = value;
        _onUpdateCallback.Invoke(this);
    }

    private void OnDeleteClicked()
    {
        _onDeleteCallback.Invoke(this);
    }

}
