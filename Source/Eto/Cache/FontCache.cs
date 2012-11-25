﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
    /// <summary>
    /// An application wide cache of fonts.
    /// </summary>
    public static class FontCache
    {
        private static Dictionary<FontKey, Font> fonts =
            new Dictionary<FontKey, Font>();

        public static Font GetFont(
            string fontFamily,
            float fontSizePixels,
            FontStyle fontStyle,
            Generator generator)
        {
            Font result = null;

            var temp = generator ?? 
                Generator.Current;

            var key = new FontKey()
            {
                Generator = temp,
                FontFamily = fontFamily,
                FontSizePixels = fontSizePixels,
                FontStyle = fontStyle
            };

            if (//Dbg.Assert(
                fonts != null //) 
                && !fonts.TryGetValue(key, out result))
            {
                result =
                    new Font(
                        temp,
                        fontFamily,
                        fontSizePixels / Constants.PointsToPixels, // convert to points
                        (FontStyle)(int)fontStyle);

                fonts[key] = result;
            }

            return result;
        }
    }

    public class FontKey
    {
        public Generator Generator { get; set; }
        public string FontFamily { get; set; }
        public float FontSizePixels { get; set; }
        public FontStyle FontStyle { get; set; }

        public override int GetHashCode()
        {
            return
                (this.Generator != null
                 ? this.Generator.GetHashCode()
                 : 0) ^
                (this.FontFamily != null
                 ? this.FontFamily.GetHashCode()
                 : 0) ^
                this.FontSizePixels.GetHashCode() ^
                this.FontStyle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var o = obj as FontKey;

            var result =
                o != null &&
                // this allows fonts from different generators
                // to be used within a single app
                SafeEquals(this.Generator, o.Generator) &&
                SafeEquals(this.FontFamily, o.FontFamily) &&
                this.FontSizePixels.Equals(o.FontSizePixels) &&
                this.FontStyle.Equals(o.FontStyle);

            return result;
        }

        private static bool SafeEquals(object a, object b)
        {
            bool result =
                a != null
                ? a.Equals(b)
                : b == null;   // two null objects are equal

            return result;
        }

        public override string ToString()
        {
            return string.Format(
                "{0} {1}px {2}",
                FontFamily ?? "",
                FontSizePixels,
                FontStyle);
        }
    }
}
