
using Cirilla;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Cirilla/Broadcast/UIMouseBroadcast", 10)]
public class UIMouseBroadcast : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler
{
	private ObserverManager<MouseUIEvent> observer;
    private void Awake()
    {
		observer = ObserverManager<MouseUIEvent>.instance;
	}
    public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.pointerId == -1)
		{
			observer.Dispatch(MouseUIEvent.ClickLeft, gameObject);
			return;
		}

		if (eventData.pointerId != -2)
			return;

		observer.Dispatch(MouseUIEvent.ClickRight, gameObject);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		observer.Dispatch(MouseUIEvent.Enter, gameObject);
	}

	public void OnPointerExit(PointerEventData eventData) {
		observer.Dispatch(MouseUIEvent.Exit, gameObject);
	}

	public void OnPointerDown(PointerEventData eventData) {
		observer.Dispatch(MouseUIEvent.Down, gameObject);
	}

	public void OnDrag(PointerEventData eventData) {
		observer.Dispatch(MouseUIEvent.Drag, gameObject);
	}

}
