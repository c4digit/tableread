using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tr_sub : panel {

	public	Vector2				_offsetPRO;
	public	Vector2				_offsetFREE;
	public	RectTransform 		_frontpagerect;
	public	GameObject _subscGO;
	public	UnityEngine.UI.Text	_subscTXT;	
	public	UnityEngine.UI.Text	_blurbTXT;	
	string	defaultblurb =	"Thanks for subscribing to tableread PRO.\nYour first 7 days are FREE and you will not be charged.\nAfter the 7 day trial period ends you will be charged on a monthly basis.\nCancel any time.\n\nEnjoy tableread PRO!";
	//public	string				_subscSTR;

	void Start() {
		#if UNITY_IOS
		_offsetPRO = new  Vector2(615,0);
		_offsetFREE = new  Vector2(615,133);
		#endif
		_frontpagerect.offsetMax = new Vector2 (0, -_offsetFREE.x);
		_frontpagerect.offsetMin = new Vector2 (0, _offsetFREE.y);
	}

	public void Setup() {
		_blurbTXT.text = "";
		if (_subscTXT.text != "") {
			_subscGO.SetActive (true);
		}
		else
			_subscGO.SetActive (false);
		if (trglobals.instance._trvs.active)
			trglobals.instance._trvs.StopSpeaking ();
		StartCoroutine (GetTextFromWWW());
	//	System.Net.WebClient client = new System.Net.WebClient ();
		//string reply = client.DownloadString ("https://s3.amazonaws.com/trsvoices/t.txt");
		//trglobals.instance.DebugLog (reply);
	}

	IEnumerator GetTextFromWWW ()
	{
		setPosition (true);
		string url = "https://s3.amazonaws.com/trsvoices/tr_offer.txt";
		WWW www = new WWW (url);
		yield return www;

		if (www.error != null)
		{
			_blurbTXT.text = defaultblurb;
			trglobals.instance.DebugLog("Ooops, something went wrong...");
		}
		else
		{
			_blurbTXT.text = www.text;
		}
		www.Dispose ();
		www = null;
	}

	public bool checkedsubscription;

	public void turnOnPro(bool cs = true) {
		trglobals.instance.DebugLog ("turnOnPro");
		trglobals.instance.goPRO = true;
		// set home button
		trglobals.instance.homeBTNIMG.sprite = trglobals.instance.homeSPR [1];
		trglobals.instance._subscrBTN[0].SetActive (false);
		for (int i = 0; i < trglobals.instance._trvs.proBTN.Length; i++)
			trglobals.instance._trvs.proBTN [i].interactable = true;
		checkedsubscription = cs;
		#if UNITY_IOS //|| UNITY_EDITOR
		trglobals.instance._trfp._restoreBTN.SetActive(false);
		_offsetPRO = new  Vector2(530,0);
		_offsetFREE = new  Vector2(530,133);
		#endif
		trglobals.instance.DebugLog ("Setting scroll position");
		_frontpagerect.offsetMax = new Vector2 (0, -_offsetPRO.x);
		_frontpagerect.offsetMin = new Vector2 (0, _offsetPRO.y);
	//	Debug.Break ();
	}

	public void turnOffPro(bool cs = true) {
		trglobals.instance.DebugLog ("turnOffPro");
		trglobals.instance.goPRO = false;
		// set home button
		trglobals.instance.homeBTNIMG.sprite = trglobals.instance.homeSPR [0];
		trglobals.instance._subscrBTN[0].SetActive (true);
		for (int i = 0; i < trglobals.instance._trvs.proBTN.Length; i++)
			trglobals.instance._trvs.proBTN [i].interactable = false;
		if (cs) {
			trglobals.instance._trfp.randomvoice ();
			checkedsubscription = true;
		}
		#if UNITY_IOS //&& UNITY_EDITOR
		trglobals.instance._trfp._restoreBTN.SetActive(true);
		_offsetPRO = new  Vector2(615,0);
		_offsetFREE = new  Vector2(615,133);
		#endif
		_frontpagerect.offsetMax = new Vector2 (0, -_offsetFREE.x);
		_frontpagerect.offsetMin = new Vector2 (0, _offsetFREE.y);
	}

	public void togglePRO() {
		if (trglobals.instance.goPRO)
			turnOffPro ();
		else
			turnOnPro ();
	}

	public void buyPro() {
		turnOnPro ();
		if (trglobals.instance.scriptLoaded)
			trglobals.instance._scriptController._Adapter.ChangeItemCountTo(trglobals.instance._trvs._scriptlines.Count);
		trglobals.instance.genericBack ();
	}
}
