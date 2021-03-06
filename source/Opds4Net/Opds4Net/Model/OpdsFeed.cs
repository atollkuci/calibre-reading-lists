﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using Opds4Net.Resources;
using Opds4Net.Util;

namespace Opds4Net.Model
{
    /// <summary>
    /// Represents an opds feed.
    /// </summary>
    public class OpdsFeed : SyndicationFeed
    {
        /// <summary>
        /// Loads an Opds4Net.Model.OpdsFeed-derived instance from the specified System.Xml.XmlReader.
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns> An Opds4Net.Model.OpdsFeed-derived instance that contains the feed.</returns>
        public new static OpdsFeed Load(XmlReader xmlReader)
        {
            var formatter = new OpdsFeedFormatter();
            formatter.ReadFrom(xmlReader);

            return formatter.Feed as OpdsFeed;
        }

        /// <summary>
        /// 
        /// </summary>
        public string SearchUri
        {
            get { return Links.GetLinkValue("search"); }
            set { Links.SetLinkValue("search", value, Resource.SearchLinkTitle, OpdsMediaType.AcquisitionFeed); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string NextUri
        {
            get { return Links.GetLinkValue("next"); }
            set { Links.SetLinkValue("next", value, Resource.NextLinkTitle, OpdsMediaType.NavigationFeed); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PreviousUri
        {
            get { return Links.GetLinkValue("previous"); }
            set { Links.SetLinkValue("previous", value, Resource.PreviousLinkTitle, OpdsMediaType.NavigationFeed); }
        }

        /// <summary>
        /// Gets the facet groups in current feed.
        /// </summary>
        public IEnumerable<IGrouping<string, OpdsLink>> FacetGroups
        {
            get
            {
                return from OpdsLink link in Links
                       where link != null && link.RelationshipType == OpdsRelations.Facet
                       group link by link.FacetGroup;
            }
        }

        /// <summary>
        /// Total count of result set. Usually used in search result feed.
        /// </summary>
        public int TotalResults { get; set; }

        /// <summary>
        /// Page size of search result.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Initialize an instance of Opds Feed.
        /// </summary>
        public OpdsFeed()
        {
            // namespaces must be initialized on construction.
            // Because:
            // 1. It is not property for user to do it everytime.
            // 2. If the namespaces is not inistalized properly,
            //    the element will contains their own namespaces.
            InitializeNamespaces(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public OpdsFeed(IEnumerable<SyndicationItem> items)
            : base(items)
        {
            InitializeNamespaces(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override SyndicationItem CreateItem()
        {
            return new OpdsItem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override SyndicationLink CreateLink()
        {
            return new OpdsLink();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public SyndicationLink FindLink(string relation)
        {
            return Links.SingleOrDefault(l => l.RelationshipType == relation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        protected override bool TryParseElement(XmlReader reader, string version)
        {
            if (reader.IsReadingElementOf(OpdsNamespaces.OpenSearch.Value, "totalResults"))
            {
                TotalResults = reader.ReadElementContentAsInt();
            }
            else if (reader.IsReadingElementOf(OpdsNamespaces.OpenSearch.Value, "itemsPerPage"))
            {
                ItemsPerPage = reader.ReadElementContentAsInt();
            }
            else
            {
                return base.TryParseElement(reader, version);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="version"></param>
        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            if (TotalResults > 0)
                writer.WriteElementString("totalResults", OpdsNamespaces.OpenSearch.Value, TotalResults.ToString(CultureInfo.InvariantCulture));
            if (ItemsPerPage > 0)
                writer.WriteElementString("itemsPerPage", OpdsNamespaces.OpenSearch.Value, ItemsPerPage.ToString(CultureInfo.InvariantCulture));

            base.WriteElementExtensions(writer, version);
        }

        /// <summary>
        /// If you want to add your own namespaces.
        /// Use RegistNamespace of OpdsNamespaces.
        /// Because this method is called from constructor,
        /// it should not be a virtual method.
        /// </summary>
        private static T InitializeNamespaces<T>(T feed)
            where T : SyndicationFeed
        {
            foreach (var @namespace in OpdsNamespaces.GetAll())
            {
                feed.AttributeExtensions[@namespace.Key] = @namespace.Value;
            }

            return feed;
        }
    }
}
