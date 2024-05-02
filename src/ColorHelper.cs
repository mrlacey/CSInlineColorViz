using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace CsInlineColorViz
{
	public static class ColorHelper
	{
		private static double Tolerance => 0.000000000000001;

		public static bool TryGetColor(string colorName, out Color color)
		{
			try
			{
				if (!colorName?.TrimStart().StartsWith("#") ?? false)
				{
					colorName = GetHexForNamedColor(colorName.Trim());
				}

				// If still don't have a hex value, then try to parse it as a known color (SystemColor)
				if (!colorName?.TrimStart().StartsWith("#") ?? false)
				{
					// The System.Windows versions of SystemColors end with "Color" (but the System.Drawing versions don't)
					if (colorName.EndsWith("Color"))
					{
						colorName = colorName.Substring(0, colorName.Length - "Color".Length);
					}

					if (Enum.TryParse(colorName, out System.Drawing.KnownColor knownColor))
					{
						colorName = ColorHelper.ToHex(System.Drawing.Color.FromKnownColor(knownColor));
					}
				}

				// By here, colorName should be a hex value
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

		public static bool TryGetFromInt(string args, out Color color)
		{
			color = default;

			try
			{
				return TryGetColor($"#{int.Parse(args).ToString("X")}", out color);
			}
			catch
			{
			}

			return false;
		}

		public static bool TryGetFromUint(string args, out Color color)
		{
			color = default;

			try
			{
				return TryGetColor($"#{uint.Parse(args).ToString("X")}", out color);
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

		public static bool TryGetHsvaColor(string args, out Color color)
		{
			color = default;

			try
			{
				var parts = args.Replace("f", string.Empty).Replace("F", string.Empty).Replace("d", string.Empty).Replace("D", string.Empty).Split(',');

				var lastPart = parts.Last().Trim();

				if (parts.Length == 3)
				{
					if (double.TryParse(parts[0], out double hue) && double.TryParse(parts[1], out double sat) && double.TryParse(parts[2], out double val))
					{
						color = ColorFromHSVA(hue, sat, val, 255);
						return true;
					}

					return false;
				}
				else if (parts.Length == 4)
				{
					if (double.TryParse(parts[0], out double hue) && double.TryParse(parts[1], out double sat) && double.TryParse(parts[2], out double val) && double.TryParse(parts[3], out double alpha))
					{
						color = ColorFromHSVA(hue, sat, val, alpha);
						return true;
					}

					return false;
				}

				return true;
			}
			catch (System.Exception)
			{
				return false;
			}
		}

		public static bool TryGetHslaColor(string args, out Color color)
		{
			color = default;

			try
			{
				var parts = args.Replace("f", string.Empty).Replace("F", string.Empty).Replace("d", string.Empty).Replace("D", string.Empty).Split(',');

				var lastPart = parts.Last().Trim();

				if (parts.Length == 3)
				{
					if (double.TryParse(parts[0], out double hue) && double.TryParse(parts[1], out double sat) && double.TryParse(parts[2], out double lum))
					{
						color = HslaToColor(hue, sat, lum, 255);
						return true;
					}

					return false;
				}
				else if (parts.Length == 4)
				{
					if (double.TryParse(parts[0], out double hue) && double.TryParse(parts[1], out double sat) && double.TryParse(parts[2], out double lum) && double.TryParse(parts[3], out double alpha))
					{
						color = HslaToColor(hue, sat, lum, alpha);
						return true;
					}

					return false;
				}

				return true;
			}
			catch (System.Exception)
			{
				return false;
			}
		}

		public static Color ColorFromHSVA(double hue, double saturation, double value, double alpha = 1.0)
		{
			return ColorFromHSVA(hue, saturation, value, (byte)(alpha * 255));
		}

		public static Color ColorFromHSVA(double hue, double saturation, double value, byte alpha = 255)
		{
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value = value * 255;
			var v = Convert.ToByte(value);
			var p = Convert.ToByte(value * (1 - saturation));
			var q = Convert.ToByte(value * (1 - f * saturation));
			var t = Convert.ToByte(value * (1 - (1 - f) * saturation));

			if (hi == 0)
				return Color.FromArgb(alpha, v, t, p);
			else if (hi == 1)
				return Color.FromArgb(alpha, q, v, p);
			else if (hi == 2)
				return Color.FromArgb(alpha, p, v, t);
			else if (hi == 3)
				return Color.FromArgb(alpha, p, q, v);
			else if (hi == 4)
				return Color.FromArgb(alpha, t, p, v);
			else
				return Color.FromArgb(alpha, v, p, q);
		}

		/// <summary>
		/// Converts RGB to HSB. Alpha is ignored.
		/// Output is: { H: [0, 360], S: [0, 1], B: [0, 1] }.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		public static double[] RgBtoHsb(Color color)
		{
			// normalize red, green and blue values
			double r = color.R / 255D;
			double g = color.G / 255D;
			double b = color.B / 255D;

			// conversion start
			double max = System.Math.Max(r, System.Math.Max(g, b));
			double min = System.Math.Min(r, System.Math.Min(g, b));

			double h = 0D;
			if ((System.Math.Abs(max - r) < Tolerance)
					&& (g >= b))
				h = (60D * (g - b)) / (max - min);
			else if ((System.Math.Abs(max - r) < Tolerance)
					&& (g < b))
				h = ((60D * (g - b)) / (max - min)) + 360D;
			else if (System.Math.Abs(max - g) < Tolerance)
				h = ((60D * (b - r)) / (max - min)) + 120D;
			else if (System.Math.Abs(max - b) < Tolerance)
				h = ((60D * (r - g)) / (max - min)) + 240D;

			double s = System.Math.Abs(max) < Tolerance
					? 0D
					: 1D - (min / max);

			return new[]
			{
				Math.Max(0D, Math.Min(360D, h)),
				Math.Max(0D, Math.Min(1D, s)),
				Math.Max(0D, Math.Min(1D, max))
			};
		}

		/// <summary>
		/// Converts RGB to HSL. Alpha is ignored.
		/// Output is: { H: [0, 360], S: [0, 1], L: [0, 1] }.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		public static double[] RgBtoHsl(Color color)
		{
			double h = 0D;
			double s = 0D;
			double l;

			// normalize red, green, blue values
			double r = color.R / 255D;
			double g = color.G / 255D;
			double b = color.B / 255D;

			double max = System.Math.Max(r, System.Math.Max(g, b));
			double min = System.Math.Min(r, System.Math.Min(g, b));

			// hue
			if (System.Math.Abs(max - min) < Tolerance)
				h = 0D; // undefined
			else if ((System.Math.Abs(max - r) < Tolerance)
					&& (g >= b))
				h = (60D * (g - b)) / (max - min);
			else if ((System.Math.Abs(max - r) < Tolerance)
					&& (g < b))
				h = ((60D * (g - b)) / (max - min)) + 360D;
			else if (System.Math.Abs(max - g) < Tolerance)
				h = ((60D * (b - r)) / (max - min)) + 120D;
			else if (System.Math.Abs(max - b) < Tolerance)
				h = ((60D * (r - g)) / (max - min)) + 240D;

			// luminance
			l = (max + min) / 2D;

			// saturation
			if ((System.Math.Abs(l) < Tolerance)
					|| (System.Math.Abs(max - min) < Tolerance))
				s = 0D;
			else if ((0D < l)
					&& (l <= .5D))
				s = (max - min) / (max + min);
			else if (l > .5D)
				s = (max - min) / (2D - (max + min)); //(max-min > 0)?

			return new[]
			{
				System.Math.Max(0D, System.Math.Min(360D, double.Parse($"{h:0.##}"))),
				System.Math.Max(0D, System.Math.Min(1D, double.Parse($"{s:0.##}"))),
				System.Math.Max(0D, System.Math.Min(1D, double.Parse($"{l:0.##}")))
			};
		}

		public static Color HslaToColor(double h, double s, double l, double a = 1.0)
		{
			return HslaToColor(h, s, l, (byte)(a * 255));
		}

		/// <summary>
		/// Converts HSL to RGB, with a specified output Alpha.
		/// Arguments are limited to the defined range:
		/// does not raise exceptions.
		/// </summary>
		/// <param name="h">Hue, must be in [0, 360].</param>
		/// <param name="s">Saturation, must be in [0, 1].</param>
		/// <param name="l">Luminance, must be in [0, 1].</param>
		/// <param name="a">Output Alpha, must be in [0, 255].</param>
		public static Color HslaToColor(double h, double s, double l, byte a = 255)
		{
			h = Math.Max(0D, Math.Min(360D, h));
			s = Math.Max(0D, Math.Min(1D, s));
			l = Math.Max(0D, Math.Min(1D, l));
			a = Math.Max((byte)0, Math.Min((byte)255, a));

			// achromatic argb (gray scale)
			if (Math.Abs(s) < Tolerance)
			{
				return Color.FromArgb(
						a,
						(byte)Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
						(byte)Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
						(byte)Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))));
			}

			double q = l < .5D
					? l * (1D + s)
					: (l + s) - (l * s);
			double p = (2D * l) - q;

			double hk = h / 360D;
			double[] T = new double[3];
			T[0] = hk + (1D / 3D); // Tr
			T[1] = hk; // Tb
			T[2] = hk - (1D / 3D); // Tg

			for (int i = 0; i < 3; i++)
			{
				if (T[i] < 0D)
					T[i] += 1D;
				if (T[i] > 1D)
					T[i] -= 1D;

				if ((T[i] * 6D) < 1D)
					T[i] = p + ((q - p) * 6D * T[i]);
				else if ((T[i] * 2D) < 1)
					T[i] = q;
				else if ((T[i] * 3D) < 2)
					T[i] = p + ((q - p) * ((2D / 3D) - T[i]) * 6D);
				else
					T[i] = p;
			}

			return Color.FromArgb(
					a,
					(byte)Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{T[0] * 255D:0.00}")))),
					(byte)Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{T[1] * 255D:0.00}")))),
					(byte)Math.Max(0, Math.Min(255, Convert.ToInt32(double.Parse($"{T[2] * 255D:0.00}")))));
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

		public static bool TryGetRgbaColor(string args, out Color color)
		{
			color = default;

			try
			{
				var parts = args.Split(',');

				if (parts.Length == 4)
				{
					// Note the order change. The method expectes rgbA, but this is taking Argb
					color = Color.FromArgb(byte.Parse(parts[3]), byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));
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
			// TODO: Add Transparent and Clear
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

		// Note that SystemColors are KnownColors
		// Also note that System.Windows.SystemColors are the same as Systmem.Drawing.SystemColors, but with "Color" on the end
		public static IEnumerable<System.Drawing.Color> SystemColorsAlphabetically()
		{
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ActiveBorder);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ActiveCaption);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ActiveCaptionText);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.AppWorkspace);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ButtonFace);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ButtonHighlight);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ButtonShadow);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlDark);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlDarkDark);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlLight);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlLightLight);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Desktop);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.GradientActiveCaption);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.GradientInactiveCaption);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Highlight);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.HighlightText);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.HotTrack);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.InactiveBorder);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.InactiveCaption);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.InactiveCaptionText);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Info);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.InfoText);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Menu);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MenuBar);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MenuHighlight);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MenuText);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ScrollBar);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Window);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.WindowFrame);
			yield return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.WindowText);
		}

		// TODO: might be nice to add a test that this produces the same as
		// Enum.GetValues(typeof(System.ConsoleColor)).Cast<object>().ToList().OrderBy(o => o.ToString()).Select(o => o.ToString())
		// In theory hardcoding this list save work at runtime.
		public static IEnumerable<string> ConsoleColorsNamesAlphabetical()
		{
			yield return nameof(System.ConsoleColor.Black);
			yield return nameof(System.ConsoleColor.Blue);
			yield return nameof(System.ConsoleColor.Cyan);
			yield return nameof(System.ConsoleColor.DarkBlue);
			yield return nameof(System.ConsoleColor.DarkCyan);
			yield return nameof(System.ConsoleColor.DarkGray);
			yield return nameof(System.ConsoleColor.DarkGreen);
			yield return nameof(System.ConsoleColor.DarkMagenta);
			yield return nameof(System.ConsoleColor.DarkRed);
			yield return nameof(System.ConsoleColor.DarkYellow);
			yield return nameof(System.ConsoleColor.Gray);
			yield return nameof(System.ConsoleColor.Green);
			yield return nameof(System.ConsoleColor.Magenta);
			yield return nameof(System.ConsoleColor.Red);
			yield return nameof(System.ConsoleColor.White);
			yield return nameof(System.ConsoleColor.Yellow);
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
			yield return System.Drawing.Color.Linen;
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
			yield return System.Drawing.Color.YellowGreen;
			yield return System.Drawing.Color.Chartreuse;
			yield return System.Drawing.Color.LawnGreen;
			yield return System.Drawing.Color.PaleGreen;
			yield return System.Drawing.Color.LightGreen;
			yield return System.Drawing.Color.MediumSpringGreen;
			yield return System.Drawing.Color.SpringGreen;
			yield return System.Drawing.Color.Lime;
			yield return System.Drawing.Color.LimeGreen;
			yield return System.Drawing.Color.MediumSeaGreen;
			yield return System.Drawing.Color.ForestGreen;
			yield return System.Drawing.Color.Green;
			yield return System.Drawing.Color.DarkGreen;
			yield return System.Drawing.Color.SeaGreen;
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
			yield return System.Drawing.Color.LightCyan;
			yield return System.Drawing.Color.PowderBlue;
			yield return System.Drawing.Color.SkyBlue;
			yield return System.Drawing.Color.LightSkyBlue;
			yield return System.Drawing.Color.LightSteelBlue;
			yield return System.Drawing.Color.DeepSkyBlue;
			yield return System.Drawing.Color.Aqua;
			yield return System.Drawing.Color.Cyan;
			yield return System.Drawing.Color.DarkTurquoise;
			yield return System.Drawing.Color.DodgerBlue;
			yield return System.Drawing.Color.CornflowerBlue;
			yield return System.Drawing.Color.SteelBlue;
			yield return System.Drawing.Color.RoyalBlue;
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
			yield return System.Drawing.Color.DarkViolet;
			yield return System.Drawing.Color.DarkOrchid;
			yield return System.Drawing.Color.Purple;
			yield return System.Drawing.Color.DarkMagenta;
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
			yield return System.Drawing.Color.Thistle;
			yield return System.Drawing.Color.LightPink;
			yield return System.Drawing.Color.Pink;
			yield return System.Drawing.Color.MistyRose;
			yield return System.Drawing.Color.Lavender;
			yield return System.Drawing.Color.LavenderBlush;
			yield return System.Drawing.Color.AliceBlue;
			yield return System.Drawing.Color.Azure;
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
			yield return System.Drawing.Color.LightSlateGray;
			yield return System.Drawing.Color.SlateGray;
			yield return System.Drawing.Color.DarkSlateGray;
			yield return System.Drawing.Color.Black;
			yield return System.Drawing.Color.Transparent;
		}
	}
}
