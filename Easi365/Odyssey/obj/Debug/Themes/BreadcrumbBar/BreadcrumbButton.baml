   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source'    !Odyssey.Controls.BreadcrumbButton �� 
TargetType �� Property �� Value   mouseOverBtnBrush �� 
TargetType � ColumnDefinitions!   Odyssey.Controls.AeroChrome  Name    	IsPressed	   RenderPressed
 J� IsMouseOver   RenderMouseOver �� Orientation ��Name �� BitmapScalingMode �� EdgeMode �� RelativeSource    IsDropDownPressed    DropDownContentTemplate F�Name  
RightArrow � ContextMenu ��Name �� IsTextSearchEnabled �� HorizontalOffset �� IsOpen �� Mode $� Orientation $� CanVerticallyScroll $� CanHorizontallyScrollO Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5<   1   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5=   1   5  /Microsoft.Windows.Themes.SystemDropShadowChrome   Color �� RadiusX �� RadiusY  J� SnapsToDevicePixels-  'System.Windows.Input.KeyboardNavigation!   DirectionalNavigation" �� Triggers# P� Property$ �� 
TargetName% P� Value& ]� 
Conditions' �� Value( �� Property  OverflowArrow) �� 
SourceName  GradientBorderBrush  	DownArrow�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     64     5      �� 5   
   $P Jpack://application:,,,/Odyssey;Component/Themes/BreadcrumbBar/Brushes.xaml?�6   %�  '         7   7  7  7  7  �� 5      "   6   .�5	   
   ��  � ��$ 1q�6,   �� 5
   
    � ��
 ���� 6(   �� 5   
    � ��8   6'   �� 5   
    �@�FocusVisualStyle 6-   Z� �� 5   
    � �� 5      �� 5      "   6"   .R�5      � #��z��6    5      �� 5      $
��Auto �6/   �� 5      $
��Auto �6/   .+�5        $ button��6N   $��0��6)   $y�Visible=�69   |�.  $��-1q�5   !   $��1q�6-   #	 z 6A   # z
 5   !   ��5      �� 5   "   $ 
Horizontal=�6-   .+�5   &   �� $ image��6,   $ NearestNeighbor=�5   -   $ Aliased=�5   -   }�. $��16��6b   $��16��6m   $
��Fill=�5   -   ��5   -   �� *Image+ �� *TemplatedParent+5   -   �� 5    &   #��z��67   #��z��5!   -   $��2,0,2,0q�6`   5"   #   5$        5'      $ PART_DropDown��69   $��1��6)   |�.  $��16��6b   $��-1q�5(   !   $��1q�6-   #	 z 6A   # z
 5)   !   #��z 5*   !   ��5+      F� 5,   "   $ dropDownPath��6'   
���   �8�� $��Center=�5-   )    5/      �� 50   "   $ 	PART_Menu��6.    . $	 -14��6\   ��51   )   Z�  52   )   �� *IsDropDownPressed+ �� *TemplatedParent+$ TwoWay=���53   &   �� 54   *   .R�55   .   $� $ Vertical=�6E    .  .  $	��300��56   S   57   +   ��59   &   �� 5:   *   D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5;   1   ,s&clr-namespace:System;assembly=mscorlib  5<   1   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5=   1   " ��5:   :   .R�5>   .     $
��Shdw��6[   $ 	#00FFFFFF��6I   $	��400��6g   ._�5?   2   �� #��z��69   #��z��5@   9   #��z��5A   9   ._�5B   6   � .+�5C   :   �� $ 2��6D   $ 2��6P   ���	#FFF1F1F1$��22��6m   $��2,2,2,2q�5D   A   $
��Left=�6R   �� 5E   :   ���	#FFE2E3E3$��1��6U   $��25,2,0,2q�6_   $
��Left=�5F   A   �� 5G   :   ���	#FFFFFFFF$��1��6U   $��26,2,0,2q�6_   $
��Left=�5H   A   �� 5I   :   $��1,0,1,0q�6G   $��2��6X   $
��Auto=�5J   A   |�.  .��5K   >   �� #��z��6M   #  z  5L   E   $! Cycle=�5M   E   5N   ;   5O   7   5P   3   5Q   /   " 5R   .   P� 5S   2   # �@��HasDropShadow. �5T   6   �� $
$ Shdw��6`    � �� 5U   :   u�5V   >   0,0,5,5q�6Q   5X   7   �� 5Y   6   $
$ Shdw��5Z   =    �@ Color 5[   :   ��5\   >   	#71000000��6O   5^   7   % 5_   6   ��5`   :   True��6J   5b   3   5d   +   5f   #   5h      5i      " 5j      P� 5k      # �@  IsImageVisible% .  . �5l      �� $$ image��6M    � y�$ 	Collapsed=�6;   6M   5m      P� 5n      % 63   Z� # �@  Image. �5o      �� $$ image��6M    � y�$ 	Collapsed=�6;   6M   5p      P� 5q      # �@  EnableVisualButtonStyle% .  . �5r      �� $$ button��6%    �@ MouseOverBackground
 ���� 6X   �� 5s      $$ button��6%    �@ MousePressedBackground
 ���� 6[   �� 5t      $$ button��6%    � ��
 ���� 6P   5u      ]� 5v      & 5w      �� 5x   "   ' 6>   Z� ( � ���� 5y   "   ( �@  IsImageVisible' .  6F   �� 5z   "   ' 6=   Z� ( �@  Image./�5|      �� $$ button��6M    � y�$ 	Collapsed=�6;   6M   5}      P� 5~      # �@��HasItems% .  . �5      �� $$ PART_DropDown��6M    � y�$ 	Collapsed=�6;   6M   5�      P� 5�      # �@  Mode$% Overflow=�62   . �5�      �� 8  $$ dropDownPath��6\    � ���� 5�      $$ dropDownPath��6I    � ��
 ���� �� 5�      $$ dropDownPath��6E    � ��
 �   �5�      P� 5�      # �@  	IsPressed% . . �5�      �� $$ PART_DropDown��6K    �@ RenderPressed . 5�      P� 5�      # �@J�IsKeyboardFocused% . . �5�      �� $$ PART_DropDown��6M    �@ RenderMouseOver . �� 5�      $$ button��6M    �@ RenderMouseOver . 5�      P� 5�      # �@  IsDropDownVisible% .  . �5�      �� $$ PART_DropDown��6%    � y�$ 	Collapsed=�6V   5�      ]� 5�      & 5�      �� 5�   "   $) PART_DropDown��6P   ( �@J�IsMouseOver' . �� 5�   "   ( �@  EnableVisualButtonStyle' . 6O   ./�5�      �� 8  $$ button��5�   %    �@ MouseOverBackground5�      P� 5�      # �@  IsButtonVisible% .  . �5�      �� $$ button��6M    � y�$ 	Collapsed=�6;   6M   5�      ]� 5�      & 5�      �� 5�   "   ( �@  IsDropDownPressed' . 6I   �� 5�   "   ( �@  Mode$' 
Breadcrumb=�6<   ./�5�      �� 8  $$ dropDownPath��6X    � ���� 5�      $$ button��6K    �@ RenderPressed . 5�      5�      5�      5�      5�      