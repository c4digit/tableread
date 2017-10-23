using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class tr_msp : panel {

	public	pdfCell			_cellprefab;
	public	List<pdfCell>	_cells = new List<pdfCell>();
	public	UnityEngine.UI.Text[]	_btnsTXT;
	public	int		whichsort;

	void Start() {
		_cellprefab.gameObject.SetActive (false);
	}

	public void Setup () {
		if (trglobals.instance._audioSource) {
			trglobals.instance._audioSource.Stop ();
			trglobals.instance._audioSource.loop = true;
		}
		if (_cells.Count != 0) {
			for (int i = 0; i < _cells.Count; i++) {
				Destroy (_cells [i].gameObject);
			}
			_cells.Clear();
			trglobals.instance.cleanup ();
		}
	//	Debug.Log ("finding pdf");
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] info = dir.GetFiles("*.*");
		foreach (FileInfo f in info)  {
		//	Debug.Log (f.FullName);
			if (f.Extension == ".trd" || f.Extension == ".TRD") {
				string n = Path.GetFileNameWithoutExtension (f.FullName);
				pdfCell p = Instantiate (_cellprefab) as pdfCell;
				p._pdfTXT.text = n;p._fullpath = f.FullName;p._name = f.Name;
			//	Debug.Log ("Setting date "+f.Name+":" + f.CreationTime);
				p._date = f.CreationTime;
				p.transform.SetParent (_cellprefab.transform.parent, false);
				p.gameObject.SetActive (true);
				p.index = _cells.Count;
				_cells.Add (p);
			}
		}
		if (whichsort == 0) {
		//	Debug.Log ("whichsort is 1");
			whichsort = 1;
			sortBtn (0);
		} else if (whichsort == 1) {
			whichsort = 0;
			sortBtn (0);
		} else if (whichsort == 2) {
			whichsort = 3;
			sortBtn (2);
		} else if (whichsort == 3) {
			whichsort = 2;
			sortBtn (2);
		}
		setPosition(true);
	}

	public void sortBtn(int ws) {
		if (ws > 1) {
			_btnsTXT [1].color = Color.white;
			_btnsTXT [0].color = trglobals.instance.unselectedButtonCLR;
		//	Debug.Log (ws + ":" + whichsort);
			if (ws == 2 && whichsort == 2)
				SortDateLow ();
			else if (ws == 2 && whichsort == 3)
				SortDateHigh ();
			else if (whichsort < 2)
				SortDateLow ();

		} else {
			_btnsTXT [0].color = Color.white;
			_btnsTXT [1].color = trglobals.instance.unselectedButtonCLR;
			if (ws == 0 && whichsort == 0)
				SortZA ();
			else if (ws == 0 && whichsort == 1)
				SortAZ ();
			else if (whichsort > 1)
				SortAZ ();
		}
		for (int i = 0; i< _cells.Count; i++) {
			_cells [i].index = i;
		}
	}

	void SortAZ() {
		//Debug.Log ("whichsort is 0");
		whichsort = 0;
		_cells.Sort((p1,p2)=>p1._pdfTXT.text.CompareTo(p2._pdfTXT.text));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
		}
	}
	void SortZA() {
		//Debug.Log ("whichsort is 1");
		whichsort = 1;
		_cells.Sort((p1,p2)=>p2._pdfTXT.text.CompareTo(p1._pdfTXT.text));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
		}
	}

	void SortDateHigh() {
		whichsort = 2;
		_cells.Sort((p1,p2)=>p1._date.CompareTo(p2._date));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
		}
	}

	void SortDateLow() {
		whichsort = 3;
		_cells.Sort((p1,p2)=>p2._date.CompareTo(p1._date));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
		}
	}
	public void loadScriptDetails() {
		//setPosition (false);
		trglobals.instance.scriptLoaded = false;
		trglobals.instance._trsdt.Setup (true);
	}

	public void DeleteCell(pdfCell p) {
		string fpn = Path.GetFileNameWithoutExtension (p._name);
		if (File.Exists (p._fullpath)) {
			string id = trglobals.instance.getProjectID (p._fullpath);
		//	Debug.Log ("Deleteing " + p._fullpath + ":" + id);
			File.Delete(p._fullpath);
			string idfolder = Path.Combine (Application.persistentDataPath, id);
			if (Directory.Exists(idfolder)) {
			//	Debug.Log("Need to delete ID folder");
				trglobals.instance.DeleteDirectory(idfolder);
			}
		}
		_cells.RemoveAt (p.index);
		Destroy (p.gameObject);
		for (int i = 0; i< _cells.Count; i++) {
			_cells [i].index = i;
		}
	//	Debug.Log ("Looking for fps " + fpn);
		trglobals.instance._trfp.CheckDeleteCell (fpn);
	}
		
	public void LoadScriptProject(pdfCell p) {
		if (File.Exists (p._fullpath)) {
			trglobals.instance.getProjectData (p._fullpath);
		} else {
			trglobals.instance.ShowError ("Can't find TRD File","FILE ERROR");
		}
	}
}
