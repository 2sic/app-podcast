<image src="app-icon.png" align="right" width="200px">

# Podcast 2 App for .net CMSs

> This is a [2sxc](https://2sxc.org) App for [DNN ☢️](https://www.dnnsoftware.com/) and [Oqtane 💧](https://www.oqtane.org/)

A standard podcast app to use with 2sxc - it's tested to work with normal PodCast clients as well as iTunes and Google Play.

| Aspect              | Status | Comments or Version
| ------------------- | :----: | -------------------
| 2sxc                | ✅    | requires 2sxc v17.07.00
| Dnn                 | ✅    | For v9.6.1+
| Oqtane              | ✅    | Requires v05.00+
| No jQuery           | ✅    |
| Live Demo           | ➖    |
| Install Checklist   | ✅    | See [Installation](https://azing.org/2sxc/r/Y2n1XQwq) on [azing.org](https://azing.org/2sxc)
| Source & License    | ✅    | included, ISC/MIT
| App Catalog         | ✅    | See [app catalog](https://2sxc.org/en/apps/app/podcast-v2-hybrid-for-dnn-and-oqtane)
| Screenshots         | ✅    | See [app catalog](https://2sxc.org/en/apps/app/podcast-v2-hybrid-for-dnn-and-oqtane)
| Best Practices      | ✅    | Uses v13.10 conventions
| Bootstrap 3         | ✔️    |
| Bootstrap 4         | ✅    |
| Bootstrap 5         | ✅    |



## Customize the App

The podcast app does not have any app settings and only a few resources(labels) you can customize.

If you want to customize the CSS, you will usually follow the ["Create Custom Styles in a Standard 2sxc App" checklist](https://azing.org/2sxc/r/gg_aB9FD)

## History

* v.02.00 2021-10
  * Updated to best-practices of 2sxc 12.05
  * RSS feed now in a web controller
  * Hybrid, so it works with Oqtane
  * Tested to look ok on Bootstrap5
* v.02.01.02 2022-02
  * Enabled data-optimizations
* v.02.02.00 2022-04
  * Change Services to come from "ToSic.Sxc.Services"
  * Use IScrub to replace Tag.Strip with IScrub.All
  * Changed images to use IImageService
  * Activated image configuration
  * Replaced data-enableoptimizations with pageSvc.AssetAttributes()
* v.02.03.00 2022-06
  * Replaced all base classes with their 2sxc 14 equivalents
  * Replaced all GetService<> with the new ServiceKit14
  * Updated webpack
  * Changed all the toolbar configurations to use the IToolbarService
* v.02.04.00 2023-02
  * Replaced turnOn Tag with `Kit.Page.TurnOn`
  * Removed _ from Filenames
  * Code in one file the bs5, less duplicated code
* v02.05.00 2023-07
  * 2sxc 16.02 coding conventions
  * everything typed
* v02.05.01 2024-01
  * fix bug which prevented RSS feed from working
* v02.05.02 2024-02
  * fix episode title (previously the channel title, which was wrong)
* v02.17.00 2024-04
  * Strong Typed
  * Auto Generated Class
  * Typed MyItem
* v02.17.01 2024-07
  * Update app.sln and app.csproj