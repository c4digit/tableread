using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptCell : MonoBehaviour {

	public	CanvasGroup	cg;
	public	Transform					_transform;
	public	UnityEngine.UI.Text			line;
	public	string						linestr;
	public	UnityEngine.UI.VerticalLayoutGroup				lineVLG;
	public	UnityEngine.UI.LayoutElement lineLO;
//	public	UnityEngine.UI.Button 		lineBTN;
	public	float						xpos;
	public	float						ypos;
	public	string						actor;
	public	int							page;
	public	int							scene;
	public	trglobals.SCRIPTLINE_TYPE	type;
	public	bool						rehearse;
	public	int							spotinscene;
	public	int 						linenumber;
	public	float						height;
	public 	GameObject[] hideme;

	void Awake() {
		_transform = transform;
	}

	public void Setup(string l, float x, float y, int p, int s, trglobals.SCRIPTLINE_TYPE st, int ln) {
		type = st;
		Debug.Log (st+ ":" + ln);
		float lmargin = trglobals.instance.actionmargin;
		float rmargin = trglobals.instance.rightMargin;
	//	if (st == trglobals.SCRIPTLINE_TYPE.SCENE)
		//	line.color = trglobals.instance.scenelineCLR;
		//else 
		if (st == trglobals.SCRIPTLINE_TYPE.ACTOR) {
			lmargin = iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.actormargin);
		} else if (st == trglobals.SCRIPTLINE_TYPE.DIALOUGE) {
			Debug.Log ("DIALOUGE " + ln);
			lmargin = iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.dialougemargin) * 0.75f;
			rmargin = iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.dialougemarginR) * 0.75f;
			lmargin = 205;
			rmargin = 205;
		} else if (st == trglobals.SCRIPTLINE_TYPE.TRANSITION) {
			lmargin = iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.transitionmargin);
		} else if (st == trglobals.SCRIPTLINE_TYPE.PARENTHETICAL) {
			lmargin = iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.parenthiticalmargin);
		}
		lineVLG.padding.left = (int)(lmargin * trglobals.instance.pdfRatio);
		lineVLG.padding.right = (int)(rmargin * trglobals.instance.pdfRatio);
		linestr = l;
		line.text = l; xpos = x; ypos = y; page = p; scene = s;
		linenumber = ln;
		actor = trglobals.instance.trnarrator;
		StartCoroutine (setHeight ());
	}

	IEnumerator setHeight() {
		yield return new WaitForEndOfFrame ();
		height = line.rectTransform.rect.height;
		lineLO.preferredHeight = height + (height-128);// + 128;
	//	if (linenumber >= 15) {
		//	cg.alpha = 0;
	//	}
	}
}
