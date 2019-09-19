using Linyx.Models;
using MaterialUI;
using System;
using Linyx.Models.Line;
using TMPro;
using UnityEngine;

namespace Linyx.Views.Project.Items
{
    public class ProjectItem : MonoBehaviour
    {
        #region UI

        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private MaterialButton _itemButton;
        [SerializeField] private MaterialButton _copyButton;
        [SerializeField] private MaterialButton _deleteButton;
        [SerializeField] private GameObject _backgroud;

        [SerializeField] private Color _selectedCopyColor;
        [SerializeField] private Color _unselectedCopyColor;

        #endregion
        public bool IsSelected { get; set; }

        public ILineModel LineModel { get; private set; }

        #region Private Attributes

        private Action<ILineModel> _onSelectAction;
        private Action<ILineModel> _onUnselectAction;
        private Action<ILineModel> _onDeleteAction;
        private Action<ILineModel, bool> _onCopyAction;

        private bool _isCopySelected;

        #endregion
        public void Initialize(ILineModel lineModel, Action<ILineModel> onSelectAction, Action<ILineModel> onUnselectAction, Action<ILineModel> onDeleteAction, Action<ILineModel, bool> onCopyAction)
        {
            LineModel = lineModel;
            _onSelectAction = onSelectAction;
            _onUnselectAction = onUnselectAction;
            _onDeleteAction = onDeleteAction;
            _onCopyAction = onCopyAction;

            _title.SetText(lineModel.DisplayName);

            _deleteButton.gameObject.SetActive(false);
            _copyButton.gameObject.SetActive(false);

            _itemButton.buttonObject.onClick.AddListener(OnButtonSelected);
            _deleteButton.buttonObject.onClick.AddListener(OnDeleteClicked);
            _copyButton.buttonObject.onClick.AddListener(OnCopyClicked);
        }

        public void UpdateTitle(string title)
        {
            _title.SetText(title);
        }

        public void UpdateLine(ILineModel line)
        {
            LineModel = line;
        }

        public void Select()
        {
            IsSelected = true;
            _deleteButton.gameObject.SetActive(true);
            _copyButton.gameObject.SetActive(true);
            _backgroud.SetActive(true);
        }

        public void UnSelect()
        {
            IsSelected = false;
            _backgroud.SetActive(false);
            _deleteButton.gameObject.SetActive(false);
            if (!_isCopySelected)
            {
                _copyButton.gameObject.SetActive(false);
                _copyButton.iconColor = _unselectedCopyColor;
            }
        }

        public void DeselectCopy()
        {
            _isCopySelected = false;
            _copyButton.gameObject.SetActive(false);
            _copyButton.iconColor = _unselectedCopyColor;
        }

        private void OnButtonSelected()
        {
            if (!IsSelected)
            {
                Select();
                _onSelectAction?.Invoke(LineModel);
            }
            else
            {
                UnSelect();
                _onUnselectAction?.Invoke(LineModel);
            }
        }

        private void OnDeleteClicked()
        {
            _onDeleteAction?.Invoke(LineModel);
            Destroy(gameObject, 0.1f);
        }

        private void OnCopyClicked()
        {
            _isCopySelected = !_isCopySelected;
            _copyButton.iconColor = _isCopySelected ? _selectedCopyColor : _unselectedCopyColor;
            _onCopyAction?.Invoke(LineModel, _isCopySelected);
            if (!IsSelected && !_isCopySelected)
                _copyButton.gameObject.SetActive(false);
        }
    }
}
