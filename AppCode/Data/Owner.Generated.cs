// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "Owner.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class Owner
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.06.02
// App/Edition: PodCast/
// User:        2sic Web-Developer
// When:        2024-04-05 13:16:36Z
namespace AppCode.Data
{
  // This is a generated class for Owner 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// Owner data. <br/>
  /// Generated 2024-04-05 13:16:36Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Email`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class Owner: AutoGenerated.ZagOwner
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.Owner in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZagOwner: Custom.Data.CustomItem
  {
    /// <summary>
    /// Email as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Email", scrubHtml: true) etc.
    /// </summary>
    public string Email => _item.String("Email", fallback: "");

    /// <summary>
    /// FullName as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("FullName", scrubHtml: true) etc.
    /// </summary>
    public string FullName => _item.String("FullName", fallback: "");
  }
}