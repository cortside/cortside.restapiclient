namespace Cortside.RestApiClient {
    using System;

    public static class StringExtensions {
        /// <summary>
        /// Returns the left part of this string instance.
        /// </summary>
        /// <param name="input">The string being operated on</param>
        /// <param name="count">Number of characters to return.</param>
        public static string Left(this string input, int count) {
            if (input == null) {
                return input;
            }

            return input.Substring(0, Math.Min(input.Length, count));
        }

        /// <summary>
        /// Returns the right part of the string instance.
        /// </summary>
        /// <param name="input">The string being operated on</param>
        /// <param name="count">Number of characters to return.</param>
        public static string Right(this string input, int count) {
            if (input == null) {
                return input;
            }

            return input.Substring(Math.Max(input.Length - count, 0), Math.Min(count, input.Length));
        }
    }
}
