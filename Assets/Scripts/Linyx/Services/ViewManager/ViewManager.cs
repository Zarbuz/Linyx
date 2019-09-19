using Linyx.Views;
using System.Collections.Generic;
using Linyx.Controllers.Project;
using UnityEngine;

namespace Linyx.Services.ViewManager
{
    public sealed class ViewManager : MonoBehaviour, IViewManager
    {
        [Inject] public FullCleanupSignal FullCleanupSignal { get; set; }

        private Dictionary<string, BaseView> _views = new Dictionary<string, BaseView>();
        public void Initialize()
        {
            BaseView[] views = FindObjectsOfType<BaseView>();
            foreach (BaseView view in views)
            {
                _views.Add(view.GetType().Name, view);
            }
            FullCleanupSignal.AddListener(OnFullCleanup);
        }

        public void ChangeView(string view)
        {
            switch (view)
            {
                case "RenderView":
                    _views["BrushView"].Hide();
                    _views["RenderView"].Show();
                    break;
                case "BrushView":
                    _views["BrushView"].Show();
                    _views["RenderView"].Hide();
                    break;
                case "EditView":
                    _views["EditView"].Show();
                    break;
                case "HideEdit":
                    _views["EditView"].Hide();
                    break;
                default:
                    Debug.Log("[ViewManager] View not found: " + view);
                    break;
            }

        }

        public void ToggleView(string view)
        {
            RectTransform rtBrush = _views["BrushView"].GetComponent<RectTransform>();
            RectTransform rtRender = _views["RenderView"].GetComponent<RectTransform>();
            RectTransform rtEdit = _views["EditView"].GetComponent<RectTransform>();
            RectTransform rtProject = _views["ProjectView"].GetComponent<RectTransform>();
            RectTransform rtUtils = _views["UtilsView"].GetComponent<RectTransform>();

            switch (view)
            {
                case "BrushView":
                    Vector2 sizeDelta = rtBrush.sizeDelta;
                    sizeDelta = _views[view].IsPanelOpen ? new Vector2(180, sizeDelta.y) : new Vector2(20, sizeDelta.y);
                    rtBrush.sizeDelta = sizeDelta;
                    rtUtils.anchoredPosition = _views[view].IsPanelOpen ? new Vector2(180, rtUtils.anchoredPosition.y) : new Vector2(20, rtUtils.anchoredPosition.y);

                    break;
                case "RenderView":
                    Vector2 delta = rtRender.sizeDelta;
                    delta = _views[view].IsPanelOpen ? new Vector2(180, delta.y) : new Vector2(20, delta.y);
                    rtRender.sizeDelta = delta;
                    rtUtils.anchoredPosition = _views[view].IsPanelOpen ? new Vector2(180, rtUtils.anchoredPosition.y) : new Vector2(20, rtUtils.anchoredPosition.y);
                    break;
                case "ProjectView":
                    if (_views[view].IsPanelOpen)
                    {
                        rtProject.sizeDelta = new Vector2(180, rtProject.sizeDelta.y);
                        rtEdit.anchoredPosition = new Vector2(-180, rtEdit.anchoredPosition.y);
                    }
                    else
                    {
                        rtProject.sizeDelta = new Vector2(20, rtProject.sizeDelta.y);
                        rtEdit.anchoredPosition = new Vector2(-20, rtEdit.anchoredPosition.y);
                    }
                    break;
                case "EditView":
                    Vector2 sizeDelta1 = rtEdit.sizeDelta;
                    sizeDelta1 = _views[view].IsPanelOpen ? new Vector2(180, sizeDelta1.y) : new Vector2(20, sizeDelta1.y);
                    rtEdit.sizeDelta = sizeDelta1;
                    break;
            }
        }

        private void OnFullCleanup()
        {
            ChangeView("HideEdit");
        }
    }
}
