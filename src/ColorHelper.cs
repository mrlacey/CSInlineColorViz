using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace CsInlineColorViz
{
    public static class ColorHelper
    {
        public static bool TryGetColor(string colorName, out Color color)
        {
            try
            {
                if (!colorName?.TrimStart().StartsWith("#") ?? false)
                {
                    colorName = GetHexForNamedColor(colorName.Trim());
                }

                color = (Color)ColorConverter.ConvertFromString(colorName.Trim());
                return true;
            }
            catch (System.Exception)
            {
                color = default;
                return false;
            }
        }

        public static string ToHex(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static bool TryGetFromName(string args, out Color color)
        {
            color = default;

            try
            {
                var sdc = ColorHelper.ToHex(System.Drawing.Color.FromName(args));

                return TryGetColor(sdc, out color);
            }
            catch
            {
            }

            return false;
        }

        public static bool TryGetArgbColor(string args, out Color color)
        {
            color = default;

            try
            {
                var parts = args.Split(',');

                var lastPart = parts.Last().Trim();

                if (parts.Length == 2 && lastPart.StartsWith("Color."))
                {
                    if (TryGetFromName(lastPart.Substring(6), out Color innerColor))
                    {
                        color = Color.FromArgb(byte.Parse(parts[0]), innerColor.R, innerColor.G, innerColor.B);
                        return true;
                    }

                    return false;
                }
                else
                {
                    if (parts.Length == 1)
                    {
                        var sdcolor = System.Drawing.Color.FromArgb(int.Parse(parts[0]));
                        color = Color.FromArgb(sdcolor.A, sdcolor.R, sdcolor.G, sdcolor.B);
                    }
                    else if (parts.Length == 3)
                    {
                        var sdcolor = System.Drawing.Color.FromArgb(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                        color = Color.FromArgb(sdcolor.A, sdcolor.R, sdcolor.G, sdcolor.B);
                    }
                    else if (parts.Length == 4)
                    {
                        color = Color.FromArgb(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]), byte.Parse(parts[3]));
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool TryGetRgbColor(string args, out Color color)
        {
            color = default;

            try
            {
                var parts = args.Split(',');

                if (parts.Length == 3)
                {
                    color = Color.FromRgb(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool TryGetHexColor(string args, out Color color)
        {
            color = default;

            try
            {
                if (!string.IsNullOrWhiteSpace(args))
                {
                    color = (Color)ColorConverter.ConvertFromString(args);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static string GetHexForNamedColor(string colorName)
        {
            switch (colorName?.ToLowerInvariant().Replace(" ", string.Empty) ?? string.Empty)
            {
                case "aliceblue": return "#F0F8FF";
                case "antiquewhite": return "#FAEBD7";
                case "aqua": return "#00FFFF";
                case "aquamarine": return "#7FFFD4";
                case "azure": return "#F0FFFF";
                case "beige": return "#F5F5DC";
                case "bisque": return "#FFE4C4";
                case "black": return "#000000";
                case "blanchedalmond": return "#FFEBCD";
                case "blue": return "#0000FF";
                case "blueviolet": return "#8A2BE2";
                case "brown": return "#A52A2A";
                case "burgendy": return "#FF6347";
                case "burlywood": return "#DEB887";
                case "cadetblue": return "#5F9EA0";
                case "chartreuse": return "#7FFF00";
                case "chocolate": return "#D2691E";
                case "coral": return "#FF7F50";
                case "cornflowerblue": return "#6495ED";
                case "cornsilk": return "#FFF8DC";
                case "crimson": return "#DC143C";
                case "cyan": return "#00FFFF";
                case "darkblue": return "#00008B";
                case "darkcyan": return "#008B8B";
                case "darkgoldenrod": return "#B8860B";
                case "darkgray": return "#A9A9A9";
                case "darkgreen": return "#006400";
                case "darkgrey": return "#A9A9A9";
                case "darkkhaki": return "#BDB76B";
                case "darkmagenta": return "#8B008B";
                case "darkolivegreen": return "#556B2F";
                case "darkorange": return "#FF8C00";
                case "darkorchid": return "#9932CC";
                case "darkred": return "#8B0000";
                case "darksalmon": return "#E9967A";
                case "darkseagreen": return "#8FBC8B";
                case "darkslateblue": return "#483D8B";
                case "darkslategray": return "#2F4F4F";
                case "darkslategrey": return "#2F4F4F";
                case "darkturquoise": return "#00CED1";
                case "darkviolet": return "#9400D3";
                case "darkyellow": return "#D7C32A";
                case "deeppink": return "#FF1493";
                case "deepskyblue": return "#00BFFF";
                case "dimgray": return "#696969";
                case "dimgrey": return "#696969";
                case "dodgerblue": return "#1E90FF";
                case "firebrick": return "#B22222";
                case "floralwhite": return "#FFFAF0";
                case "forestgreen": return "#228B22";
                case "fuchsia": return "#FF00FF";
                case "gainsboro": return "#DCDCDC";
                case "ghostwhite": return "#F8F8FF";
                case "gold": return "#FFD700";
                case "goldenrod": return "#DAA520";
                case "gray": return "#808080";
                case "green": return "#008000";
                case "greenyellow": return "#ADFF2F";
                case "grey": return "#808080";
                case "honeydew": return "#F0FFF0";
                case "hotpink": return "#FF69B4";
                case "indianred": return "#CD5C5C";
                case "indigo": return "#4B0082";
                case "ivory": return "#FFFFF0";
                case "khaki": return "#F0E68C";
                case "lavender": return "#E6E6FA";
                case "lavenderblush": return "#FFF0F5";
                case "lawngreen": return "#7CFC00";
                case "lemonchiffon": return "#FFFACD";
                case "lightblue": return "#ADD8E6";
                case "lightcoral": return "#F08080";
                case "lightcyan": return "#E0FFFF";
                case "lightgoldenrodyellow": return "#FAFAD2";
                case "lightgray": return "#D3D3D3";
                case "lightgreen": return "#90EE90";
                case "lightgrey": return "#d3d3d3";
                case "lightpink": return "#FFB6C1";
                case "lightsalmon": return "#FFA07A";
                case "lightseagreen": return "#20B2AA";
                case "lightskyblue": return "#87CEFA";
                case "lightslategray": return "#778899";
                case "lightslategrey": return "#778899";
                case "lightsteelblue": return "#B0C4DE";
                case "lightyellow": return "#FFFFE0";
                case "lime": return "#00FF00";
                case "limegreen": return "#32CD32";
                case "linen": return "#FAF0E6";
                case "magenta": return "#FF00FF";
                case "maroon": return "#800000";
                case "mediumaquamarine": return "#66CDAA";
                case "mediumblue": return "#0000CD";
                case "mediumorchid": return "#BA55D3";
                case "mediumpurple": return "#9370DB";
                case "mediumseagreen": return "#3CB371";
                case "mediumslateblue": return "#7B68EE";
                case "mediumspringgreen": return "#00FA9A";
                case "mediumturquoise": return "#48D1CC";
                case "mediumvioletred": return "#C71585";
                case "midnightblue": return "#191970";
                case "mint": return "#66CDAA";
                case "mintcream": return "#F5FFFA";
                case "mistyrose": return "#FFE4E1";
                case "moccasin": return "#FFE4B5";
                case "navajowhite": return "#FFDEAD";
                case "navy": return "#000080";
                case "ochre": return "#D7C32A";
                case "oldlace": return "#FDF5E6";
                case "olive": return "#808000";
                case "olivedrab": return "#6B8E23";
                case "orange": return "#FFA500";
                case "orangered": return "#FF4500";
                case "orchid": return "#DA70D6";
                case "palegoldenrod": return "#EEE8AA";
                case "palegreen": return "#98FB98";
                case "paleturquoise": return "#AFEEEE";
                case "palevioletred": return "#DB7093";
                case "papayawhip": return "#FFEFD5";
                case "peachpuff": return "#FFDAB9";
                case "peru": return "#CD853F";
                case "pink": return "#FFC0CB";
                case "plum": return "#DDA0DD";
                case "powderblue": return "#B0E0E6";
                case "purple": return "#800080";
                case "pumpkin": return "#FF4500";
                case "rebeccapurple": return "#663399";
                case "red": return "#FF0000";
                case "rosybrown": return "#BC8F8F";
                case "royalblue": return "#4169E1";
                case "saddlebrown": return "#8B4513";
                case "salmon": return "#FA8072";
                case "sandybrown": return "#F4A460";
                case "seagreen": return "#2E8B57";
                case "seashell": return "#FFF5EE";
                case "sienna": return "#A0522D";
                case "silver": return "#C0C0C0";
                case "skyblue": return "#87CEEB";
                case "slateblue": return "#6A5ACD";
                case "slategray": return "#708090";
                case "slategrey": return "#708090";
                case "snow": return "#FFFAFA";
                case "springgreen": return "#00FF7F";
                case "steelblue": return "#4682B4";
                case "tan": return "#D2B48C";
                case "teal": return "#008080";
                case "thistle": return "#D8BFD8";
                case "tomato": return "#FF6347";
                case "turquoise": return "#40E0D0";
                case "violet": return "#EE82EE";
                case "volt": return "#CEFF00";
                case "wheat": return "#F5DEB3";
                case "white": return "#FFFFFF";
                case "whitesmoke": return "#F5F5F5";
                case "yellow": return "#FFFF00";
                case "yellowgreen": return "#9ACD32";
                default: return colorName;
            }
        }

        public static IEnumerable<System.Drawing.Color> SystemDrawingColorsAlphabetical()
        {
            yield return System.Drawing.Color.AliceBlue;
            yield return System.Drawing.Color.AntiqueWhite;
            yield return System.Drawing.Color.Aqua;
            yield return System.Drawing.Color.Aquamarine;
            yield return System.Drawing.Color.Azure;
            yield return System.Drawing.Color.Beige;
            yield return System.Drawing.Color.Bisque;
            yield return System.Drawing.Color.Black;
            yield return System.Drawing.Color.BlanchedAlmond;
            yield return System.Drawing.Color.Blue;
            yield return System.Drawing.Color.BlueViolet;
            yield return System.Drawing.Color.Brown;
            yield return System.Drawing.Color.BurlyWood;
            yield return System.Drawing.Color.CadetBlue;
            yield return System.Drawing.Color.Chartreuse;
            yield return System.Drawing.Color.Chocolate;
            yield return System.Drawing.Color.Coral;
            yield return System.Drawing.Color.CornflowerBlue;
            yield return System.Drawing.Color.Cornsilk;
            yield return System.Drawing.Color.Crimson;
            yield return System.Drawing.Color.Cyan;
            yield return System.Drawing.Color.DarkBlue;
            yield return System.Drawing.Color.DarkCyan;
            yield return System.Drawing.Color.DarkGoldenrod;
            yield return System.Drawing.Color.DarkGray;
            yield return System.Drawing.Color.DarkGreen;
            yield return System.Drawing.Color.DarkKhaki;
            yield return System.Drawing.Color.DarkMagenta;
            yield return System.Drawing.Color.DarkOliveGreen;
            yield return System.Drawing.Color.DarkOrange;
            yield return System.Drawing.Color.DarkOrchid;
            yield return System.Drawing.Color.DarkRed;
            yield return System.Drawing.Color.DarkSalmon;
            yield return System.Drawing.Color.DarkSeaGreen;
            yield return System.Drawing.Color.DarkSlateBlue;
            yield return System.Drawing.Color.DarkSlateGray;
            yield return System.Drawing.Color.DarkTurquoise;
            yield return System.Drawing.Color.DarkViolet;
            yield return System.Drawing.Color.DeepPink;
            yield return System.Drawing.Color.DeepSkyBlue;
            yield return System.Drawing.Color.DimGray;
            yield return System.Drawing.Color.DodgerBlue;
            yield return System.Drawing.Color.Firebrick;
            yield return System.Drawing.Color.FloralWhite;
            yield return System.Drawing.Color.ForestGreen;
            yield return System.Drawing.Color.Fuchsia;
            yield return System.Drawing.Color.Gainsboro;
            yield return System.Drawing.Color.GhostWhite;
            yield return System.Drawing.Color.Gold;
            yield return System.Drawing.Color.Goldenrod;
            yield return System.Drawing.Color.Gray;
            yield return System.Drawing.Color.Green;
            yield return System.Drawing.Color.GreenYellow;
            yield return System.Drawing.Color.Honeydew;
            yield return System.Drawing.Color.HotPink;
            yield return System.Drawing.Color.IndianRed;
            yield return System.Drawing.Color.Indigo;
            yield return System.Drawing.Color.Ivory;
            yield return System.Drawing.Color.Khaki;
            yield return System.Drawing.Color.Lavender;
            yield return System.Drawing.Color.LavenderBlush;
            yield return System.Drawing.Color.LawnGreen;
            yield return System.Drawing.Color.LemonChiffon;
            yield return System.Drawing.Color.LightBlue;
            yield return System.Drawing.Color.LightCoral;
            yield return System.Drawing.Color.LightCyan;
            yield return System.Drawing.Color.LightGreen;
            yield return System.Drawing.Color.LightPink;
            yield return System.Drawing.Color.LightSalmon;
            yield return System.Drawing.Color.LightSeaGreen;
            yield return System.Drawing.Color.LightSkyBlue;
            yield return System.Drawing.Color.LightSlateGray;
            yield return System.Drawing.Color.LightSteelBlue;
            yield return System.Drawing.Color.LightYellow;
            yield return System.Drawing.Color.Lime;
            yield return System.Drawing.Color.LimeGreen;
            yield return System.Drawing.Color.Linen;
            yield return System.Drawing.Color.Magenta;
            yield return System.Drawing.Color.Maroon;
            yield return System.Drawing.Color.MediumAquamarine;
            yield return System.Drawing.Color.MediumBlue;
            yield return System.Drawing.Color.MediumOrchid;
            yield return System.Drawing.Color.MediumPurple;
            yield return System.Drawing.Color.MediumSeaGreen;
            yield return System.Drawing.Color.MediumSlateBlue;
            yield return System.Drawing.Color.MediumSpringGreen;
            yield return System.Drawing.Color.MediumTurquoise;
            yield return System.Drawing.Color.MediumVioletRed;
            yield return System.Drawing.Color.MidnightBlue;
            yield return System.Drawing.Color.MintCream;
            yield return System.Drawing.Color.MistyRose;
            yield return System.Drawing.Color.Moccasin;
            yield return System.Drawing.Color.NavajoWhite;
            yield return System.Drawing.Color.Navy;
            yield return System.Drawing.Color.OldLace;
            yield return System.Drawing.Color.Olive;
            yield return System.Drawing.Color.OliveDrab;
            yield return System.Drawing.Color.Orange;
            yield return System.Drawing.Color.OrangeRed;
            yield return System.Drawing.Color.Orchid;
            yield return System.Drawing.Color.PaleGoldenrod;
            yield return System.Drawing.Color.PaleGreen;
            yield return System.Drawing.Color.PaleTurquoise;
            yield return System.Drawing.Color.PaleVioletRed;
            yield return System.Drawing.Color.PapayaWhip;
            yield return System.Drawing.Color.PeachPuff;
            yield return System.Drawing.Color.Peru;
            yield return System.Drawing.Color.Pink;
            yield return System.Drawing.Color.Plum;
            yield return System.Drawing.Color.PowderBlue;
            yield return System.Drawing.Color.Purple;
            yield return System.Drawing.Color.Red;
            yield return System.Drawing.Color.RosyBrown;
            yield return System.Drawing.Color.RoyalBlue;
            yield return System.Drawing.Color.SaddleBrown;
            yield return System.Drawing.Color.Salmon;
            yield return System.Drawing.Color.SandyBrown;
            yield return System.Drawing.Color.SeaGreen;
            yield return System.Drawing.Color.SeaShell;
            yield return System.Drawing.Color.Sienna;
            yield return System.Drawing.Color.Silver;
            yield return System.Drawing.Color.SkyBlue;
            yield return System.Drawing.Color.SlateBlue;
            yield return System.Drawing.Color.SlateGray;
            yield return System.Drawing.Color.Snow;
            yield return System.Drawing.Color.SpringGreen;
            yield return System.Drawing.Color.SteelBlue;
            yield return System.Drawing.Color.Tan;
            yield return System.Drawing.Color.Teal;
            yield return System.Drawing.Color.Thistle;
            yield return System.Drawing.Color.Tomato;
            yield return System.Drawing.Color.Transparent;
            yield return System.Drawing.Color.Turquoise;
            yield return System.Drawing.Color.Violet;
            yield return System.Drawing.Color.Wheat;
            yield return System.Drawing.Color.White;
            yield return System.Drawing.Color.WhiteSmoke;
            yield return System.Drawing.Color.Yellow;
            yield return System.Drawing.Color.YellowGreen;
        }

        public static IEnumerable<System.Drawing.Color> SystemDrawingColorsSpectrum()
        {
            yield return System.Drawing.Color.Tan;
            yield return System.Drawing.Color.RosyBrown;
            yield return System.Drawing.Color.Sienna;
            yield return System.Drawing.Color.SaddleBrown;
            yield return System.Drawing.Color.DarkRed;
            yield return System.Drawing.Color.Maroon;
            yield return System.Drawing.Color.Purple;
            yield return System.Drawing.Color.DarkMagenta;
            yield return System.Drawing.Color.Brown;
            yield return System.Drawing.Color.Firebrick;
            yield return System.Drawing.Color.Crimson;
            yield return System.Drawing.Color.Red;
            yield return System.Drawing.Color.OrangeRed;
            yield return System.Drawing.Color.IndianRed;
            yield return System.Drawing.Color.Tomato;
            yield return System.Drawing.Color.Chocolate;
            yield return System.Drawing.Color.DarkSalmon;
            yield return System.Drawing.Color.Coral;
            yield return System.Drawing.Color.Salmon;
            yield return System.Drawing.Color.LightCoral;
            yield return System.Drawing.Color.Peru;
            yield return System.Drawing.Color.DarkOrange;
            yield return System.Drawing.Color.SandyBrown;
            yield return System.Drawing.Color.LightSalmon;
            yield return System.Drawing.Color.Orange;
            yield return System.Drawing.Color.Goldenrod;
            yield return System.Drawing.Color.BurlyWood;
            yield return System.Drawing.Color.DarkGoldenrod;
            yield return System.Drawing.Color.DarkKhaki;
            yield return System.Drawing.Color.AntiqueWhite;
            yield return System.Drawing.Color.Beige;
            yield return System.Drawing.Color.Cornsilk;
            yield return System.Drawing.Color.OldLace;
            yield return System.Drawing.Color.Honeydew;
            yield return System.Drawing.Color.LightYellow;
            yield return System.Drawing.Color.LemonChiffon;
            yield return System.Drawing.Color.BlanchedAlmond;
            yield return System.Drawing.Color.Bisque;
            yield return System.Drawing.Color.Wheat;
            yield return System.Drawing.Color.Moccasin;
            yield return System.Drawing.Color.NavajoWhite;
            yield return System.Drawing.Color.PeachPuff;
            yield return System.Drawing.Color.PapayaWhip;
            yield return System.Drawing.Color.PaleGoldenrod;
            yield return System.Drawing.Color.Khaki;
            yield return System.Drawing.Color.Gold;
            yield return System.Drawing.Color.Yellow;
            yield return System.Drawing.Color.GreenYellow;
            yield return System.Drawing.Color.PaleGreen;
            yield return System.Drawing.Color.Chartreuse;
            yield return System.Drawing.Color.LawnGreen;
            yield return System.Drawing.Color.LightGreen;
            yield return System.Drawing.Color.MediumSpringGreen;
            yield return System.Drawing.Color.Lime;
            yield return System.Drawing.Color.SpringGreen;
            yield return System.Drawing.Color.LimeGreen;
            yield return System.Drawing.Color.MediumSeaGreen;
            yield return System.Drawing.Color.ForestGreen;
            yield return System.Drawing.Color.Green;
            yield return System.Drawing.Color.DarkGreen;
            yield return System.Drawing.Color.SeaGreen;
            yield return System.Drawing.Color.YellowGreen;
            yield return System.Drawing.Color.DarkSeaGreen;
            yield return System.Drawing.Color.OliveDrab;
            yield return System.Drawing.Color.Olive;
            yield return System.Drawing.Color.DarkOliveGreen;
            yield return System.Drawing.Color.Teal;
            yield return System.Drawing.Color.DarkCyan;
            yield return System.Drawing.Color.CadetBlue;
            yield return System.Drawing.Color.LightSeaGreen;
            yield return System.Drawing.Color.MediumAquamarine;
            yield return System.Drawing.Color.MediumTurquoise;
            yield return System.Drawing.Color.Turquoise;
            yield return System.Drawing.Color.Aquamarine;
            yield return System.Drawing.Color.PaleTurquoise;
            yield return System.Drawing.Color.LightBlue;
            yield return System.Drawing.Color.PowderBlue;
            yield return System.Drawing.Color.SkyBlue;
            yield return System.Drawing.Color.LightSkyBlue;
            yield return System.Drawing.Color.DeepSkyBlue;
            yield return System.Drawing.Color.Aqua;
            yield return System.Drawing.Color.Cyan;
            yield return System.Drawing.Color.DarkTurquoise;
            yield return System.Drawing.Color.DodgerBlue;
            yield return System.Drawing.Color.CornflowerBlue;
            yield return System.Drawing.Color.RoyalBlue;
            yield return System.Drawing.Color.SteelBlue;
            yield return System.Drawing.Color.MediumBlue;
            yield return System.Drawing.Color.Blue;
            yield return System.Drawing.Color.DarkBlue;
            yield return System.Drawing.Color.Navy;
            yield return System.Drawing.Color.MidnightBlue;
            yield return System.Drawing.Color.DarkSlateBlue;
            yield return System.Drawing.Color.Indigo;
            yield return System.Drawing.Color.SlateBlue;
            yield return System.Drawing.Color.MediumSlateBlue;
            yield return System.Drawing.Color.MediumPurple;
            yield return System.Drawing.Color.BlueViolet;
            yield return System.Drawing.Color.DarkOrchid;
            yield return System.Drawing.Color.DarkViolet;
            yield return System.Drawing.Color.MediumOrchid;
            yield return System.Drawing.Color.Magenta;
            yield return System.Drawing.Color.Fuchsia;
            yield return System.Drawing.Color.Orchid;
            yield return System.Drawing.Color.Violet;
            yield return System.Drawing.Color.Plum;
            yield return System.Drawing.Color.PaleVioletRed;
            yield return System.Drawing.Color.MediumVioletRed;
            yield return System.Drawing.Color.DeepPink;
            yield return System.Drawing.Color.HotPink;
            yield return System.Drawing.Color.LightPink;
            yield return System.Drawing.Color.Pink;
            yield return System.Drawing.Color.Thistle;
            yield return System.Drawing.Color.Lavender;
            yield return System.Drawing.Color.MistyRose;
            yield return System.Drawing.Color.LavenderBlush;
            yield return System.Drawing.Color.Linen;
            yield return System.Drawing.Color.AliceBlue;
            yield return System.Drawing.Color.Azure;
            yield return System.Drawing.Color.LightCyan;
            yield return System.Drawing.Color.FloralWhite;
            yield return System.Drawing.Color.WhiteSmoke;
            yield return System.Drawing.Color.SeaShell;
            yield return System.Drawing.Color.Ivory;
            yield return System.Drawing.Color.MintCream;
            yield return System.Drawing.Color.Snow;
            yield return System.Drawing.Color.White;
            yield return System.Drawing.Color.GhostWhite;
            yield return System.Drawing.Color.Gainsboro;
            yield return System.Drawing.Color.Silver;
            yield return System.Drawing.Color.DarkGray;
            yield return System.Drawing.Color.Gray;
            yield return System.Drawing.Color.DimGray;
            yield return System.Drawing.Color.LightSteelBlue;
            yield return System.Drawing.Color.LightSlateGray;
            yield return System.Drawing.Color.SlateGray;
            yield return System.Drawing.Color.DarkSlateGray;
            yield return System.Drawing.Color.Black;
            yield return System.Drawing.Color.Transparent;
        }
    }
}
