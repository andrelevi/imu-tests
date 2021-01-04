using DG.Tweening;
using UnityEngine;

namespace IMUTest.Scripts
{
    public class MenuItem : MonoBehaviour
    {
        private Tween _scaleTween;
        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }
        
        public void Spawn(Vector3 position, Quaternion rotation)
        {
            gameObject.SetActive(true);
            
            transform.SetPositionAndRotation(position, rotation);
            
            _scaleTween?.Kill();
            transform.localScale = Vector3.zero;
            _scaleTween = transform.DOScale(_originalScale, 0.2f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(Vector3.zero, 0.1f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    
                    // Position the item far away when it is not active, so that
                    // it doesn't interrupt the distance checks when attempting to
                    // spawn in new items.
                    transform.position = Vector3.one * 10000f;
                });
        }
    }
}
