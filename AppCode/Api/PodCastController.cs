// Add namespaces to enable security in Oqtane & Dnn despite the differences
#if NETCOREAPP
using Microsoft.AspNetCore.Authorization; // .net core [AllowAnonymous] & [Authorize]
using Microsoft.AspNetCore.Mvc;           // .net core [HttpGet] / [HttpPost] etc.
#else
// 2sxclint:disable:no-dnn-namespaces - 2sxclint:disable:no-web-namespace
using System.Web.Http;		// this enables [HttpGet] and [AllowAnonymous]
using DotNetNuke.Web.Api;	// this is to verify the AntiForgeryToken
#endif
using System.Linq;
using System.Xml;
using System.IO;
using System;
using ToSic.Sxc.Data;
using ToSic.Razor.Blade;
using AppCode.Data;

[AllowAnonymous]			// define that all commands can be accessed without a login
public class PodCastController : Custom.Hybrid.ApiTyped
{
  // Atom XML Namespace for RSS
  public const string AtomNsCode = "atom";
  public const string AtomNamespace = "http://www.w3.org/2005/Atom";

  // ITunes XML Namespace for integration into Apple ecosystem
  public const string ItunesNsCode = "itunes";
  public const string ItunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";

  // CreativeCommons XML Namespace for copyright information
  public const string CreativeCommonsNsCode = "creativeCommons";
  public const string CreativeCommonsNamespace = "http://backend.userland.com/creativeCommonsRssModule";

  // Basic XML Document to start with
  public const string EmptyRssDocument = "<?xml version='1.0' encoding='utf-8'?>"
      + "<rss version='2.0' xmlns:" + AtomNsCode + "='" + AtomNamespace + "' "
      + "xmlns:" + ItunesNsCode + "='" + ItunesNamespace + "' "
      + "xmlns:" + CreativeCommonsNsCode + "='" + CreativeCommonsNamespace + "' "
      + "></rss>";

  [HttpGet]
  public object Rss()
  {
    // get all posts as delivered from the standard query
    var configChannel = As<Channel>(MyItem);
    // get all episodes from the channel
    var episodes = AsList<Episode>(configChannel.Parents(type: "Episode").OrderByDescending(e => e.DateTime("Date")));
    // results in "2019" or "2017-2019"
    var firstYear = episodes.First().Date.Year;
    var lastYear = episodes.Last().Date.Year;
    var copyrightYear = lastYear == firstYear ? firstYear.ToString() : lastYear + "-" + firstYear;

    var licUrl = configChannel.License.Link;
    var copyrightNotice = "Copyright © "
      + copyrightYear + " "
      + configChannel.Owner.FullName
      // Note: the term "copyright/" is currently hardwired into one of the licenses - adjust to your needs
      + (licUrl != "copyright/" ? " (" + licUrl + ")" : "");

    // 1. Build main XML document
    // 1.1 Create the XmlDocument and get root node
    var rssDoc = new XmlDocument();
    rssDoc.PreserveWhitespace = true;
    rssDoc.LoadXml(EmptyRssDocument);
    var root = rssDoc.DocumentElement;

    // 2. Create Channel
    // 2.1 Create <channel> node and set important values
    var channel = AddTag(root, "channel");
    AddTag(channel, "generator", "2sxc PodCast App");
    AddTag(channel, "title", configChannel.Title);
    AddTag(channel, "link", Link.To(pageId: MyPage.Id, type: "full"));
    AddTag(channel, "description", Kit.Scrub.All(configChannel.Description));
    AddTag(channel, "language", Kit.Scrub.All(configChannel.Language));
    AddTag(channel, "copyright", copyrightNotice);
    AddTag(channel, "managingEditor", configChannel.Owner.Email + " (" + configChannel.Owner.FullName + ")");
    var image = AddTag(channel, "image");
    AddTag(image, "title", configChannel.Title);
    AddTag(image, "url", Link.Image(configChannel.Image, type: "full"));
    AddTag(image, "link", Link.To(type: "full"));

    // 2.2 Create <creativeCommons:license> tag and add Creative Commons license if isn't copyright
    if (licUrl != "copyright/")
      AddNamespaceTag(channel, CreativeCommonsNsCode, "license", CreativeCommonsNamespace, licUrl);

    // 2.3 Add required Itunes values to channel
    AddChannelItunes(channel);

    // 2.4 Add Atom tag to channel
    AddChannelAtom(channel);

    // 2.5 Add all the episodes from the query to this channel
    // get protocol and host to complete the urls of the episodes and the one of the image
    var fullLink = Link.To(type: "full");
    var websiteRoot = fullLink.Substring(0, fullLink.IndexOf("/", fullLink.IndexOf("//") + 2));
    foreach (var episode in episodes)
      AddEpisode(websiteRoot, channel, episode);
    return File(download: false, fileDownloadName: "rss.xml", contents: rssDoc);
  }

  #region functions that add xml sections 

  // Adds required <itunes:xy> tags to channel
  private void AddChannelItunes(XmlElement channel)
  {
    // Get all posts as delved from the standard query
    var configChannel = As<Channel>(MyItem);

    // Get all episodes from the channel
    var episodes = AsList<Episode>(configChannel.Parents(type: "Episode").OrderByDescending(e => e.DateTime("Date")));

    var owner = configChannel.Owner;

    AddNamespaceTag(channel, ItunesNsCode, "summary", ItunesNamespace, Kit.Scrub.All(configChannel.Description));
    AddNamespaceTag(channel, ItunesNsCode, "author", ItunesNamespace, owner.FullName);
    AddNamespaceTag(channel, ItunesNsCode, "explicit", ItunesNamespace, episodes.Any(e => e.Explicit) ? "yes" : "no");
    var itunesImage = AddNamespaceTag(channel, ItunesNsCode, "image", ItunesNamespace);
    // Image size of 3000 x 3000 is required by itunes
    var imageUrl = Link.Image(configChannel.Image, type: "full");
    AddAttribute(itunesImage, "href", Link.Image(url: imageUrl, width: 3000, height: 3000));

    var itunesOwner = AddNamespaceTag(channel, ItunesNsCode, "owner", ItunesNamespace);
    AddNamespaceTag(itunesOwner, ItunesNsCode, "name", ItunesNamespace, owner.FullName);
    AddNamespaceTag(itunesOwner, ItunesNsCode, "email", ItunesNamespace, owner.Email);

    var itunesCategory = AddNamespaceTag(channel, ItunesNsCode, "category", ItunesNamespace);
    // Add main category
    var cat = configChannel.Category;
    AddAttribute(itunesCategory, "text", cat.MainCategory);

    // Add subcategory if needed
    if (cat.IsNotEmpty("SubCategory"))
    {
      var itunesSubCategory = AddNamespaceTag(itunesCategory, ItunesNsCode, "category", ItunesNamespace);
      AddAttribute(itunesSubCategory, "text", cat.SubCategory);
    }
  }

  // Adds <atom> tag to channel
  private void AddChannelAtom(XmlElement channel)
  {
    var atomLink = AddNamespaceTag(channel, AtomNsCode, "link", AtomNamespace);
    AddAttribute(atomLink, "href", Link.To(api: "api/PodCast/Rss", parameters: MyPage.Parameters, type: "full"));
    AddAttribute(atomLink, "rel", "self");
    AddAttribute(atomLink, "type", "application/rss+xml");
  }

  // Adds Episode to channel
  private void AddEpisode(string websiteRoot, XmlElement channel, Episode episode)
  {
    var publicationDate = episode.Date.ToString("R");
    var author = episode.Author;
    var authorFullName = author == null ? "" : author.FullName;
    var authorEmail = author == null ? "" : author.Email;

    var itemNode = AddTag(channel, "item");
    AddTag(itemNode, "title", episode.Title);
    AddTag(itemNode, "description", Kit.Scrub.All(episode.Description));

    var itemGuid = AddTag(itemNode, "guid", episode.Guid.ToString());
    AddAttribute(itemGuid, "isPermaLink", "false");
    var category = As<Category>(MyItem.Child("Category"));
    AddTag(itemNode, "category", Text.First(category.SubCategory, category.MainCategory));
    AddTag(itemNode, "author", authorEmail + " (" + authorFullName + ")");
    AddTag(itemNode, "pubDate", publicationDate);

    var enclosure = AddTag(itemNode, "enclosure");
    AddAttribute(enclosure, "url", websiteRoot + episode.Audio);
    AddAttribute(enclosure, "type", "audio/mpeg");
    AddAttribute(enclosure, "length", "1024");
    AddTag(itemNode, "link", Link.To());

    AddItemItunes(itemNode, episode, authorFullName);
  }

  // Adds required <itunes:xy> tags to itemNode
  private void AddItemItunes(XmlElement itemNode, Episode episode, string authorFullName)
  {
    var duration = TimeSpan.FromMinutes(episode.Duration).ToString("hh\\:mm") + ":00";
    var description = Kit.Scrub.All(episode.Description);
    AddNamespaceTag(itemNode, ItunesNsCode, "subtitle", ItunesNamespace, Text.Crop(description, 255));
    AddNamespaceTag(itemNode, ItunesNsCode, "summary", ItunesNamespace, description);
    AddNamespaceTag(itemNode, ItunesNsCode, "author", ItunesNamespace, authorFullName);
    AddNamespaceTag(itemNode, ItunesNsCode, "duration", ItunesNamespace, duration);
    AddNamespaceTag(itemNode, ItunesNsCode, "explicit", ItunesNamespace, episode.Explicit ? "yes" : "no");
  }

  #endregion

  #region helper functions for creating new XML elements and attributes 

  private XmlElement AddTag(XmlElement parent, string name, string value = null)
  {
    var node = parent.OwnerDocument.CreateElement(name);
    node.InnerText = value;
    parent.AppendChild(node);
    return node;
  }

  private XmlAttribute AddAttribute(XmlElement parent, string name, string value)
  {
    var node = parent.OwnerDocument.CreateAttribute(name);
    node.Value = value;
    return parent.Attributes.Append(node);
  }

  private XmlElement AddNamespaceTag(XmlElement parent, string name, string tagNs, string link, string value = null)
  {
    var node = parent.OwnerDocument.CreateElement(name, tagNs, link);
    if (value != null) node.InnerText = value;
    parent.AppendChild(node);
    return node;
  }

  #endregion
}
