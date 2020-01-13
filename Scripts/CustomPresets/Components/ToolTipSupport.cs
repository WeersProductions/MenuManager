using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeersProductions
{
	/// <summary>
	/// Simply adds support for tooltip for a UI object.
	/// </summary>
	public class ToolTipSupport : MonoBehaviour {
		/// <summary>
		/// If set, this menucontroller is used to control the tooltip.
		/// If not set, Global is used.
		/// </summary>
		[SerializeField]
		private MenuController _menuController;

		/// <summary>
		/// Define what menu should be shown.
		/// </summary>
		[SerializeField]
		private string _menu = "SIMPLETOOLTIP";
		/// <summary>
		/// The text that is shown when the tooltip is active.
		/// </summary>
		[SerializeField]
		private string _tooltipText;

		/// <summary>
		/// The amount of seconds hovering required, before the tooltip will show up.
		/// </summary>
		[SerializeField]
		private float _tooltipDelay = 1;

		private void Awake() 
		{
#if UNITY_EDITOR
			if (string.IsNullOrWhiteSpace(_menu))
			{
				Debug.LogError("You are trying to use tooltips, but without any menu. Please set the menu that should be used for tooltips.");
				return;
			}
#endif
			
			OnHover onHover = this.gameObject.AddComponent<OnHover>();
			onHover.Delay = _tooltipDelay;
			onHover.onPointerDelay = () => {
				MCSimpleTooltipData simpleTooltipData = new MCSimpleTooltipData("Tooltip", _tooltipText,
                    this.GetComponent<RectTransform>()) {AutoRemove = true};
				if(_menuController)
				{
					_menuController.AddPopup(_menu, false, simpleTooltipData);
				}
				else
				{
					MenuController.AddPopupGlobal(_menu, false, simpleTooltipData);
				}
			};
		}
	}
}
