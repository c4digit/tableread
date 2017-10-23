using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class panel : MonoBehaviour {

	public	Transform		_transform;
	public	RectTransform	_rect;
	public	Vector3 		offPos;
	public	Vector3 		onPos;
	public	string 		_headingSTR;
	public	string 		_backSTR;
	public	string		_backACTN;
	public	float		top = 232;
	public	float 		bottom = 124;
	public 	bool		active = false;
	public	bool		loadfirst;

	void Awake() {
		_transform = transform;
		_rect = this.GetComponent<RectTransform> ();
		StartCoroutine ("waitForLayout");
		offPos = _transform.position;
		onPos = new Vector3 (0, _transform.position.y, _transform.position.z);
	}

	IEnumerator waitForLayout() {
		yield return null;
		yield return null;
		yield return null;
		yield return null;
	//	offPos = _transform.position;
		onPos = new Vector3 (0, _transform.position.y, _transform.position.z);
		if (loadfirst)
			trglobals.instance._trfp.Setup();
	}

	public void setPosition(bool _active, bool setheading = true, bool back = false) {
		//Debug.Log ("setposition " +name + ":"+ _active + ":" + back);
		if (_active) {
			if (trglobals.instance._currentPanel) { 
				if (!back) { // only add panel if we have not pressed back
					trglobals.instance._lastPanel.Add (trglobals.instance._currentPanel);
				}
				trglobals.instance._currentPanel.setPosition (false);
			}
			if (trglobals.instance.goPRO && !trglobals.instance._trdfv.HasDefaultVoices () ) { // && false) {
				// load default voice page
				if (!trglobals.instance._trdfv.active) {
					trglobals.instance._trdfv._transform.position = trglobals.instance._trdfv.onPos;
					trglobals.instance._trdfv.active = true;
					trglobals.instance._heading.text = trglobals.instance._trdfv._headingSTR;
					trglobals.instance._backTXT.text = trglobals.instance._trdfv._backSTR;
					trglobals.instance.backaction = trglobals.instance._trdfv._backACTN;
					trglobals.instance._currentPanel = trglobals.instance._trdfv;
				}
				StartCoroutine(trglobals.instance._trdfv.checkVoices ());
			} else {
				_transform.position = onPos;
				_rect.offsetMax = new Vector2 (0, -top);
				_rect.offsetMin = new Vector2 (0, bottom);
				active = true;
				if (setheading)
					trglobals.instance._heading.text = _headingSTR;
				trglobals.instance._backTXT.text = _backSTR;
				trglobals.instance.backaction = _backACTN;
				trglobals.instance._currentPanel = this;
				if (trglobals.instance._trvs.active) // start speaking
					trglobals.instance._trvs.StartSpeaking ();
				#if UNITY_ANDROID && !UNITY_EDITOR
				if (trglobals.instance._trvs.active) {
					trglobals.instance._sleep.invokeSleep ();
					Debug.Log("SETTING TO NEVER SLEEP_____________________________");
					Screen.sleepTimeout = SleepTimeout.NeverSleep;
				}
				else
					Screen.sleepTimeout = SleepTimeout.SystemSetting;
				#endif
				
			}
		} else {
			_transform.position = offPos;
			if (trglobals.instance._trvs.active || trglobals.instance._trsav.active)
				trglobals.instance._trvs.stopScript ();
			active = false;
		}
	}

}
