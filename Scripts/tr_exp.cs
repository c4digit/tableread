using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

public class tr_exp : panel {

	public void Setup() {
		setPosition (true);
	}

	public void ExportActiveNotes() {
		trglobals.importedFile = "";
		if (trglobals.instance.saveActiveNotes ()) {
			Debug.Log("CAN EXPORT ACTIVE NOTES");
			string path = System.IO.Path.Combine (Application.persistentDataPath, trglobals.instance.projectID);
			string notepath = System.IO.Path.Combine(path,(trglobals.instance.projectName + "_" + trglobals.instance.projectMyname + "_" + trglobals.instance.projectRelation + "_ACTIVE.trn"));
			if (File.Exists (notepath)) {
				NativeToolkit.SendEmail ("tableread ready script notes (active)",
					"These script notes are sent from tableread, please open them in your tableread app.\nNotes can also be opened as a text file for viewing or printing.",
					notepath,"","","","trn");
			}
			else 
				trglobals.instance.ShowError ("ERROR CREATING ACTIVE NOTES","ERROR");
		} else {
			trglobals.instance.ShowError ("NO ACTIVE NOTES TO SEND","ERROR");
		}
	}

	public void ExportMyNotes() {
		trglobals.importedFile = "";
		bool hasnotes = false;
		string path = System.IO.Path.Combine (Application.persistentDataPath, trglobals.instance.projectID);
		string notepath = System.IO.Path.Combine(path,(trglobals.instance.projectName + "_" + trglobals.instance.projectMyname + "_" + trglobals.instance.projectRelation + ".trn"));
		if (File.Exists(notepath)) {
			Debug.Log("CAN EXPORT NOTES");
			NativeToolkit.SendEmail ("tableread ready script notes (individual)",
				"These script notes are sent from tableread, please open them in your tableread app.\nNotes can also be opened as a text file for viewing or printing.",
				notepath,"","","","trn");
		} else {
			trglobals.instance.ShowError ("YOU HAVE NO NOTES TO SEND","ERROR");
		}
	}

	public void ExportPDF() {
		string outputpath = System.IO.Path.Combine (Application.persistentDataPath, "tablereadexport");
		if (!Directory.Exists (outputpath))
			Directory.CreateDirectory (outputpath);
		string dst = System.IO.Path.Combine (outputpath,"tableread_ready_or.png");
		if (!File.Exists (dst)) {
			//	File.copy
			string src = System.IO.Path.Combine (Application.streamingAssetsPath, "tableread_ready_or.png");
			StartCoroutine (CopyFile (src, dst));
		} else {
			string pdffolder = Application.persistentDataPath;
			string pdffile = System.IO.Path.Combine(Application.persistentDataPath, trglobals.instance.projectPDF);
			if (!File.Exists(pdffile)) {
				pdffolder = trglobals.instance._trpdf.downloadFolderPath;
			}
			copyPDFFile (pdffolder, trglobals.instance.projectPDF);
		}
	}

	IEnumerator CopyFile(string read_path, string write_path) {
		if (!read_path.Contains ("file://"))
			read_path = "file://" + read_path;
		Debug.Log ("CopyFile " + write_path);
		Debug.Log ("CopyFile " + read_path);
		WWW www = new WWW(read_path);
		yield return www;
		if (string.IsNullOrEmpty(www.error)) {
			File.WriteAllBytes(write_path, www.bytes);
			string pdffolder = Application.persistentDataPath;
			string pdffile = System.IO.Path.Combine(Application.persistentDataPath, trglobals.instance.projectPDF);
			if (!File.Exists(pdffile)) {
				pdffolder = trglobals.instance._trpdf.downloadFolderPath;
			}
			copyPDFFile (Application.persistentDataPath, trglobals.instance.projectPDF);
		} else {
			Debug.Log(www.error);
		}
		www.Dispose();
		www = null;
	}

	public void sendPDF(string pdf) {
		trglobals.importedFile = "";
		NativeToolkit.SendEmail("tableread ready script project",
			"This is a script sent from tableread. It contains project, genre, voice and score information accessible by the tableread app.\nThis document can also be opened in a PDF viewer.",
		pdf);
	}
	public Texture	overlay;

	void copyPDFFile(string inputPath, string inputFile) {
		Debug.Log("copyPDFFile");
		string outputpath = System.IO.Path.Combine (Application.persistentDataPath, "tablereadexport");
		if (!Directory.Exists (outputpath))
			Directory.CreateDirectory (outputpath);
		PdfReader reader = new PdfReader(System.IO.Path.Combine (inputPath, inputFile));
		PdfStamper stamper = new PdfStamper(reader, new System.IO.FileStream(System.IO.Path.Combine (outputpath, inputFile),FileMode.OpenOrCreate));
		Dictionary<string, string> info = new Dictionary<string, string>();
		info.Add ("Title", trglobals.instance.getMETADATATitle ());
		info.Add("Author", trglobals.instance.getMETADATAAuthor());
		info.Add("Subject", trglobals.instance.getMETADATASubject());
		info.Add("Keywords", trglobals.instance.getMETADATAKeywords());
		stamper.MoreInfo = info;
		string impath = System.IO.Path.Combine(outputpath,"tableread_ready_or.png");
		iTextSharp.text.Image myImg = iTextSharp.text.Image.GetInstance(impath,true);
		for (int i = 1; i <= reader.NumberOfPages; i++) {
			float x = reader.GetPageSize(i).Width - 91;
			float y = reader.GetPageSize(i).Height - 23;
			myImg.SetAbsolutePosition(x, y);
			//   System.out.println(reader.getPageSize(i).getWidth());
			//   System.out.println(reader.getPageSize(i).getHeight());
			myImg.ScaleAbsolute(88, 23);
			stamper.GetOverContent (i).AddImage(myImg);
		}
		stamper.Close ();
		reader.Close ();
		sendPDF (System.IO.Path.Combine (outputpath, inputFile));
	}
}
