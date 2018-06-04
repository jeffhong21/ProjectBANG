namespace Bang
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PauseManager : MonoBehaviour
    {

        private Canvas _canvas;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Update()
        {
            if(InputManager.ESC)
            {
                _canvas.enabled = !_canvas.enabled;
                Pause();
            }
        }

        public void Pause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;

        }


    }
}


