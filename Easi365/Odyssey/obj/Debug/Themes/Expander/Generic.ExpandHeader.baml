   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089(   "Odyssey.Controls.OdcExpanderHeader  �� 
TargetType �� Property �� Value �� 
TargetType    CornerRadius �� CornerRadius � ColumnDefinitions �� Orientation �� BitmapScalingMode	 �� EdgeMode
    Image ��Name �Name J� SnapsToDevicePixels F�Name    CollapseGeometry �� Stretch �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value �� RelativeSource �� Path �� Angle�� 5
      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     64   %�  '         �� 5      "    6   .�5   
   ��  �@  CollapseGeometry= �� �  � � 	=  	= ��pr  � � �  �D � 	=  z ��pr  �D 	6-   �� 5   
    �@  CornerRadius$ 8,8,0,0��6)   �� 5   
    � �� �	#80FFFFFF6(   �� 5   
    � ��$ 1q�6,   �� 5   
    � ��
 �   �6'   �� 5   
    � ��$ 24��6#   �� 5   
    � �� �	#608080806'   �� 5   
    � �� 5      �� 5      "   6"   .R�5      �� #��z��6   #��z��6G   # z 5      #��z��5      ._�5      �  5      �� 5   "   $��* �63   �� 5   "   $��20 �63   .+�5      �� $��6,0,6,0q�6)   $ 
Horizontal=�6:   $��Center=�6S   .+�5    "   �� $ NearestNeighbor=�6(   $	 Aliased=�5!   )   #��z
 6J   $��Center=�5"   )   $
��Fill=�6D   $��16��6S   $��16��6a   �� 5#   "   $ content��6g   $��0��63   #��z��6C   $��Center=�5$   )   $
��Bold"�6D   #��z��5%   )   $��4,0,0,0q�6]   5&      � 5'      $ circleButton��6D   $��1��6#   $��0,0,4,0q�63   $��Center=�6Z   .+�5(   "   A� $��circle��6*   $
��16.5��68   $
��16.5��6E   ���	#30000000$	��1.5��5)   )    .  $y�Visible=�6[   ���	#e0FFFFFF5+   #   F� 5,   "   $
 path��6'    .  $	��1.1��6Q   #��z 5-   )   #��z��6S   $��Center=�5.   )   $��Center=�6D   $
 None=�6a   5/      50      51       52      P� 53       �@  ShowEllipse .  . �54      �� $ circle��6M    � y�$ 	Collapsed=�6;   6M   55      ]� 56       57      �� 58   "    �@d�	IsChecked$
 TrueY�6A   �� 59   "    �@  HasExpandGeometry . 6I   ./�5;      �� $
 path��6%    5<   %   ��  �� *TemplatedParent+$ ExpandGeometry� � ��5=      ]� 5>       5?      �� 5@   "    �@d�	IsChecked$
 TrueY�6A   �� 5A   "    �@  HasExpandGeometry .  6I   ./�5C      �� $
 path��6%    �@�LayoutTransform 5D   "   �� 5E   &   $	 180Q�66   5G      5H      5J      5L      5M      5N      