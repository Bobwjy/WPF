   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   W   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5   	   1clr-namespace:Odyssey.EffectsOdyssey.Effects  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source �� 
TargetType%    Odyssey.Controls.RibbonMenuItem   RibbonMenuItemTemplate#   Odyssey.Controls.RibbonChrome  Name   CornerRadius m� IsHighlighted   RenderMouseOver   MouseOverBackground  	odc:Skins  HighlightedDropDownBg d�Name	 �� 	ClickMode
 d� 	IsChecked �� RelativeSource �� Mode   RenderPressed J� IsMouseOver �� BitmapScalingMode �� EdgeMode  RightArrowImage �� ElementName � ColumnDefinitions �� CornerRadius   HighlightedRibbonBorderBrush  HighlightedPressedBtnBrush    Image J� SnapsToDevicePixels �� Path ��Name �Name � AllowsTransparency � 	StaysOpen5  /Microsoft.Windows.Themes.SystemDropShadowChrome   Color   CornerRadius �� Padding  PopupContainerBgBrush  RibbonBorderBrush �� Triggers P� 
SourceName  P� Property! P� Value" �� 
TargetName# �� Property$ �� Value% ]� 
Conditions& �� Property' �� Value  DisabledImageOpacity%   Odyssey.Effects.GrayscaleEffect 	 DisabledForegroundBrush( � 	Resources) �� 
TargetType 
 RibbonChromeStyle* �� BasedOn�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  6W   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5   	   $fxclr-namespace:Odyssey.Effects  5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     5      �� 5   
   $V Ppack://application:,,,/Odyssey;Component/Themes/Ribbon/InternalRibbonButton.xaml?�6   �� 5   
   $U Opack://application:,,,/Odyssey;Component/Themes/Ribbon/RibbonHLChromeStyle.xaml?�6   %�  &	        0�� 5      *�� *3 3 ++17 
 '   �    7   �� 5
      "   6   .R�5   
   � .+�5        $ bgrnd��60   $ 3��6   # z 6?   $��Stretch=�5      $��Stretch=�6:    5      2  .��5      ]� $��Stretch=�6   $��-2q�6:   .+�5      d� $ PART_DropDown��6#   $��16��6:   $��Right=�6E   $	 Press=�6\   
 5      �� *IsSubmenuOpen+ �� *TemplatedParent+$ TwoWay=���5      �� 5      " d�6.   .R�5   "     $ 0,3,3,0��63   # z
 6J   # z 5   )   $��Center=�6Y   .��5   &   �� $ NearestNeighbor=�6,   $ Aliased=�6^   $��1,1,0,0q�5   z   $
��None=�5   -   }�. ��5   -   C� *�� *3 3 ++5   <   5   #   5      5        5      $
 item��6'   $ 3,0,0,3��65    6L   �� *IsMouseOver+$
 item��.��5      �  5      �� 5    "   $��24 �63   �� 5!   "   $��* �63   .+�5#      �� $��
GlyphPanel��6%   $��0��67   $��1,1,1,1q�6G   $ 3,3,3,3��6a   $��22��5%   �   $��22��5&   %   $��1,1,1,1q�61   $y�Hidden=�6B   ��5$   %   C� *�� *3 3 ++��5%   %   C� *�� *3 3 ++._�5'   "   F� $��Glyph��6�   =����    ��M Ѡ� �XO �@�3 `Vl � z � рa�     ѠZ2 �ˤ 	���	#80000000$y�Hidden=�6l   $��9��5(   )   $��11��63   $��LeftToRight=�6?   5)      �� 5+      $
��Icon��6/   $��0��6D   $��16��6T   $��16��6_   $��1q�6k   �� 5,      $��Image��6$   $ Aliased=�5.   %   #��z 6F   $��1q�6g   $��16��6r   $��0��6~   $��Center=�5/   %   $��16��6@   }�. # z 50   %    5-   %   �� $4 .(odc:ImageRenderOptions.SmallImageScalingMode)� �� *TemplatedParent+50   %   �� 52      $ content��6/   $��Header��6@   $��1��6W   $��4,2,4,2q�6g   $��Center=�53   %   54      55      56      57      � 58      $ 
PART_Popup��6   $
��Fade=�6(   $��Right=�6>    .  .  ��59      �� *IsSubmenuOpen+ �� *TemplatedParent+$ TwoWay=�.��5:        $
��Shdw��6P   $��0,0,5,5q�6-   $ 	#81000000��6>   $ 2��6\   ._�5;      �� $ 1q�5=      $ 2��6)   $��1q�6:   ��5;      C� *�� *3 3 ++��5<      C� *�� *3 �� *3 3 +++._�5>      �� ��. $
��Auto=�6?   $��Disabled=�6b   .��5?      �� $��Visible=�6-   ��. 6`   5@      5A      5B      5C      5D       5E   
   P� 5F      $ PART_DropDown��68     �@d�	IsChecked$
! TrueY�6+   . �5G      �� $" bgrnd��6A   # �@ RenderMouseOver$ . 5H      P� 5I        �@��HasItems! .  . �5J      �� $" PART_DropDown��6/   # � y�$$ 	Collapsed=�6J   �� 5K      $" content��6<   # � ��$$ 4,2,48,2q�6+   6<   �� 5L      $
" item��6;   # �@ CornerRadius$$ 3��61   6;   5M      ]� 5N      % 5O      �� 5P      & �@m�	IsChecked' . 65   �� 5Q      & �@m�IsCheckable' . 67   �� 5R      & � {�' . 65   ./�5T      �� $" 
GlyphPanel��6?   # � y�$$ Visible=�6/   6?   5U      ]� 5W      % 5X      �� 5Y      & �@m�	IsChecked' . 65   �� 5Z      ' 61   Z� & �@  Image�� 5[      & � {�' . 65   /�5]      �� 5^      $" Glyph��6C   # � y�$$ Visible=�63   6C   5`      ]� 5a      % 5b      �� 5c      & � {�' .  65   �� 5d      & �@ IsGrayScaleEnabled' . 6O   ./�5f      �� $" Image��5g      $ 5f   ,   C� *�� *3 3 ++# �@J�Opacity5g      �� 5h      $" Image��6O   # �@��EdgeMode$$ Unspecified=�6;   6O   �� 5i      $" Image��6Q   # �@��BitmapScalingMode$
$ Fant=�6D   6Q   �� 5j      $" Image��6+   # �@J�Effect$ 5k        5l      5n      5o      P� 5p        � {�! .  . �5q      �� $ 6/   C� *�� *3 3	 ++# � ��5r      ( 5u   
   ���� 5v      '        ")  6   8*  6;   5w      5y      �� 5{      ")   6   .�5|   
   �� # � ��8$   6%   �� 5}   
   # � ��$$ 24��6&   �� 5~   
   # � ��$$ 24��6%   �� 5   
   # � ��$
$ Auto��6"   5�      5�      