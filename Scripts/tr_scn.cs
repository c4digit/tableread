using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tr_scn : panel {

	public	sceneCell		prefab;
	public	List<sceneCell>	_sceneCells = new List<sceneCell>();
	bool allon = true;

	void Start() {
		prefab.gameObject.SetActive (false);
	}

	// Use this for initialization
	public void Setup () {
		if (_sceneCells.Count == 0) {
			for (int i = 0; i < trglobals.instance._trvs._scriptscenes.Count; i++) {
				sceneCell sc = Instantiate (prefab) as sceneCell;
				sc.name = trglobals.instance._trvs._scriptscenes [i].name;;
				sc.sceneTXT.text = sc.name;
				sc.index = i;
				sc.transform.SetParent (prefab.transform.parent,false);
				sc.gameObject.SetActive(true);
				_sceneCells.Add (sc);
			}
		} else {
			for (int i = 0; i < trglobals.instance._trvs._scriptscenes.Count; i++) {
				if (trglobals.instance._trvs._scriptscenes [i].rehearse)
					_sceneCells [i].rehearseTGL.isOn = true;
				else
					_sceneCells [i].rehearseTGL.isOn = false;
			}
		}
		setPosition (true);
	}

	public void loadSceneSettings() {
		trglobals.instance._trsdt.Setup (false,false,false,false,true);
	}

	public bool checkSceneOn() {
		for (int i = 0; i < _sceneCells.Count; i++) {
			if (_sceneCells [i].rehearseTGL.isOn)
				return true;
		}
		return false;
	}

	public void toggleALL() {
		allon = !allon;
		for (int i = 0; i < _sceneCells.Count; i++) {
			_sceneCells [i].rehearseTGL.isOn = allon;
		}
	}

	public void goToScene(sceneCell sc) {
		if (sc.rehearseTGL.isOn) {
			trglobals.instance.genericBack ();
			trglobals.instance._trvs.JumpToScene (trglobals.instance._trvs._scriptscenes [sc.index].linenumber);
		}
		else
			trglobals.instance.ShowError ("Please make scene playable before attempting to jump to it.");
	}

	public void cleanup() {
		for (int i = 0; i < _sceneCells.Count; i++)
			Destroy (_sceneCells [i].gameObject);
		_sceneCells.Clear ();
	}

}
