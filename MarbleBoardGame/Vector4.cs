using System;

namespace MarbleBoardGame
{
    public class Vector4
    {
        public static readonly Vector4 Zero = new Vector4();

        public double Value1;
        public double Value2;
        public double Value3;
        public double Value4;

        /// <summary>
        /// Vector weight
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or Sets the appropriate directions value
        /// </summary>
        /// <param name="direction">Direction to change</param>
        public double this[int direction]
        {
            get 
            {
                switch (direction)
                {
                    case 0:
                        return Value1;
                    case 1:
                        return Value2;
                    case 2:
                        return Value3;
                    case 3:
                        return Value4;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (direction)
                {
                    case 0:
                        Value1 = value;
                        break;
                    case 1:
                        Value2 = value;
                        break;
                    case 2:
                        Value3 = value;
                        break;
                    case 3:
                        Value4 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets the direction of the vector
        /// </summary>
        public int GetDirection()
        {
            if (Value1 > Value2 &&
                Value1 > Value3 &&
                Value1 > Value4)
            {
                return 0;
            }
            else if (Value2 > Value1 &&
                     Value2 > Value3 &&
                     Value2 > Value4)
            {
                return 1;
            }
            else if (Value3 > Value1 &&
                     Value3 > Value2 &&
                     Value3 > Value4)
            {
                return 2;
            }
            else if (Value4 > Value1 &&
                     Value4 > Value2 &&
                     Value4 > Value3)
            {
                return 3;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the vector in array form
        /// </summary>
        public double[] GetArray()
        {
            return new double[] 
            {
                Value1,
                Value2,
                Value3,
                Value4
            };
        }

        /// <summary>
        /// Gets the magnitude of the vector in a direction
        /// </summary>
        /// <param name="dir">Direction</param>
        public double GetMagnitude(sbyte dir)
        {
            int vDir = GetDirection();
            if (vDir == -1 || vDir == dir)
            {
                return this[dir];
            }

            double dirMag = this[vDir];
            return this[dir] - dirMag;
        }

        /// <summary>
        /// Gets sum of vector
        /// </summary>
        /// <param name="excluding">Direction to exclude</param>
        /// <returns></returns>
        public double GetLength(sbyte excluding)
        {
            double sum = 0;
            for (sbyte d = 0; d < 4; d++)
            {
                if (d != excluding)
                {
                    sum += GetMagnitude(d);
                }
            }

            return sum;
        }

        /// <summary>
        /// Checks if all the vectors values are the same
        /// </summary>
        public bool IsSame()
        {
            return (Value1 == Value2 &&
                Value2 == Value3 &&
                Value3 == Value4);
        }

        /// <summary>
        /// Adds a value to the vector
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <param name="direction">Direction to add</param>
        public void Add(double value, int direction)
        {
            switch (direction)
            {
                case -1: default:
                    Value1 += value;
                    Value2 += value;
                    Value3 += value;
                    Value4 += value;
                    break;
                case 0: Value1 += value; break;
                case 1: Value2 += value; break;
                case 2: Value3 += value; break;
                case 3: Value4 += value; break;
            }
        }

        /// <summary>
        /// Creates a empty vector 4
        /// </summary>
        public Vector4()
        {
            Value1 = 0;
            Value2 = 0;
            Value3 = 0;
            Value4 = 0;
            Weight = 1;
        }

        /// <summary>
        /// Creates a new vector with 4 values
        /// </summary>
        public Vector4(double value1, double value2, double value3, double value4)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
            Weight = 1;
        }

        /// <summary>
        /// Returns the higher vector
        /// </summary>
        /// <param name="team">Team to maximize</param>
        /// <returns></returns>
        public static Vector4 Max(Vector4 v1, Vector4 v2, sbyte team)
        {
            if (v1[team] > v2[team])
            {
                return v1;
            }
            else
            {
                return v2;
            }
        }

        /// <summary>
        /// Returns the lower vector
        /// </summary>
        /// <param name="team">Team to minimize</param>
        /// <returns></returns>
        public static Vector4 Min(Vector4 v1, Vector4 v2, sbyte team)
        {
            if (v1[team] < v2[team])
            {
                return v1;
            }
            else
            {
                return v2;
            }
        }

        /// <summary>
        /// Multiplies all values of the vector4 by a value
        /// </summary>
        public static Vector4 Multiply(Vector4 value, double weight)
        {
            double revisedWeight = 0.166666666666667 - weight;
            Vector4 product = new Vector4(
                value.Value1 - (value.Value1 * revisedWeight), 
                value.Value2 - (value.Value2 * revisedWeight),
                value.Value3 - (value.Value3 * revisedWeight), 
                value.Value4 - (value.Value4 * revisedWeight));
            product.Weight = weight;
            return product;
        }


        /// <summary>
        /// Divides all values of the vector4 by a value
        /// </summary>
        public static Vector4 Divide(Vector4 value, double weight)
        {
            Vector4 quotient = new Vector4(value.Value1 / weight, value.Value2 / weight, value.Value3 / weight, value.Value4 / weight);
            quotient.Weight = weight;
            return quotient;
        }
    }
}
