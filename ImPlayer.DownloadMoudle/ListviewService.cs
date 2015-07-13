using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImPlayer.DownloadMoudle
{
    public class ListviewService
    {
        #region AutoSelect Property

        public static readonly DependencyProperty AutoSelectProperty = DependencyProperty.RegisterAttached("AutoSelect", typeof(bool), typeof(ListviewService), new PropertyMetadata(OnAutoSelectPropertyChanged));

        public static bool GetAutoSelect(DependencyObject element)
        {
            if (element == null)
                return false;

            return (bool)element.GetValue(AutoSelectProperty);
        }

        public static void SetAutoSelect(DependencyObject element, bool value)
        {
            if (element == null)
                return;

            element.SetValue(AutoSelectProperty, value);
        }

        #endregion

        private static void OnAutoSelectPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (!(element is UIElement))
                return;

            if ((bool)e.NewValue)
                (element as UIElement).GotFocus += new RoutedEventHandler(OnElementGotFocus);
            else
                (element as UIElement).GotFocus -= new RoutedEventHandler(OnElementGotFocus);
        }

        private static void OnElementGotFocus(object sender, RoutedEventArgs e)
        {
          //  Debug.Assert(e.OriginalSource is DependencyObject);
            ListViewItem item = (e.OriginalSource as DependencyObject).FindAncestor<ListViewItem>();
            if (item != null)
                item.IsSelected = true;
          //  else
              //  Debug.WriteLine(string.Format("Cannot find ListViewItem from {0}", sender));
        }
    }

    public static class VisualExtension
    {
        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            while (dependencyObject != null && !(dependencyObject is T))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            return dependencyObject as T;
        }
    }
}
