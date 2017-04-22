using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Easi365UI.Extention
{
   public static class ListBoxSelectionExt
    {
       public static List<T> GetItemAt<T>(this ListBox listbox, Rect areaOfInterest)
       {
           var list = new List<T>();
           var rect = new RectangleGeometry(areaOfInterest);
           var hitTestParams = new GeometryHitTestParameters(rect);
           var resultCallback = new HitTestResultCallback(x => HitTestResultBehavior.Continue);
           var filterCallback = new HitTestFilterCallback(x =>
           {
               if (x is ListBoxItem)
               {
                   var item = (T)((ListBoxItem)x).Content;
                   list.Add(item);
               }
               return HitTestFilterBehavior.Continue;
           });

           VisualTreeHelper.HitTest(listbox, filterCallback, resultCallback, hitTestParams);
           return list;
       } 
    }
}
