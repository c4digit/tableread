using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using TheNextFlow.UnityPlugins;

public class trglobals : MonoBehaviour {

	public	CanvasGroup	_main;
	public	Canvas	_canvas;
	public	UnityEngine.UI.CanvasScaler	_cs;
	public	sleep	_sleep;

	public void DebugLog(string s) {
	//	Debug.Log (s);
	}

	#if UNITY_ANDROID
	public void Sleep() {
		//_sleep.SetSleeping ();
	//	DebugLog ("Sleep");
		Screen.fullScreen = true;
		_main.alpha = 0;
		_main.interactable = false;
		_canvas.targetDisplay = 1;
		Application.targetFrameRate = 10;
	}

	public void WakeUp() {
		Application.targetFrameRate = 60;
	//	DebugLog ("WakeUp");
		_canvas.targetDisplay = 0;
		Screen.fullScreen = false;
		_main.alpha = 1;
		_main.interactable = true;
		_sleep.sleeping = false;
		// Lets kick it all off again
	//	#if UNITY_ANDROID
		trglobals.instance._sleep.invokeSleep ();
	//	#endif
	}
	#endif

	public	bool goPRO = false;
	public	UnityEngine.UI.Image	homeBTNIMG;
	public	Sprite[] 				homeSPR;
	public	GameObject[]			_subscrBTN;
	public	bool 					continueScript = true;
	public	AudioSource				_audioSource;
	public	DemoManager 			_iosTTS;
	//public	Purchaser _putchaser;
	public	List<tablereadActor>	_tablereadActors = new List<tablereadActor>();

	public tablereadActor getAcaActor(string n) {
		if (_tablereadActors.Count < 45) {
		//	DebugLog("NEED TO RELOAD TR ACTORS");
			loadTablereadActors();
		}
		//DebugLog ("Fidning tablrereadactor " + n);
		for (int i = 0; i < _tablereadActors.Count; i++) {
			if (_tablereadActors[i].tabrname.Equals(n)) {
				return _tablereadActors[i];
			}
		}
		return null;
	}

	public static string importedFile;
	public	string voiceURLFolder = "https://s3.amazonaws.com/trsvoices/";
	public bool	speedVoice = false;
	public	bool	scriptLoaded = false;
	void loadTablereadActors() {
		// lets set up tableread actors now
		_tablereadActors.Clear();
		_tablereadActors.Add (new tablereadActor ("Matthew(USA)", "Micah", "Peter", "M", "ADULT", 200, 100, 11, "hq-lf-USEnglish-Micah-22khz","enu_micah_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Mel(USA)","Tracy","Peter","F","ADULT",200,100,16,"hq-lf-USEnglish-Tracy-22khz","enu_hd_tracy_22k_lf.bvcu"));

		_tablereadActors.Add(new tablereadActor("Gavin(USA)","Will","Peter","M","ADULT",200,100,17,"hq-lf-USEnglish-Will-22khz","enu_hd_will_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Katie(USA)","Laura","Peter","F","ADULT",200,100,10,"hq-lf-USEnglish-Laura-22khz","enu_hd_laura_22k_lf.bvcu"));

		_tablereadActors.Add(new tablereadActor("Aaron(AUS)","Tyler","Peter","M","ADULT",200,100,1,"hq-lf-AustralianEnglish-Tyler-22khz","en_au_hd_tyler_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Sarah(AUS)","Lisa","Peter","F","ADULT",200,100,0,"hq-lf-AustralianEnglish-Lisa-22khz","en_au_hd_lisa_22k_lf.bvcu"));

		_tablereadActors.Add(new tablereadActor("Christopher(USA)","Saul","Peter","M","ADULT",200,100,14,"hq-lf-USEnglish-Saul-22khz","enu_saul_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Chelsea(UK)","Rachel","Peter","F","ADULT",200,100,8,"hq-lf-British-Rachel-22khz","eng_hd_rachel_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Michael(UK)","Graham","Peter","M","ADULT",200,100,4,"hq-lf-British-Graham-22khz","eng_hd_graham_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Helen(UK)","Lucy","Peter","F","ADULT",200,100,5,"hq-lf-British-Lucy-22khz","eng_hd_lucy_22k_lf.bvcu"));

		_tablereadActors.Add(new tablereadActor("Armin(UK)","Nizareng","Peter","M","ADULT",200,100,6,"hq-lf-British-Nizareng-22khz","eng_hd_nizareng_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Scarlett(USA)","Karen","Peter","F","ADULT",200,100,9,"hq-lf-USEnglish-karen-22khz","enu_karen_22k_ns.bvcu"));

		_tablereadActors.Add(new tablereadActor("Paul(USA)","Rod","Peter","M","ADULT",200,100,12,"hq-lf-USEnglish-Rod-22khz","enu_hd_rod_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Rebecca(USA)","Sharon","Peter","F","ADULT",200,100,15,"hq-lf-USEnglish-sharon-22khz","enu_hd_sharon_22k_lf.bvcu"));

		_tablereadActors.Add(new tablereadActor("Brad(USA)","Ryan","Peter","M","ADULT",200,100,13,"hq-lf-USEnglish-Ryan-22khz","enu_hd_ryan_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Tanya(IND)","Deepa","Peter","F","ADULT",200,100,2,"hq-lf-IndianEnglish-deepa-22khz","en_in_hd_deepa_22k_lf.bvcu"));

		_tablereadActors.Add(new tablereadActor("Dan(USA)","Willoldman","Peter","M","SENIOR",220,100,28,"hq-lf-USEnglish-Willoldman-22khz","enu_willoldman_22k_ns.bvcu"));
		_tablereadActors.Add (new tablereadActor ("Robin(SCO)", "Rhona","Peter", "F", "ADULT", 200, 100, 3, "hq-lf-ScottishEnglish-rhona-22khz","en_sct_hd_rhona_22k_lf.bvcu"));

		_tablereadActors.Add(new tablereadActor("Roger(UK)","Peter","Peter","M","ADULT",225,90,27,"hq-lf-British-Peter-22khz","eng_hd_peter_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("James(AUS)","Tyler","Peter","M","ADULT",220,89,26,"hq-lf-AustralianEnglish-Tyler-22khz","en_au_hd_tyler_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Tony(USA)","Will","Peter","M","ADULT",240,90,22,"hq-lf-USEnglish-Will-22khz","enu_hd_will_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Kunal(IND)","Deepa","Peter","M","ADULT",235,85,23,"hq-lf-IndianEnglish-deepa-22khz","en_in_hd_deepa_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Bruce(USA)","Rod","Peter","M","ADULT",230,87,19,"hq-lf-USEnglish-Rod-22khz","enu_hd_rod_22k_lf.bvcu"));
		_tablereadActors.Add(new tablereadActor("Alfred(USA)","Willbadguy","Peter","M","SENIOR",256,93,24,"hq-lf-USEnglish-willbadguy-22khz","enu_willbadguy_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Clint(USA)","Willbadguy","Peter","M","SENIOR",290,100,25,"hq-lf-USEnglish-willbadguy-22khz","enu_willbadguy_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Harold(UK)","Peter","Peter","M","ADULT",200,100,7,"hq-lf-British-Peter-22khz","eng_hd_peter_22k_lf.bvcu"));

		_tablereadActors.Add (new tablereadActor ("Gavin Gruff(USA)", "Willbadguy", "Peter", "M", "ADULT", 200, 100, 18, "hq-lf-USEnglish-willbadguy-22khz","enu_willbadguy_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Eugene(USA)","Willoldman","Peter","M","SENIOR",260,114,20,"hq-lf-USEnglish-Willoldman-22khz","enu_willoldman_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Pearl(USA)","Willoldman","Peter","F","SENIOR",200,131,21,"hq-lf-USEnglish-Willoldman-22khz","enu_willoldman_22k_ns.bvcu"));

		//CHILDREN
		_tablereadActors.Add(new tablereadActor("Max(AUS)","Liam","Peter","M","CHILD",200,100,29,"hqm-ref-AustralianEnglish-Liam-22khz","en_au_liam_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Indiana(AUS)","Olivia","Peter","F","CHILD",200,100,30,"hqm-ref-AustralianEnglish-Olivia-22khz","en_au_olivia_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Harry(UK)","Harry","Peter","M","CHILD",200,100,31,"hqm-ref-British-Harry-22khz","eng_harry_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Rosie(UK)","Rosie","Peter","F","CHILD",200,100,32,"hqm-ref-British-Rosie-22khz","eng_rosie_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Emily(USA)","Ella","Peter","F","CHILD",200,100,33,"hqm-ref-USEnglish-Ella-22khz","enu_ella_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Emilio(USA)","Emilio-English","Peter","M","CHILD",200,100,34,"hqm-ref-USEnglish-Emilio-English-22khz","enu_emilioenglish_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Arnold(USA)","Josh","Peter","M","CHILD",200,100,35,"hqm-ref-USEnglish-Josh-22khz","enu_josh_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Chloe(USA)","Valeria-English","Peter","F","CHILD",200,100,36,"hqm-ref-USEnglish-Valeria-English-22khz","enu_valeriaenglish_22k_ns.bvcu"));
		//TEEN
		_tablereadActors.Add(new tablereadActor("Zac(USA)","scott","Peter","M","TEEN",200,100,37,"hqm-ref-USEnglish-scott-22khz","enu_scott_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Austin(AUS)","Liam","Peter","M","TEEN",220,85,38,"hqm-ref-AustralianEnglish-Liam-22khz","en_au_liam_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Hannah(AUS)","Olivia","Peter","F","TEEN",220,85,39,"hqm-ref-AustralianEnglish-Olivia-22khz","en_au_olivia_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Charlie(UK)","Harry","Peter","M","TEEN",220,85,40,"hqm-ref-British-Harry-22khz","eng_harry_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Angela(UK)","Rosie","Peter","F","TEEN",220,85,41,"hqm-ref-British-Rosie-22khz","eng_rosie_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Belinda(USA)","Ella","Peter","F","TEEN",220,85,42,"hqm-ref-USEnglish-Ella-22khz","enu_ella_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Sonny(USA)","Emilio-English","Peter","M","TEEN",220,85,43,"hqm-ref-USEnglish-Emilio-English-22khz","enu_emilioenglish_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Wade(USA)","Josh","Peter","M","TEEN",220,85,44,"hqm-ref-USEnglish-Josh-22khz","enu_josh_22k_ns.bvcu"));
		_tablereadActors.Add(new tablereadActor("Penny(USA)","Valeria-English","Peter","F","TEEN",220,85,45,"hqm-ref-USEnglish-Valeria-English-22khz","enu_valeriaenglish_22k_ns.bvcu"));
		// check anuy started voice downloads have finished
		checkVoiceDownloads();
	}

	public void checkVoiceDownloads() {
		for (int i = 0; i < _tablereadActors.Count; i++) {
			string key = voiceURLFolder + _tablereadActors[i].acaName.ToLower () +".zip";
			if (PlayerPrefs.HasKey (key)) {
				float pr = trglobals.instance._trscr._muc.GetProgress(key);
			//	DebugLog("STARTED DOWNLOAD FOR " + _tablereadActors[i].acaName + ":PROGRESSS " + pr);
				if (pr == 1) {
					string dir = Path.Combine (Application.persistentDataPath, "voices");
					string destZip = Path.Combine(dir,(_tablereadActors[i].acaName.ToLower() + ".zip"));
					if (File.Exists (destZip)) {
					//	DebugLog ("+++++++++++++++NEED TO UNZIP TO FOLDER NOW " + destZip);
						ZipUtil.Unzip (destZip, (Path.Combine (Application.persistentDataPath, "voices")));
						File.Delete (destZip);
						//DebugLog ("+++++++++++++++FIN UNZIP " + destZip);
					}
				//	else
				//		DebugLog ("______________-____NO ZIP FILE" + destZip);
					PlayerPrefs.DeleteKey (key);
					PlayerPrefs.Save ();
				}
				//StartCoroutine(checkProgress(key));
			}
		}
	}

	public List<string> getVoicesfromAcavoice(string a) {
		List<string> v = new List<string> ();
		for (int i = 0; i < _tablereadActors.Count; i++) {
			if (_tablereadActors [i].acaName == a) {
				v.Add (_tablereadActors [i].tabrname);
			}
		}
		return v;
	}

	public bool hasAcaFolder(string tbn) {
	//	DebugLog ("FINDING FOLDER FOR" + tbn);
		for (int i = 0; i < _tablereadActors.Count; i++) {
			if (tbn == _tablereadActors [i].tabrname) {
				string p = Path.Combine(Path.Combine(Application.persistentDataPath,"voices"),_tablereadActors [i].acaFolder);
			//	DebugLog ("LOOKOIG FOR " + p);
				if (Directory.Exists (p))
					return true;
			}
		}
		return false;
	}
	public	ttsbuttonscript				_tts;
	public	ttsbuttonscript_ios			_tts_ios;
	public	tr_frontpage				_trfp;
	public	tr_msp						_trmsp;
	public	tr_sdt						_trsdt;
	public	tr_viewscript				_trvs;
	public	tr_pdf						_trpdf;
	public	tr_abt						_trabt;
	public	tr_score					_trscr;
	public	tr_scn						_trscn;
	public	tr_act						_tract;
	public	tr_dwa						_trdwa;
	public	tr_sav						_trsav;
	public	tr_nte						_trnte;
	public	tr_nnt						_trnnt;
	public	tr_fnt						_trfnt;
	public	tr_exp						_trexp;
	public	tr_sub						_trsub;
	public	tr_dfv						_trdfv;
	public	scriptController			_scriptController;

	public	UnityEngine.UI.Text			_heading;
	public	UnityEngine.UI.Text			_backTXT;
	public	UnityEngine.UI.RectMask2D 	_mask;
	// music
	public string[] musicTitles = {
		"a_small_problem","headhunters","northbound_suspect","over_berlin_skies","the_last_flight","thrillbound","all_a_dream","first_winter","hearts_and_memories",
		"past_lives","the_stars_are_dreaming","wise_traveler","a_big_start","count_of_three","knowing_you","the_main_feature","epic_fantsay","jeremiad","ultimate_will",
		"kingdom_hearts","dark_recesses","missing_evidence","no_way_out"
	};


	public void CheckIDFolder() {
		if (!Directory.Exists (Path.Combine (Application.persistentDataPath, projectID))) {
		//	DebugLog ("and I need to cfreate ID folder===================");
			Directory.CreateDirectory (Path.Combine (Application.persistentDataPath, projectID));
		}
	//	else
		//	DebugLog ("ID folder EXISTS-----------------");
	}

	public bool showedmissingscrores;

	public string getMusicTitle(int v) {
		string track = musicTitles [v] + ".mp3";
		DebugLog ("SEARCHINFG FOR TRACK " + track);
		if (!File.Exists (Path.Combine (Path.Combine (Application.persistentDataPath, "scores"), track))) {
			track = "the_stars_are_dreaming.mp3";
		/*	if (!showedmissingscrores && goPRO) {
				showedmissingscrores = true;
				string m = "Attempting to load track '" + displayMusicTitles [v] + "'.\nPlease download from the 'Scores' page if you would like to use this track.\nDefaulting to 'The Stars Are Dreaming'.";
				//ShowError (m, "MISSING SCORE");
				TheNextFlow.UnityPlugins.Mo() => { 
						DebugLog ("CANCEL was pressed");
					}bileNativePopups.OpenAlertDialog (
					"Missing Score", m,
					"CANCEL", "DOWNLOAD SCORE",
					() => { 
						DebugLog ("CANCEL was pressed");
					},
					() => { 
						DebugLog ("DOWNLOAD VOICES was pressed"); 
						trglobals.instance._trvs.showScoresPage ();
					});
			}*/
		}
	//	DebugLog ("RETURNING TRACK " + track);
		return track;
	}

	public  string[] displayMusicTitles = {
		"A Small Problem","Headhunters","Northbound Suspect","Over Berlin Skies","The Last Flight","Thrillbound","All a Dream","First Winter","Hearts and Memories",
		"Past Lives","The Stars Are Dreaming","Wise Traveler","A Big Start","Count of Three","Knowing You","The Main Feature","Epic Fantsay","Jeremiad","Ultimate Will",
		"Kingdom Hearts","Dark Recesses","Missing Evidence","No Way Out"
	};
		
	// TRD SETTINGS
	public	string	projectID = "";
	public	string	projectName = "";
	public	string	projectVersion = "";
	public	string	projectWriters = "";
	public	string	projectGenre = "";
	public	string	projectSynopsis = "";
	public	string	projectCommendation = "";
	public	string	projectPDF = "";
	public	string	projectPlatform = "";
	public	string	projectWGA = "";
	public	string	projectCopyright = "";
	public	string	projectMyname = "";
	public	string	projectRelation = "";
	// script settings	
	public int	readNameSetting;
	public int	playback_2X;
	public int	readSceneNumbers;
	public int	readPageNumbers;
	public int	readHeaderFooters;
	public int	readTitlePage;
	public int	readSceneHeaders;
	public int	readMoreCont;
	public int	readRevisedScenes;
	public int	muteRehearsalDialouge;
	public int	pauseForDeliver;
	public int	playRehearsalScenesOnly;
	public int	feedRehearsalLinesOnly;
	public int	readActiveNotes;
	public int	displayActiveNotes;
	public int	narratorReadAll;
	public int	peakOverAction;
	public int	scoreVolume;
	public int	currentScore;
	public	bool	muteScore = false;


	public	static	trglobals	instance;
	public	string		trnarrator = "t4bl3r34dn4rr4t0r";
	public	int			screenwidth;
	public	float		pdfRatio;
	public 	float		pdfWidth;
	public 	float		pdfHeight;
	public	float		actionmargin;
	public	float		actormargin;
	public	float		dialougemargin;
	public	float		dialougemarginR;
	public	float		parenthiticalmargin;
	public	float		transitionmargin;
	public	float		rightMargin;
	public	Color[]		scenelineCLR;
	public	Color		unselectedButtonCLR;
	public	Color		progressIMGCLR;
	public enum SCRIPTLINE_TYPE
	{
		NORMAL = 0,
		SCENE = 1,
		ACTOR = 2,
		HEADERFOOTER = 3,
		DIALOUGE = 4,
		PAGENUMBER = 5,
		PARENTHETICAL = 6,
		TRANSITION = 7,
		MORECONT = 8
	}

	public bool	reloadScript = false;

	public	panel	_currentPanel;	
	public Color	rehearseCol;
	// set up list of panels
	public	List<panel>	_lastPanel = new List<panel>();
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		instance = this;
		_mask.enabled = true;
		loadTablereadActors ();
		if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "scores")))
			Directory.CreateDirectory (Path.Combine(Application.persistentDataPath, "scores"));
		string voicefolder = Path.Combine (Application.persistentDataPath, "voices");
		if (!Directory.Exists (voicefolder)) {
			Directory.CreateDirectory(voicefolder);
		}
		//screenwidth = Screen.width;
	//	DebugLog ("screenwidth---------" + screenwidth);
		rightMargin = iTextSharp.text.Utilities.InchesToMillimeters (1);
		actionmargin = iTextSharp.text.Utilities.InchesToMillimeters (1.5f);
		actormargin = iTextSharp.text.Utilities.InchesToMillimeters (3.7f);
		// make a little wider
		parenthiticalmargin = iTextSharp.text.Utilities.InchesToMillimeters (3.1f);
		dialougemargin = iTextSharp.text.Utilities.InchesToMillimeters (2.5f);
		dialougemarginR = iTextSharp.text.Utilities.InchesToMillimeters (2.5f);
		transitionmargin = iTextSharp.text.Utilities.InchesToMillimeters (6);
		// check folders 
		if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "voices")))
			Directory.CreateDirectory (Path.Combine(Application.persistentDataPath, "voices"));
	/*	if (!Directory.Exists (Path.Combine(Path.Combine (Application.persistentDataPath, "voices"),peterfolder))) {
			StartCoroutine (CopyAndUnzip (peterfolder));
		} 
		if (!Directory.Exists (Path.Combine(Path.Combine (Application.persistentDataPath, "voices"),micahfolder))) {
			StartCoroutine(CopyAndUnzip(micahfolder));
		}
		if (!Directory.Exists (Path.Combine(Path.Combine (Application.persistentDataPath, "voices"),tracyfolder))) {
			StartCoroutine(CopyAndUnzip(tracyfolder));
		} */
		// copy music
		string dst = Path.Combine(Path.Combine (Application.persistentDataPath, "scores"),"the_stars_are_dreaming.mp3");
		if (!File.Exists(dst)) {
		//	File.copy
			string src = Path.Combine (Application.streamingAssetsPath, "the_stars_are_dreaming.mp3");
			StartCoroutine(CopyFile(src,dst));
		}

		#if UNITY_IOS && !UNITY_EDITOR
		DebugLog("______________________HERE " + Screen.width +":"+ Screen.height);
		if (Screen.width == 640 && Screen.height == 960) {
			DebugLog("4S setting resolution");
			_cs.referenceResolution = new Vector2(900,1550);
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		float screenratio = ((float)Screen.width/(float)Screen.height)/(9.0f/16.0f);
		DebugLog("______________________ANDROID " + screenratio);
		_cs.matchWidthOrHeight = 0;
		_cs.referenceResolution = new Vector2((1050 * screenratio),1800);
		#endif
	}

	public void CheckImportedFile() {
		resetSettings ();
		if (!string.IsNullOrEmpty(importedFile)) {
			if (importedFile.Contains (".trn")) {
				//	HandleTRN (importedFile);
				importedFile = "";
			} else if (importedFile.Contains (".pdf") || importedFile.Contains (".PDF")){
				// its a PDF
				// lets sert up metadata
				resetSettings();
				_trsdt.checkMetadata(System.IO.Path.Combine (Application.persistentDataPath,importedFile));
				//DebugLog ("FIN CHECKING FOR MEYTADATA");
				if (string.IsNullOrEmpty (projectID)) {
					// no metadata
					nonTRRPDF();
				} else {
					TRRPDF();
				}
			}
		}
		importedFile = "";
	}

/*	IEnumerator checkInternetConnection(){
		WWW www = new WWW("https://google.com");
		yield return www;
		Debug.Log (www.text + ":" + www.error + ":");
		if (www.error != null) {
			DebugLog ("----------------checkInternetConnection " + PlayerPrefs.GetInt ("tablreadhaspro"));
			if (PlayerPrefs.GetInt ("tablreadhaspro") == 0) {
				trglobals.instance._trsub.turnOffPro (false);
			} else {
				DebugLog ("Turning on PRO");
				trglobals.instance._trsub.turnOnPro (false);
			}
		} else {
			Debug.Log ("NO INTERNET");
			trglobals.instance._trsub.turnOffPro (false);
			PlayerPrefs.SetInt ("tablreadhaspro", 0);
			PlayerPrefs.Save ();
		}
	} */

	IEnumerator Start() {
		if (PlayerPrefs.GetInt ("mutescore") == 0)
			muteScore = false;
		else
			muteScore = true;
		Screen.fullScreen = false;
		Application.targetFrameRate = 60;
		yield return new WaitForEndOfFrame ();
	//	trglobals.instance._trsub.turnOffPro (false);
	//	PlayerPrefs.SetInt ("tablreadhaspro", 0);
	//	PlayerPrefs.Save ();
	//	 _trfp.Setup();
	}

	void resetSettings() {
		projectID = "";
		projectGenre = "";
		projectCommendation = "";
		projectCopyright = "";
		projectName = "";
		projectVersion = "";
		projectWGA = "";
		projectWriters = "";
		projectSynopsis = "";
		projectMyname = "";
		projectRelation = "";
		projectPDF = "";
		projectPlatform = "";
		readNameSetting = 0;
		playback_2X = 0;
		readSceneNumbers = 0;
		readPageNumbers = 0;
		readHeaderFooters = 0;
		readTitlePage = 0;
		readSceneHeaders = 1;
		readMoreCont = 0;
		readRevisedScenes = 0;
		muteRehearsalDialouge = 1;
		pauseForDeliver = 0;
		playRehearsalScenesOnly = 0;
		feedRehearsalLinesOnly = 0;
		readActiveNotes = 1;
		displayActiveNotes = 1;
		peakOverAction = 1;
		narratorReadAll = 0;
		scoreVolume = 60;
		currentScore = 10;
		DebugLog ("resetSettings currentScore" + trglobals.instance.currentScore);
	}

	void TRRPDF() {
		projectPDF = importedFile;
		MobileNativePopups.OpenAlertDialog (
			"PDF appears to be\ntableread ready", "Do you want to create a tableread project?",
			"NO", "YES",
			() => { 
				//DebugLog ("NO PRESSED");
			},
			() => { 
				//DebugLog ("YES PRESSED"); 
				TRRCreateProject();
			});
	}
	void TRRCreateProject() {
		string filename = projectName + "_" + projectVersion + ".trd";
	//	DebugLog ("filename = " + filename);
		string TRDPath = Path.Combine (Application.persistentDataPath, filename);
		if (!File.Exists (TRDPath)) {
			TRRPDFProject = true;
			createTRD (TRDPath);
		}
		else {
			//trd exists handle
			MobileNativePopups.OpenAlertDialog (
				"Project Exists", "Do you want to overwrite it?",
				"NO", "YES",
				() => { 
					//DebugLog ("NO PRESSED");
				},
				() => { 
					//DebugLog ("Project Exists YES PRESSED " + TRDPath); 
					overwriteTRRProject(TRDPath);
				});
		}
	}

	void overwriteTRRProject(string TRDPath) {
	//	DebugLog ("OVERWRITING FILE NEED TO CHECK PROJECTS NOTES AND MYNAME AND RELATION");
		string oldid = getProjectID (TRDPath);
		DebugLog ("IDs:" + oldid + ":" + projectID);
		projectMyname = getProjectMyName (TRDPath);
		if (string.IsNullOrEmpty (projectMyname))
			projectMyname = PlayerPrefs.GetString ("myName");
		projectRelation = getProjectMyRelation (TRDPath);
		//DebugLog (projectMyname + ":" + projectRelation);
		if (oldid.Equals (projectID)) {
			// projects match lets create a trd file
			TRRPDFProject = true;
			createTRD (TRDPath);
		} else {
			// need to do some shit here
			//DebugLog("NEED TO OVERWRITE");
			MobileNativePopups.OpenAlertDialog (
				"Project IDs differ", "Do you want to copy in any exsisting Notes?",
				"NO", "YES",
				() => { 
				//	DebugLog ("NO PRESSED");
				},
				() => { 
					//DebugLog ("Project Exists YES PRESSED " + TRDPath); 
					copyExsistingNotes(TRDPath,oldid);
				});
		}
	}

	void copyExsistingNotes(string TRDPath, string oldid) {
	//	DebugLog("COPYING IN NOTES");
		// move notes from folder to folder
		string oldIDPath = Path.Combine(Application.persistentDataPath,oldid);
		string newIDPath = Path.Combine (Application.persistentDataPath, projectID);
		if (!Directory.Exists (newIDPath))
			Directory.CreateDirectory (newIDPath);
		// list files
		DirectoryInfo dir = new DirectoryInfo(oldIDPath);
		FileInfo[] info = dir.GetFiles("*.trn");
		foreach (FileInfo f in info) {
		//	DebugLog ("Moving " + f.Name);
			File.Move (f.FullName, Path.Combine (newIDPath, f.Name));
		}
		// lets check any other trd's don't use this old id
		dir = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] infotrd = dir.GetFiles("*.trd");
		foreach (FileInfo f in infotrd) {
			//DebugLog ("NEED TO ADJUST TRD " + f.Name);
			checkProjectIDandUpdate (f.FullName,oldid);
		}
		// remove old id folder
		DeleteDirectory (oldIDPath);
		// create trd file
		TRRPDFProject = true;
		createTRD (TRDPath);
	}

	void nonTRRPDF() {
		projectPDF = importedFile;
		string _tempname = Path.GetFileNameWithoutExtension (importedFile);
		MobileNativePopups.OpenAlertDialog (
			"IMPORT PDF", "Do you want to link PDF to\nan existing tableread project?",
			"NO", "YES",
			() => { 
				//DebugLog ("NO PRESSED");
				nonTRRCreateProject(_tempname);
			},
			() => { 
				//DebugLog ("YES PRESSED"); 
				selectingPDFProject = true;
				_trmsp.Setup();
			});
	}

	void nonTRRCreateProject(string n) {
	//	DebugLog (" nonTRRCreateProject " + n + ":" + projectPDF +":");
		projectName = n;
		_trmsp.loadScriptDetails ();
	}

	[System.Serializable]
	public class trdactor {
		public string scriptname;
		public string tabrname;
		public float rate;
		public float shape;
		public trdactor(string n, string tn, float r, float s) {
			scriptname = n; tabrname = tn; rate = r; shape = s;
		}
	}

	public List<trdactor> _trdactors = new List<trdactor>();

	public string getProjectID(string file) {
		TextReader tr = new StreamReader(file);
		string trdData = tr.ReadToEnd ();
		string[] split = trdData.Split(new string[] { ";;" }, System.StringSplitOptions.None);
		string id = split [3];
		tr.Close ();
		return id;
	}

	public void checkProjectIDandUpdate(string file, string oldid) {
		TextReader tr = new StreamReader(file);
		string trdData = tr.ReadToEnd ();
		string[] split = trdData.Split(new string[] { ";;" }, System.StringSplitOptions.None);
		string id = split [3];
		tr.Close ();
		if (id.Equals (oldid)) {
			string newcontent = split [0];
			for (int i = 1; i < 3; i++) {
				newcontent = newcontent + ";;"+ split[i];
			}
			newcontent = newcontent + ";;" + projectID;
			for (int i = 4; i < split.Length; i++) {
				newcontent = newcontent + ";;"+ split[i];
			}
			TextWriter tw = new StreamWriter(file);
			tw.Write(newcontent);
			tw.Close();
		}
	}
		
	public string getProjectMyName(string file) {
		TextReader tr = new StreamReader(file);
		string trdData = tr.ReadToEnd ();
		tr.Close ();
		string[] split = trdData.Split(new string[] { ";;" }, System.StringSplitOptions.None);
	//	DebugLog (split.Length);
		if (split.Length == 34)
			return split [(split.Length - 22)];
		else
			return split [(split.Length - 21)];
	}

	public string getProjectMyRelation(string file) {
		TextReader tr = new StreamReader(file);
		string trdData = tr.ReadToEnd ();
		tr.Close ();
		string[] split = trdData.Split(new string[] { ";;" }, System.StringSplitOptions.None);
		if (split.Length == 34)
			return split [(split.Length - 21)];
		else
			return split [(split.Length - 20)];
	}

	public void AddTRDActor(string scriptname, string tabrname,float rate,float shape) {
		// maybe do a check i here if we have already added actor
		bool found = false;
		for (int i = 0; i < _trdactors.Count; i++) {
			if (_trdactors [i].Equals (scriptname)) {
				found = true;
				_trdactors [i].tabrname = tabrname;
				_trdactors [i].rate = rate;
				_trdactors [i].shape = shape;
				break;
			}
		}
		if (!found)
			_trdactors.Add(new trdactor(scriptname,tabrname,rate,shape));
	//	DebugLog("AddTRDActor NUMBER _trdactors IS " + _trdactors.Count);
	}

	public void getProjectData(string file,int speakingline = 0) {
		//     StringBuilder sb = new StringBuilder();
		_trdactors.Clear();
		TextReader tr = new StreamReader(file);
		string trdData = tr.ReadToEnd ();
		string[] split = trdData.Split(new string[] { ";;" }, System.StringSplitOptions.None);
		int l = split.Length;
		int i = 0;
		projectName = split[i];i++;projectVersion = split[i];i++;
		if (!selectingPDFProject) 
			projectPDF=split[i];
		//else
		//	DebugLog("projectPDF is " + projectPDF  + ":");
		i++;
		// lets check metadata now
		_trsdt.checkMetadata(projectPDF);
		projectID=split[i];i++;
		CheckIDFolder ();
		while (!(split[i] == "ETRV")){
			// add voices
			string scriptname = split[i];i++;string tabrname = split[i];i++;
			float rate = float.Parse(split[i]);i++;
			float shape = float.Parse(split[i]);i++;
		//	DebugLog("FOUND PROJECT ACTOR IN PROJECTDATA " + name + ":" + tabrname + ":" + rate + ":" + shape);
			//bool trActor = (name == trnarrator);
			_trdactors.Add(new trdactor(scriptname,tabrname,rate,shape));
		}
		i++;
		projectWriters=split[i];i++;projectGenre=split[i];i++;
		projectPlatform=split[i];i++;projectSynopsis=split[i];i++;
		projectCommendation=split[i];i++;projectWGA=split[i];i++;projectCopyright=split[i];i++;projectMyname=split[i];i++;projectRelation=split[i];i++;
		playback_2X=int.Parse(split[i]);i++;readNameSetting=int.Parse(split[i]);i++;readSceneNumbers=int.Parse(split[i]);i++;readPageNumbers=int.Parse(split[i]);i++;
		readHeaderFooters=int.Parse(split[i]);i++;readTitlePage=int.Parse(split[i]);i++;readSceneHeaders=int.Parse(split[i]);i++;readMoreCont=int.Parse(split[i]);i++;
		readRevisedScenes=int.Parse(split[i]);i++;muteRehearsalDialouge=int.Parse(split[i]);i++;pauseForDeliver=int.Parse(split[i]);i++;playRehearsalScenesOnly=int.Parse(split[i]);i++;feedRehearsalLinesOnly=int.Parse(split[i]);i++;
		readActiveNotes=int.Parse(split[i]);i++;displayActiveNotes=int.Parse(split[i]);i++;scoreVolume=int.Parse(split[i]);i++;peakOverAction=int.Parse(split[i]);i++;
		currentScore = int.Parse(split[i]);i++;
		DebugLog ("getProjectData currentScore" + trglobals.instance.currentScore); 
		if (!(split [i].Equals ("ETRP"))) 
			narratorReadAll=int.Parse(split[i]);i++;
	//	DebugLog("CURRENT SCORE IS " + currentScore);
		tr.Close ();
		if (!selectingTRNProject && !selectingPDFProject)
			_trabt.Setup (speakingline);
		else {
			// copy trn
			if (selectingTRNProject) {
				string idPath = Path.Combine (Application.persistentDataPath, projectID);
				string NOTEPATH = Path.Combine (idPath, NOTENAME);
				if (!Directory.Exists (idPath))
					Directory.CreateDirectory (idPath);
			//	DebugLog (TRNURL + ":" + NOTEPATH);
				if (File.Exists (NOTEPATH))
					File.Delete (NOTEPATH);
				File.Copy (TRNURL, NOTEPATH);
				File.Delete (TRNURL);
				ShowError ("Note Imported to project\n" + projectName + " " + projectVersion, "Import Project");
				selectingTRNProject = false;
				genericBack ();
			} else {
				// handle script imported into tr project
				createTRD(file);
			}
		}
	}

	public void createTRD(string TRDPath) {
	//	DebugLog ("createTRD " + reloadScript + ":" + projectPDF + ":");
		string projectcontent = projectName+";;"+projectVersion+";;"+projectPDF+";;"+projectID;
		// add voice in here
		for (int i = 0; i < _trvs._scriptactors.Count; i++) {
			scriptactor s = _trvs._scriptactors[i];
			if (s.changedVoice) {
			//	DebugLog ("ADDING TO PROJECT " + s.scriptname);
				projectcontent += ";;" + s.scriptname + ";;" + s.tabrname + ";;" + s.rate.ToString("F2") + ";;" + s.shape.ToString("F2");
			}
		}
		// add narrator
		if (_trvs._narrator.changedVoice) {
			projectcontent += ";;" + _trvs._narrator.scriptname + ";;" + _trvs._narrator.tabrname + ";;" + _trvs._narrator.rate.ToString("F2") + ";;" + _trvs._narrator.shape.ToString("F2");
		}
		projectcontent += ";;ETRV;;"+projectWriters+";;"+projectGenre+";;"+projectPlatform+";;"+projectSynopsis;
		projectcontent += ";;"+projectCommendation+";;"+projectWGA+";;"+projectCopyright+";;"+projectMyname+";;"+projectRelation;
		projectcontent += ";;"+playback_2X+";;"+readNameSetting+";;"+readSceneNumbers+";;"+readPageNumbers;
		projectcontent += ";;"+readHeaderFooters+";;"+readTitlePage+";;"+readSceneHeaders+";;"+readMoreCont;
		projectcontent += ";;"+readRevisedScenes+";;"+muteRehearsalDialouge+";;"+pauseForDeliver+";;"+playRehearsalScenesOnly+";;"+feedRehearsalLinesOnly;
		projectcontent += ";;"+readActiveNotes+";;"+displayActiveNotes+";;"+scoreVolume+";;"+peakOverAction+";;"+currentScore+";;"+narratorReadAll;
		projectcontent += ";;ETRP";
		TextWriter tw = new StreamWriter(TRDPath);
		tw.Write(projectcontent);
		tw.Close();
		if (TRRPDFProject) {
			TRRPDFProject = false;
			MobileNativePopups.OpenAlertDialog (
				"tableread project created", "Do you want to open this project now?",
				"NO", "YES",
				() => {
				//	DebugLog ("NO PRESSED");
				},
				() => { 
				//	DebugLog ("YES PRESSED"); 
					_trabt.Setup (0);
				});
			return;
		}
		if (selectingPDFProject) {
			selectingPDFProject = false;
			MobileNativePopups.OpenAlertDialog (
				"SUCCESS", "PDF linked.\nDo you want to open this project?",
				"NO", "YES",
				() => {
					//DebugLog ("NO PRESSED");
					genericBack();
				},
				() => { 
				//	DebugLog ("YES PRESSED"); 
					_trabt.Setup (0);
				});
			return;
		}
		// go to about screen now
		if (!scriptLoaded || reloadScript)
			_trabt.Setup (0);
		else {
			trglobals.instance._scriptController.UpdateItems (trglobals.instance._trvs._scriptlines.Count);
			genericBack ();
		}
	}

	IEnumerator CopyFile(string read_path, string write_path) {
		if (!read_path.Contains ("file://"))
			read_path = "file://" + read_path;
	//	DebugLog ("CopyFile " + write_path);
	//	DebugLog ("CopyFile " + read_path);
		WWW www = new WWW(read_path);
		yield return www;
		if (string.IsNullOrEmpty(www.error)) {
			File.WriteAllBytes(write_path, www.bytes);
		} //else {
		//	DebugLog(www.error);
	//	}
		www.Dispose();
		www = null;
	}

	IEnumerator CopyAndUnzip(string actorfolder) {
		actorfolder = actorfolder + ".zip";
		string write_path = Path.Combine(Application.persistentDataPath, "voices");
		write_path = Path.Combine(write_path, actorfolder);
		string read_path = Path.Combine(Application.streamingAssetsPath, actorfolder);
		if (!read_path.Contains ("file://"))
			read_path = "file://" + read_path;
	//	DebugLog ("CopyAndUnzip " + write_path);
	//	DebugLog ("CopyAndUnzip " + read_path);
		WWW www = new WWW(read_path);
		yield return www;
		if (string.IsNullOrEmpty(www.error)) {
			File.WriteAllBytes(write_path, www.bytes);
			ZipUtil.Unzip (write_path, (Path.Combine(Application.persistentDataPath, "voices")));
			File.Delete (write_path);
		} //else {
			//DebugLog(www.error);
		//}
		www.Dispose();
		www = null;
	}
		
	public string getMETADATATitle() {
		//  return [NSString stringWithFormat:@"TRD::%@::%@::%@::%@::%@::%@",projectname,projectversion,projectID,projectCopyright,projectWGA,projectCommendation];
		return "TRD::"+projectName+"::"+projectVersion+"::"+projectID+"::"+projectCopyright+"::"+projectWGA+"::"+projectCommendation;
	}
	public string getMETADATAAuthor() {
		return "TRD::"+projectGenre+"::"+projectPlatform+"::"+projectWriters+"::"+currentScore;
	}
	public string getMETADATASubject() {
		return "TRD::"+projectSynopsis;
	}
	public string getMETADATAKeywords() {
		string str = "TRD";
		bool somethingtodo = false;
		if (_trvs._narrator.changedVoice) {
			somethingtodo = true;
			str += "::" + _trvs._narrator.scriptname + "::" + _trvs._narrator.tabrname + "::" + _trvs._narrator.rate.ToString ("f2") + "::" + _trvs._narrator.shape.ToString ("f2");
		}
		for (int i = 0; i < _trvs._scriptactors.Count; i++) {
			if (_trvs._scriptactors [i].changedVoice) {
				somethingtodo = true;
				str += "::" + _trvs._scriptactors [i].scriptname + "::" + _trvs._scriptactors [i].tabrname + "::" + _trvs._scriptactors [i].rate.ToString ("f2") + 
					"::" + _trvs._scriptactors [i].shape.ToString ("f2");
			}
		}
		if (somethingtodo == false)
			str = "";
		return str;
	}
	
	/*
	- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
    NSLog(@"RETURNED URL %@",url);
    NSString *URLString = [url absoluteString];
    const char* urlchar = [URLString UTF8String];
    UnitySendMessage("tablereadGlobals", "importIOSURL",urlchar);

	return YES;
}
	 */
	#if UNITY_IOS
	[DllImport ("__Internal")]
	private static extern bool copyItemAtURLtoDocumentsDirectory(string frompath,string topath);

	void importIOSURL(string returnedurl) {
		//string returnedurl = WWW.EscapeURL(url);
		DebugLog ("KkkkKKKK " + returnedurl);
		string filename = Path.GetFileName (returnedurl);
		string returnedURI = filename.Replace ("%20", " ");
		DebugLog ("FILR IS " + filename);
		string filepath = Path.Combine (Application.persistentDataPath, filename);
		if (File.Exists (filepath)) {
			MobileNativePopups.OpenAlertDialog (
				"PDF EXISTS", "Do you want to overwrite PDF?",
				"NO", "YES",
				() => {
					DebugLog ("NO PRESSED");
					MobileNativePopups.OpenAlertDialog (
						"WARNING", "PDF not imported",
						"OK", ()=> {});
				},
				() => { 
					copyIosImported(returnedurl,filepath,returnedURI);
				});
		}
		else
			copyIosImported(returnedurl,filepath,returnedURI);
	}

	void copyIosImported(string frompath, string topath,string returnedURI) {
		DebugLog("EXTENSION IS " + Path.GetExtension(returnedURI) + ":FILE IS " + returnedURI);
		if (copyItemAtURLtoDocumentsDirectory (frompath, topath)) {
			if (Path.GetExtension (returnedURI) == ".trn") {
				importedFile = returnedURI;
				DebugLog ("NEED TO DO SOMETHING WITH THE TRN");
				HandleTRN (importedFile);
			} else { // PDF
				importedFile = returnedURI;
				trglobals.instance._trvs.clearScript ();
				trglobals.instance._trfp.Setup ();
				//UnityEngine.SceneManagement.SceneManager.LoadScene (0);
			}
		} else {
			DebugLog ("ERROR-------------------------");
		}
	}
	#endif

	void OnApplicationPause(bool pause) {
	//	DebugLog ("OnApplicationPause "+ pause);
		if (!pause) {
			#if UNITY_ANDROID
			ImportFromIntent (Application.persistentDataPath);//selectingTRNProject = true;
			#endif
		}
	}

	private void ImportFromIntent(string importPath)
	{
		try
		{
			// Get the current activity
			AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activityObject = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

			// Get the current intent
			AndroidJavaObject intent = activityObject.Call<AndroidJavaObject>("getIntent");

			// Get the intent data using AndroidJNI.CallObjectMethod so we can check for null
			System.IntPtr method_getData = AndroidJNIHelper.GetMethodID(intent.GetRawClass(), "getData", "()Ljava/lang/Object;");
			System.IntPtr getDataResult = AndroidJNI.CallObjectMethod(intent.GetRawObject(), method_getData, AndroidJNIHelper.CreateJNIArgArray(new object[0]));
			if (getDataResult.ToInt32() != 0)
			{
				// Now actually get the data. We should be able to get it from the result of AndroidJNI.CallObjectMethod, but I don't now how so just call again
				AndroidJavaObject intentURI = intent.Call<AndroidJavaObject>("getData");
			//	string url = intentURI.Call<string>("getPath");
			//	
				// Open the URI as an input channel
				AndroidJavaObject contentResolver = activityObject.Call<AndroidJavaObject>("getContentResolver");
				AndroidJavaObject inputStream = contentResolver.Call<AndroidJavaObject>("openInputStream", intentURI);
				AndroidJavaObject inputChannel = inputStream.Call<AndroidJavaObject>("getChannel");
				string ACTIVITY_NAME = "com.unity3d.player.UnityPlayer";
				string CONTEXT = "currentActivity";
				string DOWNLOAD_HELPER = "zeroonevr.zeroone.com.unitydownloadplugin.DownloadPluginClass";
				AndroidJavaObject context = new AndroidJavaClass(ACTIVITY_NAME).GetStatic<AndroidJavaObject>(CONTEXT);
				AndroidJavaObject uriHelper = new AndroidJavaObject(DOWNLOAD_HELPER, context);
				string returnedURI = uriHelper.Call<string> ("getPathFromURI", intentURI);
			
				DebugLog("URI " + returnedURI + ":"  + intentURI);

				// Open an output channel
				if (returnedURI != "") {
					if (File.Exists(Path.Combine(Application.persistentDataPath,returnedURI))) {
						MobileNativePopups.OpenAlertDialog (
							"PDF EXISTS", "Do you want to overwrite PDF?",
							"NO", "YES",
							() => {
								DebugLog ("NO PRESSED");
								MobileNativePopups.OpenAlertDialog (
									"WARNING", "PDF not imported",
									"OK", ()=> {});
							},
							() => { 
								copyImported(inputChannel,inputStream,returnedURI,importPath);
							});
					}
					else
						copyImported(inputChannel,inputStream,returnedURI,importPath);
				}
			}
		}
		catch (System.Exception ex)
		{
			// Handle error
			DebugLog("ERROR" + ex.ToString());
		}
	}


	void copyImported(AndroidJavaObject inputChannel,AndroidJavaObject inputStream, string returnedURI, string importPath) {
		AndroidJavaObject outputStream = new AndroidJavaObject("java.io.FileOutputStream", Path.Combine (importPath, returnedURI));
		AndroidJavaObject outputChannel = outputStream.Call<AndroidJavaObject>("getChannel");

		// Copy the file
		long bytesTransfered = 0;
		long bytesTotal = inputChannel.Call<long>("size");
		while (bytesTransfered < bytesTotal)
		{
			bytesTransfered += inputChannel.Call<long>("transferTo", bytesTransfered, bytesTotal, outputChannel);
		}

		// Close the streams
		inputStream.Call("close");
		outputStream.Call("close");
		// lets reload project
		//	if (File.pat
		DebugLog("EXTENSION IS " + Path.GetExtension(returnedURI));
		if (Path.GetExtension(returnedURI) == ".trn") {
			importedFile = returnedURI;
			DebugLog("NEED TO DO SOMETHING WITH THE TRN");
			HandleTRN (importedFile);
		}
		else { // PDF
			importedFile = returnedURI;
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
		//System.IntPtr method_setData = AndroidJNIHelper.GetMethodID(intent.GetRawClass(), "setData", "");
	}

	public void cleanup() {
		Resources.UnloadUnusedAssets();
		System.GC.Collect ();
	}

	public string backaction;
	public void back() {
		if (backaction != "") {
			Invoke (backaction, 0);
		} else
			genericBack ();
	}
	string TRNURL;string  NOTENAME;

	void HandleTRN(string u) {
		importedFile = "";
		string NOTEPATH = "";
		TRNURL = Path.Combine (Application.persistentDataPath, u);
		if (File.Exists (TRNURL)) {
			DebugLog ("I HAVE NOTE IN PERSTSTANT DATA PATH");
			TextReader tr = new StreamReader(TRNURL);
			string contentTRN = tr.ReadToEnd ();
			string[] linesTRN = contentTRN.Split(new string[] { "\n" }, System.StringSplitOptions.None);
			if (linesTRN.Length > 6) {
				NOTENAME = linesTRN[0].Trim();
				string[] seperatedFileName = NOTENAME.Split ('_');
				string Name =  linesTRN[1].Substring(6).Trim();
				string Version =  linesTRN[2].Substring(8).Trim();
				string IDNumber = linesTRN[3].Substring(3).Trim();
				string MyName = linesTRN[5].Trim();
				string Relation = linesTRN[6].Trim();
				string dataPath = Path.Combine (Application.persistentDataPath, IDNumber);
				NOTEPATH = Path.Combine (dataPath, NOTENAME);
				DebugLog ("dataPath " + dataPath);
				DebugLog("PROJECT DETAILS " + IDNumber + ":" + Name + ":" + Version + ":" + NOTEPATH);
				if (Directory.Exists (dataPath)) {
					DebugLog ("FOLDER EXISTS");
					string NOTEINFO = "You have recieved a note from\n" + MyName + "\n" + Relation + "\n" + Name + ": Project ID:" + IDNumber;	
					ShowError (NOTEINFO, "Tableread Note");
					if (File.Exists (NOTEPATH))
						File.Delete (NOTEPATH);
					File.Copy (TRNURL, NOTEPATH);
					File.Delete (TRNURL);
					if (scriptLoaded && IDNumber == projectID) {
						DebugLog ("UPDATE NOTES");
						_trabt.loadNotesFromFolder ();
					}
				} else {
					DebugLog ("CANT FIND FOLDER");
					string Project = Name + "_" + Version + ".trd";
					string projectPath = Path.Combine (Application.persistentDataPath, Project);
					if (File.Exists (projectPath)) {
						DebugLog ("FOUND PROJECT " + projectPath);	
						TextReader tr2 = new StreamReader (projectPath);
						string content = tr2.ReadToEnd ();
						string[] lines = content.Split (new string[] { ";;" }, System.StringSplitOptions.None);
						IDNumber = lines [3];
						string idPath = Path.Combine (Application.persistentDataPath, IDNumber);
						NOTEPATH = Path.Combine (idPath, NOTENAME);
						if (!Directory.Exists (idPath))
							Directory.CreateDirectory (idPath);
						DebugLog (TRNURL + ":" + NOTEPATH);
						if (File.Exists (NOTEPATH))
							File.Delete (NOTEPATH);
						File.Copy (TRNURL, NOTEPATH);
						File.Delete (TRNURL);
						if (scriptLoaded && IDNumber == projectID) {
							DebugLog ("UPDATE NOTES");
							_trabt.loadNotesFromFolder ();
						}
						ShowError ("Note Imported to project " + Name + " " + Version, "Found Project");
						tr2.Close ();
					} else {
						DebugLog ("HERE");
						#if UNITY_EDITOR
							selectingTRNProject = true;
							_trmsp.Setup();
						#endif
						MobileNativePopups.OpenAlertDialog(
							"Warning!", "NO VALID PROJECT ID OR SIMILAR PROJET FOUND\nDo you want to select a project to import Note too?",
							"NO", "YES",
							() => { DebugLog("NO was pressed"); DeleteTRN();}, () => { DebugLog("YES was pressed"); selectingTRNProject = true; _trmsp.Setup(); });
					}
				}
			}
			tr.Close ();
		}
	}

	public bool selectingTRNProject = false;
	public bool selectingPDFProject = false;
	public bool TRRPDFProject = false;
	void DeleteTRN() {
		if (File.Exists (TRNURL))
			File.Delete (TRNURL);
	}

	void backViewScript() {
		_trvs.clearScript ();
		_trvs.setPosition (false);
		_trpdf.Setup ();
	}

	public void cancelScriptImport() {
		_trsdt.setPosition (true,true,true);
		_trpdf.setPosition (false);
	}

	public void cancelAboutImport() {
		_trsdt.setPosition (true);
		_trabt.setPosition (false);
	}

	public void ShowError(string e, string h = "ERROR") {
		//DebugLog (h + ":" + e);
		MobileNativePopups.OpenAlertDialog(
			h, e,
			"OK", () => { 
				DebugLog ("OK was pressed");
			});
	}

	void backFromScore() {
	//	DebugLog (trglobals.instance._trscr._saveTRD.ToString());
		if (trglobals.instance._trscr._saveTRD) {
			string TRDName = trglobals.instance.projectName+"_" + trglobals.instance.projectVersion + ".trd";
			string TRDPath = System.IO.Path.Combine (Application.persistentDataPath, TRDName);
			createTRD (TRDPath);
		}
		else
			backScript ();
	}

	void backScript() {
	//	_trvs.StartSpeaking ();
		genericBack ();
	}

	void backAbout() {
		_trvs.clearScript ();
		genericBack ();
	}

	void backReadScript() {
		_trvs.StopSpeaking ();
		_trvs.createFrontPageEntry ();
		genericBack ();
	}

	void backSettings() {
	//	DebugLog ("backSettings " + scriptLoaded);
		if (scriptLoaded)
			_trsdt.processPDF (true);
		else
			genericBack ();
	}

	public void genericBack() {
		if (_lastPanel.Count != 0) {
			if (selectingTRNProject)
				DeleteTRN ();
			if (selectingPDFProject) {
				selectingPDFProject = false;
				MobileNativePopups.OpenAlertDialog (
					"WARNING", "Imported PDF not linked to any project.",
					"OK",
					() => { 
						DebugLog ("OK PRESSED");
					});
			}
			bool goback = true;
			if (_trscn.active) {
				goback = _trscn.checkSceneOn ();
				if (!goback)
					ShowError ("You need at least one scene active.");
			}
			if (goback) {
				_lastPanel [_lastPanel.Count - 1].setPosition (true, true, true);
				_lastPanel.RemoveAt (_lastPanel.Count - 1);
			}
		}
	}

	public void home() {
		//DebugLog ("home " + scriptLoaded);
		if (scriptLoaded)
			_trvs.createFrontPageEntry ();
		_trvs.clearScript ();
		_trfp.Setup ();
		_lastPanel.Clear ();
		cleanup ();
	}

	public  void DeleteDirectory(string target_dir)
	{
		if (!Directory.Exists (target_dir)) {
		//	DebugLog ("CANT FindObjectOfType FOLDER " + target_dir);
			return;
		}
		string[] files = Directory.GetFiles(target_dir);
		string[] dirs = Directory.GetDirectories(target_dir);

		foreach (string file in files)
		{
			//	DebugLog ("deleteging file " + file);
			File.SetAttributes(file, FileAttributes.Normal);
			File.Delete(file);
		}

		foreach (string dir in dirs)
		{
			//DebugLog ("deleteging folder " + dir);
			DeleteDirectory(dir);
		}

		Directory.Delete(target_dir, false);
	}

	public bool saveActiveNotes() {  // for exporting only
		bool somethingtodo = false;
		string TRName = projectName + "_" + projectMyname + "_" + projectRelation + "_ACTIVE.trn";
		string notesSTR = TRName + "\nTITLE:" + projectName + "\nVERSION:" + projectVersion + "\nID:" + projectID + "\n";
		for (int i = 0; i < trglobals.instance._trnte._scriptnote.Count; i++) {
			scriptnote tempnote =trglobals.instance._trnte._scriptnote[i];
			if (tempnote.display) {
				somethingtodo = true;
				notesSTR += "NOTE FROM\n" + tempnote.myname + "\n" + tempnote.myrelation  + "\nSCENE:" + tempnote.scene+ "\nPAGE:" + tempnote.page + "\nLINE ID:" + tempnote.linenumber + "\n";
				notesSTR += "CREATION ID:" + tempnote.creation + "\nLINE\n" + tempnote.line + "\nNOTE\n" + tempnote.note + "\n\n";
			}
			tempnote = null;
		}
		notesSTR += "END TABLE READ ACTIVE NOTES";
		if (somethingtodo) {
			string projectfolder = Path.Combine (Application.persistentDataPath, projectID);
			if (!Directory.Exists (projectfolder))
				Directory.CreateDirectory (projectfolder);
			string trnFile = Path.Combine (projectfolder, TRName);
			try {
				TextWriter tw = new StreamWriter(trnFile);
				tw.Write(notesSTR);
				tw.Close();
				return true;
			}
			catch (System.Exception e) {
				return false;
			}
		} else
			return false;
	}

	public void saveContributerNote(string n, string r) {
	//	DebugLog ("saveContributerNote:" + n + ":" + r);
		bool somethingtodo = false;
		string TRName = projectName + "_" + n + "_" + r + ".trn";
		string notesSTR = TRName + "\nTITLE:" + projectName + "\nVERSION:" + projectVersion + "\nID:" + projectID + "\n";
		for (int i = 0; i < trglobals.instance._trnte._scriptnote.Count; i++) {
			scriptnote tempnote =trglobals.instance._trnte._scriptnote[i];
			if (tempnote.myname == n && tempnote.myrelation == r) {
				somethingtodo = true;
				notesSTR += "NOTE FROM\n" + tempnote.myname + "\n" + tempnote.myrelation  + "\nSCENE:" + tempnote.scene+ "\nPAGE:" + tempnote.page + "\nLINE ID:" + tempnote.linenumber + "\n";
				notesSTR += "CREATION ID:" + tempnote.creation + "\nLINE\n" + tempnote.line + "\nNOTE\n" + tempnote.note + "\n\n";
			}
			tempnote = null;
		}
		notesSTR += "END TABLE READ NOTES";
		string projectfolder = Path.Combine (Application.persistentDataPath, projectID);
		string trnFile = Path.Combine (projectfolder, TRName);
		if (somethingtodo) {
			if (!Directory.Exists (projectfolder))
				Directory.CreateDirectory (projectfolder);
			TextWriter tw = new StreamWriter (trnFile);
			tw.Write (notesSTR);
			tw.Close ();
		} else {
			// lets delete file if it exists;
			//DebugLog("Checking fiel for dletetion " + trnFile);
			if (File.Exists (trnFile)) {
			//	DebugLog ("DELETEIGN FILE");
				File.Delete (trnFile);
			}
		}
	}
}
