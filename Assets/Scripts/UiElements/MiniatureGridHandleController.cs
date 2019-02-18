using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UiElements
{
    public class MiniatureGridHandleController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float _dragSpeed = 3f;
        [SerializeField] private float _adjustTime = 0.4f;
        [SerializeField] private float _threshold = 0.3f;
        [SerializeField] private RectTransform _miniatureGridRectTransform;
        private Vector2 _boundaries;
        private bool _isHidden = false;
        
        private void Start()
        {
            _boundaries = new Vector2(transform.parent.localPosition.y, transform.parent.localPosition.y + 160);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.parent.DOKill();
        }

        public void OnDrag(PointerEventData eventData)
        {
            float yPosition = transform.parent.localPosition.y + eventData.delta.y * _dragSpeed;

            yPosition = Mathf.Max(yPosition, _boundaries.x);
            yPosition = Mathf.Min(yPosition, _boundaries.y);
            
            transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, yPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            float threshold = _miniatureGridRectTransform.rect.size.y * _threshold;
            bool hideMiniatureGrid;

            if (_isHidden)
            {
                hideMiniatureGrid = transform.parent.localPosition.y > _boundaries.y - threshold;
            }
            else
            {
                hideMiniatureGrid = transform.parent.localPosition.y > threshold + _boundaries.x;
            }
            

            if (hideMiniatureGrid)
            {
                transform.parent.DOLocalMoveY(_boundaries.y, _adjustTime).OnComplete(() => _isHidden = true);
            }
            else
            {
                transform.parent.DOLocalMoveY(_boundaries.x, _adjustTime).OnComplete(() => _isHidden = false);
            }
        }
    }
}