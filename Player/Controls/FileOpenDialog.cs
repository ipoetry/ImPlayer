using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class FileOpenDialog
    {
        public static bool isContainSubfolder = false;
        public static string ShowDialog(bool IsFolderPicker=true)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
            commonOpenFileDialog.Title = "Cup Player";
            commonOpenFileDialog.NavigateToShortcut = true;
            commonOpenFileDialog.IsFolderPicker = IsFolderPicker;
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                return commonOpenFileDialog.FileName;
            else
                return string.Empty;
        }
        public static List<string> ShowDialog(string Title, bool IsFolderPicker = true, List<CommonFileDialogFilter> Filters = null)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
            commonOpenFileDialog.Title = Title;
            commonOpenFileDialog.NavigateToShortcut = true;
            commonOpenFileDialog.AddToMostRecentlyUsedList = true;
            if (Filters != null)
            {
                foreach (CommonFileDialogFilter cdf in Filters)
                {
                    commonOpenFileDialog.Filters.Add(cdf);
                }
            }
            CommonFileDialogCheckBox cBox = null;
            if (IsFolderPicker)
            {
                cBox = new CommonFileDialogCheckBox("IsContainsubfolder ", "是否包含子文件夹", false);
                commonOpenFileDialog.IsFolderPicker = IsFolderPicker;
                commonOpenFileDialog.Controls.Add(cBox);
            }
            else
            {
                commonOpenFileDialog.Multiselect = true;
            }

            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (cBox != null) { isContainSubfolder = cBox.IsChecked; }
                return commonOpenFileDialog.FileNames.ToList();
            }
            else
                return null;
        }
    }
}
