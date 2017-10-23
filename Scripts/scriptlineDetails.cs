using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class scriptlineDetails : MonoBehaviour {
	
	public	int	linenumber;
	public	Button	note;
	public	Image	border;
	public	Image	cell;
	public	Text		text;
//	public	bool	active;
	// Use this for initialization
	public void pressedLine() {
	//	Debug.Log (linenumber);
	//	trglobals.instance._trvs.setPosition(true);
		trglobals.instance._trvs.PressedScriptLine (this);
//		setBorderGrey ();
	}

	public void setBorderGrey() {
		trglobals.instance._trvs._scriptlines [linenumber].active = true;
		border.color = Color.grey;
	}

	public void setBorderWhite() {
		trglobals.instance._trvs._scriptlines [linenumber].active = false;
		border.color = Color.white;
	}

	public void pressedNote() {
		Debug.Log ("Note " + linenumber);
		trglobals.instance._trnte.Setup (linenumber, false);
	}
}
