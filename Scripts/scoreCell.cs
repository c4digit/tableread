using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class scoreCell : MonoBehaviour {

	public	Text		scoreTXT;
	public	Toggle		scoreTGL;
	public	GameObject	btnDownloadPNL;
	public	GameObject	btnCancelPNL;
	public	int			index;
	public	GameObject	arrowBTN;
	public	ScrollRect	horSCR;
	public	Image 		downloadPRG;
	public	GameObject	downloadPNL;
	public	Transform	progressIMG;
	string track;
	string scoreURL;

	void Start() {
		if (progressIMG) {
			Image pi = progressIMG.GetComponent<Image> ();
			if (pi != null)
				pi.color = trglobals.instance.progressIMGCLR;
		}
	}

	public void Setup(string s, int i) {
		scoreTXT.text = s;
		index = i; 
		track = trglobals.instance.musicTitles [i] + ".mp3";
		scoreURL = "https://s3.amazonaws.com/trscores/" + track;
		if (i == trglobals.instance.currentScore) {
			scoreTGL.isOn = true;
		} else {
			if (i == 10)
				Debug.Log ("Setting toogle off");
			scoreTGL.isOn = false;
		}
		if (!File.Exists (Path.Combine (Path.Combine (Application.persistentDataPath, "scores"), track))) {
			if (btnDownloadPNL)
				btnDownloadPNL.SetActive (true);
			if (btnCancelPNL)
				btnCancelPNL.SetActive (false);
			if (downloadPRG)
				downloadPRG.fillAmount = 0;
			if (downloadPNL)
				downloadPNL.SetActive (true);
			if (progressIMG)
				progressIMG.localScale = new Vector3 (0, 1, 1);
			arrowBTN.SetActive (false);
			horSCR.enabled = false;
			if (trglobals.instance._trscr._muc.ebdCtl.IsRunning (scoreURL)) {
				if (btnDownloadPNL)
					btnDownloadPNL.SetActive (false);
				if (btnCancelPNL)
					btnCancelPNL.SetActive (true);
				StartCoroutine ("checkProgress");
			}
			if (PlayerPrefs.HasKey(scoreURL)) {
				StartCoroutine ("checkProgress");
				return;
			}
		} else {
			if (btnDownloadPNL)
				btnDownloadPNL.SetActive (false);
			if (btnCancelPNL)
				btnCancelPNL.SetActive (false);
			if (progressIMG)
				progressIMG.localScale = new Vector3 (1, 1, 1);
			if (downloadPNL)
				downloadPNL.SetActive (false);
			if (index != 10) {
				arrowBTN.SetActive (true);
				horSCR.enabled = true;
			} else {
				arrowBTN.SetActive (false);
				horSCR.horizontalNormalizedPosition = 0;
				horSCR.enabled = false;
			}
		}
	}

	public void DownloadScore() {
		Debug.Log (scoreURL + ":" + trglobals.instance._trscr._muc.ebdCtl.IsRunning(scoreURL));
		if (btnDownloadPNL)
			btnDownloadPNL.SetActive (false);
		if (btnCancelPNL)
			btnCancelPNL.SetActive (true);
		if (downloadPRG)
			downloadPRG.fillAmount = 0;
		trglobals.instance._trscr._muc.OnClickStartBtn (scoreURL,(Path.Combine (Application.persistentDataPath, "scores")),scoreTXT.text);
		StartCoroutine ("checkProgress");
	}

	public void CancelDownloadScore() {
		Debug.Log ("CANCEL " + scoreURL + ":" + trglobals.instance._trscr._muc.ebdCtl.IsRunning(scoreURL));
		if (btnDownloadPNL)
			btnDownloadPNL.SetActive (true);
		if (btnCancelPNL)
			btnCancelPNL.SetActive (false);
		if (downloadPRG)
			downloadPRG.fillAmount = 0;
		trglobals.instance._trscr._muc.OnClickStopBtn (scoreURL);
		StopCoroutine("checkProgress");
	}

	IEnumerator checkProgress() {
		float f = 0;
		bool started = false;
		if (downloadPRG)
			downloadPRG.fillAmount = 0.05f;
		while (f < 1) {
			f = trglobals.instance._trscr._muc.CheckProgress(scoreURL);
			if (f != 0)
				started = true;
			if (started && f == 0)
				f = 1;
			//Debug.Log("SCORECELL checkProgress " + scoreURL + ":" + f);
			if (downloadPRG)
				downloadPRG.fillAmount = f;
			if (progressIMG)
				progressIMG.localScale = new Vector3 (f, 1, 1);
			yield return null;
		}
		if (progressIMG)
			progressIMG.localScale = new Vector3 (1, 1, 1);
		trglobals.instance._trscr.SetupScoreCells ();
	}

	public void DeleteScore() {
		string p = Path.Combine (Path.Combine (Application.persistentDataPath, "scores"), track);
		Debug.Log ("deleting " + p);
		if (File.Exists (p)) {
			File.Delete (p);
			horSCR.horizontalNormalizedPosition = 0;
			if (btnDownloadPNL)
				btnDownloadPNL.SetActive (true);
			if (downloadPRG)
				downloadPRG.fillAmount = 0;
			if (downloadPNL)
				downloadPNL.SetActive (true);
			if (progressIMG)
				progressIMG.localScale = new Vector3 (0, 1, 1);
			arrowBTN.SetActive (false);
			horSCR.enabled = false;
		}
	}
}
