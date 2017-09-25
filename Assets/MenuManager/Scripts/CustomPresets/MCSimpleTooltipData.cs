using UnityEngine;
using UnityEngine.UI;

namespace WeersProductions
{
    public class MCSimpleTooltipData
    {
        protected string _description;
        protected string _title;

        /// <summary>
        ///     If true this data has custom position information that should be used.
        /// </summary>
        public readonly bool HasCustomPosition;

        public readonly Vector2 Position;

        public readonly RectTransform ToolTipOwner;

        /// <summary>
        /// If true this tooltip will be automatically removed if the 'owner' is not selected with the mouse anymore.
        /// </summary>
        public bool AutoRemove;

        public MCSimpleTooltipData(string title, string description)
        {
            _title = title;
            _description = description;
        }

        public MCSimpleTooltipData(string title, string description, RectTransform aObject) : this(title, description)
        {
            ToolTipOwner = aObject;
            Position = GetPosition(aObject);
            HasCustomPosition = true;
        }

        public MCSimpleTooltipData(string title, string description, Button btn) : this(title, description, btn.GetComponent<RectTransform>())
        {
        }

        public MCSimpleTooltipData(string title, string description, Image img) : this(title, description, img.GetComponent<RectTransform>())
        {
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Calculates the correct position based on the recttransform.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Vector2 GetPosition(RectTransform rect)
        {
            return new Vector2(rect.position.x, rect.position.y) - rect.sizeDelta;
        }
    }
}
