using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;
//using AOT;

public class ttsbuttonscript_ios : MonoBehaviour {

	int currentvoice = 0;
	string[] voicesList;
#if UNITY_IOS

	// TTS functions
	[DllImport ("__Internal")]
	private static extern string init(callback_event event_callback);

	[DllImport ("__Internal")]
	private static extern int loadvoice(string voiceid);

	[DllImport ("__Internal")]
	private static extern void speak(string text);

	[DllImport ("__Internal")]
	private static extern void stop();
	[DllImport ("__Internal")]
	private static extern void setRate(float rate);
	[DllImport ("__Internal")]
	private static extern void setShape(float shape);
	[DllImport ("__Internal")]
	private static extern bool isAcaSpeaking();
	[DllImport ("__Internal")]
	private static extern bool isAcaNull();
	[DllImport ("__Internal")]
	private static extern string refreshVoiceList();
	// Event callback from TTS
	public delegate void callback_event(int type, int param1, int param2, int param3, int param4);
	
	[MonoPInvokeCallback(typeof(callback_event))]
	static void event_callback(int type, int param1, int param2, int param3, int param4) {
		/*if (type == 0) // didFinishSpeaking
			Debug.Log(" text " + param2 + " - finshed normally : " + param1);
		if (type == 1) // willSpeakWord
			Debug.Log(" word [" + param1 + "-" + param2 + "]");
		if (type == 2) // willSpeakViseme
			Debug.Log(" viseme [" + param1 + "]");*/
		if (changingspeed) {
			changingspeed = false;
			return;
		}
		if (type == 0) {
			//Debug.Log ("event_callback " + type + ":" + param1 + ":" + param2 + ":" + param3 + ":" + param4);
			if (param1 != 0) {
				if (trglobals.instance._trvs.active)
					trglobals.instance._trvs.startNextLine ();
				if (trglobals.instance._trsav.active)
					trglobals.instance._trsav.previewing = false;
			}
		}
	}
		
	static bool changingspeed = false;
	public void  getTTS() {
		if (isAcaNull ()) {
			Debug.Log ("ACA IS NULL___________________");
			tts_init ();
		}
	}

	public void tts_refreshVoiceList() {
		if (isAcaNull ()) {
		//	Debug.Log ("tts_refreshVoiceList ACA IS NULL___________________");
			tts_init ();
		} else {
			string voicelist = refreshVoiceList();
			if (voicelist == null) {
			//	Debug.Log ("tts_refreshVoiceList voicelist == null");
				return;
			}
			// voicelist contains voiceid separaed by : 
			voicesList = voicelist.Split(':');

		//	Debug.Log("tts_refreshVoiceList voices list length: " + voicesList.Length);

		//	for (int index = 0; index < voicesList.Length; index++) {
			//	Debug.Log("voices : " + voicesList[index]);
		//	} 
		}
	}

	// Button functions
	public void tts_init() {

		string voicelist = init(new callback_event(ttsbuttonscript_ios.event_callback));
		if (voicelist == null) {
		//	Debug.Log ("tts_init voicelist == null");
			return;
		}
		
		// voicelist contains voiceid separaed by : 
		voicesList = voicelist.Split(':');
			
	//	Debug.Log("voices list length: " + voicesList.Length);

		//for (int index = 0; index < voicesList.Length; index++) {
		//	Debug.Log("voices : " + voicesList[index]);
	//	} 

		//if (voices.Length >= 1)
		//	loadvoice(voices[0]);
	}

	public void speak() {
		getTTS ();
		loadvoice(voicesList[0]);
		setRate(200);
		setShape (100);
		tts_speak ("Make your table read really come alive. After doing this, the warning will be gone and you should be able to successfully submit the binary to Apple for review.");
	}

	public bool isSpeaking() {
	//	getTTS ();
		return isAcaSpeaking();
	}

	public void tts_speak(string text) {
		getTTS ();
		speak (text);
	}

	string name;float rate = 0; float shape = 0;
	string currenttext;
	public void tts_speedVoice() {
		getTTS();
		float setrate = rate * 1;//0.545f;
		if (trglobals.instance.speedVoice)
			setrate *= 1.5f;
		//	Debug.Log ("tts_speedVoice " + trglobals.instance.speedVoice + ":" + setrate);

		if (isSpeaking ()) {
			changingspeed = true;
			stop ();
			//	getTTS().Call<int>("pause");
			setRate (setrate);
			//	getTTS().Call<int>("resume");
			speak (currenttext); 
		} else {
			changingspeed = false;
			setRate (setrate);
		}
	}

	public void tts_speak(string text,string n, string d, float r, float s) {
		getTTS ();
		currenttext = text;
		if (n != name) {
			int result = loadvoice (n); 
			if (result != 0)
				loadvoice (d); 
			name = n;
			// force reset pf rate and shape
			rate = 0; shape = 0;
		}
		if (r != rate) {
			rate = r;
			tts_speedVoice ();
		}
		if (shape != s) {
			setShape (s);
			shape = s;
		}
		speak (text);
	}

	public void trstop() {
		getTTS ();
	//	Debug.Log ("ACA SPEAKING " + isAcaSpeaking ());
		tts_stop ();
	}

	public void tts_stop() {
		getTTS ();
		stop ();
	}

#endif

}

