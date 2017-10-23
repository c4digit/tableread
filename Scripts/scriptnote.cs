using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scriptnote: MonoBehaviour {
	public 	int    arrayPosition;
	public string ID;
	public string myname;
	public string myrelation;
	public string line;
	public string scene;
	public string note;
	public int page;
	public int linenumber;
	public bool display;
	public long creation;

	public	Text	nameTXT;
	public	Text	sceneTXT;
	public	Text	lineTXT;
	public	Text	noteTXT;

	public void Setup(string _ID, string _myname, string _myrelation, string _line, string _scene, string _note, int _page, int _linenumber, long _creation) {
		ID = _ID;
		myname = _myname;
		myrelation = _myrelation;
		line = _line;
		scene = _scene;
		note = _note;
		page = _page;
		linenumber = _linenumber;
		display = true;
		creation = _creation;
		noteTXT.text = note;
		nameTXT.text = myname + " : " + myrelation;
		sceneTXT.text = scene + "\nPAGE " + page + ": LINE NO. " + linenumber;
		lineTXT.text = line;
	}

	public void gotoNote() {
		trglobals.instance._trvs.speakingline = linenumber;
		trglobals.instance.genericBack ();
		trglobals.instance._scriptController._Adapter.ScrollTo (linenumber);
	}
}
