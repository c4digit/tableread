using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tr_nte : panel {

	public	int	linenumber;
	public	scriptnote _prefab;
	public	List<scriptnote>	_scriptnote = new List<scriptnote>();
	// Use this for initialization
	void Start() {
		_prefab.gameObject.SetActive (false);
	}

	public void Setup(int l, bool all = true) {
		if (all) {
			for (int i = 0; i < _scriptnote.Count; i++) {
				_scriptnote [i].gameObject.SetActive (true);
			}
		} else {
			for (int i = 0; i < _scriptnote.Count; i++) {
				if (_scriptnote [i].linenumber == l)
					_scriptnote [i].gameObject.SetActive (true);
				else
					_scriptnote [i].gameObject.SetActive (false);
			}
		}
		linenumber = l;
		setPosition (true);
	}
		
	public void toggleDisplay(string mn, string mr, bool d) {
		for (int i = 0; i < _scriptnote.Count; i++) {
			if (mn == _scriptnote [i].myname && mr == _scriptnote [i].myrelation) {
				_scriptnote [i].display = d;
				_scriptnote [i].gameObject.SetActive (d);
				int l = _scriptnote [i].linenumber;
				trglobals.instance._trvs._scriptlines [l].hasNote = d;
			}
		}
		trglobals.instance._scriptController._Adapter.ChangeItemCountTo(trglobals.instance._trvs._scriptlines.Count);
	}
		
	public List<string>	notelist = new List<string>();
	public void getNoteToSpeak(int linenumber) {
		//string notetoread = "";
		//int v = 0;
		notelist.Clear ();
		for (int i = 0; i < _scriptnote.Count; i++) {
			if (linenumber == _scriptnote [i].linenumber) {
				//if (v > 0)
				//	notetoread += "\n\n";
			//	notetoread += ("Note from " + _scriptnote [i].myname + ",\n" + _scriptnote [i].myrelation + ",\n\n" + _scriptnote [i].note);
				//v++;
				notelist.Add("Note from");
				notelist.Add(_scriptnote [i].myname);
				notelist.Add(_scriptnote [i].myrelation);
				notelist.Add(_scriptnote [i].note);
			}
		}
		//return notetoread;
	}

	public void NewNote() {
		trglobals.instance._trnnt.Setup (linenumber);
	}

	public void cleanup() {
		for (int i = 0; i < _scriptnote.Count; i++)
			Destroy (_scriptnote [i].gameObject);
		_scriptnote.Clear ();
	}

	public void sortNotes() {
		_scriptnote.Sort(delegate(scriptnote a, scriptnote b) {
			if (a.linenumber == b.linenumber)
				return a.creation.CompareTo(b.creation);
			return a.linenumber.CompareTo(b.linenumber);
		});
		/*_scriptnote.Sort((p1,p2)=> 
			if (p1.linenumber == p2.linenumber)
				p1.creation.CompareTo(p2.creation)
			else
				p1.linenumber.CompareTo(p2.linenumber));*/
			//
		for (int i = 0; i < _scriptnote.Count; i++) {
			_scriptnote [i].transform.SetSiblingIndex (i);
			_scriptnote [i].arrayPosition = i;
		}
	}

	/*public int CompareTo(object other) {
		if (!(other is scriptnote))
		{
			//throw new ArgumentException("object is not a PriorityEntry");
		}
		scriptnote PEOther = other as scriptnote;
		if( linenumber != PEOther.linenumber ) return linenumber.CompareTo(PEOther.linenumber);
		if( creation != PEOther.creation ) return creation.CompareTo(PEOther.creation);
	}*/

	public void deleteNote(scriptnote n) {
		#if UNITY_EDITOR
		removeNote(n);
		#endif
		TheNextFlow.UnityPlugins.MobileNativePopups.OpenAlertDialog(
			"ARE YOU SURE!", "Are you sure you want to delete this note?",
			"CANCEL", "OK",
			() => { 
				Debug.Log("NO was pressed");
			},
			() => { 
				Debug.Log("OK was pressed"); 
				removeNote(n);
			});
	}

	public void loadNoteSettings() {
		trglobals.instance._trsdt.Setup (false,true,false);
	}

	void removeNote(scriptnote n) {
		trglobals.instance._trvs._scriptlines [n.linenumber].hasNote = false;
		_scriptnote.RemoveAt (n.arrayPosition);
		Destroy (n.gameObject);
		for (int i = 0; i < _scriptnote.Count; i++) {
			_scriptnote [i].arrayPosition = i;
		}
		trglobals.instance._scriptController._Adapter.ChangeItemCountTo(trglobals.instance._trvs._scriptlines.Count);
		trglobals.instance.saveContributerNote (n.myname,n.myrelation);
	}

	public bool hasNote(string note, string line) {
		for (int i = 0; i < _scriptnote.Count; i++) {
			if (note.Equals(_scriptnote[i].note) && line.Equals(_scriptnote[i].line))
				return true;
		}
		return false;
	}
}
