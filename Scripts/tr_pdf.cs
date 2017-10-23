using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class tr_pdf : panel {

	public	pdfCell	_cellprefab;
	public	List<pdfCell>	_cells = new List<pdfCell>();
	public	UnityEngine.UI.Text[]	_btnsTXT;
	public	int		whichsort;
	public	UnityEngine.UI.Image	noscriptsImport;
	public	Sprite noscriptsImportAndroidSPR;
	IEnumerator Start() {
		_cellprefab.gameObject.SetActive (false);
		yield return new WaitForEndOfFrame ();
		downloadFolderPath = "";
		#if UNITY_ANDROID && !UNITY_EDITOR
		downloadFolderPath = trglobals.instance._trscr._muc.ebdCtl.GetDownloadFolder ();
		noscriptsImport.sprite = noscriptsImportAndroidSPR;
	//	Debug.Log("downloadFolderPath " + downloadFolderPath);
		#endif
	}

	public string downloadFolderPath;
	public void showInfo(bool v) {
		noscriptsImport.gameObject.SetActive (v);
	}

	public void selectPDF(pdfCell p) {
	//	Debug.Log ("selectPDF " + trglobals.instance.projectPDF + ":" + trglobals.instance.scriptLoaded + ":" + Path.GetFileName (p._name));
		trglobals.instance.reloadScript = false;
		if (trglobals.instance.scriptLoaded  && !trglobals.instance.projectPDF.Equals(Path.GetFileName (p._name)))
			trglobals.instance.reloadScript = true;
		trglobals.instance._trsdt.pdfTXT.text = Path.GetFileName (p._name);
		trglobals.instance._trsdt.checkMetadata (p._fullpath);
		if (!trglobals.instance.scriptLoaded)
			trglobals.instance._trsdt.nameIF.text = p._pdfTXT.text;
		trglobals.instance.genericBack ();
	}

	public void Setup() {
		if (_cells.Count != 0) {
			for (int i = 0; i < _cells.Count; i++)
				Destroy (_cells [i].gameObject);
			_cells.Clear();
			trglobals.instance.cleanup ();
		}
	//	Debug.Log ("finding pdf");
		//DirectoryInfo dir = new DirectoryInfo((Path.Combine(Application.persistentDataPath, "pdf")));
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] info = dir.GetFiles("*.*");
		foreach (FileInfo f in info)  {
			if (f.Extension == ".pdf" || f.Extension == ".PDF") {
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
		// for ANDORID
		if (downloadFolderPath != "") {
			dir = new DirectoryInfo(downloadFolderPath);
			info = dir.GetFiles("*.*");
			foreach (FileInfo f in info)  {
				if (f.Extension == ".pdf" || f.Extension == ".PDF") {
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
		}
		if (_cells.Count == 0)
			noscriptsImport.gameObject.SetActive (true);
		else
			noscriptsImport.gameObject.SetActive (false);
		if (whichsort == 0) {
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
		trglobals.instance._heading.text = _headingSTR;
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
	}

	void SortAZ() {
		//Debug.Log ("whichsort is 0");
		whichsort = 0;
		_cells.Sort((p1,p2)=>p1._pdfTXT.text.CompareTo(p2._pdfTXT.text));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
			_cells [i].index = i;
		}
	}
	void SortZA() {
		//Debug.Log ("whichsort is 1");
		whichsort = 1;
		_cells.Sort((p1,p2)=>p2._pdfTXT.text.CompareTo(p1._pdfTXT.text));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
			_cells [i].index = i;
		}
	}

	void SortDateHigh() {
		whichsort = 2;
		_cells.Sort((p1,p2)=>p1._date.CompareTo(p2._date));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
			_cells [i].index = i;
		}
	}

	void SortDateLow() {
		whichsort = 3;
		_cells.Sort((p1,p2)=>p2._date.CompareTo(p1._date));
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].transform.SetSiblingIndex (i);
			_cells [i].index = i;
		}
	}

	public void DeleteCell(pdfCell p) {
	//	Debug.Log ("Deleteing " + p._fullpath);
		if (File.Exists(p._fullpath))
			File.Delete(p._fullpath);
		_cells.RemoveAt (p.index);
		Destroy (p.gameObject);
		for (int i = 0; i < _cells.Count; i++) {
			_cells [i].index = i;
		}
		if (_cells.Count == 0) {
			noscriptsImport.gameObject.SetActive (true);
		}
	}
}
