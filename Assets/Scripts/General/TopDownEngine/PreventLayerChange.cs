using UnityEngine;

namespace KitTraden.Abeyance.TopDownEngine
{
    public class PreventLayerChange : MonoBehaviour
    {
        private int initialLayer;

        void Start()
        {
            initialLayer = gameObject.layer;
        }

        void Update()
        {
            ForceLayerToStayTheSame();
        }

        void LateUpdate()
        {
            ForceLayerToStayTheSame();
        }

        private void ForceLayerToStayTheSame()
        {
            if (gameObject.layer != initialLayer)
            {
                gameObject.layer = initialLayer;
            }
        }
    }
}