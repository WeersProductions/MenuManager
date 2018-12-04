using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct MCButtonData {
	public delegate void OnClick(Button button);

	public MCButtonData(string text, OnClick buttonClick, Sprite icon, bool tooltip, string tooltipText)
	{
		this.Text = text;
		this.ButtonClick = buttonClick;
		this.Icon = icon;
		this.Tooltip = tooltip;
		this.TooltipText = tooltipText;
	}	

	public MCButtonData(string text, OnClick buttonClick) : this(text, buttonClick, null, false, null)
	{
	}

	public MCButtonData(Sprite icon, OnClick buttonClick) : this(null, buttonClick, icon, false, null)
	{
	}

	public MCButtonData(string text, OnClick buttonClick, Sprite icon) : this(text, buttonClick, icon, false, null)
	{
	}

	/// <summary>
	/// The text of the button.
	/// </summary>
	public string Text;
	/// <summary>
	/// The function that should be called when clicking this button.
	/// </summary>
	public OnClick ButtonClick;

	/// <summary>
	/// The icon that can be used on the button.
	/// </summary>
	public Sprite Icon;
	/// <summary>
	/// If true, a tooltip is shown when hovering over the button.
	/// </summary>
	public bool Tooltip;
	/// <summary>
	/// The text to be shown when hovering over the button.
	/// </summary>
	public string TooltipText;
}
