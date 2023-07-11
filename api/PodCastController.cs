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


[AllowAnonymous]			// define that all commands can be accessed without a login
public class PodCastController : Custom.Hybrid.ApiPro
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
      +  "xmlns:" + ItunesNsCode + "='" + ItunesNamespace + "' "
      +  "xmlns:" + CreativeCommonsNsCode + "='" + CreativeCommonsNamespace + "' "
      +"></rss>";
  
  [HttpGet]
  public object Rss()
  {
    // get all posts as delivered from the standard query
    var config = MyItem;
    var episodes =  MyItem.Parents("Episode").OrderByDescending(e => e.DateTime("Date"));
    // results in "2019" or "2017-2019"
    var firstYear = episodes.First().DateTime("Date").Year;
    var lastYear = episodes.Last().DateTime("Date").Year;
    var copyrightYear = lastYear == firstYear ? firstYear.ToString() : lastYear + "-" + firstYear;

    // var license = config.Child("License");
    var licUrl = config.Child("License").Url("Link");
    var copyrightNotice = "Copyright © "
      + copyrightYear + " "
      + config.Child("Owner").String("FullName") 
      // Note: the term "copyright/" is currently hardwired into one of the licenses - adjust to your needs
      + (licUrl != "copyright/" ?  " (" + licUrl + ")" : "");

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
    AddTag(channel, "title", config.Title);
    AddTag(channel, "link", Link.To(pageId: MyPage.Id, type: "full"));
    AddTag(channel, "description", config.String("Description", scrubHtml: true));
    AddTag(channel, "language", config.String("Language", scrubHtml: true));
    AddTag(channel, "copyright", copyrightNotice);
    AddTag(channel, "managingEditor", config.Child("Owner").String("Email") + " (" + config.Child("Owner").String("FullName") + ")");
    var image = AddTag(channel, "image");
    AddTag(image, "title", config.Title);
    AddTag(image, "url", Link.Image(config.Url("Image"), type: "full"));
    AddTag(image, "link", Link.To(type: "full"));

    // 2.2 Create <creativeCommons:license> tag and add Creative Commons license if isn't copyright
    if(licUrl != "copyright/") 
      AddNamespaceTag(channel, CreativeCommonsNsCode, "license", CreativeCommonsNamespace, licUrl);
    
    // 2.3 Add required Itunes values to channel
    AddChannelItunes(channel);

    // 2.4 Add Atom tag to channel
    AddChannelAtom(channel);
    
    // 2.5 Add all the episodes from the query to this channel
    // get protocol and host to complete the urls of the episodes and the one of the image
    var fullLink = Link.To(type: "full");
    var websiteRoot = fullLink.Substring(0, fullLink.IndexOf("/", fullLink.IndexOf("//") + 2));
    foreach(var episode in episodes)
      AddEpisode(websiteRoot, channel, episode);

    return File(download: false, fileDownloadName: "rss.xml", contents: rssDoc);
  }

  #region functions that add xml sections 

  // Adds required <itunes:xy> tags to channel
  private void AddChannelItunes(XmlElement channel) {
    // Get all posts as delived from the standard query
    var config = MyItem;
    var episodes = config.Parents("Episode").OrderByDescending(e => e.DateTime("Date"));
    var imageUrl = Link.Image(config.Url("Image"), type: "full");

    AddNamespaceTag(channel, ItunesNsCode, "summary", ItunesNamespace, config.String("Description", scrubHtml: true));
    var owner = config.Child("Owner");
    AddNamespaceTag(channel, ItunesNsCode, "author", ItunesNamespace, owner.String("FullName"));
    AddNamespaceTag(channel, ItunesNsCode, "explicit", ItunesNamespace, episodes.Any(e => e.Bool("Explicit")) ? "yes" : "no");
    var itunesImage = AddNamespaceTag(channel, ItunesNsCode, "image", ItunesNamespace);
    // Image size of 3000 x 3000 is required by itunes
    AddAttribute(itunesImage, "href", Link.Image(url: imageUrl, width: 3000, height: 3000));

    var itunesOwner = AddNamespaceTag(channel, ItunesNsCode, "owner", ItunesNamespace);
    AddNamespaceTag(itunesOwner, ItunesNsCode, "name", ItunesNamespace, owner.String("FullName"));
    AddNamespaceTag(itunesOwner, ItunesNsCode, "email", ItunesNamespace, owner.String("Email"));

    var itunesCategory = AddNamespaceTag(channel, ItunesNsCode, "category", ItunesNamespace);
    var category = config.Child("Category");
    AddAttribute(itunesCategory, "text", category.String("MainCategory"));

    // Add subcategory if needed
    if (Text.Has(category.String("SubCategory"))) {
      var itunesSubCategory = AddNamespaceTag(itunesCategory, ItunesNsCode, "category", ItunesNamespace);
      AddAttribute(itunesSubCategory, "text", category.String("SubCategory"));
    }
  }

  // Adds <atom> tag to channel
  private void AddChannelAtom(XmlElement channel) {
    var atomLink = AddNamespaceTag(channel, AtomNsCode, "link", AtomNamespace);
    AddAttribute(atomLink, "href", Link.To(api: "api/PodCast/Rss", parameters: MyPage.Parameters, type: "full"));
    AddAttribute(atomLink, "rel", "self");
    AddAttribute(atomLink, "type", "application/rss+xml");
  }

  // Adds Episode to channel
  private void AddEpisode(string websiteRoot, XmlElement channel, ITypedItem episode) {
    var publicationDate = episode.DateTime("Date", fallback: DateTime.Now).ToString("R");
    var author = episode.Child("Author");
    var authorFullName = author == null ? "" : author.String("FullName");
    var authorEmail = author == null ? "" : author.String("Email");
    
    var itemNode = AddTag(channel, "item");
    AddTag(itemNode, "title", episode.Title);
    AddTag(itemNode, "description", episode.String("Description", scrubHtml: true));

    var itemGuid = AddTag(itemNode, "guid", episode.Guid.ToString());
    AddAttribute(itemGuid, "isPermaLink", "false");
    var category = MyItem.Child("Category");
    AddTag(itemNode, "category", Text.First(category.String("SubCategory"), category.String("MainCategory")));
    AddTag(itemNode, "author", authorEmail + " (" + authorFullName + ")");
    AddTag(itemNode, "pubDate", publicationDate);

    var enclosure = AddTag(itemNode, "enclosure");
    AddAttribute(enclosure, "url", websiteRoot + episode.Url("Audio"));
    AddAttribute(enclosure, "type", "audio/mpeg");
    AddAttribute(enclosure, "length", "1024");
    AddTag(itemNode, "link", Link.To());

    // Adds required <itunes:xy> tags
    AddItemItunes(itemNode, episode, authorFullName);
  }

  // Adds required <itunes:xy> tags to itemNode
  private void AddItemItunes(XmlElement itemNode, ITypedItem episode, string authorFullName) {
    var duration = TimeSpan.FromMinutes(episode.Float("Duration")).ToString("hh\\:mm") + ":00";
    var description = episode.String("Description", scrubHtml: true);
    AddNamespaceTag(itemNode, ItunesNsCode, "subtitle", ItunesNamespace, Text.Crop(description, 255));
    AddNamespaceTag(itemNode, ItunesNsCode, "summary", ItunesNamespace, description);
    AddNamespaceTag(itemNode, ItunesNsCode, "author", ItunesNamespace, authorFullName);
    AddNamespaceTag(itemNode, ItunesNsCode, "duration", ItunesNamespace, duration);
    AddNamespaceTag(itemNode, ItunesNsCode, "explicit", ItunesNamespace, episode.Bool("Explicit") ? "yes" : "no");
  }

  #endregion
  
  
  #region helper functions for creating new XML elements and attributes 

  private XmlElement AddTag(XmlElement parent, string name, string value = null) {
    var node = parent.OwnerDocument.CreateElement(name);
    node.InnerText = value;
    parent.AppendChild(node);
    return node;
  }

  private XmlAttribute AddAttribute(XmlElement parent, string name, string value) {
    var node = parent.OwnerDocument.CreateAttribute(name);
    node.Value = value;
    return parent.Attributes.Append(node);
  }

  private XmlElement AddNamespaceTag(XmlElement parent, string name, string tagNs, string link, string value = null) {
    var node = parent.OwnerDocument.CreateElement(name, tagNs, link);
    if(value != null) node.InnerText = value;
    parent.AppendChild(node);
    return node;
  }

  #endregion
}
