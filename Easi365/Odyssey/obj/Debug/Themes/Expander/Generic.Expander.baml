   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   animTime"    Odyssey.Controls.OdcExpander  �� 
TargetType �� Property �� Value   Odyssey.Controls.KeyTip �� 
TargetType    HeaderBorderThickness    CornerRadius �� CornerRadius � RowDefinitions(  "Odyssey.Controls.OdcExpanderHeader  Name	    HeaderBackground
    ShowEllipse   ShowEllipse   CornerRadius    Image   Image d� 	IsChecked �� RelativeSource �� Path ��Name K� ZIndex J� Opacity �� ElementName)   #Odyssey.Controls.AnimationDecorator  Name   Duration   
IsExpanded �� Padding � LayoutTransform �� ScaleY �� Triggers P� Property P� Value �� 
TargetName  P� 
SourceName! ]� 
Conditions" �� 
SourceName# �� Property$ �� Value% :� RoutedEvent& �� TargetProperty	' W� To( g� Duration) �� 
TargetName* g� DecelerationRatio+ � 	Resources  	odc:Skins  TreeViewItemTemplate  ListViewItemTemplate�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     64   %  &	        '         7   7   7   E�5      0:0:0.20D�6*   �� 5      "    6   .�5   
   ��  � �� �	#408080806'   �� 5   
    � �� �	#604040406(   �� 5   
    �@  HeaderBackground �	#40A0A0A06-   �� 5   
   ! �@  MouseOverHeaderBackground �	#60FFFFFF66   �� 5   
   ! �@  MouseOverHeaderForeground �	#F000000066   �� 5   
    �@ Stop .  6,   �� 5   
    �@ YOffset$ 2Q�6/   �� 5   
    �@ XOffset$	 -24Q�6/   �� 5   
    �@  HeaderBorderThickness$ 1q�62   �� 5    
    �@  CornerRadius$ 0��6)   �� 5!   
    �@  ShowEllipse . 6(   �� 5"   
    � ��$ 1,0,1,1q�6,   �� 5#   
    � ��$ 8q�6$   �� 5$   
    � �� 5&      �� 5'      "   6"   .R�5(      �� #��z 6   #��z��5)      #��z��6G   # z 5*      ._�5+      �  5,      �� 5-   "   $
��Auto �60   �� 5.   "   $��* �60   .+�50        $ header��64   $��0��6D   #��z��6Q   #��z��52   %   #��z	 53   %   # z
 54   %   # z 55   %   # z 6S    51   %   ��  �� *TemplatedParent+$ 
IsExpanded�55   S   �� 56      $ panel��6%   $��1��64   $ -1��6A   $��0,-1,0,0q�6S   #��z��58   %   #��z��59   %    57   %   �� $ 	decorator��$ AnimationOpacity�._�5:   "     $ 	decorator��69   $	��Top=�6L   8   #��z��5;   N   #��z��5<   )   #��z��6Q    5=   )   ��  �� *TemplatedParent+$ 
IsExpanded�._�5>   &   �� # z��6-   ._�5?   *   �� #��z��6;   #��z��5@   1   5A   '   5B   #   5C      5E       5F      �� 5G      
scaleT��$ 1Q�6=   5I       5K      P� 5L       �@  IsHeaderVisible .  . �5M      �� $ header��6%    � y�$ 	Collapsed=�6O   5N      P� 5O      $  header��6F    �@J�IsMouseOver . . �5P      �� $ header��5R   %    5Q   %   ��  �� *TemplatedParent+$ MouseOverHeaderForeground� � ��5R   %   �� 5S      $ header��5U   %    5T   %   ��  �� *TemplatedParent+$ MouseOverHeaderBackground� � ��5U   %   5V      ]� 5W      ! 5X      �� 5Y   "   $" header��6N   # � ��$ . �� 5Z   "   # �@  HasPressedBackground$ . 6L   ./�5\      �� $ header��5^   %    5]   %   ��  �� *TemplatedParent+$ PressedHeaderBackground� � ��5^   %   5_      :� 5`      $$% odc:OdcExpander.MaximizedEvent��6'   [�5a      �� 5b   "   .��5c   &   �� $& ScaleY�61   .��5d   *   W� $' 1X�6:   8(  $) scaleT��5e   1   $	* 0.8Q�6P   5f   '   5g   #   5i      :� 5j      $$% odc:OdcExpander.MinimizedEvent��6'   [�5k      �� 5l   "   .��5m   &   �� $& ScaleY�61   .��5n   *   W� $' 0X�6:   8(  $) scaleT��5o   1   $	* 0.8Q�6P   5p   '   5q   #   5s      + 5u      ���� 5v      'Q�       "  Q�6    .�5w      ��  � �� 5x   %   C� *�� *3 3 ++5y      �� 5z      '��       "  ��6    .�5{      ��  � �� 5|   %   C� *�� *3 3 ++5}      5~      5      5�      5�      5�      