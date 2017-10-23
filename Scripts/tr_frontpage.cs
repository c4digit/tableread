using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class tr_frontpage : panel {

	public	frontpagescript	_prefab;
	public	GameObject _restoreBTN;
//	public	GameObject _filler;
	public	List<frontpagescript>	_frontpagescripts = new List<frontpagescript>();

	string[]	actormp3s = { "Sarah(AUS)","Aaron(AUS)","Tanya(IND)","Robin(SCO)","Michael(UK)","Helen(UK)","Armin(UK)","Harold(UK)","Chelsea(UK)","Scarlett(USA)","Katie(USA)","Katie(USA)",
		"Matthew(USA)","Paul(USA)","Brad(USA)","Christopher(USA)","Rebecca(USA)","Mel(USA)","Gavin(USA)","Gavin Gruff(USA)","Bruce(USA)","Eugene(USA)","Pearl(USA)","Tony(USA)","Kunal(IND)",
		"Alfred(USA)","Clint(USA)","James(AUS)","Roger(UK)","Dan(USA)","Max(AUS)","Indiana(AUS)","Harry(UK)","Rosie(UK)","Emily(USA)","Emilio(USA)","Arnold(USA)","Chloe(USA)","Zac(USA)",
		"Austin(AUS)","Hannah(AUS)","Charlie(UK)","Angela(UK)","Belinda(USA)","Sonny(USA)","Wade(USA)","Penny(USA)"};

	void Start() {
		_prefab.gameObject.SetActive (false);
		#if UNITY_IOS
		_restoreBTN.SetActive(true);
		#endif
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
	}

	// Use this for initialization
	public void Setup () {
		if (_frontpagescripts.Count > 0)
			saveFrontPageScripts ();
		else {
			string fpsFile = Path.Combine(Application.persistentDataPath, "frontpagescripts.trf");
			if (File.Exists (fpsFile)) {
				TextReader tr = new StreamReader (fpsFile);
				string contentTRN = tr.ReadToEnd ();
				string[] linesTRN = contentTRN.Split(new string[] { ";;" }, System.StringSplitOptions.None);
				for (int i = 0; i < linesTRN.Length; i+=3) {
					AddFrontPageScript(linesTRN[i],int.Parse(linesTRN[i+1]),float.Parse(linesTRN[i+2]),i/3);
				}
				tr.Close ();
			}
		}
		setPosition(true);
		if (trglobals.instance._trsub.checkedsubscription) {
		//	Debug.Log ("RANDOM VOICE CALLING");
			randomvoice ();
		}
		//#if UNITY_ANDROID && !UNITY_EDITOR
		//	trglobals.instance._tts.tts_wav ();
	//	#endif
	//	randomvoice ();
	//	StartCoroutine(playRandomVoice());

	/*	string[] files = Directory.GetFiles(Application.persistentDataPath);
		string[] dirs = Directory.GetDirectories(Application.persistentDataPath);

		foreach (string file in files)
		{
			Debug.Log ("FILE " + file);
		}

		foreach (string dir in dirs)
		{
			Debug.Log ("DIR " + dir);
		}
		files = Directory.GetFiles(Path.Combine(Application.persistentDataPath,"voices"));
		dirs = Directory.GetDirectories(Path.Combine(Application.persistentDataPath,"voices"));

		foreach (string file in files)
		{
			Debug.Log ("FILE voice folder" +  file);
			//string voicepath = Path.Combine (Application.persistentDataPath, "voices");
			//ZipUtil.Unzip (file, voicepath);
			//File.Delete (file);
		}

		foreach (string dir in dirs)
		{
			Debug.Log ("DIR voice folder" + dir);
		}*/

		trglobals.instance.CheckImportedFile ();

	}

/*	IEnumerator playRandomVoice() {
		yield return new WaitForEndOfFrame ();
		yield return null;
		yield return null;
		yield return null;
		Debug.Log ("PRO VERSION " + trglobals.instance.goPRO);
		randomvoice ();
	}*/

	public void TellAFriend() {
		string mobile_num = "";
		string message = "Check out this tableread app that lets you listen to scripts. Pretty amazing.\n\n" +
		                 "App Store\nhttps://itunes.apple.com/us/app/tableread/id1009176110?mt=8\n" +
			"Google Play\nhttps://play.google.com/store/apps/details?id=com.gmail.android.tableread";
		
		#if UNITY_ANDROID
		string url = string.Format("sms:{0}?body={1}",mobile_num,System.Uri.EscapeDataString(message));
		#endif
		#if UNITY_IOS
		//ios SMS URL - ios requires encoding for sms call to work
		//string URL = string.Format("sms:{0}?&body={1}",mobile_num,WWW.EscapeURL(message)); //Method1 - Works but puts "+" for spaces
		//string URL ="sms:"+mobile_num+"?&body="+WWW.EscapeURL(message); //Method2 - Works but puts "+" for spaces
		//string URL = string.Format("sms:{0}?&body={1}",mobile_num,System.Uri.EscapeDataString(message)); //Method3 - Works perfect
		string url ="sms:"+mobile_num+"?&body="+ System.Uri.EscapeDataString(message); //Method4 - Works perfectly
		#endif
		Application.OpenURL (url);
	/*	NativeToolkit.SendEmail ("tableread Subject - Tell a Friend",
			"<html><body>tableread Message - Tell a Friend;<br><br>Get it now" +
			"<br>App Store<br>https://itunes.apple.com/us/app/tableread/id1009176110?mt=8" +
			"<br>Google Play<br>https://play.google.com/store/apps/details?id=com.gmail.android.tableread<br></body></html>");*/
	}

	public void randomvoice(string sb = "harolduk_long") {
	//	Debug.Log ("randomvoice0000000000000000000000000 " + trglobals.instance._trsub.checkedsubscription);
		if (trglobals.instance.goPRO)
			return;
	/*	int r = Random.Range (0, actormp3s.Length);
		string sb = actormp3s [r].Replace("(","");
		sb = sb.Replace(")","");
		sb = sb.Replace(" ","");
		sb = sb.ToLower ();
		if (trglobals.instance.goPRO)
			sb += "_pro";*/
		//	Debug.Log (sb);
		if (!trglobals.instance._audioSource.isPlaying) {
			AudioClip ac = Resources.Load<AudioClip> ("sound/" + sb);
			trglobals.instance._audioSource.clip = ac;
			trglobals.instance._audioSource.Play ();
			trglobals.instance._audioSource.loop = false;
		}
	}

	public void loadScriptProducts() {
		trglobals.instance._trmsp.Setup ();
		//setPosition (false);
	}
		
	public void FAQ() {
		Application.OpenURL ("http://www.tablereadpro.com/#!help/c1dsz");
	}

	public void AddFrontPageScript(string n, int ln, float p, int i) {
		frontpagescript fp = Instantiate (_prefab) as frontpagescript;
		fp.Setup(n,ln,p,i);
		fp.transform.SetParent (_prefab.transform.parent,false);
		fp.gameObject.SetActive (true);
		_frontpagescripts.Add (fp);
	}

	public void LoadScriptProject(frontpagescript p) {
		string fullpath = Path.Combine (Application.persistentDataPath, p.name);
	//	Debug.Log ("LoadScriptProject " + fullpath);
		fullpath += ".trd";
		if (File.Exists (fullpath)) {
			trglobals.instance.getProjectData (fullpath, p.linenumber);
		} else {
			trglobals.instance.ShowError ("Can't find TRD File","FILE ERROR");
		}
	}
		
	void saveFrontPageScripts() {
		string content = "";
		for (int i = 0; i <  _frontpagescripts.Count; i++) {
			frontpagescript f = _frontpagescripts[i];
			content += f.name + ";;" + f.linenumber + ";;" + f.percentage.ToString("f2");
			if (i <  _frontpagescripts.Count - 1)
				content += ";;";
		}
		string fpsFile = Path.Combine(Application.persistentDataPath, "frontpagescripts.trf");
		TextWriter tw = new StreamWriter(fpsFile);
		tw.Write(content);
		tw.Close();
	}

	public void CheckDeleteCell(string n) {
		for (int i = 0; i < _frontpagescripts.Count; i++) {
			Debug.Log (_frontpagescripts [i].name + ":" + n);
			if (_frontpagescripts [i].name.Equals (n)) {
				DeleteCell (_frontpagescripts [i]);
				break;
			}
		}
	}

	public void DeleteCell(frontpagescript p) {
		_frontpagescripts.RemoveAt (p.index);
		Destroy (p.gameObject);
		for (int i = 0; i< _frontpagescripts.Count; i++) {
			_frontpagescripts [i].index = i;
		}
		if (_frontpagescripts.Count > 0)
			saveFrontPageScripts ();
		else {
			string fpsFile = Path.Combine(Application.persistentDataPath, "frontpagescripts.trf");
			Debug.Log ("Deleteing " + fpsFile);
			if (File.Exists(fpsFile))
				File.Delete(fpsFile);
		}
	}
}
