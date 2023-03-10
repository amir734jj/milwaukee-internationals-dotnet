using System;
using System.Collections.Generic;
using System.Text;
using TimeZoneConverter;

namespace Models.Constants;

public static class ApplicationConstants
{
    /// <summary>
    ///     Available themes
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> Themes = new Dictionary<string, string>
    {
        ["cerulean"] = "cerulean.bootstrap.min.css",
        ["darkly"] = "darkly.bootstrap.min.css",
        ["lumen"] = "lumen.bootstrap.min.css",
        ["sandstone"] = "sandstone.bootstrap.min.css",
        ["spacelab"] = "spacelab.bootstrap.min.css",
        ["yeti"] = "yeti.bootstrap.min.css",
        ["cosmo"] = "cosmo.bootstrap.min.css",
        ["flatly"] = "flatly.bootstrap.min.css",
        ["paper"] = "paper.bootstrap.min.css",
        ["simplex"] = "simplex.bootstrap.min.css",
        ["superhero"] = "superhero.bootstrap.min.css",
        ["cyborg"] = "cyborg.bootstrap.min.css",
        ["journal"] = "journal.bootstrap.min.css",
        ["readable"] = "readable.bootstrap.min.css",
        ["slate"] = "slate.bootstrap.min.css",
        ["united"] = "united.bootstrap.min.css",
        ["default"] = ""
    };

    public const string ConfigFile = "milwaukee-internationals-config";

    public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    
    public static TimeZoneInfo TimeZoneInfo = TZConvert.GetTimeZoneInfo("America/Chicago");
}