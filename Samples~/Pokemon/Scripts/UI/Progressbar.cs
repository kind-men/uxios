using UnityEngine;
using UnityEngine.UI;

namespace KindMen.Uxios.UI
{
    [RequireComponent(typeof(Slider))]
    public class Progressbar : MonoBehaviour
    {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            slider.value = 0f;
        }

        void Update()
        {
            if (slider.value >= 0.95f)
            {
                slider.value = 0f;
            }
            slider.value += 0.1f;
        }
    }
}