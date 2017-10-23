[System.Serializable]
public class tablereadActor {
	public string tabrname;
	public string acaName;
	public string acaFolder;
	public string defaultname;
	public string gender;
	public string age;
	public float rate;
	public float shape;
	public int index;
	public string iOSacaName;
	public string iOSdefaultname = "eng_hd_peter_22k_lf.bvcu";

	public tablereadActor(string n, string an, string dn, string g, string a, int r, int s, int i, string f,string ian) {
		tabrname = n;acaName = an;defaultname = dn;gender = g;age = a;rate = r;shape = s;index = i;acaFolder = f;iOSacaName = ian;
	}
}
