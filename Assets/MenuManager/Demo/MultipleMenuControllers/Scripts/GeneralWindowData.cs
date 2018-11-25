using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralWindowData {

	private string _text;
	private Color _color;
	private string[] _buttonsTexts;
	private string[] _buttonFunctionTexts;

	public GeneralWindowData(string text, Color color, string[] buttonsTexts, string[] buttonFunctionTexts)
	{
		this._text = text;
		this._color = color;
		this._buttonsTexts = buttonsTexts;
		this._buttonFunctionTexts = buttonFunctionTexts;
	}

	public string text
	{
		get { return _text; }
	}

	public Color Color
	{
		get { return _color; }
	}

	public string[] ButtonsTexts
	{
		get { return _buttonsTexts; }
	}

	public string[] ButtonFunctionTexts
	{
		get { return _buttonFunctionTexts; }
	}
}
