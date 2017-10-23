using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

public class ttsbuttonscript : MonoBehaviour {

	int currentvoice = 0;
	string[] voicesList;
#if UNITY_ANDROID
	
	//static string text = "Hello I'm the unity plugin test voice";

	class EventsCallback : AndroidJavaProxy {
		public EventsCallback() : base("com/acapelagroup/android/tts/acattsandroid$iTTSEventsCallback") { }
		public void ttsevents(long type,long param1,long param2,long param3,long param4) {
			if (changingspeed) {
				changingspeed = false;
				return;
			}
			if (type == 7) {
			//	Debug.Log ("ttsevents " + type + ":" + param1 + ":" + param2 + ":" + param3 + ":" + param4 + ":");
				if (param2 != 0) {
					if (trglobals.instance._trvs.active)
						trglobals.instance._trvs.startNextLine ();
					if (trglobals.instance._trsav.active)
						trglobals.instance._trsav.previewing = false;
				}
			}
		}
	}

	public	AndroidJavaObject TTS;
	static bool changingspeed;
	public void tts_voicelist() {
		//	string streamingAssets = Application.streamingAssetsPath + "/voices";
		string persistentDataPath  = Application.persistentDataPath + "/voices/";
		//	string[] voicespath = new string[] { "/sdcard/voices", "/sdcard/voices" };
		//	string[] voicespathstreamingAssets = new string[] { streamingAssets, streamingAssets };
		string[] voicespersistentDataPath = new string[] {persistentDataPath };
		object[] parameters = new object[1];
		//	parameters[0] = voicespath;
		parameters[0] = voicespersistentDataPath;

		voicesList =  getTTS().Call<string[]>("getVoicesList",parameters); 

	//	Debug.Log("LISTING VOICES FROM " + persistentDataPath + ":" + voicesList.Length);
	//	for (int index = 0; index < voicesList.Length; index++) {
		//	Debug.Log(".voices : " + voicesList[index]);
	//	} 
	}

	public void UpdateVoice() {
		TTS = null;
		tts_init();
	}

	private AndroidJavaObject getTTS() {
		if (TTS == null) {
			tts_init ();
		}
		return TTS;
	}

	public void tts_init() {

		AndroidJavaClass javaUnityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");

		var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

		TTS = new AndroidJavaObject ("com.acapelagroup.android.tts.acattsandroid", currentActivity, new EventsCallback (), null);

		string version = TTS.Call<string>("getVersion");
	//	Debug.Log("TTS version : " + version);

		tts_voicelist ();

		string acapelaLicense = "\"3519 0 ZkFQ #COMMERCIAL#2 Large Productions Australia\"\nTiUziMscuULYjdpzW3atoRKZbFcxHBAAloAFhFrD7Z8k96WgnydP7zUym@UYdXvgpYSjid67\nWWGdiSgrIQPeZX@lU6aue8L5J36bzfNWTxX5qVJ!Q2bj%4P2\nUG$NInGXz!2ijWSJ6JUEHQ##\n";
		TTS.Call<int>("setLicense",(long)0x51466b5a,(long)0x007bbd68,acapelaLicense); 
		//setLicense(0x51466b5a, 0x007bbd68, varablestore.getInstance().acapelaLicense);
		int result = TTS.Call<int>("load",voicesList[currentvoice]); 
		name = voicesList [currentvoice];
		rate = 0;
		shape = 0;
	//	Debug.Log ("NAME IS " + name);
	}

	public bool isSpeaking() {
		return (getTTS().Get<int>("isSpeaking") == 1);
	}

	public void tts_speak(string text) {
		getTTS().Call<int>("speak",text); 
	}

	string currenttext;

	public void tts_speedVoice() {
		//Debug.Log ("speedVoice");
		float setrate = rate * 0.545f;
		if (trglobals.instance.speedVoice)
			setrate *= 1.5f;
	//	Debug.Log ("tts_speedVoice " + trglobals.instance.speedVoice + ":" + setrate);

		//getTTS().Call<int> ("setSpeechRate", setrate);
		if (isSpeaking ()) {
			changingspeed = true;
			getTTS().Call<int>("stop"); 
		//	getTTS().Call<int>("pause");
			getTTS().Call<int> ("setSpeechRate", setrate);
		//	getTTS().Call<int>("resume");
			getTTS().Call<int>("speak",currenttext); 
		}
		else {
			changingspeed = false;
			getTTS().Call<int> ("setSpeechRate", setrate);
		}
	}

	string name;float rate = 0; float shape = 0;

	/*public void tts_wav() {
		int result = getTTS().Call<int>("load","Peter");
		//string text = "Make your table read really come to life. Subscribe to Pro now";
		string text = "Make your table read really come to life. Have it read by an entire cast of characters, add an inspiring score, utilize rehearsal features, create and share notes. Subscribe to Pro now";
		string fileName = Path.Combine (Application.persistentDataPath, "long.wav");
		getTTS().Call<int>("synthesizeToFile",text,fileName); 
	}*/

	public void tts_speak(string text,string n, string d, float r, float s) {
		currenttext = text;
		if (n != name) {
			int result = getTTS().Call<int>("load",n); 
			if (result != 0) 
				getTTS().Call<int> ("load", d); 
			name = n;
			// force reset pf rate and shape
			rate = 0; shape = 0;
		}
		if (r != rate) {
			rate = r;
			tts_speedVoice ();
		}
		if (shape != s) {
			getTTS().Call<int> ("setPitch", s); 
			shape = s;
		}
		getTTS().Call<int>("speak",text); 
	}

	public void tts_stop() {
		getTTS().Call<int>("stop"); 
	}

#endif

}

