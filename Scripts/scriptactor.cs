[System.Serializable]
public class scriptactor
{
	public	string	scriptname;
	public	string	acaname;
	public	string	tabrname;
	public	bool	rehearse;
	public	int 	frequency;
	public	string	gender;
	public	float 	rate;
	public	float	shape;
	public	int index;

	public	int spotinscene;

	public bool	changedVoice;
	public scriptactor(string n,string g, int ind, string tra = "") {
		index = ind; scriptname = n; gender = g; tabrname = trglobals.instance.trnarrator; rehearse = false; frequency = 1; rate = 200; shape = 100;changedVoice = false;
		spotinscene = 0;
	}
}