using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sleep : MonoBehaviour {

	public bool sleeping = false;

	#if UNITY_ANDROID

	public void SetSleeping() {	
		if (trglobals.instance._trvs.active) {
		//	Debug.Log ("SETSLEEPING =============================================================");
			trglobals.instance.Sleep ();
			Invoke ("SetSleep", 1);
		}
	}

	void SetSleep() {
		sleeping = true;
	}
		
	public void invokeSleep() {
		//if (trglobals.instance._trvs.active)
		//	Invoke ("SetSleeping", 90);//90
	}

/*	void OnApplicationPause(bool pause) {
		if (pause) {
			CancelInvoke ("SetSleeping");
		}
		else
			invokeSleep ();
	}*/
	// Use this for initialization
	void Update() {
		if (!trglobals.instance.scriptLoaded)
			return;
		if (!trglobals.instance._trvs.active) {
			return;
		}
		if (Input.GetMouseButtonUp (0)) {
		//	Debug.Log ("pressed");
			if (sleeping) {
				trglobals.instance.WakeUp ();
			} //else {
				//CancelInvoke ("SetSleeping");
				//invokeSleep ();
			//}
		}
	}
	#endif
}
