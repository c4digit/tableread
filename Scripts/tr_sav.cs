using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class tr_sav : panel {

	public bool isNarrator;
	public	string tabrName;
	public	string acaName;
	public float rate;
	public float shape;
	public	GameObject[]	_missingVoicesGO;
	public	Dropdown	_voiceDD;
	public Button		_setBTN;
	public Button		_downloadBTN;
	public Text			_missingTXT;
	public bool hasFolder;

	public	Slider _rateSLD;
	public	Slider _shapeSLD;
	public	Slider	_ageSLD;
	public	Toggle	isMaleTGL;
	public	Toggle	isFemaleTGL;
	public int index;

	public	Sprite	_optionspr;

	public	Transform	progressIndicator;
	//string acapreviewSTR = "Make your table reed really come to life. Writers, listen to your scripts as read by over 40 unique characters, even add an inspiring score. Directors and Producers, " +
	//	"create and share script notes in app, and keep your cast and crew on the same page. Actors, rehearse with an entire cast.";

	string acapreviewSTR  = "Make your table read really come to life. Have it read by an entire cast of characters, add an inspiring score, utilize rehearsal features, create and share notes";
	// Use this for initialization

	public void getDropdownOptions() {
		_voiceDD.ClearOptions ();
		List<string> v = new List<string> ();
		v.Add ("SELECT VOICE");
		for (int i = 0; i < trglobals.instance._tablereadActors.Count; i++) {
			bool addname = false;
			if (_ageSLD.value == 0) {
				addname = true;
			} else if (_ageSLD.value == 1 && trglobals.instance._tablereadActors [i].age == "CHILD") {
				addname = true;
			}
			else if (_ageSLD.value == 2 && trglobals.instance._tablereadActors [i].age == "TEEN") {
				addname = true;
			}
			else if (_ageSLD.value == 3 && trglobals.instance._tablereadActors [i].age == "ADULT") {
				addname = true;
			}
			else if (_ageSLD.value == 4 && trglobals.instance._tablereadActors [i].age == "SENIOR") {
				addname = true;
			}
		//	Debug.Log (_ageSLD.value + ":" + trglobals.instance._tablereadActors [i].age + ":" + addname);
			if (isMaleTGL.isOn && !isFemaleTGL.isOn) {
				if (trglobals.instance._tablereadActors [i].gender != "M")
					addname = false;
			}
			else if (!isMaleTGL.isOn && isFemaleTGL.isOn) {
				if (trglobals.instance._tablereadActors [i].gender != "F")
					addname = false;
			}
			if (addname)
				v.Add (trglobals.instance._tablereadActors [i].tabrname);
		}
		List<Dropdown.OptionData> voiceItems = new List<Dropdown.OptionData> ();
		for (int i = 0; i < v.Count; i++) {
			if (trglobals.instance.hasAcaFolder (v[i])) {
				var voiceOption = new Dropdown.OptionData (v[i],_optionspr);
				voiceItems.Add (voiceOption);
			}
			else {
				var voiceOption = new Dropdown.OptionData (v[i],null);
				voiceItems.Add (voiceOption);
			}
		}

		_voiceDD.AddOptions (voiceItems);
		_voiceDD.value = 1;
	//	for (int i = 0; i < _voiceDD.options.Count; i++) {
			//_voiceDD.
		//	Dropdown.OptionData od = _voiceDD.options [i];
	//	}
		changeVoice ();
	}

	bool settingup = true;

	public void Setup(string h,string tba,bool isn,string aca, float r, float s, int ind) {
		_headingSTR = h;
		settingup = true;
		tabrName = tba;
		acaName = aca;
		rate = r;
		shape = s;
		isNarrator = isn;
		index = ind;
		_rateSLD.value = rate;
		_shapeSLD.value = shape;
		getDropdownOptions ();
	//	_voiceDD.value = 0;
		_voiceDD.captionText.text = tabrName;
		for (int i = 0; i < _voiceDD.options.Count; i++) {
			if (tabrName.Equals(_voiceDD.options[i].text)) {
				_voiceDD.value = i;
				break;
			}
		}
		checkVoice (tabrName);
		currentdownloadvoice = null;
		progressIndicator.localScale = new Vector3 (0, 1, 1);
		previewing = false;
		setPosition (true);
		settingup = false;
	}

	void Update() {
		if (currentdownloadvoice != null) {
			progressIndicator.localScale = currentdownloadvoice.progressIMG.localScale;
		}
	}

	public void setRate() {
	//	Debug.Log ("setRate "+_rateSLD.value);
		rate = _rateSLD.value;
	}

	public void setShape() {
	//	Debug.Log ("setShape " + _shapeSLD.value);
		shape = _shapeSLD.value;
	}

//	bool changedvoice = false;

	public void changeVoice() {
	//	Debug.Log ("changeVoice " + settingup);
		//if (changedvoice) {
		//	changedvoice = false;return;
		//}
		if (settingup)
			return;
	//	Debug.Log (_voiceDD.value);
		tabrName = _voiceDD.options [_voiceDD.value].text;
		tablereadActor ta = trglobals.instance.getAcaActor (tabrName);
		#if UNITY_ANDROID
		acaName = ta.acaName;
		#elif UNITY_IOS
		acaName = ta.iOSacaName;
		#endif
		shape = ta.shape;
		_rateSLD.value = rate;
		_shapeSLD.value = shape;
		checkVoice (tabrName);
	//	index = _voiceDD.value;
		// force stop
		previewing = true;
		PreviewVoice ();
		//changedvoice = true;
		//_voiceDD.value = 0;
	}

	public bool previewing = false;

	public void PreviewVoice() {
	//	Debug.Log ("PreviewVoice " + tabrName + ":" + acaName + ":" + _voiceDD.captionText.text);
		if (previewing) {
			previewing = false;
			if (trglobals.instance._audioSource.isPlaying)
				trglobals.instance._audioSource.Stop ();
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (trglobals.instance._tts.isSpeaking())
				trglobals.instance._tts.tts_stop ();
			#elif UNITY_IOS && !UNITY_EDITOR
			if (trglobals.instance._tts_ios.isSpeaking())
				trglobals.instance._tts_ios.tts_stop ();
			#endif
			return;
		}
		if (!hasFolder) {
				string sb = _voiceDD.captionText.text;
				sb = sb.Replace ("(", "");
				sb = sb.Replace (")", "");
				sb = sb.Replace (" ", "");
				sb = sb.ToLower ();
				Debug.Log (sb);
				AudioClip ac = Resources.Load<AudioClip> ("sound/" + sb);
				trglobals.instance._audioSource.clip = ac;
				trglobals.instance._audioSource.Play ();
				trglobals.instance._audioSource.loop = false;
				previewing = true;
		} else {
			#if UNITY_ANDROID && !UNITY_EDITOR
				previewing = true;
				if (trglobals.instance._tts.TTS == null)
					trglobals.instance._tts.tts_init ();
				if (trglobals.instance._tts.isSpeaking())
					trglobals.instance._tts.tts_stop ();
				Debug.Log(rate + ":" + shape);
				trglobals.instance._tts.tts_speak (acapreviewSTR,acaName,acaName,rate,shape);
			#elif UNITY_IOS && !UNITY_EDITOR
				previewing = true;
				trglobals.instance._tts_ios.getTTS();
				if (trglobals.instance._tts_ios.isSpeaking())
					trglobals.instance._tts_ios.tts_stop ();
				Debug.Log(rate + ":" + shape);
				trglobals.instance._tts_ios.tts_speak (acapreviewSTR,acaName,acaName,rate,shape);
			#endif
		}
	}

	public GameObject missingTXTPNL;

	public void checkVoice(string f) {
		if (currentdownloadvoice != null) 
			Debug.Log ("checkvoice " + f + ":" + currentdownloadvoice.voiceTXT.text + ":"+tabrName);
		if (string.IsNullOrEmpty(f) && currentdownloadvoice != null)
			f = currentdownloadvoice.voiceTXT.text;
		progressIndicator.localScale = new Vector3 (0, 1, 1);
		if (!trglobals.instance.hasAcaFolder (f)) {
			Debug.Log ("NO FOLDER " + tabrName);
			hasFolder = false;
			for (int i = 0; i < _missingVoicesGO.Length; i++) {
				_missingVoicesGO [i].SetActive (false);
			}
			missingTXTPNL.SetActive (true);
			//_missingTXT.gameObject.SetActive(true);
			_setBTN.gameObject.SetActive(false);
			if (currentdownloadvoice == null) {
				_downloadBTN.gameObject.SetActive (true);
				_downloadBTN.interactable = true;
				_missingTXT.text = "Voice needs to be downloaded";
			}

			CheckDownloadInProgress ();
		} else {
			Debug.Log ("FOLDER " + tabrName);
			hasFolder = true;
			for (int i = 0; i < _missingVoicesGO.Length; i++) {
				_missingVoicesGO [i].SetActive (true);
			}
			missingTXTPNL.SetActive (false);
			//_missingTXT.gameObject.SetActive(false);
			_setBTN.gameObject.SetActive(true);
			_downloadBTN.gameObject.SetActive(false);
		}
	}

	public voiceCell	currentdownloadvoice;

	void CheckDownloadInProgress() {
		for (int i = 0; i < trglobals.instance._trdwa._actorCells.Count; i++) {
			if (trglobals.instance._trdwa._actorCells [i].voiceTXT.text.Equals (tabrName)) {
				if (trglobals.instance._trdwa._actorCells [i].progressIMG.localScale.x > 0) {
					currentdownloadvoice = trglobals.instance._trdwa._actorCells [i];
					_downloadBTN.interactable = false;
					_missingTXT.text = "downloading voice " + tabrName;
					break;
				}
			}
		}
	}

	public void DownloadSet() {
	//	Debug.Log ("Need to Donwload & set scriptactords " + tabrName);
		for (int i = 0; i < trglobals.instance._trdwa._actorCells.Count; i++) {
			if (trglobals.instance._trdwa._actorCells [i].voiceTXT.text.Equals (tabrName)) {
			//	Debug.Log ("found voice cell");
				currentdownloadvoice = trglobals.instance._trdwa._actorCells [i];
				currentdownloadvoice.AutoDownload ();
				_downloadBTN.interactable = false;
				_missingTXT.text = "downloading voice " + tabrName;
			}
		}
	}

	public void Set() {
		// check this
	//	Debug.Log ("Need to set scriptactords " + index);
		if (index >= 0) {
			trglobals.instance._trvs._scriptactors [index].tabrname = tabrName;
			trglobals.instance._trvs._scriptactors [index].acaname = acaName;
			trglobals.instance._trvs._scriptactors [index].rate = rate;
			trglobals.instance._trvs._scriptactors [index].shape = shape;
			trglobals.instance._trvs._scriptactors [index].changedVoice = true;
		} else {
			trglobals.instance._trvs._narrator.tabrname = tabrName;
			trglobals.instance._trvs._narrator.acaname = acaName;
			trglobals.instance._trvs._narrator.rate = rate;
			trglobals.instance._trvs._narrator.shape = shape;
			trglobals.instance._trvs._narrator.changedVoice = true;
		}
		previewing = true;
		PreviewVoice ();
		trglobals.instance.ShowError("","Voice Set");
	//	trglobals.instance.genericBack ();
		string TRDName = trglobals.instance.projectName+"_" + trglobals.instance.projectVersion + ".trd";
		Debug.Log (TRDName);
		string TRDPath = System.IO.Path.Combine (Application.persistentDataPath, TRDName);
		trglobals.instance.createTRD(TRDPath);
	}
}
