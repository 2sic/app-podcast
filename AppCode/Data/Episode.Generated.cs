// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "Episode.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class Episode
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.06.02
// App/Edition: PodCast/
// User:        2sic Web-Developer
// When:        2024-04-05 13:16:36Z
using System;
using System.Collections.Generic;
using ToSic.Sxc.Adam;

namespace AppCode.Data
{
  // This is a generated class for Episode 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// Episode data. <br/>
  /// Generated 2024-04-05 13:16:36Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Audio`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class Episode: AutoGenerated.ZagEpisode
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.Episode in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagEpisode: Custom.Data.CustomItem
  {
    /// <summary>
    /// Audio as link (url). <br/>
    /// To get the underlying value like 'file:72' use String("Audio")
    /// </summary>
    public string Audio => _item.Url("Audio");

    /// <summary>
    /// Get the file object for Audio - or null if it's empty or not referencing a file.
    /// </summary>
    public IFile AudioFile => _item.File("Audio");

    /// <summary>
    /// Get the folder object for Audio.
    /// </summary>
    public IFolder AudioFolder => _item.Folder("Audio");

    /// <summary>
    /// Author as single item of Owner.
    /// </summary>
    /// <remarks>
    /// Generated to only return 1 child because field settings had Multi-Value=false. The type Owner was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// A single item OR null if nothing found, so you can use ?? to provide alternate objects.
    /// </returns>
    public Owner Author => _author ??= _item.Child<Owner>("Author");
    private Owner _author;

    /// <summary>
    /// Channels as list of Channel.
    /// </summary>
    /// <remarks>
    /// Generated to return child-list child because field settings had Multi-Value=true. The type Channel was specified in the field settings.
    /// </remarks>
    /// <returns>
    /// An IEnumerable of specified type, but can be empty.
    /// </returns>
    public IEnumerable<Channel> Channels => _channels ??= _item.Children<Channel>("Channels");
    private IEnumerable<Channel> _channels;

    /// <summary>
    /// Date as DateTime.
    /// </summary>
    public DateTime Date => _item.DateTime("Date");

    /// <summary>
    /// Description as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Description", scrubHtml: true) etc.
    /// </summary>
    public string Description => _item.String("Description", fallback: "");

    /// <summary>
    /// Duration as int. <br/>
    /// To get other types use methods such as .Decimal("Duration")
    /// </summary>
    public int Duration => _item.Int("Duration");

    /// <summary>
    /// Explicit as bool. <br/>
    /// To get nullable use .Get("Explicit") as bool?;
    /// </summary>
    public bool Explicit => _item.Bool("Explicit");

    /// <summary>
    /// Title as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Title", scrubHtml: true) etc.
    /// </summary>
    /// <remarks>
    /// This hides base property Title.
    /// To access original, convert using AsItem(...) or cast to ITypedItem.
    /// Consider renaming this field in the underlying content-type.
    /// </remarks>
    public new string Title => _item.String("Title", fallback: "");
  }
}