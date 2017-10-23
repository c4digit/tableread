using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class frontpagescript : MonoBehaviour {

	public	string	name;
	public	string	displayname;
	public	int		linenumber;
	public	float	percentage;
	public	int		index;
	public	Text	displayTXT;
	public	Image	bar;

	public void Setup(string n, int ln, float p,int i) {
		name = n; linenumber = ln; percentage = p;
		if (percentage > 1)
			percentage = 1;
		displayname = name.Replace ("_", " ");
		displayTXT.text = displayname;
		bar.transform.localScale = new Vector3 (p, 1, 1);
		index = i;
	}
}
