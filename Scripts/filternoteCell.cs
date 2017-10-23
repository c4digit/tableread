using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class filternoteCell : MonoBehaviour {

	public	string	myname;
	public	string	myrelation;
	public	string	displayname;
	public	bool	display;

	public	Text	nameTXT;
	public	Toggle	displayTGL;

	public void Setup(string mn, string mr, string dn, bool disp = true) {
		myname = mn; myrelation = mr; displayname = dn;
		display = disp;
		displayTGL.isOn = disp;
		nameTXT.text = dn;
	}

	public void toggleFilterNote() {
		display = displayTGL.isOn;
		trglobals.instance._trnte.toggleDisplay (myname, myrelation, display);
	}
}
