using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

public class DialogueParser : MonoBehaviour {

	//TextAsset textfile;

	public struct DialogueLine{
		public string key;
		public string content;
		public string expression;
		public string[] options;

		public DialogueLine(string Key, string Content, string Expression){
			key = Key;
			content = Content;
			expression = Expression;
			options = new string[0];
		}
	}

	public List<DialogueLine> NPCLines;
	public List<DialogueLine> loveLines;
	public List<DialogueLine> spyLines;

	public List<DialogueLine> activeLines;

	public Dictionary<string, bool> Flags = new Dictionary<string, bool> ();
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

	void Start() {
		NPCLines = new List<DialogueLine> ();
		loveLines = new List<DialogueLine> ();
		spyLines = new List<DialogueLine> ();

		string file = "Text/NPCText";
		LoadDialogue (file, NPCLines);

		//Debug.Log ("NPC loaded");
		file = "Text/LoveText";
		LoadDialogue (file, loveLines);
		//Debug.Log ("love loaded");

		file = "Text/SpyText";
		LoadDialogue (file, spyLines);
		//Debug.Log ("spy loaded");


		activeLines = NPCLines;

		//PrintAllContent(activeLines);

	}

	void Update () {
	
	}
		

	void LoadDialogue(string textfile, List<DialogueLine> list){
		string line;

		TextAsset data = Resources.Load (textfile) as TextAsset;

		var lines_of_text = Regex.Split (data.text, LINE_SPLIT_RE);


		int l = 1;
		for(var ii=0; ii < lines_of_text.Length; ii++) 
		{
			line = lines_of_text[ii];
			//Debug.Log ("LINE: " + line);
			if (line!= null){
				string[] lineData = line.Split ('|');

				//normal parsing
				if (lineData[0] == "Choice"||lineData[0] == "endChoice")
				{
					DialogueLine lineEntry = new DialogueLine(lineData[0], "", "");
					lineEntry.options = new string[lineData.Length-1];
					for (int i = 1; i < lineData.Length; i++)
					{
						CreateDictionary(lineData[i]);
						lineEntry.options [i-1] = lineData[i];
					}
					list.Add(lineEntry);
				}
				else if (lineData[0].Contains("Check"))
					{
						DialogueLine lineEntry = new DialogueLine(lineData[0],lineData[1],"");
						lineEntry.options = new string[lineData.Length-1];
						for (int i = 1; i < lineData.Length; i++)
						{
							lineEntry.options[i-1] = lineData[i];
						}
						list.Add(lineEntry);
					}
				else{
					if (lineData.Length != (3))
					{
						Debug.Log("error cannot parse at  line: " + l + ", which is: " + line);
					}
					else
					{
						CreateDictionary(lineData[1]);
						DialogueLine lineEntry = new DialogueLine(lineData[0], lineData[1], lineData[2]);
						list.Add(lineEntry);
					}
				}

				if (lineData[1] == "over")
				{
					//Debug.Log ("over called");
					//activeLines = NPCLines;
				}



			}
			l++;
		} //end for

	}

	public void CreateDictionary (string line)
	{
		if (line.Contains("SetFlag"))
		{
			//Debug.Log ("found line with SetFlag: " + line);
			string newFlag = "";
			string commands = line.Split('~')[1];
			if (commands.Contains(":"))
			{
				string[] command = commands.Split(':');
				for (int i = 0; i < command.Length; i++)
				{
					//Debug.Log ("here's one isolated command: " + command [i].Split (',') [0]);
					if (command[i].Split(',')[0] == "SetFlag")
					{
						newFlag = command[i].Split(',')[1];
//						if (FlagExists (newFlag))
						if (Flags.ContainsKey(newFlag))
							return;
						else
							Flags.Add(newFlag, false);
						//Debug.Log("adding flag as false: " +  newFlag);
					}
					else
					{
						//Debug.Log("flag not found in: " + command[i]);
					}
				}
			}
			else
			{
				if (!commands.Contains (",")) {
					Debug.Log ("Error: incorrect flag setting at line: " + line + " with commands: " + commands);
				}
				newFlag = commands.Split(',')[1];
				Debug.Log ("adding key: " + newFlag);
				if (FlagExists (newFlag))
					return;
				else
					Flags.Add(newFlag, false);
				//Debug.Log("adding this only flag as false: " +  newFlag);
			}
		}
	}//Create Dictionary

	public int SearchStory(string code)
	{
		for (int i = 0; i<activeLines.Count;i++)
		{
			if (activeLines[i].key == code)
			{
				return i;
			}
		}
		Debug.Log ("searched text asset for key [" + code + "], key not found");
		return -1;

	}

	public string GetKey(int lineNumber){
		if (lineNumber <activeLines.Count){
			return activeLines [lineNumber].key;
		}
		return "";
	}

	public string GetExpression (int lineNumber){
		if (lineNumber < activeLines.Count){
			return activeLines[lineNumber].expression;
		}
		return "";
	}

	public string GetContent(int lineNumber){
		if (lineNumber < activeLines.Count){
			return activeLines [lineNumber].content;
		}
		return "";
	}

	public string[] GetOptions (int lineNumber){
		if (lineNumber < activeLines.Count){
			return activeLines [lineNumber].options;
		}
		return new string[0];
	}

	public bool FlagExists(string flagString)
	{
		foreach (KeyValuePair<string, bool> temp in Flags) 
		{
			if (temp.Key.CompareTo(flagString) == 0)
				return true;
			else
				return false;
		}
		return false;
	}//flagexists

	public bool FlagCheck(string statcheck)
	{
		Debug.Log ("FlagCheck[" + statcheck + "]");
		string flag = statcheck.Split ('=') [0];
		string checkString = statcheck.Split ('=') [1];
		bool check;

		if (checkString == "T")
			check = true;
		else if (checkString == "F")
			check = false;
		else {
			check = false;
			Debug.Log ("Error: " + statcheck + " does not contain true or false");
		}

		Debug.Log ("Flag: " + flag);
		if (!Flags.ContainsKey (flag)) {
			Debug.Log ("not in dictionary: " + flag);
			return false;
		}
		if (Flags [flag] == check) {
			return true;
		} else {
			return false;
		}
			
	}//Flag Check

	public void DisplayFlags()
	{
		foreach (KeyValuePair<string, bool> temp in Flags) 
		{
			Debug.Log (temp.Key + ": " + temp.Value);
		}
	}//Display Flags	

	public void PrintAllContent(List<DialogueLine> list)
	{
		for (int i = 0; i < list.Count;i++)
		{
			Debug.Log(list[i].content);
		}
	}//PrintAllContent



}
