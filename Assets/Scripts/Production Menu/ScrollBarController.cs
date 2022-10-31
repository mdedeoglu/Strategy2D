using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Strategy2D
{
    public class ScrollBarController : MonoBehaviour, IScrollHandler
    {
        public int TotalCount = -1;             // Total count, negative means INFINITE mode

        private Pool _pool;                     // Pool for Production Menu
        private float _threshold = 1;
        private int _firstIndex = 0;            // Index of first child of ScrollBar Content
        private int _lastIndex = 0;             // Index of last child of ScrollBar Content
        private bool _onTop = true;             // When index is zero in [0, positive infinite] interval

        [SerializeField]
        protected int scrollSpeed = 60;         // Scrolling speed
        [SerializeField]
        protected RectTransform content;       // Rectangle of Content object of ScrollBar

        private RectTransform _viewRect { get { return (RectTransform)transform; } }  // Bound of children of Content object
        private int contentConstraintCount = 2; // Columns count for grid
        private Vector3[] _corners = new Vector3[4];
        private ProductionMenu _productionMenu;


        // Initializing the scrollBar content
        public void CreateScrollBar(Pool pool, ProductionMenu productionMenu)
        {
            _productionMenu = productionMenu;
            _pool = pool;
            _pool.InitPool(transform);

            // Filling ScrollBar with poolObject
            float sizeFilled = 0;
            while (sizeFilled < _viewRect.rect.size.y)
                sizeFilled += NewItemAtEnd();

            content.anchoredPosition -= Vector2.up * scrollSpeed;
        }

        // When Scrolling
        public void OnScroll(PointerEventData data)
        {
            // Controlling very top of data (0 point)
            if (!_onTop || data.scrollDelta.y < 0)
                content.anchoredPosition -= (data.scrollDelta * scrollSpeed);

            // Updating Content of ScrollBar, if it is possible
            _onTop = false;
            UpdateItems();
        }

        // Updates Content of ScrollBar
        private void UpdateItems()
        {
            Bounds contentBounds = GetBounds();
            Bounds viewBounds = new Bounds(_viewRect.rect.center, _viewRect.rect.size);

            if (viewBounds.min.y < contentBounds.min.y)
            {
                float size = NewItemAtEnd();
                float totalSize = size;
                while (size > 0 && viewBounds.min.y < contentBounds.min.y - totalSize)
                {
                    size = NewItemAtEnd();
                    totalSize += size;
                }
            }

            if (viewBounds.max.y > contentBounds.max.y)
            {
                float size = NewItemAtStart();
                float totalSize = size;
                while (size > 0 && viewBounds.max.y > contentBounds.max.y + totalSize)
                {
                    size = NewItemAtStart();
                    totalSize += size;
                }
            }

            if (viewBounds.min.y > contentBounds.min.y + _threshold)
            {
                float size = DeleteItemAtEnd();
                float totalSize = size;
                while (size > 0 && viewBounds.min.y > contentBounds.min.y + _threshold + totalSize)
                {
                    size = DeleteItemAtEnd();
                    totalSize += size;
                }
            }

            if (viewBounds.max.y < contentBounds.max.y - _threshold)
            {
                float size = DeleteItemAtStart();
                float totalSize = size;
                while (size > 0 && viewBounds.max.y < contentBounds.max.y - _threshold - totalSize)
                {
                    size = DeleteItemAtStart();
                    totalSize += size;
                }
            }
        }

        // Gets bound of all children of Content
        private Bounds GetBounds()
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = _viewRect.worldToLocalMatrix;
            content.GetWorldCorners(_corners);
            for (int i = 0; i < 4; i++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(_corners[i]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        // Adds two cells on the top,
        private float NewItemAtStart()
        {
            // If very top of data
            if (_firstIndex - contentConstraintCount < 0)
            {
                _onTop = true;
                return 0;
            }

            // Adding cells
            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                _firstIndex--;
                RectTransform newItem = BringCell(_firstIndex);
                newItem.SetAsFirstSibling();
                size = LayoutUtility.GetPreferredHeight(newItem);
            }

            _threshold = size * 1.5f;
            content.anchoredPosition += new Vector2(0, size + 5);
            return size;
        }

        // Deletes two cell from the top 
        private float DeleteItemAtStart()
        {
            // If data count limited
            if ((TotalCount >= 0 && _lastIndex >= TotalCount - 1) || content.childCount == 0)
                return 0;

            // Adding cells
            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                RectTransform oldItem = content.GetChild(0) as RectTransform;
                size = LayoutUtility.GetPreferredHeight(oldItem);
                _pool.ReturnObjectToPool(oldItem);
                _firstIndex++;
            }

            content.anchoredPosition -= new Vector2(0, size + 5);
            return size;
        }

        // Adds two cells on the end
        private float NewItemAtEnd()
        {
            // If data count limited
            if (TotalCount >= 0 && _lastIndex >= TotalCount)
                return 0;

            // Adding cells
            float size = 0;
            int count = contentConstraintCount - (content.childCount % contentConstraintCount);
            for (int i = 0; i < count; i++)
            {
                RectTransform newItem = BringCell(_lastIndex);
                size = LayoutUtility.GetPreferredHeight(newItem);
                _lastIndex++;
                if (TotalCount >= 0 && _lastIndex >= TotalCount)
                    break;
            }

            _threshold = size * 1.5f;
            return size;
        }

        // Deletes two cells from the end
        private float DeleteItemAtEnd()
        {
            // If data count limited
            if ((TotalCount >= 0 && _firstIndex < contentConstraintCount) || content.childCount == 0)
                return 0;

            // Adding cells
            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                RectTransform oldItem = content.GetChild(content.childCount - 1) as RectTransform;
                size = LayoutUtility.GetPreferredHeight(oldItem);
                _pool.ReturnObjectToPool(oldItem);
                _lastIndex--;
                if (_lastIndex % contentConstraintCount == 0 || content.childCount == 0)
                    break;  // Just delete the whole row
            }

            return size;
        }

        // Brings a cell from the pool
        private RectTransform BringCell(int itemIndex)
        {
            GameObject nextItem = _pool.PopObject();
            nextItem.transform.SetParent(content, false);
            nextItem.SetActive(true);
            nextItem.GetComponent<ProductionMenuCell>().CellIndexing(itemIndex, _productionMenu);
            return nextItem.transform as RectTransform;
        }
    }
}