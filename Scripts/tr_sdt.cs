using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

public class tr_sdt : panel {

	public	GameObject[]	settingPNL;
	public	GameObject[]	bottomBTNS;
	public	InputField	nameIF;
	public	InputField	versionIF;
	public	InputFieldExt	writersIF;
	public	Dropdown	genreDD;
	public	Dropdown	mediumDD;
	public	InputFieldExt	synopsisIF;
	public	InputFieldExt	mynameIF;
	public	Dropdown	relationDD;
	public	InputFieldExt	wgaIF;
	public	InputFieldExt	copyrightIF;
	public	InputFieldExt	commendationIF;
	public	Text pdfTXT;
//	bool	setscorefromtrd = false;
	public 	Button selectPDFBTN;
	List<string> genreTypes = new List<string> {
		"SELECT GENRE","Action","Adventure","Animation","Biopic","Bollywood","Comedy",
		"Crime","Documentary","Drama","Family","Horror","Music & Dance",
		"Musical","Period & Historical","Romance","Science Fiction","Fantasy","Sport",
		"Thriller","War","Western","World Cinema","Other"
	};
	List<string> mediumTypes = new List<string> {
		"SELECT MEDIUM","Film","TV","Game"
	};
	List<string> relationTypes = new List<string> {"SELECT RELATIONSHIP",
		"Writer","Producer","Director","Actor","Casting","Production","Script","Location","Camera","Sound",
		"Lighting","Grip","Electrical","Art","Hair & Makeup","Wardrobe","Edit","Post Production","Visual Effects","Post Sound & Music",
		"Other Crew","Publicist","Interactive Media","Accounts","Craft Services","Agent","Manager","Studio","Reader","Contributor","No Relation"
	};
	public int[] genreScores = {5,5,5,8,10,12,20,8,10,14,22,10,10,8,10,18,17,4,20,3,1,10,0,0};
	void Start() {
		genreDD.AddOptions (genreTypes);
		mediumDD.AddOptions (mediumTypes);
		relationDD.AddOptions(relationTypes);
	}

	public Toggle[]	_settingsToggles;

	public void ToggleReadSettings(int i) {
		if (i == 0) {
			_settingsToggles [0].isOn = true;_settingsToggles [1].isOn = false;_settingsToggles [2].isOn = false;
		} else if (i == 1) {
			_settingsToggles [0].isOn = false;_settingsToggles [1].isOn = true;_settingsToggles [2].isOn = false;
		}
		else if (i == 2) {
			_settingsToggles [0].isOn = false;_settingsToggles [1].isOn = false;_settingsToggles [2].isOn = true;
		}
	}

	public void ToggleRehersalSettings(int i) {
		if (i == 0) {
			_settingsToggles [8].isOn = true;_settingsToggles [9].isOn = false;
		} else {
			_settingsToggles [9].isOn = true;_settingsToggles [8].isOn = false;
		}
	}

	public void Setup (bool newTRD,bool shownotes = false,bool showsettings = true,bool showrehearsal = false, bool showscene = false) {
		nameIF.text = trglobals.instance.projectName;
		versionIF.text = "1.0";
		if (string.IsNullOrEmpty (trglobals.instance.projectPDF))
			pdfTXT.text = "touch to select script";
		else
			pdfTXT.text = trglobals.instance.projectPDF;
		// rest
		writersIF._inputfield.text = "";
		synopsisIF._inputfield.text = "";
		mynameIF._inputfield.text = PlayerPrefs.GetString ("myName");
		wgaIF._inputfield.text = "";
		copyrightIF._inputfield.text = "";
		commendationIF._inputfield.text = "";
		genreDD.value = 0;
		mediumDD.value = 0;
		relationDD.value = 0;
		hidefirst (genreDD);
		hidefirst (mediumDD);
		hidefirst (relationDD);
		selectPDFBTN.interactable = true;
		if (newTRD) {
		//	trglobals.instance.DebugLog ("NOT NEW TRD");
			resetInputFields ();
			settingPNL [0].SetActive (false);
			settingPNL [2].SetActive (false);
			settingPNL [3].SetActive (false);
			settingPNL [4].SetActive (false);
			settingPNL[1].SetActive(true);
			bottomBTNS[0].SetActive(true);
			bottomBTNS[1].SetActive(false);
			// RESET PROJECT SETTINGS
			//trglobals.instance.projectName = "";
			//trglobals.instance.projectVersion = "";
			//trglobals.instance.projectPDF = "";
			trglobals.instance.projectVersion = "1.0";
			trglobals.instance.projectID = "";
			trglobals.instance.projectWriters = "";
			trglobals.instance.projectGenre = "";
			trglobals.instance.projectSynopsis = "";
			trglobals.instance.projectCommendation = "";
			trglobals.instance.projectPlatform = "";
			trglobals.instance.projectWGA = "";
			trglobals.instance.projectCopyright = "";
			trglobals.instance.projectMyname = "";
		} else {
		//	trglobals.instance.DebugLog ("NOT NEW TRD");
			bottomBTNS [0].SetActive (false);
			bottomBTNS [1].SetActive (true);
			nameIF.text = trglobals.instance.projectName;
			pdfTXT.text = trglobals.instance.projectPDF;
			trglobals.instance.DebugLog ("Setup checkMetadata");
			if (!trglobals.instance.scriptLoaded)
				checkMetadata ();
			relationDD.captionText.text = trglobals.instance.projectRelation;
			mynameIF._inputfield.text = trglobals.instance.projectMyname;
			if (shownotes) {
				_settingsToggles [11].isOn = trglobals.instance.readActiveNotes == 1;
				_settingsToggles [12].isOn = trglobals.instance.displayActiveNotes == 1;
				settingPNL [0].SetActive (true);
			} else {
				settingPNL [0].SetActive (false);
			}
			if (showscene) {
				_settingsToggles [3].isOn = trglobals.instance.readPageNumbers == 1;
				_settingsToggles [4].isOn = trglobals.instance.readHeaderFooters == 1;
				_settingsToggles [5].isOn = trglobals.instance.readTitlePage == 1;
				_settingsToggles [6].isOn = trglobals.instance.readSceneHeaders == 1;
				_settingsToggles [7].isOn = trglobals.instance.readMoreCont == 1;
				settingPNL [4].SetActive (true);
			} else {
				settingPNL [4].SetActive (false);
			}
			if (showsettings) {
				settingPNL [1].SetActive (true);
				writersIF._inputfield.text = trglobals.instance.projectWriters;
				synopsisIF._inputfield.text = trglobals.instance.projectSynopsis;
				genreDD.captionText.text = trglobals.instance.projectGenre;
				mediumDD.captionText.text = trglobals.instance.projectPlatform;
				wgaIF._inputfield.text = trglobals.instance.projectWGA;
				copyrightIF._inputfield.text = trglobals.instance.projectCopyright;
				commendationIF._inputfield.text = trglobals.instance.projectCommendation;
			} else {
				settingPNL [1].SetActive (false);
			}
			if (showrehearsal) {
				_settingsToggles [13].isOn = trglobals.instance.narratorReadAll == 1;
				if (trglobals.instance.readNameSetting == 0) {
					_settingsToggles [0].isOn = true;_settingsToggles [1].isOn = false;_settingsToggles [2].isOn = false;
				} else if (trglobals.instance.readNameSetting == 1) {
					_settingsToggles [0].isOn = false;_settingsToggles [1].isOn = true;_settingsToggles [2].isOn = false;
				}
				else if (trglobals.instance.readNameSetting == 2) {
					_settingsToggles [0].isOn = false;_settingsToggles [1].isOn = false;_settingsToggles [2].isOn = true;
				}
				_settingsToggles [8].isOn = trglobals.instance.muteRehearsalDialouge == 1;
				_settingsToggles [9].isOn = trglobals.instance.pauseForDeliver == 1;
				_settingsToggles [10].isOn = trglobals.instance.playRehearsalScenesOnly == 1;
				settingPNL [3].SetActive (true);
				settingPNL [2].SetActive (true);
			} else {
				settingPNL [3].SetActive (false);
				settingPNL [2].SetActive (false);
			}
		}
		setPosition(true);
	}

	public void loadPDFLibrary() {
		setPosition(false);
		trglobals.instance._trpdf.Setup ();
	}

	void saveTRD(string TRDName,string name,string version,string pdf,string id,string TRDPath,bool fromsettings) {
		trglobals.instance.projectName = name;
		trglobals.instance.projectVersion = version;
		if (id == "") // check we don't laready hav eth ID
			id = trglobals.instance.projectID;
		if (id == "") {
			long currenttime = (long)System.DateTime.Now.TimeOfDay.TotalMilliseconds;
			int randomEnd = Random.Range (0, 100);
			int randomStart = Random.Range (0, 100);
			trglobals.instance.DebugLog (randomStart + ":" + currenttime + ":" + randomEnd);
			trglobals.instance.projectID = randomStart.ToString ("d3") + currenttime.ToString () + randomEnd.ToString ("d3");
		//	trglobals.instance.DebugLog("PROJECT SETUP NEED TO CREATE ID " + trglobals.instance.projectID);
		}
		else {
		//	trglobals.instance.DebugLog("PROJECT HAS ID -----------------------------------------------" + id);
			trglobals.instance.projectID = id;
		}
		trglobals.instance.CheckIDFolder ();
		trglobals.instance.projectWriters = writersIF._inputfield.text;
		trglobals.instance.projectGenre = genreDD.captionText.text;
		trglobals.instance.projectSynopsis = synopsisIF._inputfield.text;
		trglobals.instance.projectCommendation = commendationIF._inputfield.text;
		trglobals.instance.projectPDF = pdfTXT.text;
		trglobals.instance.projectPlatform = mediumDD.captionText.text;
		trglobals.instance.projectWGA = wgaIF._inputfield.text;
		trglobals.instance.projectCopyright = copyrightIF._inputfield.text;
		trglobals.instance.projectMyname = mynameIF._inputfield.text;
		PlayerPrefs.SetString ("myName", mynameIF._inputfield.text);
		PlayerPrefs.Save ();
		trglobals.instance.projectRelation = relationDD.captionText.text;
		// default versions
		if (!fromsettings) {
				trglobals.instance.readNameSetting = 0;
				trglobals.instance.playback_2X = 0;
				trglobals.instance.readSceneNumbers = 0;
				trglobals.instance.readPageNumbers = 0;
				trglobals.instance.readHeaderFooters = 0;
				trglobals.instance.readTitlePage = 0;
				trglobals.instance.readSceneHeaders = 1;
				trglobals.instance.readMoreCont = 0;
				trglobals.instance.readRevisedScenes = 0;
				trglobals.instance.muteRehearsalDialouge = 1;
				trglobals.instance.pauseForDeliver = 0;
				trglobals.instance.playRehearsalScenesOnly = 0;
				trglobals.instance.feedRehearsalLinesOnly = 0;
				trglobals.instance.readActiveNotes = 1;
				trglobals.instance.displayActiveNotes = 1;
				trglobals.instance.peakOverAction = 1;
				trglobals.instance.narratorReadAll = 0;
				trglobals.instance.scoreVolume = 60;
		} else {
			if (_settingsToggles [0].isOn)
				trglobals.instance.readNameSetting = 0;
			else if (_settingsToggles [1].isOn)
				trglobals.instance.readNameSetting = 1;
			else if (_settingsToggles [2].isOn)
				trglobals.instance.readNameSetting = 2;
			trglobals.instance.readPageNumbers = (_settingsToggles [3].isOn ? 1 : 0);
			trglobals.instance.readHeaderFooters = (_settingsToggles [4].isOn ? 1 : 0);
			trglobals.instance.readTitlePage = (_settingsToggles [5].isOn ? 1 : 0);
			trglobals.instance.readSceneHeaders = (_settingsToggles [6].isOn ? 1 : 0);
			trglobals.instance.readMoreCont = (_settingsToggles [7].isOn? 1 : 0);
			// scenes
			trglobals.instance.muteRehearsalDialouge = (_settingsToggles [8].isOn ? 1 : 0);
			trglobals.instance.pauseForDeliver = (_settingsToggles [9].isOn ? 1 : 0);
			trglobals.instance.playRehearsalScenesOnly = (_settingsToggles [10].isOn ? 1 : 0);
			//Notes
			trglobals.instance.readActiveNotes = (_settingsToggles [11].isOn ? 1 : 0);
			trglobals.instance.displayActiveNotes = (_settingsToggles [12].isOn ? 1 : 0);
			trglobals.instance.narratorReadAll = (_settingsToggles [13].isOn ? 1 : 0);
		}
		if (!trglobals.instance.scriptLoaded) {
			trglobals.instance.DebugLog ("SCRIPT NOT LOADED, GETTING SCORE FROM GENRE");
			getScoreFromGenre (genreDD.captionText.text);
		}
		trglobals.instance.createTRD (TRDPath);
	}

	public void getScoreFromGenre(string genre) {
		trglobals.instance.currentScore = 10;
		trglobals.instance.DebugLog ("getScoreFromGenre currentScore " + trglobals.instance.currentScore);
		for (int i = 0; i < genreTypes.Count; i++) {
			trglobals.instance.DebugLog (genre + ":" + genreTypes [i]);
			if (genre == genreTypes [i]) {
				trglobals.instance.currentScore = genreScores [i];
				trglobals.instance.DebugLog ("getScoreFromGenre Setting score " + trglobals.instance.currentScore);
			}
		}
	}

	public void processPDF(bool ignoreExists) {
	//	trglobals.instance._trvs.getPDF (pdfTXT.text);
		if (pdfTXT.text == "touch to select script") {
			trglobals.instance.ShowError ("You need to select a script to continue","PROJECT ERROR");
			return;
		}
		if (nameIF.text == "" || versionIF.text == "") {
			trglobals.instance.ShowError ("Project Name or Version number is blank","PROJECT ERROR");
			return;
		}
		string TRDName = nameIF.text+"_"+versionIF.text+".trd";
	//	trglobals.instance.DebugLog (TRDName);
		string TRDPath = System.IO.Path.Combine (Application.persistentDataPath, TRDName);
		if (!ignoreExists) {
			if (File.Exists (TRDPath)) {
			//	trglobals.instance.DebugLog ("Handle file exists ");
				trglobals.instance.ShowError ("Please change the name or version number of your project.","tableread project exists");
				return;
			}
		}
		saveTRD(TRDName,nameIF.text,versionIF.text,pdfTXT.text,"",TRDPath,ignoreExists);
	}

	public void hidefirst(Dropdown dd) {
		if (dd.value == 0) {
			dd.captionText.text = "";
		}
	}

	void resetInputFields() {
		genreDD.interactable = true; genreDD.captionText.color = Color.black;
		mediumDD.interactable = true;  mediumDD.captionText.color = Color.black;
		writersIF.clearBTN.gameObject.SetActive (true); writersIF._clickInputField.interactable = true; writersIF._inputfield.textComponent.color = Color.black;
		synopsisIF.clearBTN.gameObject.SetActive (true); synopsisIF._clickInputField.interactable = true; synopsisIF._inputfield.textComponent.color = Color.black;
		copyrightIF.clearBTN.gameObject.SetActive (true); copyrightIF._clickInputField.interactable = true; copyrightIF._inputfield.textComponent.color = Color.black;
		wgaIF.clearBTN.gameObject.SetActive (true); wgaIF._clickInputField.interactable = true; wgaIF._inputfield.textComponent.color = Color.black;
		commendationIF.clearBTN.gameObject.SetActive (true);commendationIF._clickInputField.interactable = true;commendationIF._inputfield.textComponent.color = Color.black;
	}

	public void checkMetadata(string p = "") {
	//	trglobals.instance.DebugLog ("P is " + p);
		resetInputFields ();
		if (string.IsNullOrEmpty(p)) {
			p = System.IO.Path.Combine (Application.persistentDataPath, pdfTXT.text);
			if (!File.Exists (p))
				p = System.IO.Path.Combine (trglobals.instance._trpdf.downloadFolderPath, pdfTXT.text);
		} else {
		//	trglobals.instance.DebugLog ("I have a string checking now");
			string pth = p;
			if (!File.Exists (pth))
				p = System.IO.Path.Combine (Application.persistentDataPath, pth);
			//trglobals.instance.DebugLog ("P now is " + p);
			if (!File.Exists (p))
				p = System.IO.Path.Combine (trglobals.instance._trpdf.downloadFolderPath, pth);
			//trglobals.instance.DebugLog ("P from downloaded folder is " + p + ":" + trglobals.instance._trpdf.downloadFolderPath);
		}
	//	trglobals.instance.DebugLog("CHECKING METADATA " + p);
		PdfReader reader = new PdfReader(p);
		Dictionary<string, string> info = reader.Info;
		string md = "";
		if (info.TryGetValue ("Title", out md)) {
		//	trglobals.instance.DebugLog(md);
			if (md.Length > 0) {
				string[] minfo = md.Split(new string[] { "::" }, System.StringSplitOptions.None);
			//	trglobals.instance.DebugLog (minfo.Length + ":" + minfo [0]);
				if (minfo.Length == 7 && minfo [0].Equals ("TRD")) {
					//we only want to adjust the project name and version if its a new project
					if (!trglobals.instance.scriptLoaded) {
						nameIF.text = minfo [1]; trglobals.instance.projectName = minfo [1];
						versionIF.text = minfo [2];trglobals.instance.projectVersion = minfo [2];
						trglobals.instance.projectID = minfo [3];trglobals.instance.projectID = minfo [3];
						trglobals.instance.CheckIDFolder ();
					} else
						nameIF.text = trglobals.instance.projectName;
					if (!string.IsNullOrEmpty (minfo [4])) {
						if (!trglobals.instance.scriptLoaded)
							trglobals.instance.projectCopyright = minfo [4];
						copyrightIF._inputfield.text = minfo [4];	
						copyrightIF._clickInputField.interactable = false;
						copyrightIF.clearBTN.gameObject.SetActive (false);
						copyrightIF._inputfield.textComponent.color = Color.grey;
					}
					if (!string.IsNullOrEmpty (minfo [5])) {
						if (!trglobals.instance.scriptLoaded)
							trglobals.instance.projectWGA = minfo [5];
						wgaIF._inputfield.text = minfo [5];	
						wgaIF._clickInputField.interactable = false;
						wgaIF.clearBTN.gameObject.SetActive (false);
						wgaIF._inputfield.textComponent.color = Color.grey;
					}
					if (!string.IsNullOrEmpty (minfo [6])) {
						if (!trglobals.instance.scriptLoaded)
							trglobals.instance.projectCommendation = minfo [6];
						commendationIF._inputfield.text = minfo [6];	
						commendationIF._clickInputField.interactable = false;
						commendationIF.clearBTN.gameObject.SetActive (false);
						commendationIF._inputfield.textComponent.color = Color.gray;
					}
				} else {
					if (trglobals.instance.projectVersion != "")
						versionIF.text = trglobals.instance.projectVersion;
					if (trglobals.instance.projectName != "")
						nameIF.text = trglobals.instance.projectName;
					if (trglobals.instance.projectWGA != "")
						wgaIF._inputfield.text = trglobals.instance.projectWGA;
					if (trglobals.instance.projectCopyright != "")
						copyrightIF._inputfield.text = trglobals.instance.projectCopyright;
					if (trglobals.instance.projectCommendation!= "")
						commendationIF._inputfield.text = trglobals.instance.projectCommendation;
				}
			}
		}
		md = "";
		if (info.TryGetValue ("Author", out md)) {
			string[] minfo = md.Split(new string[] { "::" }, System.StringSplitOptions.None);
			if (minfo.Length == 5 && minfo [0].Equals ("TRD")) {
				if (!string.IsNullOrEmpty (minfo [1])) {
					if (!trglobals.instance.scriptLoaded)
						trglobals.instance.projectGenre = minfo [1];
					genreDD.captionText.text = minfo [1];
					genreDD.interactable = false;
					genreDD.captionText.color = Color.grey;
				}
				if (!string.IsNullOrEmpty (minfo [2])) {
					if (!trglobals.instance.scriptLoaded)
						trglobals.instance.projectPlatform = minfo [2];
					mediumDD.captionText.text = minfo [2];
					mediumDD.interactable = false;
					mediumDD.captionText.color = Color.grey;
				}
				if (!string.IsNullOrEmpty (minfo [3])) {
					if (!trglobals.instance.scriptLoaded)
						trglobals.instance.projectWriters = minfo [3];
					writersIF._inputfield.text = minfo [3];
					writersIF._clickInputField.interactable = false;
					writersIF._inputfield.textComponent.color = Color.grey;
					writersIF.clearBTN.gameObject.SetActive (false);
				}
				if (!string.IsNullOrEmpty (minfo [4])) {
				//	setscorefromtrd = true;
					trglobals.instance.currentScore = int.Parse (minfo [4]);
					trglobals.instance.DebugLog ("checkMetadata currentScore" + trglobals.instance.currentScore);
				}
			} else {
				genreDD.captionText.text = trglobals.instance.projectGenre;
				mediumDD.captionText.text = trglobals.instance.projectPlatform;
				writersIF._inputfield.text = trglobals.instance.projectWriters;
			}
		}
		md = "";
		if (info.TryGetValue ("Subject", out md)) {
			string[] minfo = md.Split(new string[] { "::" }, System.StringSplitOptions.None);
			if (minfo.Length == 2 && minfo [0].Equals ("TRD")) {
				if (!string.IsNullOrEmpty (minfo [1])) {
					if (!trglobals.instance.scriptLoaded)
						trglobals.instance.projectSynopsis = minfo [1];
					synopsisIF._inputfield.text = minfo [1];
					synopsisIF._clickInputField.interactable = false;
					synopsisIF._inputfield.textComponent.color = Color.gray;
					synopsisIF.clearBTN.gameObject.SetActive (false);
				}
			} else {
				synopsisIF._inputfield.text = trglobals.instance.projectSynopsis;
			}
		}
		md = "";
		if (info.TryGetValue ("Keywords", out md)) {
			trglobals.instance._trdactors.Clear ();
			string[] split = md.Split(new string[] { "::" }, System.StringSplitOptions.None);
			if (split.Length >= 4 && split [0].Equals ("TRD")) {
				int i = 1;
				while (i < split.Length) {
					string scriptname = split[i];i++;string tabrname = split[i];i++;
					float rate = float.Parse(split[i]);i++;
					float shape = float.Parse(split[i]);i++;
				//	trglobals.instance.DebugLog("FOUND PROJECT ACTOR IN METADATA " + scriptname + ":" + tabrname + ":" + rate + ":" + shape);
					// check its narrator
					trglobals.instance.AddTRDActor(scriptname,tabrname,rate,shape);
				}
			}
		}
	//	trglobals.instance.DebugLog("PROJECT ID:" + trglobals.instance.projectID + ":");
	}
}
