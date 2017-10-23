using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using UnityEngine.UI;

public class itextPDF : MonoBehaviour {

	public	scriptCell			_scriptPREFAB;
	public	List<scriptCell>	_scriptCells = new List<scriptCell>();
	public	Transform			_scrollCONTENT;
	public	ScrollRect 			_scriptSCROLL;
	string strURL;

	float currentscrollY;

	int firstcell; int lastcell; int scriptcount;

	public void scrolling() {
		float v = Mathf.Abs (_scrollCONTENT.transform.position.y - currentscrollY);
		if (v > 128) {
			if (_scrollCONTENT.position.y > currentscrollY) {
				if (lastcell < _scriptCells.Count) {
					if (_scriptCells [lastcell]._transform.position.y > -500) {
						if (firstcell >= 0)
							_scriptCells [firstcell].cg.alpha = 0;
						_scriptCells [lastcell].cg.alpha = 1;
						if (lastcell < scriptcount) {
							firstcell++;
							lastcell++;
						}
					} 
				}
			} else {
				if (firstcell >= 0) {
					//	Debug.Log (_scriptCells [firstcell]._transform.position.y);
					if (_scriptCells [firstcell]._transform.position.y < 2100) {
						if (lastcell < _scriptCells.Count)
							_scriptCells [lastcell].cg.alpha = 0;
						_scriptCells [firstcell].cg.alpha = 1;
						if (firstcell >= 0) {
							firstcell--;
							lastcell--;
						}
					} 
				}
			}
			currentscrollY = _scrollCONTENT.position.y;
		}
	}

	IEnumerator Start() {
	//	System.Text.Encoding.GetEncoding (10000);

	//	strURL = "Maverick.PDF";
	//	strURL = "Cards.pdf";
		strURL = "argo.pdf";
	//	strURL = "Crimson-Owl.pdf";

		string write_path = System.IO.Path.Combine(Application.persistentDataPath, strURL);
		string read_path = System.IO.Path.Combine(Application.streamingAssetsPath, strURL);
		Debug.Log("Copying pdf to persistant : " + read_path);

		if (!read_path.Contains ("file://"))
			read_path = "file://" + read_path;

		WWW www = new WWW(read_path);
		yield return www;

		Debug.Log ("www.text " + www.bytes.Length);
		if (string.IsNullOrEmpty(www.error)) {
				Debug.Log(write_path);
				System.IO.File.WriteAllBytes(write_path, www.bytes);
		} else {
				Debug.Log(www.error);
		}
		www.Dispose();
		www = null;
		_scriptSCROLL.enabled = false; 
	}

	public void getPDF() {
		string pdfpath = System.IO.Path.Combine (Application.persistentDataPath, strURL);
		ReadPdfFile (pdfpath);
	}

	bool CheckSceneLine(float v,string s) {
		// 108 should be scene position
		if (v <= (trglobals.instance.actionmargin + 2)) {
			isaction = true;
		//		Debug.Log(s + ":IS ACTION");
			if (s.Contains ("INT.") || s.Contains ("EXT.")) {
				currentscene++;
				return true;
			} else
				return false;
		} else {
		//	Debug.Log(s + ":NOT ACTION");
			isaction = false;
			return false;
		}
	}
	bool isActorRange(float v) {
		// 252
		if (v >= trglobals.instance.parenthiticalmargin && v <= trglobals.instance.actormargin)
			return true;
		else
			return false;
	}
	bool isDialougeRange(float v) {
		// 252
		if (v >= trglobals.instance.actionmargin && v <= trglobals.instance.dialougemargin)
			return true;
		else
			return false;
	}
	bool isParentheticalRange(float v, string s) {
		// 252
		if (v >= trglobals.instance.dialougemargin && v <= trglobals.instance.parenthiticalmargin) {
			if (s [0] == '(' || s [s.Length - 1] == ')')
				return true;
			else
				return false;
		}
		else
			return false;
	}
	bool isTransition(float v) {
		// 252
		if (v >= trglobals.instance.transitionmargin)// && v <= trglobals.instance.parenthiticalmargin)
			return true;
		else
			return false;
	}
	bool isaction = false;
	int currentscene = 0;
	int currentline = 0;
	public void ReadPdfFile(string fileName)
	{
	///	StringBuilder text = new StringBuilder();
		for (int i = 0; i < _scriptCells.Count; i++) {
			Destroy (_scriptCells [i].gameObject);
		}
		_scriptCells.Clear ();
		trglobals.instance.cleanup ();
		Debug.Log ("ReadPdfFile");
		if (File.Exists(fileName))
		{
			PdfReader pdfReader = new PdfReader(fileName);
			PdfReaderContentParser parser = new PdfReaderContentParser(pdfReader);
			int totalpagecount = pdfReader.NumberOfPages;
			trglobals.instance.pdfWidth = pdfReader.GetPageSize (1).Width;
			trglobals.instance.pdfRatio = (float)trglobals.instance.screenwidth / trglobals.instance.pdfWidth;
			_scriptPREFAB.gameObject.SetActive (true);
			currentline = 0; currentscene = 0;firstcell = -15; lastcell = 15;
			for (int page = 1; page <= totalpagecount; page++)
			{
				LocationTextExtractionStrategyWithPosition strategy = parser.ProcessContent(page, new LocationTextExtractionStrategyWithPosition());
			//	strategy.pagenumber = page;
				List<ScriptLines>  scriptLines = strategy.GetLocations ();
				// add extra info in here
				Debug.Log("PAGE " + page);
				for (int i = 0; i < scriptLines.Count; i++) {
					//Debug.Log (scriptLines [i].page);
					string l = scriptLines[i].Text;
					l = l.Replace("*","");
					l = l.Trim ();
					if (l != "") {
						trglobals.SCRIPTLINE_TYPE st = trglobals.SCRIPTLINE_TYPE.NORMAL;
						if (CheckSceneLine (scriptLines [i].X, l)) {
							st = trglobals.SCRIPTLINE_TYPE.SCENE;
						}
						else {
							if (isaction)
								st = trglobals.SCRIPTLINE_TYPE.NORMAL;
							else if (isActorRange (scriptLines [i].X)) 
								st = trglobals.SCRIPTLINE_TYPE.ACTOR;
							else if (isDialougeRange(scriptLines [i].X))
								st = trglobals.SCRIPTLINE_TYPE.DIALOUGE;
							else if (isParentheticalRange(scriptLines [i].X,l))
								st = trglobals.SCRIPTLINE_TYPE.PARENTHETICAL;
							else if (isTransition(scriptLines [i].X))
								st = trglobals.SCRIPTLINE_TYPE.TRANSITION;
						}
						scriptCell _scriptCell = Instantiate (_scriptPREFAB) as scriptCell;
						_scriptCell.Setup (l, scriptLines [i].X, scriptLines [i].Y, page, currentscene,st,currentline);
						currentline++;
						_scriptCell.transform.SetParent (_scriptPREFAB.transform.parent, false);
						//if (currentline < 30)
						//	_scriptCell.hideme[0].SetActive (true);
						_scriptCells.Add (_scriptCell);
					}
				}
				scriptcount = _scriptCells.Count;
				currentscrollY = _scrollCONTENT.position.y;
				_scriptSCROLL.enabled = true;
			}
			pdfReader.Close();
			_scriptPREFAB.gameObject.SetActive (false);
		}
		trglobals.instance.cleanup ();
	//	return text.ToString();
	}

	public class LocationTextExtractionStrategyWithPosition : LocationTextExtractionStrategy
	{
	//	public int pagenumber;
	//	public int scenenumber;
		private readonly List<TextChunk> locationalResult = new List<TextChunk>();
		private readonly ITextChunkLocationStrategy tclStrat;
		public LocationTextExtractionStrategyWithPosition() : this(new TextChunkLocationStrategyDefaultImp()) {
		}

		/**
         * Creates a new text extraction renderer, with a custom strategy for
         * creating new TextChunkLocation objects based on the input of the
         * TextRenderInfo.
         * @param strat the custom strategy
         */
		public LocationTextExtractionStrategyWithPosition(ITextChunkLocationStrategy strat) {
			tclStrat = strat;
		}

		private bool StartsWithSpace(string str) {
			if (str.Length == 0) return false;
			return str[0] == ' ';
		}

		private bool EndsWithSpace(string str) {
			if (str.Length == 0) return false;
			return str[str.Length - 1] == ' ';
		}
		/**
         * Filters the provided list with the provided filter
         * @param textChunks a list of all TextChunks that this strategy found during processing
         * @param filter the filter to apply.  If null, filtering will be skipped.
         * @return the filtered list
         * @since 5.3.3
         */

		private List<TextChunk> filterTextChunks(List<TextChunk> textChunks, ITextChunkFilter filter) {
			if (filter == null) {
				return textChunks;
			}
			var filtered = new List<TextChunk>();
			foreach (var textChunk in textChunks) {
				if (filter.Accept(textChunk)) {
					filtered.Add(textChunk);
				}
			}
			return filtered;
		}

		public override void RenderText(TextRenderInfo renderInfo)
		{
			LineSegment segment = renderInfo.GetBaseline();
			if (renderInfo.GetRise() != 0)
			{ // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to 
				Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
				segment = segment.TransformBy(riseOffsetTransform);
			}
			TextChunk tc = new TextChunk(renderInfo.GetText(), tclStrat.CreateLocation(renderInfo, segment));
			locationalResult.Add(tc);
		}

		public List<ScriptLines> GetLocations()
		{
			float lastX = 0;
			float lastY = 0;
			string textline = "";
			var filteredTextChunks = filterTextChunks(locationalResult, null);
			filteredTextChunks.Sort();

			TextChunk lastChunk = null;

			var textLocations = new List<ScriptLines>();

			foreach (var chunk in filteredTextChunks) {
				if (lastChunk == null) {
					lastY = 0;lastX = 0;
					//initial
					if (chunk.Text.Trim ().Length > 0) {
						float currentX = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [0]);
						float currentY = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [1]);
						if (currentY > 0) {
							bool newLine = true;
							if (currentX == lastX && (currentY - lastY) > -5.5f)
								newLine = false;
							if (newLine) {
								textLocations.Add (new ScriptLines {
									Text = chunk.Text,
									X = currentX,
									Y = currentY,
								});
							} else {
								textline = "";
								// we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
								if (IsChunkAtWordBoundary (chunk, lastChunk) && !StartsWithSpace (chunk.Text) && !EndsWithSpace (lastChunk.Text))
									textline += ' ';
								textline += chunk.Text;
								textLocations [textLocations.Count - 1].Text += textline;
							}
						//	Debug.Log (currentX + ":" + currentY + ":" + chunk.Text);
						//	Debug.Log ("lastChunk == null: " + (currentY - lastY) + ":" + newLine);
						}
						lastY = currentY;
						lastX = currentX;
					}
				}
				else
				{
					if (chunk.SameLine(lastChunk)) {
					//	if (!sameline)
						textline = "";
						// we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
						if (IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.Text) && !EndsWithSpace(lastChunk.Text))
							textline += ' ';
						textline += chunk.Text;
						if (textLocations.Count > 0)
							textLocations[textLocations.Count - 1].Text += textline;
					}
					else {
						if (chunk.Text.Trim ().Length > 0) {
							float currentX = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [0]);
							float currentY = iTextSharp.text.Utilities.PointsToMillimeters (chunk.Location.StartLocation [1]);
							if (currentY > 0) {
								bool newLine = true;
							//	Debug.Log (chunk.Text + ":" + currentX + ":" + lastX + ":" + (Mathf.Approximately(currentX, lastX)) );
								if (Mathf.Approximately(currentX, lastX) && (currentY - lastY) > -4.5f)
									newLine = false;
								if (newLine) {
									textLocations.Add (new ScriptLines {
										Text = chunk.Text,
										X = currentX,
										Y = currentY,
									});
								} else {
									textline = "";
									// we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
									if (IsChunkAtWordBoundary (chunk, lastChunk) && !StartsWithSpace (chunk.Text) && !EndsWithSpace (lastChunk.Text))
										textline += ' ';
									textline += chunk.Text;
									textLocations [textLocations.Count - 1].Text += textline;
								}
							//	Debug.Log (currentX + ":" + currentY + ":" + chunk.Text);
							//	Debug.Log ((currentY - lastY) + ":" + newLine);
							}
							lastY = currentY;
							lastX = currentX;
						}
					}
				}
				lastChunk = chunk;
			}
			//now find the location(s) with the given texts
			return textLocations;
		}

	}
		
	public class ScriptLines
	{
		public float X { get; set; }
		public float Y { get; set; }
		public	string			Text 		{ get; set; }
	//	public	string			actor		{ get; set; }
	//	public	int				page		{ get; set; }
	//	public	int				scene		{ get; set; }
	//	public SCRIPTLINE_TYPE	lineType	{ get; set; }
	}
}
