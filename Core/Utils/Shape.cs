namespace Core.Utils;

public class Shape {
    
    public string Name { get; }
    public double? X { get; }
    public double? Y { get; }
    public double? Width { get; }
    public double? Height { get; }
    public int? Rotation { get; }
    public bool HFlip { get; }
    public bool VFlip { get; }

    public Shape(string name, double? x, double? y, double? width, double? height, int? rotation, bool hFlip = false, bool vFlip = false) {
        Name = name;
        X = x is {} xPos ? Math.Round(xPos, 2) : null;
        Y = y is {} yPos ? Math.Round(yPos, 2) : null;
        Width = width is {} w ? Math.Round(w, 2) : null;
        Height = height is {} h ? Math.Round(h, 2) : null;
        Rotation = rotation;
        HFlip = hFlip;
        VFlip = vFlip;
    }
    
}