namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class Margin : VisualElement
    {
        public Margin(int margin) : this(margin, margin, margin, margin)
        {
            
        }

        public Margin(int marginVertical, int marginHorizontal) :
            this(marginVertical, marginVertical, marginHorizontal,  marginHorizontal)
        {

        }

        public Margin(int marginTop, int marginBottom, int marginHorizontal) :
            this(marginTop, marginBottom, marginHorizontal, marginHorizontal)
        {

        }

        public Margin(int marginTop, int marginBottom, int marginLeft, int marginRight)
        {
            style.SetMargin(marginTop, marginBottom, marginLeft, marginRight);
        }
    }
}
