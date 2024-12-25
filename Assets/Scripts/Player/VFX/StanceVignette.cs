using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Player.VFX
{
    /// <summary>
    /// Затемняет экран во время приседа или подката
    /// </summary>
    public class StanceVignette : MonoBehaviour
    {
        [Header("Vignette params")]
        [SerializeField] private float min = 0.1f;
        [SerializeField] private float max = 0.35f;
        [SerializeField] private float response = 10f;
        
        private VolumeProfile _profile;
        private Vignette _vignette;

        public void Init(VolumeProfile profile)
        {
            _profile = profile;

            if (!profile.TryGet(out _vignette))
            {
                _vignette = profile.Add<Vignette>();
            }
            _vignette.active = true;
            _vignette.intensity.Override(min);
        }

        public void UpdateVignette(float deltaTime, Stance stance)
        {
            var targetIntensity = stance is Stance.Walk or Stance.Sprint ? min : max;
            _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, targetIntensity,
                1f - Mathf.Exp(-response * deltaTime));
        }
    }
}