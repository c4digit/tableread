using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class clickInputField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler, IBeginDragHandler, IEndDragHandler {

	private bool	dragging;
	private ScrollRect	_scrollrect;
	private	InputField	_self;
	public bool	interactable = true;
	void Awake() {
		dragging = false;
		interactable = true;
		_scrollrect = GetComponentInParent<ScrollRect> ();
		_self = GetComponent<InputField> ();
		//resetInputfield ();
	}

	void Start() {
		_self.interactable = false;
	}

	public void OnPointerDown (PointerEventData eventData) {
		// Do action
	//	interactable = _self.interactable;
		_self.interactable = false;
	}

	public void OnPointerUp (PointerEventData eventData) {
		// Do action
		//if (!dragging) {
		//	Debug.Log ("pointer up");
		if (!dragging)
			_self.interactable = interactable;
		dragging = false;
		//}
	}

	public void OnBeginDrag (PointerEventData eventData) {
		// Do action
	//	Debug.Log("started drag");
		dragging = true;
		_scrollrect.SendMessage("OnBeginDrag", eventData);
	}

	public void OnEndDrag (PointerEventData eventData) {
		// Do action
	//	Debug.Log("end drag");
	//	dragging = false;
		_scrollrect.SendMessage("OnEndDrag", eventData);
	//		_self.interactable = false;
	//	_self.interactable = interactable;
	}

	public void OnDrag (PointerEventData eventData) {
		// Do action
		_scrollrect.SendMessage("OnDrag", eventData);
	}

	//public void turnOffInteractable() {
	//	Invoke ("resetInputfield", 0.5f);
	//}

	public void resetInputfield() {
	//	if (_self.interactable)
		_self.interactable = false;
	}
}
