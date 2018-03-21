namespace MarbleBoardGame
{
    public class HashNode
    {
        public PlySearch Search { get; set; }
        public Vector4 Value { get; set; }

        public HashNode(PlySearch search, Vector4 value)
        {
            Search = search;
            Value = value;
        }
    }
}
