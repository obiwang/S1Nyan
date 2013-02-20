using System.Globalization;
// -----------------------------------------------------------------------
// Copyright (c) David Kean. All rights reserved.
// -----------------------------------------------------------------------
using System.IO;
using System.Net.Entities;

namespace System.Net
{
    /// <summary>
    ///     Provides methods for encoding and decoding HTML when processing Web requests.
    /// </summary>
    public static class WebUtility
    {
        /// <summary>
        ///     Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="value">
        ///     The string to encode.
        /// </param>
        /// <returns>
        ///     An encoded string.
        /// </returns>
        public static string HtmlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                HtmlEncode(value, writer);

                return writer.ToString();
            }
        }

        /// <summary>
        ///     Converts a string into an HTML-encoded string, and writes the output to a <see cref="TextWriter"/> object.
        /// </summary>
        /// <param name="value">
        ///     The string to encode.
        /// </param>
        /// <param name="output">
        ///     A <see cref="TextWriter"/> output stream.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is not <see langword="null"/> and <paramref name="output"/> is <see langword="null"/>.
        /// </exception>
        public static void HtmlEncode(string value, TextWriter output)
        {
            // Very weird behavior that we don't throw on a null value, but 
            // do on empty, however, this mimics the platform implementation
            if (value == null)
                return;

            if (output == null)
                throw new ArgumentNullException("output");

            HtmlEncodingServices.Encode(value, output);
        }

        /// <summary>
        ///     Converts a string that has been HTML-encoded into a decoded string.
        /// </summary>
        /// <param name="value">
        ///     The string to decode.
        /// </param>
        /// <returns>
        ///     The decoded string.
        /// </returns>
        public static string HtmlDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                HtmlDecode(value, writer);

                return writer.ToString();
            }
        }

        /// <summary>
        ///     Converts a string that has been HTML-encoded into a decoded string, and writes the output to a <see cref="TextWriter"/> object.
        /// </summary>
        /// <param name="value">
        ///     The string to decode.
        /// </param>
        /// <param name="output">
        ///     A <see cref="TextWriter"/> output stream.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is not <see langword="null"/> and <paramref name="output"/> is <see langword="null"/>.
        /// </exception>
        public static void HtmlDecode(string value, TextWriter output)
        {
            // Very weird behavior that we don't throw on a null value, but 
            // do on empty, however, this mimics the platform implementation
            if (value == null)
                return;

            if (output == null)
                throw new ArgumentNullException("output");

            HtmlEncodingServices.Decode(value, output);
        }
    }
}
