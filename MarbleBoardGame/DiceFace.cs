namespace MarbleBoardGame
{
    public struct DiceFace
    {
        /// <summary>
        /// Length of face pointing in the +y direction
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// Face value 1-6
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Representation of a die face
        /// </summary>
        /// <param name="length">Length of face pointing in the +y direction</param>
        /// <param name="value">Face value 1-6</param>
        public DiceFace(float length, int value) : this()
        {
            Length = length;
            Value = value;
        }
    }
}
