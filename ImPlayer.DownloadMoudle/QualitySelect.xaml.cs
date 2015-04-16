using ImPlayer.DownloadMoudle.BaiduMusic;
using MyDownloader.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImPlayer.DownloadMoudle
{
    /// <summary>
    /// QualitySelect.xaml 的交互逻辑
    /// </summary>
    public partial class QualitySelect : Window
    {


        List<BaiduMusic.Bitrate> Bitrate = null;
        BaiduMusic.SongInfo SongInfo = null;

        public QualitySelect(SearchSongResultById dsr)
        {
            InitializeComponent();
            Bitrate = dsr.Bitrate;
            if (Bitrate == null) { return; }
            Bitrate.RemoveAll(b => b.file_bitrate == 0);
            Bitrate.Sort();
            if (Bitrate.Count > 4)
            {
                Bitrate = Bitrate.Take(4).ToList();
            }
            SongInfo = dsr.songInfo;
            SongInfo.album_title = "专辑：" + SongInfo.album_title;
            this.DataContext = SongInfo;
        }



        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dd.ItemsSource = Bitrate;
        }
        int lastRow = -1;
        private void dd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadioButton rb = FindCellControl<RadioButton>("showSelected", lastRow);
            if (rb != null) { rb.IsChecked = false; }
            FindCellControl<RadioButton>("showSelected").IsChecked = true;
        }

        #region
        public T FindCellControl<T>(string name) where T : Visual
        {
            DataRowView selectItem = dd.SelectedItem as DataRowView;
            DataGridCell cell = GetCell(dd, dd.SelectedIndex, 0);
            lastRow = dd.SelectedIndex;
            return FindVisualChildByName<T>(cell, name) as T;
        }

        public T FindCellControl<T>(string name, int index) where T : Visual
        {
            //  DataRowView selectItem = dd.SelectedItem as DataRowView;
            if (index == -1) { return null; }
            DataGridCell cell = GetCell(dd, index, 0);

            return FindVisualChildByName<T>(cell, name) as T;
        }

        public T FindVisualChildByName<T>(Visual parent, string name) where T : Visual
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as Visual;
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if (controlName == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }



        public static DataGridCell GetCell(DataGrid datagrid, int rowIndex, int columnIndex)
        {
            DataGridRow rowContainer = GetRow(datagrid, rowIndex);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                if (cell == null)
                {
                    datagrid.ScrollIntoView(rowContainer, datagrid.Columns[columnIndex]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                }
                return cell;
            }
            return null;
        }

        public static DataGridRow GetRow(DataGrid datagrid, int columnIndex)
        {
            if (columnIndex == -1) { return null; }
            DataGridRow row = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            if (row == null)
            {
                datagrid.UpdateLayout();
                datagrid.ScrollIntoView(datagrid.Items[columnIndex]);
                row = (DataGridRow)datagrid.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        #endregion
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb=FindCellControl<RadioButton>("showSelected");
            if (rb == null) { return; }
            string fileLink = rb.Tag.ToString();
            if (fileLink == string.Empty) { return; }
            string file_Ext = "mp3";
            if (fileLink.Length < 5)
            {
                file_Ext = fileLink;
                List<HightQualitySongInfo> li = await BaiduMusicOp.GetLosslessMusicUrl(SongInfo.song_id, fileLink);
                fileLink = li[0].SongLink;
            }
            if (Common.downloadPage == null||!Common.downloadPage.IsLoaded)
            {
                Common.downloadPage = new DownloadPage();
            }
            Downloader download = DownloadManager.Instance.Add(
                      new ResourceLocation { URL = fileLink, Password = SongInfo.album_title },
                      null,
                      MainWindow.DownloadFolder+ SongInfo.author + "-" + SongInfo.title + "." + file_Ext,
                      2,
                      false);
            Console.WriteLine(MainWindow.DownloadFolder  + SongInfo.author + "-" + SongInfo.title + "." + file_Ext);
            Common.downloadPage.Show();
            this.Close();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }
        
    }
}
