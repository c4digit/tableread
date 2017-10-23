using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class tr_dwa : panel {

	public	voiceCell			_acprefab;
	public	List<voiceCell>		_actorCells = new List<voiceCell>();
	public	Transform[]			_ageheadings;

	public	MobileUIController	_muc;
	// Use this for initialization
	void Start () {
		_acprefab.gameObject.SetActive (false);
		SetupVoices();
	}

	public void SetupVoices() {
		for (int i = 0; i < _actorCells.Count; i++) {
			Destroy (_actorCells [i].gameObject);
		}
		_actorCells.Clear ();
		for (int i = trglobals.instance._tablereadActors.Count - 1; i >= 0; i--) {
			tablereadActor ta = trglobals.instance._tablereadActors [i];
			//	Debug.Log ("CHECKING " + ta.acaName);
			//	if (!checkDefault (ta.acaName)) {
			voiceCell ac = Instantiate (_acprefab) as voiceCell;
			ac.Setup (ta.tabrname, ta.acaFolder, ta.acaName);
			ac.transform.SetParent (_acprefab.transform.parent, false);
			ac.transform.SetSiblingIndex (_ageheadings [getAgeGroup (ta.age)].GetSiblingIndex () + 1);
			ac.gameObject.SetActive (true);
			_actorCells.Add (ac);
		}
	//	Debug.Log ("SetupVoiceCells SetupVoices");
		SetupVoiceCells ();
		CheckDefaultVoices ();
	}

	bool checkDefault(string n) {
		if (n.Equals("Peter"))
			return true;
		if (n.Equals("Micah"))
			return true;
		if (n.Equals("Tracy"))
			return true;
		return false;
	}

	int getAgeGroup(string a) {
		if (a == "ADULT")
			return 0;
		if (a == "TEEN")
			return 1;
		if (a == "CHILD")
			return 2;
		if (a == "SENIOR")
			return 3;
		return 0;
	}

	public void Setup() {
		trglobals.instance._audioSource.Stop ();
		if (_actorCells.Count == 0)
			SetupVoices ();
		SetupVoiceCells ();
		CheckDefaultVoices ();
		setPosition (true);
	}

	public void DownloadVoicePage() {
		trglobals.instance._trdwa.Setup ();
	}

	public void UnzipVoices() {
		Debug.Log ("CHECKING FOR ZIPS DownloadedVoice tr_dwa");
		DirectoryInfo dir = new DirectoryInfo ((Path.Combine (Application.persistentDataPath, "voices")));
		FileInfo[] info = dir.GetFiles ("*.zip");
		for (int i = 0; i < info.Length; i++) {
			Debug.Log("FOUND :" + info[i].Name);
			ZipUtil.Unzip (info[i].FullName, (Path.Combine(Application.persistentDataPath, "voices")));
			File.Delete (info[i].FullName);
		}
		Debug.Log ("SetupVoiceCells UnzipVoices");
		SetupVoiceCells ();
	}

	/*public void DownloadedVoice() {
		Debug.Log ("CHECKING FOR ZIPS DownloadedVoice tr_dwa");
		DirectoryInfo dir = new DirectoryInfo ((Path.Combine (Application.persistentDataPath, "voices")));
		FileInfo[] info = dir.GetFiles ("*.zip");
		for (int i = 0; i < info.Length; i++) {
		//	Debug.Log("FOUND :" + info[i].Name);
			ZipUtil.Unzip (info[i].FullName, (Path.Combine(Application.persistentDataPath, "voices")));
			File.Delete (info[i].FullName);
		}
		SetupVoiceCells ();
	}*/

	public void SetupVoiceCells() {
		for (int i = 0; i < _actorCells.Count; i++) {
			_actorCells [i].checkFolder ();
		}
	}

	public void CheckDefaultVoices() {
		for (int i = 0; i < _actorCells.Count; i++) {
		//	Debug.Log ("Checking " + _actorCells[i].acaname);
			if (checkDefault (_actorCells[i].acaname)) {
				_actorCells[i].arrowBTN.SetActive (false);
				_actorCells[i].horSCR.enabled = false;
			}
		}
	}
}
