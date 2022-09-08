using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FastFolderOpener
{
    internal class Utils
    {
        public static void OpenWithNotepad(string filePath)
        {
            using Process process = new Process();
            process.StartInfo.FileName = "notepad.exe";
            process.StartInfo.Arguments = "\"" + filePath + "\"";
            try
            {
                process.Start();
            }
            catch
            {
                MessageBox.Show(Constants.COULD_NOT_OPEN_FILE_MESSAGE,
                    Constants.APPLICATION_TITLE);
            }
        }
    }
}
