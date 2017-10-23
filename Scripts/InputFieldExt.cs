using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldExt : MonoBehaviour {

	public	InputField	_inputfield;
	public	Button		clearBTN;

	public	clickInputField	_clickInputField;

	void Start() {
		_clickInputField = GetComponent<clickInputField> ();
	}

	public void clearTXT() {
		_inputfield.text = "";
	}
}
