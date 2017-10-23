using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tr_act : panel {

	public	sceneCell		prefab;
	public	List<sceneCell>	_actorCells = new List<sceneCell>();
	public	UnityEngine.UI.Text[]	_btnsTXT;
	public	int		whichsort = 2;

	public void Setup() {
		if (_actorCells.Count == 0) {
			for (int i = 0; i < trglobals.instance._trvs._scriptactors.Count; i++) {
				sceneCell sc = Instantiate (prefab) as sceneCell;
				sc.name = trglobals.instance._trvs._scriptactors [i].scriptname;
				sc.sceneTXT.text = sc.name;
				sc.index = i;
				sc.frequency = trglobals.instance._trvs._scriptactors [i].frequency;
				if (trglobals.instance._trvs._scriptactors [i].gender == "M")
					sc.gender = "Male";
				else
					sc.gender = "Female";
				sc.transform.SetParent (prefab.transform.parent,false);
				sc.rehearseTGL.gameObject.SetActive (true);
				sc.gameObject.SetActive(true);
				sc.isNarrator = false;
				_actorCells.Add (sc);
			}
		}
		else {
			for (int i = 0; i < _actorCells.Count; i++) {
			//	Debug.Log ("CHECKING " + _actorCells [i].name);
				if (trglobals.instance._trvs.isRehearsalActor(_actorCells [i].name))
					_actorCells [i].rehearseTGL.isOn = true;
				else
					_actorCells [i].rehearseTGL.isOn = false;
			}
		}
		if (whichsort == 0) {
			whichsort = 1;
			sortBtn (0);
		} else if (whichsort == 1) {
			whichsort = 0;
			sortBtn (0);
		} else if (whichsort == 3) {
			whichsort = 3;
			sortBtn (3);
		} else if (whichsort == 4) {
			whichsort = 4;
			sortBtn (3);
		} else if (whichsort == 2) {
			whichsort = 2;
			sortBtn (2);
		}
		setPosition (true);
	}

	public void loadRehearsalSettings() {
		trglobals.instance._trsdt.Setup (false,false,false,true);
	}

	public void SetActorVoice(sceneCell sc) {
	//	Debug.Log ("SetActorVoice");
		scriptactor actor = null;
		if (sc.isNarrator)
			actor = trglobals.instance._trvs.getTabrNarratroName();
		else
			actor = trglobals.instance._trvs.getTabrName (sc.sceneTXT.text);
		trglobals.instance._trsav.Setup (sc.sceneTXT.text,actor.tabrname,sc.isNarrator,actor.acaname,actor.rate,actor.shape,actor.index);
	}

	public void cleanup() {
		for (int i = 0; i < _actorCells.Count; i++)
			Destroy (_actorCells [i].gameObject);
		_actorCells.Clear ();
	}

	public void sortBtn(int ws) {
		if (ws > 2) {
			_btnsTXT [2].color = Color.white;
			_btnsTXT [1].color = trglobals.instance.unselectedButtonCLR;
			_btnsTXT [0].color = trglobals.instance.unselectedButtonCLR;
			Debug.Log (ws + ":" + whichsort);
			if (ws == 3 && whichsort == 3)
				SortFM ();
			else if (ws == 3 && whichsort == 4)
				SortMF ();
			else if (whichsort < 3)
				SortMF ();

		} else if (ws < 2) {
			_btnsTXT [0].color = Color.white;
			_btnsTXT [1].color = trglobals.instance.unselectedButtonCLR;
			_btnsTXT [2].color = trglobals.instance.unselectedButtonCLR;
			if (ws == 0 && whichsort == 0)
				SortZA ();
			else if (ws == 0 && whichsort == 1)
				SortAZ ();
			else if (whichsort > 1)
				SortAZ ();
		} else {
			_btnsTXT [1].color = Color.white;
			_btnsTXT [0].color = trglobals.instance.unselectedButtonCLR;
			_btnsTXT [2].color = trglobals.instance.unselectedButtonCLR;
			SortPrevalence ();
		}
	}

	void SortAZ() {
	//	Debug.Log ("whichsort is 0");
		whichsort = 0;
		_actorCells.Sort((p1,p2)=>p1.name.CompareTo(p2.name));
		for (int i = 0; i < _actorCells.Count; i++) {
			_actorCells [i].transform.SetSiblingIndex (i);
		}
		prefab.transform.SetSiblingIndex (0);
	}

	void SortZA() {
	//	Debug.Log ("whichsort is 1");
		whichsort = 1;
		_actorCells.Sort((p1,p2)=>p2.name.CompareTo(p1.name));
		for (int i = 0; i < _actorCells.Count; i++) {
			_actorCells [i].transform.SetSiblingIndex (i);
		}
		prefab.transform.SetSiblingIndex (0);
	}

	void SortPrevalence() {
	//	Debug.Log ("whichsort is 2");
		whichsort = 2;
		_actorCells.Sort((p1,p2)=>p2.frequency.CompareTo(p1.frequency));
		for (int i = 0; i < _actorCells.Count; i++) {
			_actorCells [i].transform.SetSiblingIndex (i);
		}
		prefab.transform.SetSiblingIndex (0);
	}

	void SortMF() {
	//	Debug.Log ("whichsort is 3");
		whichsort = 3;
		_actorCells.Sort((p1,p2)=>p1.gender.CompareTo(p2.gender));
		for (int i = 0; i < _actorCells.Count; i++) {
			_actorCells [i].transform.SetSiblingIndex (i);
		}
		prefab.transform.SetSiblingIndex (0);
	}
	void SortFM() {
	//	Debug.Log ("whichsort is 4");
		whichsort = 4;
		_actorCells.Sort((p1,p2)=>p2.gender.CompareTo(p1.gender));
		for (int i = 0; i < _actorCells.Count; i++) {
			_actorCells [i].transform.SetSiblingIndex (i);
		}
		prefab.transform.SetSiblingIndex (0);
	}
}
