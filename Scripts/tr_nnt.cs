using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class tr_nnt : panel {

	public	int			linenumber;
	public	Text		_namerelationTXT;
	public	Text		_sceneTXT;
	public	Text		_lineTXT;
	public	InputField	_noteIF;

	string sceneName;
	// Use this for initialization
	public void Setup(int l) {
		linenumber = l;
		_namerelationTXT.text = trglobals.instance.projectMyname + ":" + trglobals.instance.projectRelation;
		int sv = trglobals.instance._trvs._scriptlines [linenumber].scene - 1;
		trglobals.instance.DebugLog ("Scene is " + sv);
		if (sv >= 0) {
			sceneName = trglobals.instance._trvs._scriptscenes [sv].name;
			_sceneTXT.text = sceneName + "\nPAGE " + trglobals.instance._trvs._scriptlines [linenumber].page + ": LINE NO. " + linenumber;
		} else {
			sceneName = "FRONT PAGE";
			_sceneTXT.text = sceneName;
		}
		_lineTXT.text = trglobals.instance._trvs._scriptlines [linenumber].linestr;
		setPosition (true);
	}

	public void AddNote() {
		trglobals.instance.DebugLog (_noteIF.text);
		if (_noteIF.text == "") {
			trglobals.instance.ShowError ("Please Add a note.", "NOTE ERROR");
			return;
		}
		string id = trglobals.instance.projectID;
		string myname = trglobals.instance.projectMyname;
		string myrelation = trglobals.instance.projectRelation;
		string line = _lineTXT.text;
		int page = trglobals.instance._trvs._scriptlines [linenumber].page;
		System.DateTime Jan1st2001 = new System.DateTime(2001, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		long creation = ((long)(System.DateTime.UtcNow - Jan1st2001).TotalMilliseconds / 1000) - 458627800;
		trglobals.instance.DebugLog ("CREATION IS " + creation);
		scriptnote thenote = Instantiate (trglobals.instance._trnte._prefab) as scriptnote;
		thenote.Setup(id,myname,myrelation,line,sceneName,_noteIF.text,page,linenumber,creation);
		thenote.transform.SetParent (trglobals.instance._trnte._prefab.transform.parent, false);
		trglobals.instance._trvs._scriptlines [linenumber].hasNote = true;
		trglobals.instance._trnte._scriptnote.Add (thenote);
		thenote.gameObject.SetActive (true);
		trglobals.instance._trnte.sortNotes ();
		// save note trn
		trglobals.instance.saveContributerNote (myname, myrelation);
		// reorder notes
		// will also need to add note here
		trglobals.instance.genericBack ();
		trglobals.instance._scriptController._Adapter.ChangeItemCountTo(trglobals.instance._trvs._scriptlines.Count);
		_noteIF.text = "";
	}
}
