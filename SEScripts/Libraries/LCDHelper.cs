using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI.Ingame;

namespace mze9412.SEScripts.Libraries
{
    /**Begin copy here**/

    /// <summary>
    /// LCD Helper class
    /// </summary>
    public static class LCDHelper
    {
        static LCDHelper()
        {
            displays = new Dictionary<string, Display>();
        }

        /// <summary>
        /// existing dictionaries
        /// </summary>
        private static readonly Dictionary<string, Display> displays;

        private static bool AutoFlush = false;

        /// <summary>
        /// Spacer char
        /// </summary>
        public static char Spacer = ' ';

        #region Create / delete / print

        /// <summary>
        /// Creates a new display instance for the given LCD
        /// </summary>
        /// <param name="displayId"></param>
        /// <param name="lcd"></param>
        /// <returns></returns>
        public static string CreateDisplay(string displayId, IMyTextPanel lcd)
        {
            if (!displays.ContainsKey(displayId))
            {
                displays.Add(displayId, new Display {Id = displayId, LCD = lcd, Builder = new StringBuilder()});
                return displayId;
            }
            return null;
        }

        /// <summary>
        /// Deletes display with given id
        /// </summary>
        /// <param name="displayId"></param>
        public static void DeleteDisplay(string displayId)
        {
            if (displays.ContainsKey(displayId))
            {
                displays.Remove(displayId);
            }
        }

        /// <summary>
        /// Prints display to given terminal block, then deletes it
        /// </summary>
        /// <param name="displayId"></param>
        /// <param name="append"></param>
        public static void PrintDisplay(string displayId, bool append = false)
        {
            if (displays.ContainsKey(displayId))
            {
                var disp = displays[displayId];
                disp.LCD.WritePublicText(disp.Builder.ToString(), append);
                disp.LCD.ShowTextureOnScreen();
                disp.LCD.ShowPublicTextOnScreen();
            }
        }

        #endregion

        #region Add lines

        public static void WriteLine(string displayId, string line)
        {
            var disp = GetDisplay(displayId);
            if (disp.HasValue)
            {
                disp.Value.Builder.AppendLine(line);
            }

            CheckAutoFlush(displayId);
        }

        public static void WriteHeader(string displayId, string line)
        {
            WriteLine(displayId, "__" + line + "__");
        }

        public static void WriteFormattedLine(string displayId, string format, params object[] args)
        {
            var disp = GetDisplay(displayId);
            if (disp.HasValue)
            {
                disp.Value.Builder.AppendLine(string.Format(format, args));
            }

            CheckAutoFlush(displayId);
        }

        /// <summary>
        /// Adds a progress bar with progress in %
        /// </summary>
        /// <param name="displayId"></param>
        /// <param name="title"></param>
        /// <param name="progress"></param>
        public static void WriteProgressBar(string displayId, string title, double progress)
        {
            var disp = GetDisplay(displayId);
            if (disp.HasValue)
            {
                var builder = disp.Value.Builder;
                builder.Append(title.PadRight(15, Spacer));
                builder.Append("[");

                //we have 10 characters for progress bar
                var usedChars = (int)Math.Round(20*progress, 0);
                var restChars = 20 - usedChars;

                for (int i = 0; i < usedChars; i++)
                {
                    builder.Append("|");
                }
                for (int i = 0; i < restChars; i++)
                {
                    builder.Append(Spacer);
                }

                builder.Append("] ");
                builder.Append(string.Format("{0:0.00}%", progress*100));
                builder.AppendLine();
            }

            CheckAutoFlush(displayId);
        }

        #endregion

        #region Helper

        private static void CheckAutoFlush(string displayId)
        {
            if (AutoFlush)
            {
                PrintDisplay(displayId, false);
            }
        }

        private static Display? GetDisplay(string displayId)
        {
            if (displays.ContainsKey(displayId))
            {
                return displays[displayId];
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Struct to save display list
        /// </summary>
        private struct Display
        {
            public string Id { get; set; }

            public IMyTextPanel LCD { get; set; }

            public StringBuilder Builder { get; set; }
        }
    }
    /**End copy here**/

}
