using System;
using System.Text.RegularExpressions;

namespace DiagramConstructor.utills
{
    class CodeUtils
    {

        /// <summary>
        /// Replace fist ocurence in some string of some string 
        /// </summary>
        /// <param name="text">text to search ocurence</param>
        /// <param name="textToReplace">text for replace</param>
        /// <param name="replace">text to replace </param>
        /// <returns>modifyed text</returns>
        public static String replaceFirst(String text, String textToReplace, String replace)
        {
            Regex regex = new Regex(Regex.Escape(textToReplace));
            text = regex.Replace(text, replace, 1);
            return text;
        }

    }
}
