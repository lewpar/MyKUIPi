using System.Xml.Serialization;
using MyPiUI.Drawing;

namespace MyPiUI.UI.Controls;

[XmlRoot("Frame", Namespace = MyUI.Namespace)]
public class FrameElement : UIElement
{
    public override void Init(MyGraphicsContext graphicsContext)
    {
        if (Children.Count < 1)
        {
            return;
        }

        if (Height == 0)
        {
            Height = MyEngine.Instance?.MyOptions.RenderHeight ?? 0;
        }

        if (Width == 0)
        {
            Width = MyEngine.Instance?.MyOptions.RenderWidth ?? 0;
        }

        var child = Children[0];
        
        if (child.Width != Width && child.Width == 0)
        {
            child.X = Padding != 0 ? Padding : 0;
            child.Width = Padding != 0 ? Width - (Padding * 2) : Width;
        }

        if (child.Height != Height && child.Height == 0)
        {
            child.Y = Padding != 0 ? Padding : 0;
            child.Height = Padding != 0 ? Height - (Padding * 2) : Height;
        }
        
        child.Init(graphicsContext);
    }

    public override void Draw(DrawBuffer buffer)
    {
        if (Children.Count < 1)
        {
            return;
        }

        var child = Children[0];
        
        child.Draw(buffer);
    }
}