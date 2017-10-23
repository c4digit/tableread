[System.Serializable]
public class scriptline {

	public	int 						linenumber;
	public	string						linestr;
	public	float						xpos;
	public	float						ypos;
	public	string						actor;
	public	int							page;
	public	int							scene;
	public	trglobals.SCRIPTLINE_TYPE	type;
	//public	bool						rehearse;
	public	int							spotinscene;

	public	int							lmargin;
	public	int							rmargin;
	public	bool 						hasNote;
	public	bool 						active;

	public scriptline(string l, float x, float y, int p, int s, trglobals.SCRIPTLINE_TYPE st, int ln, string ac, int sic)
	{
		type = st;
		lmargin = (int)trglobals.instance.actionmargin;
		rmargin = (int)trglobals.instance.rightMargin;
		if (st == trglobals.SCRIPTLINE_TYPE.ACTOR) {
			lmargin = (int)iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.actormargin);
		} else if (st == trglobals.SCRIPTLINE_TYPE.DIALOUGE) {
			lmargin = (int)(iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.dialougemargin) * 0.75f);
				rmargin = (int)(iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.dialougemarginR) * 0.75f);
		} else if (st == trglobals.SCRIPTLINE_TYPE.TRANSITION) {
			lmargin = (int)iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.transitionmargin);
		} else if (st == trglobals.SCRIPTLINE_TYPE.PARENTHETICAL) {
			lmargin = (int)iTextSharp.text.Utilities.MillimetersToPoints (trglobals.instance.parenthiticalmargin);
		}
		linestr = l;
		xpos = x; ypos = y; page = p; scene = s;
		linenumber = ln;
		actor = ac;
		lmargin = (int)(lmargin * trglobals.instance.pdfRatio);rmargin = (int)(rmargin *trglobals.instance.pdfRatio);
		spotinscene = 0;
	}
}
