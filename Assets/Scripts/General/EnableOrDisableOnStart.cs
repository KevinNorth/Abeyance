using Unity;
using UnityEngine;

namespace KitTraden.Abeyance.General
{
    public class EnableOrDisableOnStart : MonoBehaviour
    {
        public enum EnableOrDisable
        {
            Enable,
            Disable
        }

        public EnableOrDisable enableOrDisable;

        void Start()
        {
            gameObject.SetActive(enableOrDisable == EnableOrDisable.Enable);
        }
    }
}