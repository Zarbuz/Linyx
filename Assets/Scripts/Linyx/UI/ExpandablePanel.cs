using Linyx.Utils;
using MaterialUI;
using UnityEngine;
using UnityEngine.UI;

namespace Linyx.UI
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class ExpandablePanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private MaterialButton _toggleButton;

        private VerticalLayoutGroup _verticalLayoutGroup;
        private bool _isPanelOpen = true;

        private void Start()
        {
            _verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
            _toggleButton.buttonObject.onClick.AddListener(OnToggleClicked);
        }

        private void OnToggleClicked()
        {
            _isPanelOpen = !_isPanelOpen;
            _panel.SetActive(_isPanelOpen);
            _toggleButton.ToggleOpenButton(_isPanelOpen);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
        }
    }
}
