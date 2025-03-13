namespace TowerBuilder
{
    public class Dimensions<NumberType>
    {
        public NumberType width;
        public NumberType height;

        public Dimensions(NumberType width, NumberType height)
        {
            this.width = width;
            this.height = height;
        }

        public Dimensions(NumberType size) : this(size, size) { }

        public override string ToString() => $"{width} x {height}";

        public (NumberType, NumberType) AsTuple() => (width, height);
    }
}