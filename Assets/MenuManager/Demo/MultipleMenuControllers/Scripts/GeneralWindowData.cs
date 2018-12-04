using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralWindowData {
	private string _text;
	private Color _backgroundColor;
	private string[] _buttonsTexts;
	/// <summary>
	/// The text that is shown when you click a button.
	/// </summary>
	private string[] _buttonFunctionTexts;

	public GeneralWindowData(string text, Color backgroundColor, string[] buttonsTexts, string[] buttonFunctionTexts)
	{
		this._text = text;
		this._backgroundColor = backgroundColor;
		this._buttonsTexts = buttonsTexts;
		this._buttonFunctionTexts = buttonFunctionTexts;
	}

	public string text
	{
		get { return _text; }
	}

	public Color BackgroundColor
	{
		get { return _backgroundColor; }
	}

	public string[] ButtonsTexts
	{
		get { return _buttonsTexts; }
	}

	/// <summary>
	/// The text that is shown when you click a button.
	/// </summary>
	public string[] ButtonFunctionTexts
	{
		get { return _buttonFunctionTexts; }
	}
}
