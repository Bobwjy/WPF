   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089(   "Odyssey.Controls.OdcExpanderHeader  �� 
TargetType �� Property �� Value �� 
TargetType    CornerRadius �� CornerRadius ��Name � ColumnDefinitions �� Orientation	 �� BitmapScalingMode
 �� EdgeMode    Image ��Name �Name J� SnapsToDevicePixels F�Name �� StrokeEndLineCap �� StrokeStartLineCap    CollapseGeometry �� Stretch �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value �� RelativeSource �� Path �� Angle�� 5
      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     64   %T	  '         �� 5      "    6   .�5   
   ��  �@  CollapseGeometry" ��`� `� �@KL @KL �@T� `� 	6-   �� 5   
    �@  CornerRadius$ 8,8,0,0��6)   �� 5   
    � �� �	#80FFFFFF6(   �� 5   
    � ��$ 1q�6,   �� 5   
    � ��
 �   �6'   �� 5   
    � ��$ 24��6#   �� 5   
    � �� �	#608080806'   �� 5   
    � �� 5      �� 5      "   6"   .R�5      �� #��z��6   #��z��6G   # z 5      #��z��5      ._�5      �� $ innerBorder��6:   
������ $��1q�6O   
������ ._�5      �  5   "   �� 5   &   $��* �67   �� 5   &   $��20 �67   .+�5!   "   �� $��6,0,0,0q�6-   $ 
Horizontal=�6>   $��Center=�6W   .+�5"   &   �� $	 NearestNeighbor=�6,   $
 Aliased=�5#   -   #��z 6N   }�. $
��Fill=�5$   A   $��16��6P   $��16��6^   �� 5%   &   $ content��5&   -   $��0��5%   7   #��z��6G   $
��Bold"�5&   >   #��z��5'   -   $��4,0,0,0q�6a   5(   #   � 5)   "   $ circleButton��6H   $��1��6'   $��0,0,0,0q�67   $��Center=�6^   .+�5*   &   A� $��circle��6.   $
��16.5��6<   $
��16.5��6I   ���	#30000000$	��1.5��5+   -    .  $y�Visible=�6_   ���	#e0FFFFFF5-   '   F� 5.   &   $ path2��6+   $��	0.5,1,0,0q�6:    .  $	��3.5��5/   -   $ Round=�6C   $ Round=�6\   #��z 50   -   
�������$��Center=�51   -   $��Center=�6H   $
 None=�6e   F� 52   &   $
 path��6+   $��-1,0,0,0q�69    .  #��z 53   -   #��z��54   -   $��Center=�6S   $��Center=�55   -   $
 None=�6J   56   #   57      58      59       5:      P� 5;       �@J�IsMouseOver . . �5<      �� $ innerBorder��6%    � ��
 �����6U   5=      P� 5>       �@  ShowEllipse .  . �5?      �� $ circle��6M    � y�$ 	Collapsed=�6;   6M   5@      ]� 5A       5B      �� 5C   "    �@d�	IsChecked$
 TrueY�6A   �� 5D   "    �@  HasExpandGeometry . 6I   ./�5F      �� $
 path��6%    5G   %   ��  �� *TemplatedParent+$ ExpandGeometry� � ��5H      ]� 5I       5J      �� 5K   "    �@d�	IsChecked$
 TrueY�6A   �� 5L   "    �@  HasExpandGeometry .  6I   ./�5N      �� $ circleButton��6%    �@�LayoutTransform 5O   "   �� 5P   &   $	 180Q�66   5R      5S      5U      5W      5X      5Y      