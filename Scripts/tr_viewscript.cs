using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.xml;
using UnityEngine.UI;

public class tr_viewscript : panel {

	public	Button[]	proBTN;

	string[] male = {
		"Matthew(USA)","Gavin(USA)","Aaron(AUS)","Christopher(USA)","Michael(UK)","Armin(UK)","Paul(USA)","Brad(USA)","Dan(USA)","Roger(UK)","James(AUS)","Tony(USA)","Kunal(IND)","Bruce(USA)","Alfred(USA)","Clint(USA)"
	};
	string[] female = {
		"Mel(USA)","Katie(USA)","Sarah(AUS)","Chelsea(UK)","Helen(UK)","Scarlett(USA)","Rebecca(USA)","Tanya(IND)","Robin(SCO)"
	};

	public	AudioSource _scoreAudio;

	public	List<scriptline>	_scriptlines = new List<scriptline>();
	public	List<scriptscene>	_scriptscenes = new List<scriptscene>();
	public	List<scriptactor>	_scriptactors = new List<scriptactor>();

	public	scriptactor			_narrator;
	public	ScrollRect			_scriptScroll;
	public int speakingline;
//	float currentscrollY;

	public	bool paused = false;
	public	UnityEngine.UI.Image	speedIndicator;
	public	GameObject	sleepbutton;
	int scriptcount;

	void Start(){
		#if UNITY_ANDROID && !UNITY_EDITOR
		TTSManager.Initialize(transform.name, "OnTTSInit");
		#elif UNITY_IOS
		sleepbutton.SetActive(false);
		#endif
	}

	void OnApplicationQuit() 
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		TTSManager.Shutdown();
		#elif UNITY_IOS
		if (trglobals.instance)
			trglobals.instance._iosTTS.StopSpeaking();
		#endif
	}

	private string[] _localeStrings;
	private bool _initializeError = false;

	void OnTTSInit(string message)
	{
		int response = int.Parse(message);

		switch (response)
		{
		case TTSManager.SUCCESS:
			List<TTSManager.Locale> l = TTSManager.GetAvailableLanguages();
			_localeStrings = new string[l.Count];
			for (int i = 0; i < _localeStrings.Length; ++i)
				_localeStrings [i] = l [i].Name;

			break;
		case TTSManager.ERROR:
			_initializeError = true;
			break;
		}
	}

	public scriptactor getTabrName(string n) {
		for (int i = 0; i < _scriptactors.Count; i++) {
			if (n == _scriptactors [i].scriptname)
				return _scriptactors [i];
		}
		return _narrator;
	}

	public scriptactor getTabrNarratroName() {
		return _narrator;
	}

	public void getPDF(string strURL, int sp) {
		string pdfpath = System.IO.Path.Combine(Application.persistentDataPath, strURL);
		speakingline = sp;
	//	trglobals.instance.DebugLog ("+++++++++++++++++SET speaking line to " + speakingline);
		if (!File.Exists(pdfpath))
			pdfpath = System.IO.Path.Combine (trglobals.instance._trpdf.downloadFolderPath, strURL);
		ReadPdfFile (pdfpath);
	}
	private int _speechId = 0;
	int noteline = 0;
	public void startNextLine() {
	//	trglobals.instance.DebugLog ("startNextLine " + speakingline + ":" + _scriptlines.Count);
		if (_scriptlines [speakingline].hasNote && trglobals.instance.readActiveNotes == 1 && trglobals.instance.goPRO) {
		//	trglobals.instance.DebugLog ("NEED TO READ NOTE");
			trglobals.instance._trnte.getNoteToSpeak (speakingline);
			noteline = 0;
			speakTTSNote ();
		} else {
		//	trglobals.instance.DebugLog ("finding nextline");
			StartCoroutine (nextLine ());
		}
	}

	void speakTTSNote() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (TTSManager.IsInitialized ()) {
		cancelledNOTE = false;
		TTSManager.Speak(trglobals.instance._trnte.notelist [noteline], false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnNoteCompleted","speech_" + (++_speechId));
		}
		#elif UNITY_IOS
		cancelledNOTE = false;
		trglobals.instance._iosTTS.SpeakTxt(trglobals.instance._trnte.notelist [noteline],"OnNoteCompleted",1);
		#endif
	}

	void EditorNextLine() {
		StartCoroutine (nextLine ());
	}

	public void OnMuteReherasalDialougeCompleted() {
		StartCoroutine (nextLine ());
	}

	public void OnPauseDeliverDialougeCompleted() {
		forcespeak = true;
		speakingline--;
		StartCoroutine (nextLine ());
	}

	public void OnNoteCompleted(string id) {
	//	trglobals.instance.DebugLog("Speech '" + id + "' is complete.");
		if (!cancelledNOTE) {
			noteline++;
			if (noteline == trglobals.instance._trnte.notelist.Count)
				StartCoroutine (nextLine ());
			else
				speakTTSNote ();
		}
	}

	public void loadNotes() {
		if (!paused) {
			_scoreAudio.Pause ();
			acaSpeak (false);
		}
		if (trglobals.instance.projectRelation == "" || trglobals.instance.projectMyname == "") {
			trglobals.instance.ShowError ("You are not associated with this script project.\nPlease enter your name and relation in the script project settings.", "USER UNKNOWN");
			trglobals.instance._trsdt.Setup (false,false,true);
			return;
		}
		trglobals.instance._trnte.Setup (speakingline);
	}

	public void loadSettings() {
		if (!paused) {
			_scoreAudio.Pause ();
			acaSpeak (false);
		}
		trglobals.instance._trsdt.Setup (false,true,true,true,true);
	}

	void scrollToLine(float t = 1f) {
	//	trglobals.instance.DebugLog ("SCROLL TO LINE ");
		if (trglobals.instance._sleep.sleeping)
			return;
		trglobals.instance._scriptController._Adapter.ScrollTo (speakingline);
		var vh = trglobals.instance._scriptController._Adapter.GetItemViewsHolderIfVisible(speakingline);
		if (vh != null) {
			if (vh.scriptlinedetail != null) {
				vh.scriptlinedetail.setBorderGrey ();
				// set this depedning if line is scene
				if (trglobals.instance._trvs._scriptlines [speakingline].type == trglobals.SCRIPTLINE_TYPE.SCENE)
					vh.scriptlinedetail.text.color = trglobals.instance.scenelineCLR [1];
				else
					vh.scriptlinedetail.text.color = trglobals.instance.scenelineCLR [0];
			}
		}
		else
			trglobals.instance._trvs._scriptlines [speakingline].active = true;
	}

	void resetOldScriptLine(int ln) {
		if (trglobals.instance._sleep.sleeping)
			return;
		if (_scriptlines.Count == 0)
			return;
		var vh = trglobals.instance._scriptController._Adapter.GetItemViewsHolderIfVisible(ln);
		if (vh != null) {
			if (vh.scriptlinedetail != null) {
				vh.scriptlinedetail.setBorderWhite ();
				// set this depedning if line is scene
				if (trglobals.instance._trvs._scriptlines [ln].type == trglobals.SCRIPTLINE_TYPE.SCENE)
					vh.scriptlinedetail.text.color = trglobals.instance.scenelineCLR [1];
				else
					vh.scriptlinedetail.text.color = trglobals.instance.scenelineCLR [0];
			}
		}
		trglobals.instance._trvs._scriptlines [ln].active = false;
		if (oldscriptline) {
			oldscriptline.setBorderWhite ();
			oldscriptline = null;
		}
	}

	public IEnumerator nextLine() {
	//	trglobals.instance.DebugLog ("next line " + speakingline);
		resetOldScriptLine (speakingline);
		if (speakingline == (_scriptlines.Count -1))
			speakingline = -1;
		speakingline = FindNextSpeakLine (speakingline + 1);
	//	trglobals.instance.DebugLog ("Nextline found " + speakingline + ":" + paused + ":" + Application.isFocused);
		#if UNITY_IOS
		if (!Application.isFocused)
			acaSpeak ();
		#endif
	//	string line = _scriptlines[speakingline].linestr;
		yield return null;
		yield return null;
		yield return null;
		if (!paused) {
			acaSpeak ();
		}
	}

	public scriptlineDetails oldscriptline;

	public void showScoresPage() {
		if (!paused) {
			_scoreAudio.Pause ();
			stopScript ();
		//	acaSpeak (false);
		}
		trglobals.instance._trscr.Setup ();
	}

	public void showScenesPage() {
		if (!paused) {
			_scoreAudio.Pause ();
			stopScript ();
			//acaSpeak (false);
		}
		trglobals.instance._trscn.Setup ();
	}

	public void showActorsPage() {
		if (!paused) {
			_scoreAudio.Pause ();
			stopScript ();
			//acaSpeak (false);
		}
		trglobals.instance._tract.Setup ();
	}

	public void stopScript() {
		paused = true;
		if (_scoreAudio.isPlaying)
			_scoreAudio.Stop ();
		if (trglobals.instance._audioSource.isPlaying)
			trglobals.instance._audioSource.Stop ();
		#if UNITY_EDITOR
	//	trglobals.instance.DebugLog("UNITY_EDITOR " + DemoManager.isSpeaking);
		if (DemoManager.isSpeaking)
			trglobals.instance._iosTTS.StopSpeaking ();
		#elif UNITY_ANDROID && !UNITY_EDITOR
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
		if (trglobals.instance._tts.TTS != null)
			if (trglobals.instance._tts.isSpeaking ())
				trglobals.instance._tts.tts_stop ();
		if (TTSManager.IsSpeaking())
			TTSManager.Stop ();
		#elif UNITY_IOS  && !UNITY_EDITOR
			trglobals.instance._tts_ios.getTTS();
			if (trglobals.instance._tts_ios.isSpeaking ())
				trglobals.instance._tts_ios.tts_stop ();
			if (DemoManager.isSpeaking)
				trglobals.instance._iosTTS.StopSpeaking ();
		#endif
	}

	public void haveRehearsalActors() {
		hasrehersalactors = false;
		for (int i = 0; i < _scriptactors.Count; i++) {
			if (_scriptactors [i].rehearse) {
				hasrehersalactors = true;
				break;
			}
		}
		trglobals.instance._scriptController._Adapter.ChangeItemCountTo(trglobals.instance._trvs._scriptlines.Count);
	}

	public bool isRehearsalActor(string n) {
		for (int i = 0; i < _scriptactors.Count; i++) {
			if (_scriptactors [i].scriptname.Equals (n)) {
				if (_scriptactors [i].rehearse)
					return true;
				else
					return false;
			}
		}
		return false;
	}

	bool hasrehersalactors;
	public bool rehearseActor;
	int FindNextSpeakLine(int l) {
		// check if scene rehersal is off
		//trglobals.instance.DebugLog(_scriptlines[l].scene + ":" + _scriptlines[l].rehearse);
	//	trglobals.instance.DebugLog("FindNextSpeakLine " + l + ":" + _scriptlines[l].linestr);
		if (_scriptlines.Count == 0)
			return l;
		if (l >= _scriptlines.Count)
			l = 0;
		int line = l;
		if (!trglobals.instance.goPRO) {
			if (line == _scriptlines.Count)
				line = 0;
			return line;
		}
		int scene = _scriptlines[l].scene;
		if (scene != 0)
			scene -= 1;
	//	trglobals.instance.DebugLog ("scene is " + scene);
		// check scene is rehearsal

		if (trglobals.instance.readTitlePage == 0) {
			while (_scriptlines [line].page == 1) {
				line++;
				if (line == _scriptlines.Count)
					line = 0;
			}
		}
		if (trglobals.instance.playRehearsalScenesOnly == 1 && hasrehersalactors) {
			bool actorrehearse = false;
			while (!actorrehearse) {
				for (int i = 0; i < _scriptactors.Count; i++) {
					if (_scriptactors [i].rehearse) {
					//	trglobals.instance.DebugLog ("REHEARSING ACTOR " + _scriptactors [i].scriptname);
						if (_scriptscenes [scene].actorsInScene.Contains (_scriptactors [i].scriptname)) {
							actorrehearse = true;
							break;
						}
					}
				}
				if (!actorrehearse) {
					scene++;
					if (scene == _scriptscenes.Count)
						scene = 0;
				//	trglobals.instance.DebugLog ("setting line was " + line + ":" + scene);
					line = _scriptscenes [scene].linenumber;
				//	trglobals.instance.DebugLog ("setting line to " + line + ":" + scene);
				}
			}
		} else if (!_scriptscenes [scene].rehearse) { // else lets find next rehearse scene
				while (!_scriptscenes [scene].rehearse) {
					scene++;
					if (scene == _scriptscenes.Count)
						scene = 0;
				}
				line = _scriptscenes [scene].linenumber;
		}
		// more cont
		if (trglobals.instance.readMoreCont == 0) {
			while (_scriptlines [line].type == trglobals.SCRIPTLINE_TYPE.MORECONT) 
				line++;
			if (line == _scriptlines.Count)
				line = 0;
		}
		// pagenumbers
		if (trglobals.instance.readPageNumbers == 0) {
			while (_scriptlines [line].type == trglobals.SCRIPTLINE_TYPE.PAGENUMBER) 
				line++;
			if (line == _scriptlines.Count)
				line = 0;
		}
		// header footers
		if (trglobals.instance.readHeaderFooters == 0) {
			while (_scriptlines [line].type == trglobals.SCRIPTLINE_TYPE.HEADERFOOTER) 
				line++;
			if (line == _scriptlines.Count)
				line = 0;
		}
		// scene headers
		if (trglobals.instance.readSceneHeaders == 0) {
			while (_scriptlines [line].type == trglobals.SCRIPTLINE_TYPE.SCENE) 
				line++;
			if (line == _scriptlines.Count)
				line = 0;
		}
		// reead name settings
		if (trglobals.instance.readNameSetting == 2 && _scriptlines [line].type == trglobals.SCRIPTLINE_TYPE.ACTOR) {
			line++;
			if (line == _scriptlines.Count)
				line = 0;
		} else if (trglobals.instance.readNameSetting == 0 && _scriptlines [line].type == trglobals.SCRIPTLINE_TYPE.ACTOR) {
			//trglobals.instance.DebugLog (_scriptlines [line].spotinscene);
			if (_scriptlines [line].spotinscene != 0)
				line++;
			if (line == _scriptlines.Count)
				line = 0;
		}
	//	trglobals.instance.DebugLog ("END FIND NEW " + line + ":OLD NUUMBER " + l);
		if (line != l) {
			//trglobals.instance.DebugLog ("Need to adjust script line here");
			resetOldScriptLine (l);
			line = FindNextSpeakLine (line);
		}
		return line;
	}

	public void PressedScriptLine(scriptlineDetails sld) {
		bool speak = true;
	//	trglobals.instance.DebugLog ("PressedScriptLine " + sld.linenumber + ":" + speakingline + ":" + paused);
		if (sld.linenumber == speakingline && !paused) { // pause it here
		//	trglobals.instance.DebugLog("Pressed same line");
			paused = true;
			peakscore = false;
			fadescrore = false;
			_scoreAudio.Pause ();
			if (!trglobals.instance._sleep.sleeping) {
				var vh = trglobals.instance._scriptController._Adapter.GetItemViewsHolderIfVisible (speakingline);
				if (vh != null) {
					if (vh.scriptlinedetail != null) {
						vh.scriptlinedetail.text.color = trglobals.instance.scenelineCLR [2];
					}
				}
			}
			StopSpeaking (false);
			_scriptScroll.enabled = true;
			speak = false;
		}
		else {
		//	trglobals.instance.DebugLog ("pressed different line " + speak);// + "OLDSCRIPTLINE: " + oldscriptline.linenumber);
			if (!paused) {
			//	trglobals.instance.DebugLog ("changedLine = true");
				if (!userstoppedLine)
					changedLine = true;
				userstoppedLine = false;
			}
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (TTSManager.IsSpeaking ()) {
				cancelledNOTE = true;
				TTSManager.Stop ();
			}
			#elif UNITY_IOS
			if (DemoManager.isSpeaking) {
				cancelledNOTE = true;
				trglobals.instance._iosTTS.StopSpeaking ();
			}
			#endif
			resetOldScriptLine (speakingline);
			//need to check here that we have the right line
			speakingline = FindNextSpeakLine(sld.linenumber);
	//		trglobals.instance.DebugLog ("Setting old script line to " + sld.linenumber);
			oldscriptline = sld;
			_scriptScroll.enabled = false;
			paused = false;
			if (!trglobals.instance._sleep.sleeping) {
				var vh = trglobals.instance._scriptController._Adapter.GetItemViewsHolderIfVisible (speakingline);
				if (vh != null) {
					if (vh.scriptlinedetail != null) {
						vh.scriptlinedetail.setBorderGrey ();
					}
				}
			}
			if (trglobals.instance.goPRO) {
				if (!trglobals.instance.muteScore)
					_scoreAudio.Play ();
				// cancel Note if it's speaking
				#if UNITY_ANDROID && !UNITY_EDITOR
				if (TTSManager.IsSpeaking ()) {
					cancelledNOTE = true;
					TTSManager.Stop ();
				}
				#elif UNITY_IOS
			if (DemoManager.isSpeaking) {
					cancelledNOTE = true;
					trglobals.instance._iosTTS.StopSpeaking ();
				}
				#endif
			}
			acaSpeak (speak,false);
		}
	}
	public void StopSpeaking(bool scrollto = true) {
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (TTSManager.IsSpeaking() && trglobals.instance.goPRO) {
			cancelledNOTE = true;
			TTSManager.Stop ();
		}
		#elif UNITY_IOS
		if (DemoManager.isSpeaking) {
			cancelledNOTE = true;
			trglobals.instance._iosTTS.StopSpeaking ();
		}
		#endif
		acaSpeak (false,scrollto);
	}

	scriptactor getTablereadActor(string a) {
		for (int i = 0; i < _scriptactors.Count; i++) {
			if (_scriptactors [i].scriptname == a)
				return _scriptactors [i];
		}
		return _narrator;
	}

	bool forcespeak = false;

	void acaSpeak(bool speak = true, bool scrollto = true) {
	//	trglobals.instance.DebugLog ("acaSpeak Start " + speak + ":" + scrollto);
		if (scrollto && Application.isFocused)
			scrollToLine ();
		string actor = _scriptlines[speakingline].actor;
		string acaactor = "";
		string defacaactor = "";
		string tabrname = "";
		float rate = 200;
		float shape = 100;
		if (actor == trglobals.instance.trnarrator  || trglobals.instance.narratorReadAll == 1) {
			tabrname = _narrator.tabrname;
			tablereadActor ta = trglobals.instance.getAcaActor (tabrname);
			#if UNITY_ANDROID
			acaactor = ta.acaName;
			defacaactor = ta.defaultname;
			#elif UNITY_IOS
			acaactor = ta.iOSacaName;
			defacaactor = ta.iOSdefaultname;
			#endif
			rate = _narrator.rate;
			shape = _narrator.shape;
			if (trglobals.instance.peakOverAction == 1 && !paused && actor == trglobals.instance.trnarrator) {
				if (peakscore == false) {
					peakscore = true;
					fadescrore = false;
					Invoke ("doPeak", 0);
				}
			}
		}
		else {
			if (string.IsNullOrEmpty (actor))
				actor = trglobals.instance.trnarrator;
			scriptactor tbractor = getTablereadActor (actor);
			tabrname = tbractor.tabrname;
			tablereadActor ta = trglobals.instance.getAcaActor (tabrname);
			if (ta != null) {
				#if UNITY_ANDROID
				acaactor = ta.acaName;
				defacaactor = ta.defaultname;
				#elif UNITY_IOS
				acaactor = ta.iOSacaName;
				defacaactor = ta.iOSdefaultname;
				#endif
				rate = tbractor.rate;
				shape = tbractor.shape;
			}
			if (trglobals.instance.peakOverAction == 1 && !paused) {
				if (fadescrore == false) {
					peakscore = false;
					fadescrore = true;
					Invoke ("doFade", 0);
				}
			}
		}
	//	trglobals.instance.DebugLog (tabrname + ":" + acaactor + ":" + rate + ":" + shape + ":" + defacaactor);
		rehearseActor = isRehearsalActor (actor);
	//	trglobals.instance.DebugLog("REHEARSE IS " + rehearseActor + ":" + speak);
		string line = _scriptlines[speakingline].linestr;
		if (_scriptlines [speakingline].type == trglobals.SCRIPTLINE_TYPE.SCENE)
			line = RemoveScenePageNumber (line);
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (trglobals.instance.goPRO) {
			if (trglobals.instance._tts.TTS == null)
				trglobals.instance._tts.tts_init ();
			if (trglobals.instance._tts.isSpeaking())
				trglobals.instance._tts.tts_stop ();
			if (speak) {
				if (!rehearseActor || forcespeak) {
						forcespeak = false;
						trglobals.instance._tts.tts_speak (line,acaactor,defacaactor,rate,shape);
				}
					else {
						if (trglobals.instance.muteRehearsalDialouge == 1)
							TTSManager.Speak(line,false,TTSManager.STREAM.Music,0,0,transform.name,"OnMuteReherasalDialougeCompleted","speech_" + (++_speechId));
						else if (trglobals.instance.pauseForDeliver == 1) 
							TTSManager.Speak(line,false,TTSManager.STREAM.Music,0,0,transform.name,"OnPauseDeliverDialougeCompleted","speech_" + (++_speechId));
					}
			}
		}
		else {
			if (!TTSManager.IsInitialized())
				TTSManager.Initialize(transform.name, "OnTTSInit");
			if (TTSManager.IsSpeaking())
				TTSManager.Stop ();
			if (speak) {
				TTSManager.Speak (line, false, TTSManager.STREAM.Music, 1f, 0f, transform.name, "OnLineCompleted", "speech_" + (++_speechId));
			}
		}
		#elif UNITY_IOS 
			#if !UNITY_EDITOR
			if (trglobals.instance.goPRO) {
				trglobals.instance._tts_ios.getTTS ();
				if (trglobals.instance._tts_ios.isSpeaking())
					trglobals.instance._tts_ios.tts_stop ();
				if (speak) {
				if (!rehearseActor || forcespeak) {
					forcespeak = false;
						trglobals.instance._tts_ios.tts_speak (line,acaactor,defacaactor,rate,shape);
				}
				else {
					if (trglobals.instance.muteRehearsalDialouge == 1)
						trglobals.instance._iosTTS.SpeakTxt(line,"OnMuteReherasalDialougeCompleted",0);
					else if (trglobals.instance.pauseForDeliver == 1) 
						trglobals.instance._iosTTS.SpeakTxt(line,"OnPauseDeliverDialougeCompleted",0);
					}
				}
			}
			else {
				//trglobals.instance.DebugLog("IOS TTS HANDLING " + trglobals.instance._iosTTS.isSpeaking);
			if (DemoManager.isSpeaking)
					trglobals.instance._iosTTS.StopSpeaking ();
				if (speak) {
					trglobals.instance.DebugLog("IOS SPEAKING LINE " + line);
				//	Invoke ("callwait", 0.5f);
					trglobals.instance._iosTTS.SpeakTxt(line,"OnLineCompleted",1);

				}
			}
			#else
	if (DemoManager.isSpeaking)
				trglobals.instance._iosTTS.StopSpeaking ();
			if (speak) {
			//	trglobals.instance.DebugLog("IOS SPEAKING LINE " + line);
			//	Invoke ("callwait", 0.5f);
				trglobals.instance._iosTTS.SpeakTxt(line,"OnLineCompleted",1);
			}
			#endif
	//	#elif UNITY_EDITOR
		//if (speak)
		//	Invoke("EditorNextLine",1);
		#elif UNITY_EDITOR 
		if (DemoManager.isSpeaking)
			trglobals.instance._iosTTS.StopSpeaking ();
		if (speak) {
			//trglobals.instance.DebugLog("IOS SPEAKING LINE " + _scriptlines[speakingline].linestr);
			//Invoke ("callwait", 0.5f);
			trglobals.instance._iosTTS.SpeakTxt(line,"OnLineCompleted",1);
		}
		#endif
	}

	void callwait() {
		if (_scriptlines.Count > 0)
			trglobals.instance._iosTTS.SpeakTxt(_scriptlines[speakingline].linestr,"OnLineCompleted",1);
	}

	bool changedLine = false;
	bool userstoppedLine = false;
	public void ResetChangedLine() {
	//	trglobals.instance.DebugLog ("userstoppedLine = false");
		userstoppedLine = true;
		changedLine = false;
	}

	public void OnLineCompleted(string id) {
	//	trglobals.instance.DebugLog ("ONline completed " + changedLine + ":" + speakingline +":"+paused);
		//if (trglobals.instance._trvs.active)
		if (!changedLine) {
			if (!paused)
				trglobals.instance._trvs.startNextLine ();
		}
		changedLine = false;
	}

	bool cancelledNOTE;

	public void StartSpeaking() {
	//	trglobals.instance.DebugLog ("Start Spekaing " + paused);
		paused = false;
		#if UNITY_ANDROID && !UNITY_EDITOR
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		#endif
		if (trglobals.instance._audioSource.isPlaying)
			trglobals.instance._audioSource.Stop ();
		if (_scoreAudio.isPlaying)
			_scoreAudio.Stop ();
		_scriptScroll.enabled = false;
		acaSpeak();
		if (!trglobals.instance.muteScore && trglobals.instance.goPRO)
			StartCoroutine (getAudio ());
	}

	public void ChangeScene(int v) {
		//trglobals.instance.DebugLog ("Current scene is " + _scriptlines [speakingline].scene);
		int cs = _scriptlines [speakingline].scene;
		if (v == 0) {
			if ((cs - 1) < 0) {
				trglobals.instance.ShowError ("This is the first scene.", "SCENE");
				return;
			}
			if (_scriptscenes [cs - 1].linenumber == speakingline) {
				v = -1;
			}
			if (cs == 0) {
				trglobals.instance.ShowError ("This is the first scene.", "SCENE");
				return;
			}

		} else {
			if (cs == _scriptscenes.Count) {
				trglobals.instance.ShowError ("This is the last scene.", "SCENE");
				return;
			}
		}
		cs += v;
		if ((cs - 1) < 0) 
			trglobals.instance.ShowError ("This is the first scene.", "SCENE");
		else {
			trglobals.instance.DebugLog ("changedLine = true");
			changedLine = true;
			JumpToScene (_scriptscenes [cs - 1].linenumber);
		}
	}

	public void JumpToScene(int l) {
	//	if (oldscriptline)
		resetOldScriptLine (speakingline);
		speakingline = FindNextSpeakLine(l);
		paused = false;
		acaSpeak ();
	}
	public Color	_speedONCLR;
	public void setSpeedVoice() {
		if (trglobals.instance.speedVoice) {
			trglobals.instance.speedVoice = false;
			speedIndicator.color = Color.white;
		} else {
			trglobals.instance.speedVoice = true;
			speedIndicator.color = _speedONCLR;
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		trglobals.instance._tts.tts_speedVoice ();
		#elif UNITY_IOS
		trglobals.instance._tts_ios.tts_speedVoice();
		#endif
	}
	
	public void Setup(int l = 0,bool showpage = true) {
		_speechId = 0;
		if (trglobals.instance.speedVoice)
			speedIndicator.color = _speedONCLR;
		else
			speedIndicator.color = Color.white;
		paused = false;
		currentactor = trglobals.instance.trnarrator;
		recentactor = currentactor;
		speakingline = FindNextSpeakLine(l);
	//	trglobals.instance.DebugLog ("+++++++++++++++++SET speaking line to " + speakingline + ":" + showpage + ":" + oldscriptline + ":");
		if (showpage) {
			oldscriptline = null;
			_headingSTR = trglobals.instance.projectName + " " + trglobals.instance.projectVersion;
			setPosition (true);
		}
		// Lets start speaking text
	}

	public void loadedPage() {
		//trglobals.instance.DebugLog ("loadedPage " + oldscriptline);
		if (oldscriptline == null) {
			StartSpeaking ();
		}
		else
			PressedScriptLine (oldscriptline);
	}

	bool peakscore = false;
	bool fadescrore = false;
	public void doPeak() {
		trglobals.instance.DebugLog ("doPeak " + _scoreAudio.volume);
		if (_scoreAudio.volume < trglobals.instance.scoreVolume/100.0f) {
			_scoreAudio.volume += 0.015f;
			if (peakscore && active) {
				Invoke ("doPeak", 0.2f);
			}
		} else {
			peakscore = false;
		_scoreAudio.volume = trglobals.instance.scoreVolume/100.0f;
		}
	}

	public void doFade() {
	trglobals.instance.DebugLog ("fading " + trglobals.instance.scoreVolume);
		if (_scoreAudio.volume > (trglobals.instance.scoreVolume / 400.0f)) {
			_scoreAudio.volume -= 0.03f;
			if (fadescrore && active) {
				Invoke ("doFade", 0.1f);
			}
		} else {
			fadescrore = false;
			_scoreAudio.volume = trglobals.instance.scoreVolume / 400.0f;
		}
	}

	public void AdjustVolume(Slider s) {
		trglobals.instance.scoreVolume = (int)s.value;
		_scoreAudio.volume = trglobals.instance.scoreVolume / 100.0f;
		trglobals.instance._trscr._saveTRD = true;
	}

	IEnumerator getAudio() {
		
		string track = trglobals.instance.getMusicTitle(trglobals.instance.currentScore);
		trglobals.instance.DebugLog ("get Audio " + track + ":" + trglobals.instance.currentScore);
		string url = System.IO.Path.Combine (System.IO.Path.Combine (Application.persistentDataPath, "scores"), track);
		WWW www = new WWW ("file://" +url);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
	//		trglobals.instance.DebugLog (trglobals.instance.scoreVolume + ":" + trglobals.instance.peakOverAction);
			_scoreAudio.clip = www.GetAudioClip ();
			_scoreAudio.Play ();
			_scoreAudio.volume = trglobals.instance.scoreVolume / 100.0f;
		}
	}

	bool StringHasNumber(string s) {
	//	trglobals.instance.DebugLog ("StringHasNumber " + s);
		if (s.Contains ("0"))
			return true;
		if (s.Contains ("1"))
			return true;
		if (s.Contains ("2"))
			return true;
		if (s.Contains ("3"))
			return true;
		if (s.Contains ("4"))
			return true;
		if (s.Contains ("5"))
			return true;
		if (s.Contains ("6"))
			return true;
		if (s.Contains ("7"))
			return true;
		if (s.Contains ("8"))
			return true;
		if (s.Contains ("9"))
			return true;
		return false;
	}

	bool CharIsNumber(char s) {
	//	trglobals.instance.DebugLog ("CharIsNumber " + s);
		if (s.Equals('0'))
			return true;
		if (s.Equals('1'))
			return true;
		if (s.Equals('2'))
			return true;
		if (s.Equals('3'))
			return true;
		if (s.Equals('4'))
			return true;
		if (s.Equals('5'))
			return true;
		if (s.Equals('6'))
			return true;
		if (s.Equals('7'))
			return true;
		if (s.Equals('8'))
			return true;
		if (s.Equals('9'))
			return true;
		if (s.Equals('.'))
			return true;
		return false;
	}

	string RemoveScenePageNumber(string s) {
		s = s.Trim ();
		int intPOS = s.IndexOf("INT.");
		int extPOS = s.IndexOf ("EXT.");
		if (intPOS == 0 || extPOS == 0)
			return s;
		int startpos = extPOS;
		if (intPOS < 0)
			startpos = extPOS;
		else if (extPOS < 0)
			startpos = intPOS;	
		else if (intPOS < extPOS)
			startpos = intPOS;
		string news = s.Substring (startpos, s.Length - startpos);
		string startscene = s.Substring (0, startpos).Trim ();
		int atendpos = news.IndexOf (startscene);
	//	Debug.Log("HEADER SCENE " + startscene + ":" + news + ":" +atendpos);
		if (atendpos < 0)
			return news;
		else
			return news.Substring (0,atendpos);
	/*	bool searchstart = true;
		while (searchstart) {
		//	trglobals.instance.DebugLog (s [startpos] + ":" + startpos);
			if (!CharIsNumber (s [startpos])) {
				startpos--;
				if (startpos < 0) {
					searchstart = false;
				}
			} else
				searchstart = false;
		}
		startpos++;
		if (StringHasNumber (s.Substring (0, startpos))) {
			string news = (s.Substring (startpos, (s.Length - ((startpos * 2) - 1)))).Trim ();
			if (CharIsNumber(news[news.Length-1]))
				news = news.Substring(0,news.Length-2);
			return (news);
		}
		return s;*/
	}

	// string url = Application.dataPath + "/Resources/audio1.wav"; //Application.persistentDataPath; //"C:/simFIL/Audio/audio2015-10-23 14h59m04s.wav"; public AudioSource source; private void Start() { WWW www = new WWW(url); source = GetComponent(); source.clip = www.audioClip; } private void Update() { if (!source.isPlaying && source.clip.isReadyToPlay) source.Play(); } }
	bool CheckSceneLine(float v,string s) {
		// 108 should be scene position
		if (v <= (trglobals.instance.actionmargin + 12)) {
			isaction = true;
			if (s.Contains ("INT.") || s.Contains ("EXT.")) {
				int intPOS = s.IndexOf("INT.");
				int extPOS = s.IndexOf ("EXT.");
				if (intPOS > 0) {
					if (!s [intPOS - 1].Equals (' '))
						return false;
					//Debug.Log (":" + s [intPOS - 1] + ":");
				} else if (extPOS > 0) {
					if (!s [extPOS - 1].Equals (' '))
						return false;
				//Debug.Log (":" + s [extPOS - 1] + ":");
				}
			//	Debug.Log(intPOS + ":" + extPOS);
				currentscene++;
				scriptscene sc = new scriptscene (currentscene, s, true, (_scriptlines.Count));
				_scriptscenes.Add (sc);
				for (int i = 0; i < _scriptactors.Count; i++) {
					_scriptactors [i].spotinscene = 0;
				}
				return true;
			} else
				return false;
		} else {
		//	trglobals.instance.DebugLog(s + ":NOT ACTION");
			isaction = false;
			return false;
		}
	}

	string findActorName(string s) {
		string[] s2 = s.Split ('(');
		return s2 [0].Trim ();
	}

	string findActorGender(string s) {
	//	trglobals.instance.DebugLog (trglobals.instance._trabt.content.Length);
		for (int i = 0; i < trglobals.instance._trabt.content.Length; i++) {
			string[] filen = trglobals.instance._trabt.content [i].Split (',');
			// check fullname STEP MON etc
			if (filen[1].Trim() == s.Trim()) {
				if (trglobals.instance._trabt.content[i][0]== 'B')
					return "M";
				else
					return "F";
			}
			string[] names = s.Split (' '); // check parts of name MRS MR etc
			for (int j = 0; j < names.Length; j++) {
				if (filen[1].Trim() == names[j].Trim()) {
					if (trglobals.instance._trabt.content[i][0]== 'B')
						return "M";
					else
						return "F";
				}
			}
		}
		return "M";
	}

	bool isHeaderFooter(float y) {
		if (y <= trglobals.instance.rightMargin)
			return true;
		if (y >= (trglobals.instance.pdfHeight - trglobals.instance.rightMargin))
			return true;
		return false;
	}

	bool isPage(string s) {
		int o;
		string ss = s.Substring (0, s.Length - 1);
		if (int.TryParse(ss, out o))
			return true;
		if (int.TryParse (s, out o))
			return true;
		if (s.Length > 4)
		if (s.Substring(0,3).Equals("page"))
			return true;
		s = s.Replace(" ","");
		s = s.Replace("\u00A0","");
		ss = s.Substring (0, s.Length - 1);
		if (int.TryParse(ss, out o))
			return true;
		if (int.TryParse (s, out o))
			return true;
		return false;
	}

	bool checkMORECONT(string s) {
		if (s.Equals("(CONT'D)"))
			return true;
		if (s.Equals("(CONTINUED)"))
			return true;
		if (s.Equals("CONTINUED:"))
			return true;
		if (s.Equals("(MORE)"))
			return true;
		if (s.Equals("MORE:"))
			return true;
		return false;
	}

	bool isActorRange(float v, string s) {
		// 252
		if (currentscene == 0)
			return false;
		if (!s.ToUpper ().Equals (s))
			return false;
		if (v >= trglobals.instance.parenthiticalmargin && v <= (trglobals.instance.actormargin + 12)) {
			string sname = findActorName (s);
			if (sname.Length > 0) {
				recentactor = sname;
				//trglobals.instance.DebugLog (currentscene + ":" + sname);
				if (currentscene > 0) {
					for (int i = 0; i < _scriptactors.Count; i++) {
						if (_scriptactors [i].scriptname == sname) {
							_scriptactors [i].frequency++;
							spotinscene = _scriptactors [i].spotinscene;
							//trglobals.instance.DebugLog ("spot in scene is " + spotinscene);
							_scriptactors [i].spotinscene++;
							updateSceneActorList (sname);
							return true;
						}
					}
					// new actor
					spotinscene = 0;
					//trglobals.instance.DebugLog ("spot in scene is " + spotinscene);
					scriptactor sa = new scriptactor (sname, findActorGender (sname),_scriptactors.Count);
					sa.spotinscene = 1;
					_scriptactors.Add (sa);
				}
			}
			updateSceneActorList (sname);  
			return true;
		}
		else
			return false;
	}

	void updateSceneActorList(string n) {
		int v = _scriptscenes.Count - 1;
	//	trglobals.instance.DebugLog (_scriptscenes [v].name + ":" + n);
		if (!_scriptscenes [v].actorsInScene.Contains (n))
			_scriptscenes [v].actorsInScene.Add (n);
	}

	List<string> checkForParetnthicalsinDialouge(string l) {
		List<string> split = new List<string> ();
		if (l.Contains ("(")) {
			if (!l.Equals (l.ToUpper ())) {
				List<int> ob = new List<int> ();
				List<int> cb = new List<int> ();
				for (int i = 0; i < l.Length; i++) {
					if (l [i].Equals ('('))
						ob.Add (i);
					else if (l [i].Equals (')'))
						cb.Add (i);
				}
				if (ob.Count == cb.Count) {
					if (ob [0] != 0) {
						//trglobals.instance.DebugLog ("Adding start" + l.Substring (0, (ob [0] - 1)).Trim ());
						split.Add (l.Substring (0, (ob [0] - 1)).Trim ());
					}
					for (int i = 0; i < ob.Count; i++) {
						//trglobals.instance.DebugLog ("Adding " + l.Substring (ob [i], (cb [i] - ob [i]) + 1));
						split.Add (l.Substring (ob [i], (cb [i] - ob [i]) + 1)); // add parentheticals
						if (i < ob.Count - 1) { 
							//trglobals.instance.DebugLog ("Adding inside" + l.Substring (cb [i] + 1, (ob [i + 1] - cb [i]) - 1));
							split.Add (l.Substring (cb [i] + 1, (ob [i + 1] - cb [i]) - 1).Trim ());
						} 
					}
					if (cb [cb.Count - 1] != l.Length - 1) {
						//trglobals.instance.DebugLog ("Adding end" + l.Substring (cb [cb.Count - 1] + 1, (l.Length - cb [cb.Count - 1]) - 2));
						split.Add (l.Substring (cb [cb.Count - 1] + 1, (l.Length - cb [cb.Count - 1]) - 2).Trim ());
					}
				}
			}
		}
		return split;
	}

	bool isDialougeRange(float v) {
		// 252
		if (currentscene == 0)
			return false;
		if (v >= trglobals.instance.actionmargin && v <= (trglobals.instance.dialougemargin + 12))
			return true;
		else
			return false;
	}
	bool isParentheticalRange(float v, string s) {
		// 252
		if (v >= trglobals.instance.dialougemargin && v <= trglobals.instance.parenthiticalmargin) {
			if (s [0] == '(' && s [s.Length - 1] == ')')
				return true;
			else
				return false;
		}
		else
			return false;
	}
	bool isTransition(float v) {
		// 252
		if (v >= trglobals.instance.transitionmargin)// && v <= trglobals.instance.parenthiticalmargin)
			return true;
		else
			return false;
	}
	bool isaction = false;
	int currentscene = 0;
	int currentline = 0;
	string currentactor;
	string recentactor;
	int currentpage = 0;
	int totalpagecount;

	int spotinscene;
	IEnumerator ProcessPage(int page, PdfReaderContentParser parser) {
			currentpage = page;
			LocationTextExtractionStrategyWithPosition strategy = parser.ProcessContent(page, new LocationTextExtractionStrategyWithPosition());
			//	strategy.pagenumber = page;
			List<ScriptLines>  scriptLines = strategy.GetLocations ();
		for (int i = 0; i < scriptLines.Count; i++) {
			if (scriptLines [i] != null) {
				//trglobals.instance.DebugLog (scriptLines [i].page);
				string l = scriptLines [i].Text;
				l = l.Replace ("*", "");
				l = l.Replace ("’", "'");
				#if UNITY_IOS
				l = l.Replace ("Õ", "'");
				l = l.Replace ("Ò", "\"");
				l = l.Replace ("Ó", "\"");
				l = l.Replace ("Ô", "'");
				#endif
				l = l.Trim ();
				if (l != "") {
					bool addline = true;
					spotinscene = 0;
					trglobals.SCRIPTLINE_TYPE st = trglobals.SCRIPTLINE_TYPE.NORMAL;
					currentactor = trglobals.instance.trnarrator;
					if (isPage (l)) {
						st = trglobals.SCRIPTLINE_TYPE.PAGENUMBER;
					} else if (isHeaderFooter (scriptLines [i].Y)) {
						st = trglobals.SCRIPTLINE_TYPE.HEADERFOOTER;
					} else if (CheckSceneLine (scriptLines [i].X, l)) {
						st = trglobals.SCRIPTLINE_TYPE.SCENE;
					//	l = RemoveScenePageNumber (l);
					} else if (isaction) {
						st = trglobals.SCRIPTLINE_TYPE.NORMAL;
					} else if (checkMORECONT (l)) {
						st = trglobals.SCRIPTLINE_TYPE.MORECONT;
					} else if (isActorRange (scriptLines [i].X, l)) {
						st = trglobals.SCRIPTLINE_TYPE.ACTOR;
					} else if (isParentheticalRange (scriptLines [i].X, l)) {
						st = trglobals.SCRIPTLINE_TYPE.PARENTHETICAL;
					} else if (isDialougeRange (scriptLines [i].X)) {
					List<string> splitd = checkForParetnthicalsinDialouge (l);
						if (splitd.Count > 0) {
							addline = false;
							for (int k = 0; k < splitd.Count; k++) {
								//trglobals.instance.DebugLog ("SPLIT DIALOUGE " + splitd [k]);
								if (splitd [k].Contains ("(")) {
									scriptline sl = new scriptline (splitd [k], scriptLines [i].X, scriptLines [i].Y, page, currentscene, trglobals.SCRIPTLINE_TYPE.PARENTHETICAL,
										               _scriptlines.Count, trglobals.instance.trnarrator, spotinscene);
									_scriptlines.Add (sl);
									sl.spotinscene = spotinscene;
								} else {
									scriptline sl = new scriptline (splitd [k], scriptLines [i].X, scriptLines [i].Y, page, currentscene, trglobals.SCRIPTLINE_TYPE.DIALOUGE,
										                _scriptlines.Count, recentactor, spotinscene);
									_scriptlines.Add (sl);
									sl.spotinscene = spotinscene;
								}
							}
						} else {
							st = trglobals.SCRIPTLINE_TYPE.DIALOUGE;
							currentactor = recentactor;
						}
					} else if (isTransition (scriptLines [i].X)) {
						st = trglobals.SCRIPTLINE_TYPE.TRANSITION;
					}
					if (addline) {
						scriptline sl = new scriptline (l, scriptLines [i].X, scriptLines [i].Y, page, currentscene, st, _scriptlines.Count, currentactor, spotinscene);
						_scriptlines.Add (sl);
						sl.spotinscene = spotinscene;
					}
				}
			}
		}
		yield return null;
		page++;
		float prg = (float)currentpage / (float)totalpagecount;
	//	trglobals.instance.DebugLog ("PERCENTAGE IS " + prg);
		trglobals.instance._trabt._progressBAR.localScale = new Vector3 (prg, 1, 1);
	//	totalpagecount = 74;
		if (page <= totalpagecount) {
			if (trglobals.instance.continueScript)
				StartCoroutine (ProcessPage (page, parser));
			else {
				clearScript ();
				pdfReader.Close ();
			}
		} else {
			pdfReader.Close ();
			trglobals.instance.cleanup ();
			// lets check here we have scriptlines 
			if (_scriptlines.Count > 0) {
				trglobals.instance.scriptLoaded = true;
				_scriptactors.Sort ((p1, p2) => p2.frequency.CompareTo (p1.frequency));
				// lets add actors now
				int malecount = 0;
				int femalecount = 0;
				for (int i = 0; i < _scriptactors.Count; i++) {
					_scriptactors [i].index = i;
				//	if (!_scriptactors [i].changedVoice) {
						if (_scriptactors [i].gender == "M") {
							//	trglobals.instance.DebugLog (i + ":" + _scriptactors [i].scriptname + ":" + malecount + ":" + male.Length);
							_scriptactors [i].tabrname = male [malecount];
							tablereadActor a = trglobals.instance.getAcaActor (male [malecount]);
							#if UNITY_ANDROID
						_scriptactors [i].acaname = a.acaName;
							#elif UNITY_IOS
							_scriptactors [i].acaname = a.iOSacaName;
							#endif
							_scriptactors [i].rate = a.rate;
							_scriptactors [i].shape = a.shape;
							malecount++;
							if (malecount >= male.Length)
								malecount = 7;
						} else {
							_scriptactors [i].tabrname = female [femalecount];
							tablereadActor a = trglobals.instance.getAcaActor (female [femalecount]);
							#if UNITY_ANDROID
						_scriptactors [i].acaname = a.acaName;
							#elif UNITY_IOS
							_scriptactors [i].acaname = a.iOSacaName;
							#endif
							_scriptactors [i].rate = a.rate;
							_scriptactors [i].shape = a.shape;
							femalecount++;
							if (femalecount >= female.Length)
								femalecount = 7;
						}
					//}
				}
				//if (!_narrator.changedVoice) {
					_narrator = new scriptactor (trglobals.instance.trnarrator, "M", -1);
					_narrator.tabrname = "Harold(UK)";
					_narrator.acaname = "Peter";
					_narrator.rate = 200;
					_narrator.shape = 100;
			//	}
				// update voices wirth saved ones
			//	trglobals.instance.DebugLog(trglobals.instance._trdactors.Count + ":" + _scriptactors.Count);
				for (int i = 0; i < trglobals.instance._trdactors.Count; i++) {
					for (int j = 0; j < _scriptactors.Count; j++) {
				//		trglobals.instance.DebugLog (trglobals.instance._trdactors [i].scriptname + ":" + _scriptactors [j].scriptname);
						if (trglobals.instance._trdactors [i].scriptname == _scriptactors [j].scriptname) {
							_scriptactors [j].tabrname = trglobals.instance._trdactors [i].tabrname;
							tablereadActor a = trglobals.instance.getAcaActor (_scriptactors [j].tabrname);
							#if UNITY_ANDROID
							_scriptactors [j].acaname = a.acaName;
							#elif UNITY_IOS
							_scriptactors [j].acaname = a.iOSacaName;
							#endif
							_scriptactors [j].rate = trglobals.instance._trdactors [i].rate;
							_scriptactors [j].shape = trglobals.instance._trdactors [i].shape;
							_scriptactors [j].changedVoice = true;
							break;
						}
					}
				}
				for (int i = 0; i < trglobals.instance._trdactors.Count; i++) {
					if (trglobals.instance._trdactors [i].scriptname == _narrator.scriptname) {
						_narrator.tabrname = trglobals.instance._trdactors [i].tabrname;
						tablereadActor a = trglobals.instance.getAcaActor (_narrator.tabrname);
						#if UNITY_ANDROID
						_narrator.acaname = a.acaName;
						#elif UNITY_IOS
						_narrator.acaname = a.iOSacaName;
						#endif
						_narrator.rate = trglobals.instance._trdactors [i].rate;
						_narrator.shape = trglobals.instance._trdactors [i].shape;
						_narrator.changedVoice = true;
						break;
					}
				}
				if (_scriptscenes.Count != 0)
					trglobals.instance._trabt.endProcess ();
				else
					ScriptError ();
					
			}
			else {
				ScriptError ();
			}
		}
	}

	void ScriptError() {
		TheNextFlow.UnityPlugins.MobileNativePopups.OpenAlertDialog (
			"ERROR", "This PDF cannot be read by tableread.\nPlease export from professional screenwriting software.",
			"DISMISS",
			() => { 
				trglobals.instance.DebugLog ("CANCEL was pressed");
			});
	}

	public void createFrontPageEntry() {
	//	trglobals.instance.DebugLog ("createFrontPageEntry");
		bool foundentry = false;
		string pf = trglobals.instance.projectName + "_" + trglobals.instance.projectVersion;
		float pc = (float)speakingline/(float)_scriptlines.Count;
		for (int i = 0; i < trglobals.instance._trfp._frontpagescripts.Count; i++) {
			if (trglobals.instance._trfp._frontpagescripts[i].name.Equals(pf)) {
				foundentry = true;
				trglobals.instance._trfp._frontpagescripts[i].linenumber =  speakingline;
				trglobals.instance._trfp._frontpagescripts [i].percentage = pc;
				trglobals.instance._trfp._frontpagescripts [i].bar.transform.localScale = new Vector3 (pc,1,1);
			}
		}
		if (!foundentry) {
			trglobals.instance._trfp.AddFrontPageScript(pf,speakingline,pc,trglobals.instance._trfp._frontpagescripts.Count);
		}
	}

	public void clearScript() {
	//	trglobals.instance.DebugLog ("clearScript");
		trglobals.instance.continueScript = false;
		trglobals.instance.scriptLoaded = false;
		trglobals.instance.showedmissingscrores = false;
		_scriptlines.Clear ();
		_scriptscenes.Clear ();
		_scriptactors.Clear ();
		trglobals.instance._trnte.cleanup ();
		trglobals.instance._trscn.cleanup ();
		trglobals.instance._tract.cleanup ();
		if (_scoreAudio.isPlaying)
			_scoreAudio.Stop ();
		if (trglobals.instance._audioSource.isPlaying)
			trglobals.instance._audioSource.Stop ();
		//trglobals.instance._scriptController.clear ();
	}

	PdfReader pdfReader;
public void ReadPdfFile(string fileName, bool unembedfont = true)
	{
		isMacEncoded = false;
		clearScript ();
		trglobals.instance.cleanup ();
	//	trglobals.instance.DebugLog ("ReadPdfFile " + fileName);
		if (File.Exists (fileName)) {
			pdfReader = new PdfReader (fileName);
			PdfReaderContentParser parser = new PdfReaderContentParser (pdfReader);
			totalpagecount = pdfReader.NumberOfPages;
		//	trglobals.instance.DebugLog("NUMBER OF PAGES IS " + totalpagecount);
			trglobals.instance.pdfWidth = pdfReader.GetPageSize (1).Width;
			trglobals.instance.pdfHeight = iTextSharp.text.Utilities.PointsToMillimeters (pdfReader.GetPageSize (1).Height);
			trglobals.instance.pdfRatio = (float)trglobals.instance.screenwidth / trglobals.instance.pdfWidth;
			//	_scriptPREFAB.gameObject.SetActive (true);
			currentline = 0;
			currentscene = 0;
			trglobals.instance.continueScript = true;
		#if UNITY_IOS
		string destpath = System.IO.Path.Combine (Application.persistentDataPath,"voices");
		string dest = System.IO.Path.Combine (destpath, "abc123.pdf");

			if (unembedfont) {
				PdfObject obj;
				for (int i = 1; i <= totalpagecount; i++) {
				//Get first page,Generally we get font information on first page,however we can loop throw pages e.g for(int i=0;i<=pdfReader.NumberOfPages;i++)
					PdfDictionary cpage = pdfReader.GetPageN(i);
					if (cpage != null) {
						//return;
					//	trglobals.instance.DebugLog("checking page " + i);
						PdfDictionary dictFonts = cpage.GetAsDict(PdfName.RESOURCES).GetAsDict(PdfName.FONT);
						if (dictFonts != null)
						{
						foreach (var font in dictFonts) {
								var dictFontInfo = dictFonts.GetAsDict(font.Key);

								if (dictFontInfo != null)
								{
									foreach (var f in dictFontInfo)
									{
			
										PdfName baseFont = dictFontInfo.GetAsName(PdfName.BASEFONT);
										PdfName baseEncoding = dictFontInfo.GetAsName(PdfName.ENCODING);
									//	if (baseFont.ToString ().Contains("Italic"))
										//	trglobals.instance.DebugLog (baseFont.ToString ());
										if (baseEncoding != null) {
											if (baseEncoding.ToString ().Equals ("/MacRomanEncoding")) {
												//	trglobals.instance.DebugLog (":ENCD" + baseEncoding.ToString () +";");
												isMacEncoded = true;
												baseEncoding = new PdfName ("WinAnsiEncoding");
											//	baseFont = new PdfName ("Courier");
											//	dictFontInfo.Put(PdfName.BASEFONT, baseFont);
												dictFontInfo.Put (PdfName.ENCODING, baseEncoding);
											}
										break;
										}
									}
								}
							}
						}

					}
				}
				// removing unused objects will remove unused font file streams
				pdfReader.RemoveUnusedObjects ();
				// we persist the altered document
		//		Debug.Log("MAC ENCODED " + isMacEncoded);
				if (isMacEncoded) {
					FileStream os = new FileStream (dest, FileMode.Create, FileAccess.Write, FileShare.None);
					PdfStamper stamper = new PdfStamper (pdfReader, os);
					stamper.Close ();
					ReadPdfFile (dest, false);
				}
				else
					StartCoroutine (ProcessPage (1, parser));
			}
		else {
				File.Delete(dest);
				StartCoroutine (ProcessPage (1, parser));
		}
		#elif UNITY_ANDROID
			StartCoroutine (ProcessPage (1, parser));
		#endif

		} else
			trglobals.instance.DebugLog ("NO PDF");
	}

bool isMacEncoded = false;

	public void unembedTTF(PdfDictionary dict) {
			// we ignore all dictionaries that aren't font dictionaries
			if (!dict.IsFont ()) {
				return;
			}
			// we only remove TTF fonts
			if (dict.GetAsDict(PdfName.FONTFILE2) != null) {
				return;
			}
		trglobals.instance.DebugLog ("CONTINUE");
			// check if a subset was used (in which case we remove the prefix)
			PdfName baseFont = dict.GetAsName(PdfName.BASEFONT);

			PdfName baseEncoding = dict.GetAsName(PdfName.ENCODING);
			trglobals.instance.DebugLog (baseFont.ToString () + ":ENCD" + baseEncoding.ToString () +";");
			if (baseEncoding.ToString ().Equals ("/MacRomanEncoding")) {
				isMacEncoded = true;
	
				baseEncoding = new PdfName ("WinAnsiEncoding");
				baseFont = new PdfName ("Courier");
				/*	if (baseFont.GetBytes()[7] == '+') {
					baseFont = new PdfName(baseFont.ToString().Substring(8));
					baseEncoding = new PdfName ("WinAnsiEncoding");
				//	baseFont = new PdfName ("Courier");
					dict.Put(PdfName.BASEFONT, baseFont);
					
					//dict.Put(PdfName.BASEENCODING,"Ansi");
				}*/
				dict.Put(PdfName.BASEFONT, baseFont);
				dict.Put (PdfName.ENCODING, baseEncoding);
				// we check if there's a font descriptor
				PdfDictionary fontDescriptor = dict.GetAsDict (PdfName.FONTDESCRIPTOR);
				if (fontDescriptor == null) {
					trglobals.instance.DebugLog ("NO FONT DESCRIPTOR");
					return;
				}
				// is there is, we replace the fontname and remove the font file

				fontDescriptor.Put(PdfName.FONTNAME, baseFont);
				fontDescriptor.Put (PdfName.ENCODING, baseEncoding);
				fontDescriptor.Remove (PdfName.FONTFILE2);
			}
		}

	public class LocationTextExtractionStrategyWithPosition : LocationTextExtractionStrategy
	{
	//	public int pagenumber;
	//	public int scenenumber;
		private readonly List<TextChunk> locationalResult = new List<TextChunk>();
		private readonly ITextChunkLocationStrategy tclStrat;
		public LocationTextExtractionStrategyWithPosition() : this(new TextChunkLocationStrategyDefaultImp()) {
		
		}

		/**
         * Creates a new text extraction renderer, with a custom strategy for
         * creating new TextChunkLocation objects based on the input of the
         * TextRenderInfo.
         * @param strat the custom strategy
         */
		public LocationTextExtractionStrategyWithPosition(ITextChunkLocationStrategy strat) {
			tclStrat = strat;
		}

		private bool StartsWithSpace(string str) {
			if (str.Length == 0) return false;
			return str[0] == ' ';
		}

		private bool EndsWithSpace(string str) {
			if (str.Length == 0) return false;
			return str[str.Length - 1] == ' ';
		}
		/**
         * Filters the provided list with the provided filter
         * @param textChunks a list of all TextChunks that this strategy found during processing
         * @param filter the filter to apply.  If null, filtering will be skipped.
         * @return the filtered list
         * @since 5.3.3
         */

		private List<TextChunk> filterTextChunks(List<TextChunk> textChunks, ITextChunkFilter filter) {
			if (filter == null) {
				return textChunks;
			}
			var filtered = new List<TextChunk>();
			foreach (var textChunk in textChunks) {
				if (filter.Accept(textChunk)) {
					filtered.Add(textChunk);
				}
			}
			return filtered;
		}

		public override void RenderText(TextRenderInfo renderInfo)
		{
			LineSegment segment = renderInfo.GetBaseline();
			if (renderInfo.GetRise() != 0)
			{ // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to 
				Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
				segment = segment.TransformBy(riseOffsetTransform);
			}
			TextChunk tc = new TextChunk(renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
			locationalResult.Add(tc);
		}


		public bool Approx_X(float x1, float x2) {
			float d = x1 - x2;
			if (d < 0)
				d *= -1;
			if (d <= 2.5f)
				return true;
			else
				return false;
		}

		public List<ScriptLines> GetLocations()
		{
			float lastX = 0;
			float lastY = 0;
			string textline = "";
			var filteredTextChunks = filterTextChunks(locationalResult, null);
			filteredTextChunks.Sort();

			TextChunk lastChunk = null;

			var textLocations = new List<ScriptLines>();

			foreach (var chunk in filteredTextChunks) {
				if (lastChunk == null) {
					lastY = 0;lastX = 0;
					//initial
					if (chunk.Text.Trim ().Length > 0) {
						float currentX = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [0]);
						float currentY = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [1]);
						if (currentY > 0) {
							bool newLine = true;
							if (Approx_X(currentX, lastX) && (currentY - lastY) > -5.5f)
								newLine = false;
							if (true) {//newline
							//	trglobals.instance.DebugLog (chunk.Text + ":" + currentX);
								textLocations.Add (new ScriptLines {
									Text = chunk.Text,
									X = Mathf.FloorToInt(currentX),//(int)currentX,
									Y = Mathf.FloorToInt(currentY),//(int)currentY,
								});
							} else {
								textline = "";
								// we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
								if (IsChunkAtWordBoundary (chunk, lastChunk) && !StartsWithSpace (chunk.Text) && !EndsWithSpace (lastChunk.Text))
									textline += ' ';
								textline += chunk.Text;
								textLocations [textLocations.Count - 1].Text += textline;
							}
						//	trglobals.instance.DebugLog (currentX + ":" + currentY + ":" + chunk.Text);
						//	trglobals.instance.DebugLog ("lastChunk == null: " + (currentY - lastY) + ":" + newLine);
						}
						lastY = currentY;
						lastX = currentX;
					}
				}
				else
				{
					if (chunk.SameLine(lastChunk)) {
					//	if (!sameline)
						textline = "";
						// we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
						if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.Text) && !EndsWithSpace(lastChunk.Text))
							textline += ' ';
						textline += chunk.Text;
						if (textLocations.Count > 0)
							textLocations[textLocations.Count - 1].Text += textline;
					}
					else {
						if (chunk.Text.Trim ().Length > 0) {
							float currentX = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [0]);
							float currentY = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [1]);
							if (currentY > 0) {
								bool newLine = true;
							//	trglobals.instance.DebugLog (chunk.Text + ":" + currentX + ":" + lastX + ":" + (Mathf.Approximately(currentX, lastX)) );
								if (Approx_X(currentX, lastX) && (currentY - lastY) > -5.5f)
									newLine = false;
								if (true) {// newline
								//	trglobals.instance.DebugLog (chunk.Text + ":" + currentX);
									textLocations.Add (new ScriptLines {
										Text = chunk.Text,
										X = Mathf.FloorToInt(currentX),//(int)currentX,
										Y = Mathf.FloorToInt(currentY),//(int)currentY,
									});
								} else {
									textline = "";
									// we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
									if (IsChunkAtWordBoundary (chunk, lastChunk) && !StartsWithSpace (chunk.Text) && !EndsWithSpace (lastChunk.Text))
										textline += ' ';
									textline += chunk.Text;
									textLocations [textLocations.Count - 1].Text += textline;
								}
					//			Debug.Log (currentX + ":" + lastX + ":" +currentY + ":" + lastY + ":" + chunk.Text + ":" + newLine.ToString());
							}
							lastY = (int)currentY;
							lastX = (int)currentX;
						}
					}
				}
				lastChunk = chunk;
			}
			//now find the location(s) with the given texts
			textLocations.Sort(delegate(ScriptLines a, ScriptLines b) {
			//	Debug.Log(a.Y - b.Y);
				if (a.Y - b.Y < 2 && a.Y - b.Y >= 0)
					return a.X.CompareTo(b.X);
				return b.Y.CompareTo(a.Y);
			});
			// lets combine lines on the same line
			int currY = 10000;
			int currX = 10000;
			int backwards = 1;
			for (int i = 0; i < textLocations.Count; i++) {
				if ((currY - textLocations [i].Y) < 1.5f) {
					textLocations [i - backwards].Text += (" " + textLocations [i].Text);
					textLocations [i] = null;
					backwards++;
				} else {
					currY = (int)textLocations [i].Y;
					currX = (int)textLocations [i].X;
					backwards = 1;
				}
			}
			 currY = 10000;
			 currX = 10000;
			 backwards = 1;
			for (int i = 0; i < textLocations.Count; i++) {
				if (textLocations [i] != null) {
					if ((currX == textLocations [i].X) && (currY - textLocations [i].Y) < 5.5f) {
						textLocations [i - backwards].Text += (" " + textLocations [i].Text);
						currY = (int)textLocations [i].Y;
						textLocations [i] = null;
						backwards++;
					} else {
						currY = (int)textLocations [i].Y;
						currX = (int)textLocations [i].X;
						backwards = 1;
					}
				} else
					backwards++;
			}
			return textLocations;
		}
	}

	public class ScriptLines
	{
		public float X { get; set; }
		public float Y { get; set; }
		public	string			Text 		{ get; set; }
	}

	[System.Serializable]
	public class scriptscene
	{
		public	int 	scenenumber;	
		public	string	name;
		public	bool	rehearse;
		public	int 	linenumber;
		public scriptscene(int sn, string n, bool r, int ln) {
			scenenumber = sn; name = n; rehearse = r; linenumber = ln;
		}
		public	List<string> actorsInScene = new List<string> ();
	}

}
