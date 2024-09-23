using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace OriNoco
{
    [ExecuteInEditMode]
    public class ViewportAdjustment : MonoBehaviour
    {
        public Camera viewportCamera;

        public RectTransform canvas;
        public RectTransform sidebarTransform;
        public RectTransform leftTransform;
        public RectTransform topTransform;
        public RectTransform toolbarTransform;
        public RectTransform bottomTransform;
        public RectTransform rightTransform;

        public Rect currentRect = new Rect();
        public Vector2 screenSize;

        private void Update()
        {
            UpdateScreens();
        }

        public void UpdateScreens()
        {
            screenSize = canvas.sizeDelta;

            currentRect.x = (sidebarTransform.sizeDelta.x + leftTransform.sizeDelta.x) / screenSize.x;
            currentRect.y = bottomTransform.sizeDelta.y / screenSize.y;
            currentRect.width = (screenSize.x - rightTransform.sizeDelta.x) / screenSize.x - currentRect.x;
            currentRect.height = (screenSize.y - (toolbarTransform.sizeDelta.y + topTransform.sizeDelta.y)) / screenSize.y - currentRect.y;

            viewportCamera.rect = currentRect;
        }
    }
}