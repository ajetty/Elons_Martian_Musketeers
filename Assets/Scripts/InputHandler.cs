using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class InputHandler : MonoBehaviour, IPointerClickHandler
    {
        private void OnEnable()
        {
        
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + " handled click of " + eventData.rawPointerPress.name);
        }
    }
}
