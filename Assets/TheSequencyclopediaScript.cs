using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using KModkit;
using Newtonsoft.Json.Linq;

public class TheSequencyclopediaScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
	
	public AudioClip[] SFX;
	public KMSelectable[] Buttons;
	public KMSelectable Display, Negative;
	public MeshRenderer[] InnerLED, OuterLED;
	public Material[] LEDColor;
	public TextMesh ALister, Number, TrueNumber;
	
	List<string> MochaBerry = new List<string>();
	string Answer = "", YourAnswer = "", Connecting = "CONNECTING";
	Coroutine PartTime;
	bool Interacted = false, Interactable = false;
	char[] ValidNumbers = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
	
	//Souvenir dedicated Variables
	string Tridal = "";
	string APass = "";
    
    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;
	
	void Awake()
    {
		moduleId = moduleIdCounter++;
		for (int i = 0; i < Buttons.Length; i++)
		{
			int Press = i;
			Buttons[i].OnInteract += delegate ()
			{
				ButtonPress(Press);
				return false;
			};
		}
		Display.OnInteract += delegate(){DisplayClick(); return false;};
		Negative.OnInteract += delegate(){MakeItNegative(); return false;};
	}
	
	void DisplayClick()
	{
		Display.AddInteractionPunch(0.2f);
		if (!ModuleSolved && Interactable)
		{
			if (YourAnswer == "")
			{
				Debug.LogFormat("[The Sequencyclopedia #{0}] You submitted: [EMPTY]", moduleId);
			}
			
			else
			{
				Debug.LogFormat("[The Sequencyclopedia #{0}] You submitted: {1}", moduleId, YourAnswer);
			}
			
			Audio.PlaySoundAtTransform(SFX[0].name, transform);
			Interactable = false;
			StartCoroutine(Inspection());
		}
	}
	
	IEnumerator Inspection()
	{
		for (int a = 0; a < 10; a++)
		{
			OuterLED[a].material = LEDColor[0];
			InnerLED[a].material = LEDColor[0];
		}
		APass = ALister.text;
		PartTime = StartCoroutine(LightCycle());
		TrueNumber.text = "";
		ALister.text = "";
		string Inspect = "AUTHENTICATING";
		for (int b = 0; b < Inspect.Length; b++)
		{
			Number.text += Inspect[b].ToString();
			yield return new WaitForSeconds(0.08f);
		}
		yield return new WaitForSeconds(2f);
		if (YourAnswer == Answer)
		{
			Debug.LogFormat("[The Sequencyclopedia #{0}] The answer submitted was correct. Module solved.", moduleId);
			Number.text = "";
			Number.color = Color.black;
			Audio.PlaySoundAtTransform(SFX[1].name, transform);
			string Apelao = "ACCEPTED";
			for (int b = 0; b < Apelao.Length; b++)
			{
				Number.text += Apelao[b].ToString();
				yield return new WaitForSeconds(0.01f);
			}
			StopCoroutine(PartTime);
			for (int a = 0; a < 10; a++)
			{
				OuterLED[a].material = LEDColor[1];
				InnerLED[a].material = LEDColor[1];
			}
			Module.HandlePass();
			ModuleSolved = true;
		}
		
		else
		{
			Debug.LogFormat("[The Sequencyclopedia #{0}] The answer submitted was incorrect. Module striked and performed a reset.", moduleId);
			StopCoroutine(PartTime);
			for (int a = 0; a < 10; a++)
			{
				OuterLED[a].material = LEDColor[0];
				InnerLED[a].material = LEDColor[0];
			}
			Number.text = "";
			Number.color = Color.black;
			Audio.PlaySoundAtTransform(SFX[2].name, transform);
			string Apelao = "DECLINED";
			for (int b = 0; b < Apelao.Length; b++)
			{
				Number.text += Apelao[b].ToString();
				yield return new WaitForSeconds(0.01f);
			}
			yield return new WaitForSeconds(0.5f);
			Module.HandleStrike();
			Connecting = "RESTARTING";
			YourAnswer = "";
			Interacted = false;
			Number.text = "";
			Number.color = new Color(189f/255f, 0f/255f, 0f/255f);
			yield return new WaitForSeconds(0.2f);
			StartCoroutine(DetermineSomething());
		}
	}
	
	void MakeItNegative()
	{
		Negative.AddInteractionPunch(0.2f);
		if (!Interacted && !ModuleSolved && Interactable)
		{
			Audio.PlaySoundAtTransform(SFX[0].name, transform);
			Interacted = true;
			TrueNumber.text += "-";
			YourAnswer += "-";
		}
	}
    
    void Start()
    {
        Module.OnActivate += OnActivate;
    }
	
	void ButtonPress(int Press)
	{
		Buttons[Press].AddInteractionPunch(0.2f);
		if (YourAnswer.Length < 102 && Interactable && !ModuleSolved)
		{
			Audio.PlaySoundAtTransform(SFX[0].name, transform);
			Interacted = true;
			if (YourAnswer.Length == 34 || YourAnswer.Length == 68)
			{
				TrueNumber.text += "\n";
			}
			YourAnswer += Press.ToString();
			TrueNumber.text += Press.ToString();
		}
	}
	
	void OnActivate()
    {
        StartCoroutine(DetermineSomething());
    }
	
	IEnumerator LightCycle()
	{
		while (true)
		{
			for (int x = 0; x < 10; x++)
			{
				Audio.PlaySoundAtTransform(SFX[5].name, transform);
				OuterLED[x].material = LEDColor[3];
				InnerLED[x].material = LEDColor[3];
				OuterLED[(x - 1 + 10) % 10].material = LEDColor[0];
				InnerLED[(x - 1 + 10) % 10].material = LEDColor[0];
				yield return new WaitForSeconds(0.05f);
			}
		}
	}
	
	IEnumerator DetermineSomething()
	{
		Debug.LogFormat("[The Sequencyclopedia #{0}] Gathering a connection to https://oeis.org/", moduleId);
		PartTime = StartCoroutine(LightCycle());
		for (int b = 0; b < Connecting.Length; b++)
		{
			Number.text += Connecting[b].ToString();
			yield return new WaitForSeconds(0.1f);
		}
		string Query = "https://oeis.org/";
		WWW www = new WWW(Query);
		while (!www.isDone) { yield return null; };
		if (www.error == null)
        {
			string[] Mechanon = www.text.Split('\n');
			Mechanon.Reverse();
			for (int x = 0; x < Mechanon.Length; x++)
			{
				if (Regex.IsMatch(Mechanon[x].ToUpper(), "CONTAINS"))
				{
					string[] Aura = Mechanon[x].Split('.');
					string[] MechaFor = Aura[1].Split(' ');
					for (int a = 0; a < MechaFor.Length - 1; a++)
					{
						if (MechaFor[a].ToUpper() == "CONTAINS")
						{
							Tridal = MechaFor[a + 1];
							break;
						}
					}
					break;
				}
				
				if (x == Mechanon.Length - 1)
				{
					Debug.LogFormat("[The Sequencyclopedia #{0}] The module was able to gather information on https://oeis.org/.", moduleId);
					StopCoroutine(PartTime);
					for (int a = 0; a < 10; a++)
					{
						OuterLED[a].material = LEDColor[0];
						InnerLED[a].material = LEDColor[0];
					}
					Number.text = "";
					ALister.text += "A";
					Audio.PlaySoundAtTransform(SFX[3].name, transform);
					yield return new WaitForSeconds(0.6f);
					for (int d = 0; d < 6; d++)
					{
						if (d < 5)
						{
							ALister.text += "0";
						}
						
						else
						{
							ALister.text += "4";
						}
						Audio.PlaySoundAtTransform(SFX[3].name, transform);
						yield return new WaitForSeconds(0.6f);
					}
					Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence name gathered by the module: {1}", moduleId, ALister.text);
					Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence gathered: 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0", moduleId);
					Debug.LogFormat("[The Sequencyclopedia #{0}] The length of the sequence gathered: 102");
					StartCoroutine(GenerateBinary());
				}
			}
			
			int Hopper = UnityEngine.Random.Range(1, Int32.Parse(Tridal) + 1);
			string SecondQuery = "https://oeis.org/A" + Hopper.ToString();
			WWW www2 = new WWW(SecondQuery);
			while (!www2.isDone) { yield return null; };
			if (www2.error == null)
			{
				if (Regex.IsMatch(www2.text.ToUpper(), "ALLOCATED FOR") || Regex.IsMatch(www2.text.ToUpper(), "SORRY,"))
				{
					for (int x = 0; x < 9; x++)
					{
						Hopper = UnityEngine.Random.Range(1, Int32.Parse(Tridal) + 1);
						SecondQuery = "https://oeis.org/A" + Hopper.ToString();
						www2 = new WWW(SecondQuery);
						while (!www2.isDone) { yield return null; };
						if (www2.error == null && !Regex.IsMatch(www2.text.ToUpper(), "ALLOCATED FOR") && !Regex.IsMatch(www2.text.ToUpper(), "SORRY,"))
						{
							Debug.LogFormat("[The Sequencyclopedia #{0}] The module was able to gather information on https://oeis.org/.", moduleId);
							string[] Auracell = www2.text.Split('\n');
							string Forloin = "";
							for (int a = 0; a < Auracell.Length; a++)
							{
								if (Regex.IsMatch(Auracell[a], "<tt>") && Regex.IsMatch(Auracell[a], ",") && ValidNumbers.Any(d => Regex.Replace(Auracell[a], "[t<>/]", "").ToUpper().ToCharArray().Contains(d)))
								{
									Forloin = Auracell[a];
									break;
								}
							}
							Forloin = Regex.Replace(Forloin, " ", "");
							Forloin = Regex.Replace(Forloin, "<tt>", "");
							Forloin = Regex.Replace(Forloin, "</tt>", "");
							MochaBerry = new List<string>(Forloin.Split(','));
							string Chronicler = Hopper.ToString();
							int BuiltInNumber = 0;
							StopCoroutine(PartTime);
							for (int d = 0; d < 10; d++)
							{
								OuterLED[d].material = LEDColor[0];
								InnerLED[d].material = LEDColor[0];
							}
							Number.text = "";
							ALister.text += "A";
							Audio.PlaySoundAtTransform(SFX[3].name, transform);
							yield return new WaitForSeconds(0.6f);
							for (int m = 0; m < Tridal.Length; m++)
							{
								if (m < Tridal.Length - Chronicler.Length)
								{
									ALister.text += "0";
								}
								
								else
								{
									ALister.text += Chronicler[BuiltInNumber].ToString();
									BuiltInNumber++;
								}
								Audio.PlaySoundAtTransform(SFX[3].name, transform);
								yield return new WaitForSeconds(0.6f);
							}
							string Aloin = Regex.Replace(Forloin, ",", ", ");
							Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence name gathered by the module: {1}", moduleId, ALister.text);
							Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence gathered: {1}", moduleId, Aloin);
							Debug.LogFormat("[The Sequencyclopedia #{0}] The length of the sequence gathered: {1}", moduleId, MochaBerry.Count().ToString());
							break;
						}
						
						if (x == 8)
						{
							StopCoroutine(PartTime);
							Debug.LogFormat("[The Sequencyclopedia #{0}] Unable to gather information on https://oeis.org/ during its 10 cycles. Using the failsafe.", moduleId);
							for (int a = 0; a < 10; a++)
							{
								OuterLED[a].material = LEDColor[0];
								InnerLED[a].material = LEDColor[0];
							}
							Number.text = "";
							ALister.text += "A";
							Audio.PlaySoundAtTransform(SFX[3].name, transform);
							yield return new WaitForSeconds(0.6f);
							for (int d = 0; d < 6; d++)
							{
								if (d < 5)
								{
									ALister.text += "0";
								}
								
								else
								{
									ALister.text += "4";
								}
								Audio.PlaySoundAtTransform(SFX[3].name, transform);
								yield return new WaitForSeconds(0.6f);
							}
							Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence name gathered by the module: {1}", moduleId, ALister.text);
							Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence gathered: 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0", moduleId);
							Debug.LogFormat("[The Sequencyclopedia #{0}] The length of the sequence gathered: 102");
							StartCoroutine(GenerateBinary());
						}
					}
				}
				
				else
				{
					Debug.LogFormat("[The Sequencyclopedia #{0}] The module was able to gather information on https://oeis.org/.", moduleId);
					string[] Auracell = www2.text.Split('\n');
					string Forloin = "";
					for (int a = 0; a < Auracell.Length; a++)
					{
						if (Regex.IsMatch(Auracell[a], "<tt>") && Regex.IsMatch(Auracell[a], ",") && ValidNumbers.Any(d => Regex.Replace(Auracell[a], "[t<>/]", "").ToUpper().ToCharArray().Contains(d)))
						{
							Forloin = Auracell[a];
							break;
						}
					}
					Forloin = Regex.Replace(Forloin, " ", "");
					Forloin = Regex.Replace(Forloin, "<tt>", "");
					Forloin = Regex.Replace(Forloin, "</tt>", "");
					MochaBerry = new List<string>(Forloin.Split(','));
					string Chronicler = Hopper.ToString();
					int BuiltInNumber = 0;
					StopCoroutine(PartTime);
					for (int d = 0; d < 10; d++)
					{
						OuterLED[d].material = LEDColor[0];
						InnerLED[d].material = LEDColor[0];
					}
					Number.text = "";
					ALister.text += "A";
					Audio.PlaySoundAtTransform(SFX[3].name, transform);
					yield return new WaitForSeconds(0.6f);
					for (int m = 0; m < Tridal.Length; m++)
					{
						if (m < Tridal.Length - Chronicler.Length)
						{
							ALister.text += "0";
						}
						
						else
						{
							ALister.text += Chronicler[BuiltInNumber].ToString();
							BuiltInNumber++;
						}
						Audio.PlaySoundAtTransform(SFX[3].name, transform);
						yield return new WaitForSeconds(0.6f);
					}
					string Aloin = Regex.Replace(Forloin, ",", ", ");
					Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence name gathered by the module: {1}", moduleId, ALister.text);
					Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence gathered: {1}", moduleId, Aloin);
					Debug.LogFormat("[The Sequencyclopedia #{0}] The length of the sequence gathered: {1}", moduleId, MochaBerry.Count().ToString());
					StartCoroutine(GenerateBinary());
				}
			}
			
			else
			{
				Debug.LogFormat("[The Sequencyclopedia #{0}] Unable to gather information on https://oeis.org/. Using the failsafe.", moduleId);
				StopCoroutine(PartTime);
				for (int a = 0; a < 10; a++)
				{
					OuterLED[a].material = LEDColor[0];
					InnerLED[a].material = LEDColor[0];
				}
				Number.text = "";
				ALister.text += "A";
				yield return new WaitForSeconds(0.6f);
				for (int d = 0; d < 6; d++)
				{
					if (d < 5)
					{
						ALister.text += "0";
					}
					
					else
					{
						ALister.text += "4";
					}
					Audio.PlaySoundAtTransform(SFX[3].name, transform);
					yield return new WaitForSeconds(0.6f);
				}
				Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence name gathered by the module: {1}", moduleId, ALister.text);
				Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence gathered: 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0", moduleId);
				Debug.LogFormat("[The Sequencyclopedia #{0}] The length of the sequence gathered: 102");
				StartCoroutine(GenerateBinary());
			}
		}
		
		else
		{
			Debug.LogFormat("[The Sequencyclopedia #{0}] Unable to gather information on https://oeis.org/. Using the failsafe.", moduleId);
			StopCoroutine(PartTime);
			for (int a = 0; a < 10; a++)
			{
				OuterLED[a].material = LEDColor[0];
				InnerLED[a].material = LEDColor[0];
			}
			Number.text = "";
			ALister.text += "A";
			Audio.PlaySoundAtTransform(SFX[3].name, transform);
			yield return new WaitForSeconds(0.6f);
			for (int d = 0; d < 6; d++)
			{
				if (d < 5)
				{
					ALister.text += "0";
				}
				
				else
				{
					ALister.text += "4";
				}
				Audio.PlaySoundAtTransform(SFX[3].name, transform);
				yield return new WaitForSeconds(0.6f);
			}
			Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence name gathered by the module: {1}", moduleId, ALister.text);
			Debug.LogFormat("[The Sequencyclopedia #{0}] The sequence gathered: 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0", moduleId);
			Debug.LogFormat("[The Sequencyclopedia #{0}] The length of the sequence gathered: 102");
			StartCoroutine(GenerateBinary());
		}
	}
	
	IEnumerator GenerateBinary()
	{
		int LowFocus = UnityEngine.Random.Range(1,1024);
		int GuideLine = LowFocus;
		string Result = "";
		for (int i = 0; LowFocus > 0; i++)
		{
			Result = LowFocus % 2 + Result;
			LowFocus = LowFocus / 2;
		}
		Result = ReverseString(Result);
		while (Result.Length != 10)
		{
			Result += "0";
		}
		Result = ReverseString(Result);
		int MechaMech = UnityEngine.Random.Range(0,2);
		for (int c = 0; c < 10; c++)
		{
			if (Result[c].ToString() == "0")
			{
				OuterLED[c].material = LEDColor[0];
				InnerLED[c].material = LEDColor[0];
			}
			
			else
			{
				Audio.PlaySoundAtTransform(SFX[4].name, transform);
				OuterLED[c].material = LEDColor[1 + MechaMech];
				InnerLED[c].material = LEDColor[1 + MechaMech];
				yield return new WaitForSeconds(0.1f);
			}
		}
		Debug.LogFormat("[The Sequencyclopedia #{0}] The decimal/binary number generated: {1}/{2}", moduleId, GuideLine.ToString(), Result);
		string Halos = MechaMech == 0 ? "Green" : "Blue";
		Debug.LogFormat("[The Sequencyclopedia #{0}] The color of the lit LEDs: {1}", moduleId, Halos);
		if (ALister.text == "A000004")
		{
			Answer = "0";
		}
		
		else if (MechaMech == 0)
		{
			Answer = MochaBerry[GuideLine % MochaBerry.Count()];
		}
		
		else
		{
			Answer = MochaBerry[MochaBerry.Count() - (GuideLine % MochaBerry.Count()) - 1];
		}
		Interactable = true;
		Debug.LogFormat("[The Sequencyclopedia #{0}] The correct answer: {1}", moduleId, Answer);
	}
	
	string ReverseString(string Texter)
	{
		if (Texter.Length == 0)
		{
			return null;
		}
		char[] charArray = Texter.ToCharArray();
		Array.Reverse(charArray);
		return new string (charArray);
	}
    
    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To submit the number on the module, use the command !{0} submit <number> (The number can only have a maximum length of 102 [including the negative sign])";
    #pragma warning restore 414
	
	string[] ValidStuff = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "-"};
	
	IEnumerator ProcessTwitchCommand(string command)
    {
		string[] parameters = command.Split(' ');
		if (!Interactable)
		{
			yield return "sendtochaterror You can not interact with the module currently. The command was not processed.";
			yield break;
		}
		
		if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
			yield return null;
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Parameter length invalid. The command was not processed.";
				yield break;
			}
			
			if (parameters[1].Length < 1 || parameters[1].Length > 102)
			{
				yield return "sendtochaterror Number length invalid. The command was not processed.";
				yield break;
			}
			
			if (parameters[1].ToCharArray().Count(c => c == '-') > 1)
			{
				yield return "sendtochaterror Negative sign is more than 1. The command was not processed.";
				yield break;
			}
			
			if (parameters[1].ToCharArray().Count(c => c == '-') == 1 && parameters[1][0].ToString() != "-")
			{
				yield return "sendtochaterror Negative sign is not in the proper position. The command was not processed.";
				yield break;
			}
			
			for (int x = 0; x < parameters[1].Length; x++)
			{
				if (!parameters[1][x].ToString().EqualsAny(ValidStuff))
				{
					yield return "sendtochaterror Number being sent contains an invalid character. The command was not processed.";
					yield break;
				}
			}
			
			for (int x = 0; x < parameters[1].Length; x++)
			{
				if (parameters[1][x].ToString() == "-")
				{
					Negative.OnInteract();
				}
				
				else
				{
					Buttons[Int32.Parse(parameters[1][x].ToString())].OnInteract();
				}
				yield return new WaitForSeconds(0.05f);
			}
			
			yield return "solve";
			yield return "strike";
			Display.OnInteract();
		}
	}
}
