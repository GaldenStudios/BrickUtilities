// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

namespace BrickUtilities.BrickLink
{
    /// <summary>
    /// Represents a color code
    /// </summary>
    public struct WantListColorId
    {
        private readonly int value;

        /// <summary>
        /// Constructor
        /// </summary>
        public WantListColorId(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="otherCode">Other code</param>
        /// <returns>True if values are equal</returns>
        public override bool Equals(object otherCode)
        {
            if (!(otherCode is WantListColorId))
                return false;

            return Equals((WantListColorId) otherCode);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="otherCode">Other code</param>
        /// <returns>True if values are equal</returns>
        public bool Equals(WantListColorId otherCode)
        {
            return otherCode.value == value;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        /// <summary>
        /// Equals operator
        /// </summary>
        /// <param name="code1">First code</param>
        /// <param name="code2">Second code</param>
        /// <returns>True if equal</returns>
        public static bool operator ==(WantListColorId code1, WantListColorId code2)
        {
            return code1.Equals(code2);
        }

        /// <summary>
        /// Not equals operator
        /// </summary>
        /// <param name="code1">First code</param>
        /// <param name="code2">Second code</param>
        /// <returns>True if unequal</returns>
        public static bool operator !=(WantListColorId code1, WantListColorId code2)
        {
            return !code1.Equals(code2);
        }

        /// <summary>
        /// Convert color code to integer
        /// </summary>
        /// <param name="code">Color code</param>
        public static implicit operator int(WantListColorId code)
        {
            return code.value;
        }

        /// <summary>
        /// Return the string
        /// </summary>
        public override string ToString()
        {
            return value.ToString();
        }
    }
}