using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class tr_dfv : panel {

	string	peterfolder = "hq-lf-British-Peter-22khz";
	string	micahfolder = "hq-lf-USEnglish-Micah-22khz";
	string	tracyfolder = "hq-lf-USEnglish-Tracy-22khz";

	public voiceCell[]	_defaultVoiceCells;

	void Start() {
		_defaultVoiceCells [0].Setup ("Harold(UK)", peterfolder, "peter");
		_defaultVoiceCells [1].Setup ("Matthew(USA)", micahfolder, "micah");
		_defaultVoiceCells [2].Setup ("Mel(USA)", tracyfolder, "tracy");
	}

	public IEnumerator checkVoices() {
		yield return new WaitForEndOfFrame();
		_defaultVoiceCells [0].AutoDownload ();
		_defaultVoiceCells [1].AutoDownload ();
		_defaultVoiceCells [2].AutoDownload ();
		InvokeRepeating ("checkHasVoices", 1, 1);
	}

	void checkHasVoices() {
		if (HasDefaultVoices ()) {
			CancelInvoke ("checkHasVoices");
			trglobals.instance.home ();
		}
	}

	public bool HasDefaultVoices() {
		if (!Directory.Exists (Path.Combine(Path.Combine (Application.persistentDataPath, "voices"),peterfolder))) {
		//	StartCoroutine (CopyAndUnzip (peterfolder));
			return false;
		} 
		if (!Directory.Exists (Path.Combine(Path.Combine (Application.persistentDataPath, "voices"),micahfolder))) {
			//StartCoroutine(CopyAndUnzip(micahfolder));
			return false;
		}
		if (!Directory.Exists (Path.Combine(Path.Combine (Application.persistentDataPath, "voices"),tracyfolder))) {
			//StartCoroutine(CopyAndUnzip(tracyfolder));
			return false;
		}
		return true;
	}
}
