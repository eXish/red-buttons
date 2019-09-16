using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class redButtonsScript : MonoBehaviour 
{
	public KMAudio Audio;
	public KMBombInfo bomb;
	
	public KMSelectable[] buttons;
	
	private int digit = 0;
	public TextMesh DisplayedDigit;
	public TextMesh buttonText0;
	public TextMesh buttonText1;
	public TextMesh buttonText2;
	public TextMesh buttonText3;
	public TextMesh buttonText4;
	public TextMesh buttonText5;
	private int stage = 0;
	private int final = 0;

	private bool wrong;
	
	// Logging
	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		foreach (KMSelectable button in buttons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
		}

	}

	void Start()
	{
		PickRandomDigit();
		CalculateAnswer();
	}

	void PickRandomDigit()
	{
		{
		digit = UnityEngine.Random.Range(0,10);
		Debug.LogFormat("[Red Buttons #{0}] the displayed digit is {1}", moduleId, digit);
		DisplayedDigit.text = digit.ToString();
		}
	}

	void CalculateAnswer()
	{
		int lastDigitToCube = bomb.GetSerialNumberNumbers().Last();
		int lastDigitCubed = lastDigitToCube * lastDigitToCube * lastDigitToCube;
		int firstDigitToSquare = bomb.GetSerialNumberNumbers().First();
		int firstDigitSquared = firstDigitToSquare * firstDigitToSquare;
		int initial = (digit * (lastDigitCubed + firstDigitSquared));
		Debug.LogFormat("[Red Buttons #{0}] the number before modulo 6 was {1}", moduleId, initial);
		final = initial % 6;
		Debug.LogFormat("[Red Buttons #{0}] the number after modulo 6 was {1}", moduleId, final);
	}

	void ButtonPress(KMSelectable button)
	{
		button.AddInteractionPunch();
		Debug.LogFormat("[Red Buttons #{0}] You pressed " + button.GetComponentInChildren<TextMesh>().text, moduleId);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
		if(moduleSolved)
		{
			return;
		}
		if(button.GetComponentInChildren<TextMesh>().text != final.ToString())
		{
			wrong = true;
		}
		stage++;
		if(stage == 10)
		{
			stage = 0;
			if(wrong)
			{
				StartCoroutine(Strike());
				Debug.LogFormat("[Red Buttons #{0}] the sequence was incorrect! Strike!.", moduleId);
			}
			else
			{
				moduleSolved = true;
				StartCoroutine(Solve());
				Debug.LogFormat("[Red Buttons #{0}] the sequence was correct! Module solved.", moduleId);
			}
		}
		else
		{
			Start();
		}
	}
	IEnumerator Solve()
	{
		DisplayedDigit.text = "GG OK!";
		yield return new WaitForSeconds(0.2f);
		buttonText0.text = "G";
		yield return new WaitForSeconds(0.2f);
		buttonText1.text = "G";
		yield return new WaitForSeconds(0.2f);
		buttonText4.text = "O";
		yield return new WaitForSeconds(0.2f);
		buttonText3.text = "K";
		yield return new WaitForSeconds(0.2f);
		buttonText5.text = "!";
		yield return new WaitForSeconds(0.2f);
		buttonText2.text = "!";
		yield return new WaitForSeconds(0.2f);
		Audio.PlaySoundAtTransform("solveSound", transform);
		GetComponent<KMBombModule>().HandlePass();
	}

	IEnumerator Strike()
	{
		DisplayedDigit.text = "LOL NO";
		yield return new WaitForSeconds(0.5f);
		Audio.PlaySoundAtTransform("strikeSound", transform);
		GetComponent<KMBombModule>().HandleStrike();
		wrong = false;
		Start();
	}
}
