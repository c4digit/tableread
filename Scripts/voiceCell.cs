using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class voiceCell : MonoBehaviour {

	public	Text		voiceTXT;
//	public	int			index;
	public	GameObject	btnDownloadPNL;
	public	GameObject	btnCancelPNL;
	public	GameObject	arrowBTN;
	public	ScrollRect	horSCR;
	public	Image 		downloadPRG;
	public	GameObject	downloadPNL;

	public	Transform	progressIMG;

	public string voiceURL;
	public string destZip = "";
	string folder;
	public string acaname;
	// Use this for initialization
	void Start() {
		if (progressIMG) {
			Image pi = progressIMG.GetComponent<Image> ();
			if (pi != null)
				pi.color = trglobals.instance.progressIMGCLR;
		}
	}

	public void Setup(string s, string f, string a) {
		voiceTXT.text = s;
		//index = i;
		voiceURL = "https://s3.amazonaws.com/trsvoices/" +  a.ToLower () +".zip";
		folder = f;
		acaname = a;
		string dir = Path.Combine (Application.persistentDataPath, "voices");
		destZip = Path.Combine(dir,(acaname.ToLower() + ".zip"));
	/*	#if UNITY_IOS
		Debug.Log ("Defualt voices Setup " + destZip);
		if (File.Exists (destZip)) {
			Debug.Log ("I HAVE ZIP FILE TRYING TO UNZIP");
			ZipUtil.Unzip (destZip, (Path.Combine (Application.persistentDataPath, "voices")));
			File.Delete (destZip);
		}
		#endif*/
	}

	public void checkFolder() {
	//	Debug.Log ("voiceCell CheckFolder " + folder);
		if (!Directory.Exists (Path.Combine (Path.Combine (Application.persistentDataPath, "voices"), folder))) {
			// don't have folder
			if (progressIMG)
				progressIMG.localScale = new Vector3 (0, 1, 1);
			if (btnDownloadPNL)
				btnDownloadPNL.SetActive (true);
			if (btnCancelPNL)
				btnCancelPNL.SetActive (false);
			if (downloadPRG)
				downloadPRG.fillAmount = 0;
			if (downloadPNL)
				downloadPNL.SetActive (true);
			if (arrowBTN)
				arrowBTN.SetActive (false);
			if (horSCR)
				horSCR.enabled = false;
			if (trglobals.instance._trscr._muc.ebdCtl.IsRunning (voiceURL)) {
				if (btnDownloadPNL)
					btnDownloadPNL.SetActive (false);
				if (btnCancelPNL)
					btnCancelPNL.SetActive (true);
				StartCoroutine ("checkProgress");
				return;
			}
			if (PlayerPrefs.HasKey(voiceURL)) {
				#if UNITY_ANDROID && !UNITY_EDITOR
				StartCoroutine ("checkProgress");
				return;
				#elif UNITY_IOS
				// lets just check if its here
				DownloadedVoice();
				#endif
			}
		} else {
			if (progressIMG)
				progressIMG.localScale = new Vector3 (1, 1, 1);
			if (btnDownloadPNL)
				btnDownloadPNL.SetActive (false);
			if (btnCancelPNL)
				btnCancelPNL.SetActive (false);
			if (downloadPNL)
				downloadPNL.SetActive (false);
			if (arrowBTN)
				arrowBTN.SetActive (true);
			if (horSCR)
				horSCR.enabled = true;
		}
	}

	IEnumerator checkProgress() {
	//	Debug.Log ("checkProgress");
		float f = 0;
		bool started = false;
		if (downloadPRG)
			downloadPRG.fillAmount = 0.05f;
		//while (f < 1 && trglobals.instance._trscr._muc.ebdCtl.IsInvoking(voiceURL)) {
		while (f < 1) {
			f = trglobals.instance._trscr._muc.CheckProgress(voiceURL);
		//	Debug.Log (voiceURL + ":" +  f);
			if (f != 0)
				started = true;
			if (started && f == 0)
				f = 1;
		//	Debug.Log("checkProgress " +folder +":" + f);
			if (downloadPRG)
				downloadPRG.fillAmount = f;
			if (progressIMG)
				progressIMG.localScale = new Vector3 (f, 1, 1);
			yield return null;
		}
		if (progressIMG)
			progressIMG.localScale = new Vector3 (1, 1, 1);
		#if UNITY_ANDROID && !UNITY_EDITOR
		DownloadedVoice ();
		#endif
	}

	public void PreviewVoice() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (trglobals.instance._tts.TTS != null) {
			if (trglobals.instance._tts.isSpeaking())
				trglobals.instance._tts.tts_stop ();
		}
		#elif UNITY_IOS
		trglobals.instance._tts_ios.getTTS();
		if (trglobals.instance._tts_ios.isSpeaking())
			trglobals.instance._tts_ios.tts_stop ();
		#endif
		string sb = voiceTXT.text;
		sb = sb.Replace("(","");
		sb = sb.Replace(")","");
		sb = sb.Replace(" ","");
		sb = sb.ToLower ();
	//	Debug.Log (sb);
		AudioClip ac = Resources.Load<AudioClip>("sound/"+sb);
		trglobals.instance._audioSource.clip = ac;
		trglobals.instance._audioSource.Play ();
		trglobals.instance._audioSource.loop = false;
	}

	void OnApplicationPause(bool pause) {
		if (!pause && !string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(voiceURL)) {
			AutoDownload (false);
		}
	}

	public void AutoDownload(bool download = true) {
	//	Debug.Log ("--------------------------------------AutoDownload " + voiceURL + ":" + download);
		string voicefolder = Path.Combine (Application.persistentDataPath, "voices");
		if (!Directory.Exists (voicefolder)) {
			Directory.CreateDirectory(voicefolder);
		}
		if (Directory.Exists (Path.Combine (voicefolder, folder))) {
		//	Debug.Log ("I have voice  :" + folder);
			return;
		}
		if (trglobals.instance._trscr._muc.ebdCtl.IsRunning (voiceURL)) {
			//Debug.Log ("voiceurl is running " + voiceURL);
			if (btnDownloadPNL)
				btnDownloadPNL.SetActive (false);
			if (btnCancelPNL) {
				btnCancelPNL.SetActive (true);
			}
			StartCoroutine ("checkProgress");
			return;
		}

		if (PlayerPrefs.HasKey(voiceURL)) {
			#if UNITY_ANDROID && !UNITY_EDITOR
			StartCoroutine ("checkProgress");
			return;
			#elif UNITY_IOS
			// lets just check if its here
			DownloadedVoice();
			#endif
		}

		if (download) {
	//		Debug.Log ("I NEED TO START DOWNLOAD " + voiceURL);
			DownloadVoice ();
		}
	}

	public bool showvoices  = true;

	public void DownloadVoice() {
	//	Debug.Log (voiceURL + ":" + trglobals.instance._trscr._muc.ebdCtl.IsRunning(voiceURL));
		if (showvoices) {
			string m = "Voices includes with this download:";
			List<string> n = trglobals.instance.getVoicesfromAcavoice (acaname);
			for (int i = 0; i < n.Count; i++)
				m += ("\n" + n [i]);
			trglobals.instance.ShowError (m, "");
		}
		if (btnDownloadPNL) {
			btnDownloadPNL.SetActive (false);
			if (btnCancelPNL)
				btnCancelPNL.SetActive (true);
		}
		if (downloadPRG) {
			downloadPRG.fillAmount = 0;
		}
		if (progressIMG)
			progressIMG.localScale = new Vector3 (0, 1, 1);
		trglobals.instance._trscr._muc.OnClickStartBtn (voiceURL,(Path.Combine (Application.persistentDataPath, "voices")),voiceTXT.text);
		StartCoroutine ("checkProgress");
	}


	IEnumerator WaitForUnzippedVoice() {
		string voicefolder = Path.Combine (Path.Combine (Application.persistentDataPath, "voices"), folder);
		while (!Directory.Exists (voicefolder)) {
		//	Debug.Log ("waiting for unzip " + voicefolder);
			yield return null;
		}
		#if UNITY_ANDROID
		trglobals.instance._tts.UpdateVoice();
		#endif
	}

	public void DownloadedVoice() {
		// lets chek after here that voice has been unzipped properly
		PlayerPrefs.DeleteKey (voiceURL);
		PlayerPrefs.Save ();
	//	Debug.Log ("DOWNLOADED VOCIE " + voiceURL);
		if (File.Exists (destZip)) {
		//	Debug.Log ("ZIP FILE EXISTS " + destZip);
			ZipUtil.Unzip (destZip, (Path.Combine (Application.persistentDataPath, "voices")));
			File.Delete (destZip);
			#if UNITY_ANDROID
			//StartCoroutine ("WaitForUnzippedVoice");
			trglobals.instance._tts.UpdateVoice();
			#elif UNITY_IOS
			trglobals.instance._tts_ios.tts_init();
			#endif
		} else
			Debug.Log ("NO ZIP FILE " + destZip);
	//	if (trglobals.instance._trdwa.active)
	//	Debug.Log ("SetupVoiceCells DownloadedVoice");
		trglobals.instance._trdwa.SetupVoiceCells ();
		trglobals.instance._trdwa.CheckDefaultVoices ();
		trglobals.instance._trsav.currentdownloadvoice = null;
		if (trglobals.instance._trsav.active)
			trglobals.instance._trsav.checkVoice (voiceTXT.text);
	}

	public void CancelDownloadVoice() {
	//	Debug.Log ("CANCEL " + voiceURL + ":" + trglobals.instance._trscr._muc.ebdCtl.IsRunning(voiceURL));
		if (btnDownloadPNL)
			btnDownloadPNL.SetActive (true);
		if (btnCancelPNL)
			btnCancelPNL.SetActive (false);
		if (downloadPRG)
			downloadPRG.fillAmount = 0;
		if (progressIMG)
			progressIMG.localScale = new Vector3 (0, 1, 1);
		trglobals.instance._trscr._muc.OnClickStopBtn (voiceURL);
		StopCoroutine("checkProgress");
	}

	public void DeleteVoiceFolder() {
		string fullpath = Path.Combine (Path.Combine (Application.persistentDataPath, "voices"), folder);
		Debug.Log ("deleting " + fullpath);
		if (Directory.Exists (fullpath)) {
		//	File.Delete (p);
			trglobals.instance.DeleteDirectory(fullpath);
			if (horSCR) {
				horSCR.horizontalNormalizedPosition = 0;
				horSCR.enabled = false;
			}
			if (btnDownloadPNL)
				btnDownloadPNL.SetActive (true);
			if (downloadPRG)
				downloadPRG.fillAmount = 0;
			if (downloadPNL)
				downloadPNL.SetActive (true);
			if (arrowBTN)
				arrowBTN.SetActive (false);
			if (progressIMG)
				progressIMG.localScale = new Vector3 (0, 1, 1);
			// load voice list agina
			#if UNITY_ANDROID && !UNITY_EDITOR
			trglobals.instance._tts.UpdateVoice();
			#endif
			trglobals.instance._trdwa.SetupVoices ();
		}
	}

}
