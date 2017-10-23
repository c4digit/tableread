using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tr_fnt : panel {

	public	filternoteCell			_prefab;
	public	List<filternoteCell>	_cells = new List<filternoteCell>();

	void Start() {
		_prefab.gameObject.SetActive (false);
	}

	public void Setup() {
		// lets re-create
		FindContributors();
		setPosition (true);
	}

	public void clearFilterNotes() {
		for (int i = 0; i < _cells.Count; i++) {
			Destroy (_cells [i].gameObject);
		}
		_cells.Clear ();
	}

	public void AddFilterNote(string mn, string mr, string dn, bool disp) {
		filternoteCell fn = Instantiate (_prefab) as filternoteCell;
		fn.Setup (mn, mr, dn, disp);
		fn.transform.SetParent (_prefab.transform.parent, false);
		fn.gameObject.SetActive(true);
		_cells.Add (fn);
	}

	public void FindContributors() {
		if (trglobals.instance._trnte._scriptnote.Count > 0) {
		//	Debug.Log ("NUMBER OF NOTES " + trglobals.instance._trnte._scriptnote.Count);
			// now lets save all the notes out again
			trglobals.instance._trnte.sortNotes ();
			trglobals.instance._trfnt.clearFilterNotes ();
			List<string> mynamesrelations = new List<string> ();
		//	Debug.Log (trglobals.instance._trnte._scriptnote[0].myname + ":" + trglobals.instance._trnte._scriptnote[0].myrelation);
			string mynamerelation = trglobals.instance._trnte._scriptnote [0].myname + " : " + trglobals.instance._trnte._scriptnote [0].myrelation;
			mynamesrelations.Add(mynamerelation);
			AddFilterNote (trglobals.instance._trnte._scriptnote [0].myname, trglobals.instance._trnte._scriptnote [0].myrelation, mynamerelation, trglobals.instance._trnte._scriptnote [0].display);
			trglobals.instance.saveContributerNote (trglobals.instance._trnte._scriptnote[0].myname, trglobals.instance._trnte._scriptnote[0].myrelation);
			for (int i = 1; i < trglobals.instance._trnte._scriptnote.Count; i++) {
				mynamerelation = trglobals.instance._trnte._scriptnote [i].myname + " : " + trglobals.instance._trnte._scriptnote [i].myrelation;
			//	Debug.Log (i + ":" + mynamerelation);
				if (!mynamesrelations.Contains (mynamerelation)) {
					//Debug.Log ("need to save note ");
					mynamesrelations.Add(mynamerelation);
					AddFilterNote (trglobals.instance._trnte._scriptnote [i].myname, trglobals.instance._trnte._scriptnote [i].myrelation, mynamerelation,trglobals.instance._trnte._scriptnote [i].display);
					trglobals.instance.saveContributerNote (trglobals.instance._trnte._scriptnote[i].myname, trglobals.instance._trnte._scriptnote[i].myrelation);
				}
			}
		}
		if (trglobals.instance.scriptLoaded) {
			trglobals.instance._scriptController.UpdateItems (trglobals.instance._trvs._scriptlines.Count);
		}
	}
}
