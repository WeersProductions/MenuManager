using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WeersProductions
{
	public class GeneralWindow : MCMenu {

		[SerializeField]
		private Text _text;
		public override void Show(object data) 
		{
			string text = data as string;
			_text.text = text;
			base.Show(data);
		}
	}
}
