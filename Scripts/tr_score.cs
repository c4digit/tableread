using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class tr_score : panel {

	public	Text	btnTXT;
	public	Toggle	poaTGL;
	public	Toggle	scoreONOFFTGL;
	public	Slider	volumeSLD;
	public	scoreCell[]			_scoreCells;
	public	MobileUIController	_muc;

	public bool _saveTRD;
	void Start() {
		_scoreCells = GetComponentsInChildren<scoreCell> ();
	}

	public void Setup() {
		_saveTRD = false;
		//if (trglobals.instance.muteScore) {
		//	btnTXT.text = "Score Off";
	///	} else {
		//	btnTXT.text = "Score On";
	//	}
		for (int i = 0; i < _scoreCells.Length; i++) {
			_scoreCells [i].Setup (trglobals.instance.displayMusicTitles [i], i);
		}
		for (int i = 0; i < _scoreCells.Length; i++) {
			if (_scoreCells [i].scoreTGL.isOn) {
				int v = _scoreCells [i].index;
				string track = trglobals.instance.musicTitles [v] + ".mp3";
				if (!System.IO.File.Exists (System.IO.Path.Combine (System.IO.Path.Combine (Application.persistentDataPath, "scores"), track))) {
					//	trglobals.instance.ShowError ("Track not found '" + trglobals.instance.displayMusicTitles [v] + "'.\nPlease download if you would like to use this track.", "Missing Score");
					_scoreCells [v].scoreTGL.isOn = false;
					trglobals.instance.DebugLog ("Setting toogle ON");
					_scoreCells [10].scoreTGL.isOn = true;
				} 
			}
		}
		volumeSLD.value = trglobals.instance.scoreVolume;
		poaTGL.isOn = (trglobals.instance.peakOverAction == 1);
		scoreONOFFTGL.isOn = !trglobals.instance.muteScore;
		trglobals.instance._trvs._scoreAudio.Stop ();
		setPosition (true);
	}

	public void SetupScoreCells() {
		for (int i = 0; i < _scoreCells.Length; i++) {
			_scoreCells [i].Setup (trglobals.instance.displayMusicTitles [i], i);
		}
		for (int i = 0; i < _scoreCells.Length; i++) {
			if (_scoreCells [i].scoreTGL.isOn) {
				int v = _scoreCells [i].index;
				string track = trglobals.instance.musicTitles [v] + ".mp3";
				if (!System.IO.File.Exists (System.IO.Path.Combine (System.IO.Path.Combine (Application.persistentDataPath, "scores"), track))) {
					//	trglobals.instance.ShowError ("Track not found '" + trglobals.instance.displayMusicTitles [v] + "'.\nPlease download if you would like to use this track.", "Missing Score");
					_scoreCells [v].scoreTGL.isOn = false;
					trglobals.instance.DebugLog ("Setting toogle ON");
					_scoreCells [10].scoreTGL.isOn = true;
				} 
			}
		}
	}

	public void toggleMuteScore() {
		_saveTRD = true;
		trglobals.instance.muteScore = !scoreONOFFTGL.isOn;
		if (trglobals.instance.muteScore)
			PlayerPrefs.SetInt ("mutescore", 1);
		else
			PlayerPrefs.SetInt ("mutescore", 0);
		PlayerPrefs.Save ();
		
		//	if (PlayerPrefs.GetInt ("mutescore") == 0)
		//if (trglobals.instance.muteScore) {
		//	btnTXT.text = "Score Off";
		//} else {
		//	btnTXT.text = "Score On";
		//}
	}

	public void SetPeakOverAction() {
		trglobals.instance.peakOverAction = (poaTGL.isOn ? 1 : 0);
		trglobals.instance.DebugLog (trglobals.instance.scoreVolume + ":" + trglobals.instance.peakOverAction);
		_saveTRD = true;
	}

	int selectedscore;

	public void previewScore(scoreCell sc) {
		int v = sc.index;
		if (trglobals.instance._audioSource.isPlaying && selectedscore == v) {
			trglobals.instance._audioSource.Stop();
			return;
		}
		selectedscore = v;
		string sb = trglobals.instance.musicTitles [v] + "_prv";
	//	trglobals.instance.DebugLog ("loading " + sb);
		AudioClip ac = Resources.Load<AudioClip>("sound/"+sb);
		trglobals.instance._audioSource.clip = ac;
		trglobals.instance._audioSource.Play ();
	}

	public void toggleScore(scoreCell sc) {
		if (sc.scoreTGL.isOn)
			return;
		int v = sc.index;
		for (int i = 0; i < _scoreCells.Length; i++) {
			if (i == v)
				_scoreCells [i].scoreTGL.isOn = true;
			else
				_scoreCells [i].scoreTGL.isOn = false;
		}
		string track = trglobals.instance.musicTitles [v] + ".mp3";
		if (!System.IO.File.Exists (System.IO.Path.Combine (System.IO.Path.Combine (Application.persistentDataPath, "scores"), track))) {
			trglobals.instance.ShowError ("Track not found '" + trglobals.instance.displayMusicTitles [v] + "'.\nPlease download if you would like to use this track.", "Missing Score");
			_scoreCells [v].scoreTGL.isOn = false;
			_scoreCells [10].scoreTGL.isOn = true;
		} else {
			trglobals.instance.currentScore = v;
			trglobals.instance.DebugLog ("toggleScore currentScore " + trglobals.instance.currentScore);
			_saveTRD = true;
		}
	}
}
