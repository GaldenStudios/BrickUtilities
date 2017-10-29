﻿using System;

namespace BrickUtilities.BrickLink
{
    /// <summary>
    /// Represents an item in a parts list
    /// </summary>
    /// <remarks>
    /// Based on the spec at https://www.bricklink.com/help.asp?helpID=207 .
    /// </remarks>
    public class BLPartsListItem
    {
        /// <summary>
        /// Item type
        /// </summary>
        public BLItemType ItemType { get; }

        /// <summary>
        /// Item number
        /// </summary>
        public string ItemNumber { get; }

        /// <summary>
        /// Color id, or null if none
        /// </summary>
        public BLColorId? ColorId { get; }

        /// <summary>
        /// Maximum price, or null if none
        /// </summary>
        public double? MaximumPrice { get; }

        /// <summary>
        /// Minimum desired quantity
        /// </summary>
        public int? MinimumDesiredQuantity { get; }

        /// <summary>
        /// Quantity filled
        /// </summary>
        public int? QuantityFilled { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemType">Item type</param>
        /// <param name="itemNumber">Item number</param>
        /// <param name="colorId">Color id</param>
        /// <param name="maximumPrice">Maximum price</param>
        /// <param name="minimumDesiredQuantity">Minimum desired quantity</param>
        /// <param name="quantityFilled">Quantity filled</param>
        public BLPartsListItem(BLItemType itemType, string itemNumber, BLColorId? colorId = null, double? maximumPrice = null, int? minimumDesiredQuantity = null, int? quantityFilled = null)
        {
            ItemType = itemType;
            if(String.IsNullOrEmpty(itemNumber))
                throw new ArgumentNullException(nameof(itemNumber));
            ItemNumber = itemNumber;
            ColorId = colorId;
            MaximumPrice = maximumPrice;
            MinimumDesiredQuantity = minimumDesiredQuantity;
            QuantityFilled = quantityFilled;
        }

        /// <summary>
        /// Update minimum desired quantity.
        /// </summary>
        /// <param name="newMinimumDesiredQuantity">New value for minimum desired quantity.</param>
        /// <returns>New object with updated minimum desired quantity.</returns>
        public BLPartsListItem UpdateMinimimDesiredQuantity(int? newMinimumDesiredQuantity)
        {
            return new BLPartsListItem(ItemType, ItemNumber, ColorId, MaximumPrice, newMinimumDesiredQuantity, QuantityFilled);
        }

        /// <summary>
        /// Update minimum desired quantity.
        /// </summary>
        /// <param name="newQuantityFilled">New value for quantity filled.</param>
        /// <returns>New object with updated minimum desired quantity.</returns>
        public BLPartsListItem UpdateQuantityFilled(int? newQuantityFilled)
        {
            return new BLPartsListItem(ItemType, ItemNumber, ColorId, MaximumPrice, MinimumDesiredQuantity, newQuantityFilled);
        }
    }
}
