using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class tr_abt : panel {

	public	Text		writerTXT;
	public	Text		genreTXT;
	public	Text		synopsisTXT;
	public	Text		buttonTXT;
	public	Button		readBTN;
	public	Transform	_progressBAR;
	public string[]	content;
	int abtspeakingline;
	// Use this for initialization
	public int noscripts = 0;
	public int hasrated = -1;

	void Start() {
		hasrated = PlayerPrefs.GetInt ("tablereadratedapp");
		noscripts = PlayerPrefs.GetInt ("tablereadscripts");
	}

	public void Setup (int sp) {
		abtspeakingline = sp;
		readBTN.interactable = false;
		_progressBAR.localScale = new Vector3 (0, 1, 1);
		buttonTXT.text = "Processing Script";
		trglobals.instance._heading.text = trglobals.instance.projectName + " " + trglobals.instance.projectVersion;
		writerTXT.text = "Written by\n\n" + trglobals.instance.projectWriters;
		genreTXT.text = trglobals.instance.projectGenre;
		synopsisTXT.text = trglobals.instance.projectSynopsis + "\n\n" + trglobals.instance.projectCommendation;
		content = null;
		TextAsset txt = (TextAsset)Resources.Load("trnames", typeof(TextAsset));
		content = txt.text.Split ('\n');
		setPosition (true, false);
		if (!trglobals.instance.goPRO) {
			if (trglobals.instance._audioSource.isPlaying)
				trglobals.instance._audioSource.Stop ();
			trglobals.instance._trfp.randomvoice ("harolduk_short");
		} else {
			#if UNITY_IOS && !UNITY_EDITOR
			trglobals.instance._tts_ios.getTTS();
			#endif
		}
		StartCoroutine (processPDF ());
		hasrated = PlayerPrefs.GetInt ("tablereadratedapp");
		if (hasrated == 0) {
			noscripts++;
			if (noscripts == 6) {
				noscripts = 0;
				NativeToolkit.RateApp ("Rate this app", 
					"Help us spread the word!", 
					"Rate Now", "Later", "No, Thanks", "1009176110", AppRated);
			}
			PlayerPrefs.SetInt ("tablereadscripts", noscripts);
			PlayerPrefs.Save ();
		}
	}

	void AppRated(string result) {
		//	Debug.Log ("result " + result);
		if (!result.Equals ("1")) {
			trglobals.instance.DebugLog ("I've either rated or said NO THANKS");
			hasrated = 1;
			PlayerPrefs.SetInt ("tablereadratedapp",1);
			PlayerPrefs.Save ();
		}
	}

	public void endProcess() {
		loadNotesFromFolder();
		readBTN.interactable = true;
		buttonTXT.text = "tableread Script";
		_progressBAR.localScale = new Vector3 (1, 1, 1);
	}
		
	public void loadNotesFromFolder() {
		string datapath = Path.Combine (Application.persistentDataPath, trglobals.instance.projectID);
	//	Debug.Log ("loadNotesFromFolder " + datapath);
		if (!Directory.Exists (datapath))
			return;
		DirectoryInfo dir = new DirectoryInfo(datapath);
		FileInfo[] info = dir.GetFiles("*.*");
		foreach (FileInfo f in info) {
			trglobals.instance.DebugLog ("FOUND FILE " + f.Name + ":" + f.Extension + ":");
			if (f.Extension.ToLower().Contains ("trn")) {
				TextReader tr = new StreamReader (f.FullName);
				File.Delete (f.FullName);
				string trdData = tr.ReadToEnd ();
				string[] split = trdData.Split (new string[] { "\n" }, System.StringSplitOptions.None);
				trglobals.instance.DebugLog (split.Length.ToString());
				int ln = 4;
				while (!split [ln].Equals ("END TABLE READ NOTES") && !split [ln].Equals ("END TABLE READ ACTIVE NOTES") && ln < split.Length) {
					ln++;
					string name = split [ln].Trim();
					ln++;
					string relation = split [ln].Trim();
					ln++;
					string scene = split [ln].Substring (6).Trim();
					ln++;
					string[] page = split [ln].Split (':');
					ln++;
					string[] linenumber = split [ln].Split (':');
					ln++;
					string[] creation = split [ln].Split (':');
					ln++;
					ln++;
					string line = split [ln].Trim();
					ln++;
					ln++;
					string note = split [ln].Trim();
				//	Debug.Log ("NOTE IS " + note)
					ln++;
					while (!split [ln].Trim().Equals ("NOTE FROM") && !split [ln].Equals ("END TABLE READ NOTES") && !split [ln].Equals ("END TABLE READ ACTIVE NOTES") && ln < split.Length) {
						Debug.Log (split [ln] + ":" + ln);
					//	note += (" " + split [ln].Trim());
						ln++;
					}
					note.Trim ();
					Debug.Log ("NOTE IS " + note);
					if (!trglobals.instance._trnte.hasNote (note, line)) {
						// make note
						scriptnote thenote = Instantiate (trglobals.instance._trnte._prefab) as scriptnote;
						int linn = int.Parse (linenumber [1].Trim());
						if (linn < trglobals.instance._trvs._scriptlines.Count) {
						//	Debug.Log (name + ":" + relation + ":" + line + ":" + scene + ":" + note + ":" + int.Parse (page [1].Trim()));
							thenote.Setup (trglobals.instance.projectID, name, relation, line, scene, note, int.Parse (page [1].Trim()), linn, long.Parse (creation [1].Trim()));
							thenote.transform.SetParent (trglobals.instance._trnte._prefab.transform.parent, false);
							trglobals.instance._trvs._scriptlines [linn].hasNote = true;
							trglobals.instance._trnte._scriptnote.Add (thenote);
							thenote.gameObject.SetActive (true);
						}
					}
				}
				//if (split [ln].Equals ("END TABLE READ ACTIVE NOTES")) {
	
				//}
				tr.Close ();
			}
		}
		trglobals.instance._trfnt.FindContributors ();
	}

	IEnumerator processPDF() {
		yield return null;
		yield return null;
		yield return null;
		trglobals.instance.checkVoiceDownloads();
		trglobals.instance._trvs.getPDF (trglobals.instance.projectPDF,abtspeakingline);
	}

	public void ShowScript() {
	//	Debug.Log ("ShowScript " + abtspeakingline + ":" + trglobals.instance._trvs._scriptlines.Count);
	//	trglobals.instance._scriptController.clear ();
		trglobals.instance._trvs.Setup (abtspeakingline);
	//	trglobals.instance._scriptController.UpdateItems (trglobals.instance._trvs._scriptlines.Count);
	//	trglobals.instance._trvs.Setup (abtspeakingline);
	//	trglobals.instance._scriptController._Adapter.ScrollTo (abtspeakingline);
	}

	IEnumerator LoadScriptPage() {
		yield return null;
		yield return null;
		yield return null;
		trglobals.instance._trvs.Setup (abtspeakingline);
	}
}
