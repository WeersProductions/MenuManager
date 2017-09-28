using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeersProductions;

namespace WeersProductions
{
    /// <summary>
    /// Simple interface used for E.G. tooltips and menus that listen to the mouse.
    /// </summary>
    public interface IMouseInsideListener
    {
        /// <summary>
        /// Called when a mouse leaves a UI object.
        /// </summary>
        /// <param name="owner"></param>
        void MouseLeaves(MCToolTipOwner owner);
    }
}
