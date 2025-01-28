using Entity.Player.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace Entity.Player.UI
{
    public class Pointer : MonoBehaviour
    {
        [Header("Entities")]
        [SerializeField] private Image image;
        [SerializeField] private PlayerManager player;
        [Header("Params")]
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color detectedColor;
        [SerializeField] private float defaultScale;
        [SerializeField] private float detectedScale;

        private PlayerInteractor _interactor;
        // private bool _currentState = false;

        private void Awake()
        {
            image.color = defaultColor;
            image.rectTransform.localScale = Vector3.one * defaultScale;
        }

        private void Start()
        {
            _interactor = player.Interactor;
            
            _interactor.OnDetectSmth += ChangeColorDueDetection;
        }

        private void ChangeColorDueDetection(bool isDetected)
        {
            if (isDetected)
            {
                image.color = detectedColor;
                image.rectTransform.localScale = Vector3.one * detectedScale;
            }
            else
            {
                image.color = defaultColor;
                image.rectTransform.localScale = Vector3.one * defaultScale;
            }
        }
    }
}