// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace BrickUtilities.BrickLink
{
    /// <summary>
    /// Represents a parts list file from BrickLink
    /// </summary>
    /// <remarks>
    /// Based on the spec at https://www.bricklink.com/help.asp?helpID=207 .
    /// </remarks>
    public class BLPartsListFile
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private BLPartsListFile(List<BLPartsListItem> items)
        {
            Items = new ReadOnlyCollection<BLPartsListItem>(items);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BLPartsListFile(
            IEnumerable<BLPartsListItem> items)
        {
            Items = new ReadOnlyCollection<BLPartsListItem>(new List<BLPartsListItem>(items));
        }

        /// <summary>
        /// Items
        /// </summary>
        public ReadOnlyCollection<BLPartsListItem> Items { get; }

        /// <summary>
        /// Get child element
        /// </summary>
        private static XElement ParseChildElement(XElement element, string elementName, bool optional)
        {
            var foundElements = element.Elements(elementName).ToArray();
            if (foundElements.Length == 0)
            {
                if (!optional)
                    throw new FileParseException("Missing '" + elementName + "' element",
                        (element as IXmlLineInfo).LineNumber);
                return null;
            }
            if (foundElements.Length > 1)
                throw new FileParseException("Duplicate '" + elementName + "' elements",
                    (element as IXmlLineInfo).LineNumber);
            var foundElement = foundElements[0];
            if (!optional)
            {
                if (String.IsNullOrEmpty(foundElement.Value))
                    throw new FileParseException("Invalid '" + elementName + "' value: '" + foundElement.Value + "'",
                        ((IXmlLineInfo) foundElement).LineNumber);
            }
            return foundElement;
        }

        /// <summary>
        /// Parse item type
        /// </summary>
        private static BLItemType ParseItemType(XElement element)
        {
            var s = ParseChildElement(element, "ITEMTYPE", false).Value;
            switch (s)
            {
                case "S": return BLItemType.Set;
                case "P": return BLItemType.Part;
                case "M": return BLItemType.Minifig;
                case "B": return BLItemType.Book;
                case "G": return BLItemType.Gear;
                case "C": return BLItemType.Catalog;
                case "I": return BLItemType.Instruction;
                case "O": return BLItemType.OriginalBox;
                case "U": return BLItemType.UnsortedLot;
                default:
                    throw new FileParseException("Invalid 'ITEMTYPE' value: '" + s + "'",
                        (element as IXmlLineInfo).LineNumber);
            }
        }

        /// <summary>
        /// Parse item type
        /// </summary>
        private static BLColorId? ParseColorId(XElement element)
        {
            var e = ParseChildElement(element, "COLOR", true);
            if (e == null)
                return null;
            if (String.IsNullOrEmpty(e.Value))
                return null;

            if (!Int32.TryParse(e.Value, out var colorId))
                throw new FileParseException("Invalid 'COLOR' value: '" + e.Value + "'", ((IXmlLineInfo) e).LineNumber);

            return new BLColorId(colorId);
        }

        /// <summary>
        /// Parse int
        /// </summary>
        private static int? ParseInt(XElement element, string name)
        {
            var e = ParseChildElement(element, name, true);
            if (e == null)
                return null;
            if (String.IsNullOrEmpty(e.Value))
                return null;

            if (!Int32.TryParse(e.Value, out var value))
                throw new FileParseException("Invalid '" + name + "' value: '" + e.Value + "'",
                    ((IXmlLineInfo) e).LineNumber);

            return value;
        }

        /// <summary>
        /// Parse int
        /// </summary>
        private static double? ParseDouble(XElement element, string name)
        {
            var e = ParseChildElement(element, name, true);
            if (e == null)
                return null;
            if (String.IsNullOrEmpty(e.Value))
                return null;

            if (!double.TryParse(e.Value, out var value))
                throw new FileParseException("Invalid '" + name + "' value: '" + e.Value + "'",
                    ((IXmlLineInfo) e).LineNumber);

            return value;
        }

        /// <summary>
        /// Loads a BrickLink-style parts list file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Parts list file</returns>
        public static BLPartsListFile Load(string path)
        {
            XDocument xdoc;
            try
            {
                xdoc = XDocument.Load(path, LoadOptions.SetLineInfo);
            }
            catch (XmlException e)
            {
                throw new FileParseException("XML exception", e);
            }
            var root = xdoc.Root;
            if (root.Name != "INVENTORY")
                throw new FileParseException("Missing root 'INVENTORY' element", ((IXmlLineInfo) root).LineNumber);

            var items = new List<BLPartsListItem>();
            foreach (var itemElement in root.Elements("ITEM"))
            {
                var itemType = ParseItemType(itemElement);
                var itemNumber = ParseChildElement(itemElement, "ITEMID", false).Value;
                var colorId = ParseColorId(itemElement);
                var minQuantity = ParseInt(itemElement, "MINQTY");
                var quantityFilled = ParseInt(itemElement, "QTYFILLED");
                var maxPrice = ParseDouble(itemElement, "MAXPRICE");

                var item = new BLPartsListItem(itemType, itemNumber, colorId, maxPrice, minQuantity, quantityFilled);
                items.Add(item);
            }

            return new BLPartsListFile(items);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="path">Path to the file to be saved</param>
        public void Save(string path)
        {
            using (var writer = new XmlTextWriter(path, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("INVENTORY");
                foreach (var item in Items)
                {
                    writer.WriteStartElement("ITEM");
                    writer.WriteStartElement("ITEMTYPE");
                    string itemType;
                    switch (item.ItemType)
                    {
                        case BLItemType.Book:
                            itemType = "B";
                            break;
                        case BLItemType.Catalog:
                            itemType = "C";
                            break;
                        case BLItemType.Gear:
                            itemType = "G";
                            break;
                        case BLItemType.Instruction:
                            itemType = "I";
                            break;
                        case BLItemType.Minifig:
                            itemType = "M";
                            break;
                        case BLItemType.OriginalBox:
                            itemType = "O";
                            break;
                        case BLItemType.Part:
                            itemType = "P";
                            break;
                        case BLItemType.Set:
                            itemType = "S";
                            break;
                        case BLItemType.UnsortedLot:
                            itemType = "U";
                            break;
                        default:
                            throw new InvalidOperationException("Unknonw item type: " + item.ItemType);
                    }
                    writer.WriteString(itemType);
                    writer.WriteEndElement();
                    writer.WriteStartElement("ITEMID");
                    writer.WriteString(item.ItemNumber);
                    writer.WriteEndElement();
                    if (item.ColorId != null)
                    {
                        writer.WriteStartElement("COLOR");
                        writer.WriteString(item.ColorId.ToString());
                        writer.WriteEndElement();
                    }
                    if (item.MaximumPrice != null)
                    {
                        writer.WriteStartElement("MAXPRICE");
                        writer.WriteString(item.MaximumPrice.Value.ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement();
                    }
                    if (item.MinimumDesiredQuantity != null)
                    {
                        writer.WriteStartElement("MINQTY");
                        writer.WriteString(item.MinimumDesiredQuantity.Value.ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement();
                    }
                    if (item.QuantityFilled != null)
                    {
                        writer.WriteStartElement("QTYFILLED");
                        writer.WriteString(item.QuantityFilled.Value.ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
    }
}