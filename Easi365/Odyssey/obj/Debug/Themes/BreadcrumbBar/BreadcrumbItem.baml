   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source �� 
TargetType%    Odyssey.Controls.BreadcrumbItem   BreadcrumbItemTemplate'   !Odyssey.Controls.BreadcrumbButton  Name    IsRoot   IsImageVisible    IsButtonVisible   IsButtonVisible    OverflowItemTemplate#	    OverflowItemTemplateSelector
    Image   Image    Header � DataContext   IsDropDownPressed �� RelativeSource �� Mode   SelectedItem ��Name    SelectedBreadcrumb �� Triggers P� Property P� Value �� 
TargetName �� Property �� Value �� 
TargetType  borderBrush�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     64     5      �� 5   
   $P Jpack://application:,,,/Odyssey;Component/Themes/BreadcrumbBar/Brushes.xaml?�6   %f  &	        '   �    7  7   �� 5      "   6   .R�5   
   �� 
������ |�.  #��z��6<   #��z��5	      ._�5
      ]� .+�5        $ PART_Header��6'   # z 5      # z 6C   #��z 5      
���   �#��z	 5      # z
 5      #��z 69    5   <   ��  5      �� *IsDropDownPressed+ �� *TemplatedParent+$ TwoWay=� 5      �� *SelectedItem+ �� *TemplatedParent+$ TwoWay=���5      �� *Items+ �� *TemplatedParent+5      �� 5      $ PART_Selected��6!   |�.  #��z 6J   5      5       5   
   P� 5       �@  IsRoot . . �5      �� $ PART_Header��6J    �@ EnableVisualButtonStyle .  5      5      �� 5       "   6   .�5!   
   ��  � |� .  6&   �� 5"   
    � ��8   6(   �� 5#   
    � ��8  6%   5$      5&      