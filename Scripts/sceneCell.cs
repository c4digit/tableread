	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneCell : MonoBehaviour {

	public	UnityEngine.UI.Text		sceneTXT;
	public	int						index;
	public	UnityEngine.UI.Toggle	rehearseTGL;

// actor settings
	public	string	name;
	public	int		frequency;
	public	string	gender;
	public	bool	isNarrator = false;
	public void toggleRehearse() {
		if (frequency > 0) {
			trglobals.instance._trvs._scriptactors [index].rehearse = rehearseTGL.isOn;
			trglobals.instance._scriptController._Adapter.ChangeItemCountTo(trglobals.instance._trvs._scriptlines.Count);
		}
		else
			trglobals.instance._trvs._scriptscenes [index].rehearse = rehearseTGL.isOn;
	}
}
