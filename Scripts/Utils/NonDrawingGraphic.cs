using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace WeersProductions
{
    /// <inheritdoc />
    /// <summary>
    ///     A concrete subclass of the Unity UI `Graphic` class that just skips drawing.
    ///     Useful for providing a raycast target without actually drawing anything.
    /// </summary>
    /// <remarks>http://answers.unity3d.com/answers/1157876/view.html</remarks>
    public class NonDrawingGraphic : Graphic, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private UnityAction _onClick;

        public UnityAction OnClick
        {
            get { return _onClick; }
            set { _onClick = value; }
        }

        public override void SetMaterialDirty()
        {
        }

        public override void SetVerticesDirty()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Probably not necessary since the chain of calls `Rebuild()`-&gt;`UpdateGeometry()`-&gt;`DoMeshGeneration()`-&gt;
        ///     `OnPopulateMesh()` won't happen; so here really just as a fail-safe.
        /// </summary>
        /// <param name="vh"></param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                OnClick();
            }
        }
    }
}
