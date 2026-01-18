namespace AffectLights.Api.Models
{
    public class RgbColor
    {
        public int R { get; set; }
        public int G { get; set; } 
        public int B { get; set; }  

        public override string ToString() => $"({R}, {G}, {B})";
    }
}
