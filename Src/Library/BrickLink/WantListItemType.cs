// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

namespace BrickUtilities.BrickLink
{
    /// <summary>
    /// Represents an item type
    /// </summary>
    /// <remarks>
    /// Based on the spec at https://www.bricklink.com/help.asp?helpID=207 .
    /// </remarks>
    public enum WantListItemType
    {
        /// <summary>
        /// Set
        /// </summary>
        Set = 1,

        /// <summary>
        /// Part
        /// </summary>
        Part = 2,

        /// <summary>
        /// Minifig
        /// </summary>
        Minifig = 3,

        /// <summary>
        /// Book
        /// </summary>
        Book = 4,

        /// <summary>
        /// Gear
        /// </summary>
        Gear = 5,

        /// <summary>
        /// Catalog
        /// </summary>
        Catalog = 6,

        /// <summary>
        /// Instruction
        /// </summary>
        Instruction = 7,

        /// <summary>
        /// Original box
        /// </summary>
        OriginalBox = 8,

        /// <summary>
        /// Unsorted lot
        /// </summary>
        UnsortedLot = 9,
    }
}