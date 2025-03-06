namespace TowerBuilder
{
    public static class Money
    {
        // Comma-seperates number
        // e.g 10000 -> 10,000
        public static string Format(int amount) =>
            $"§{amount:n0}";
    }
}