   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   W   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5   	   1clr-namespace:Odyssey.EffectsOdyssey.Effects  6`   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source �� 
TargetType,    &Odyssey.Controls.RibbonApplicationMenu (  $RibbonApplicationMenuControlTemplate+   %Odyssey.Controls.RibbonDropDownButton  Name   IsFlat   PopupAnimation J� SnapsToDevicePixels    Odyssey.Controls.RibbonBar   Size   CornerRadius	   	IsChecked
 �� RelativeSource �� Mode   IsDropDownPressed  	odc:Skins ! ApplicationMenuButtonTemplate  RibbonMenuButtonBrush   Content ��Name    MenuButtonImage �� EdgeMode �� Path �� BitmapScalingMode   DropDownHeader �� Padding �� CornerRadius  RibbonBarControlBrush  BorderBrush � ColumnDefinitions ��Name    
MenuHeader    MenuHeaderTemplate    RecentItemsList    RecentItemsListTemplate    Footer    FooterTemplate ��Name �� Left
  �� Top! J� IsHitTestVisible" j� Stretch# !� Visual$ �� ElementName% �� Triggers& ]� 
Conditions' �� Property( �� Value#   Odyssey.Controls.RibbonChrome) �� 
TargetName* �� Property+ �� Value%   Odyssey.Effects.GrayscaleEffect, P� Property- P� Value. � 	Resources/ �� 
TargetType  RibbonChromeStyle0 �� BasedOn   Odyssey.Controls.KeyTip1 �� TextAlignment  RibbonBarForgroundBrush�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  6W   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5   	   $fxclr-namespace:Odyssey.Effects  6`   :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     5      �� 5   
   $V Ppack://application:,,,/Odyssey;Component/Themes/Ribbon/InternalRibbonButton.xaml?�6   �� 5   
   $\ Vpack://application:,,,/Odyssey;Component/Themes/Ribbon/DefaultRibbonButtonBrushes.xaml?�6   �� 5   
   $S Mpack://application:,,,/Odyssey;Component/Themes/Ribbon/RibbonAppMenuItem.xaml?�6   �� 5   
   $Y Spack://application:,,,/Odyssey;Component/Themes/Ribbon/RibbonApplicationButton.xaml?�6   �� 5	   
   $[ Upack://application:,,,/Odyssey;Component/Themes/Ribbon/RibbonGroupDropDownButton.xaml?�6   %�  &	        7  '   �	    7   �� 5      "   6   .R�5   
   � $��0q�6   .+�5        $ PART_AppButton��6'    .  $��37��6N   $��37��6Y   $
 Fade=�6e   #|�z|�6{    . 
�������$ Medium=�5      $ 0,5,0,0��61   	 5   0   �� *IsOpen+
 �� *TemplatedParent+$ OneWay=� 5      �� *IsOpen+
 �� *TemplatedParent+$ TwoWay=���5      C� *�� *3 3 ++��5      C� *�� *3 3 ++ 5      �� 5      $ image��6^   $��24��6   $��24��6'   #��z 63   $ Unspecified=�5      ��5      �� $% (odc:RibbonButton.ImageStretch)�
 �� *TemplatedParent+ 5      �� $4 .(odc:ImageRenderOptions.SmallImageScalingMode)�
 �� *TemplatedParent+5       5      � 5      |�.  .+�5      �� |�.  $ 5,18,5,5q�5   !   $��1q�64   $ 0,4,4,4��6H   ��5   !   C� *�� *3 3 ++��5   !   C� *�� *3 3 ++._�5      �� |�.  .+�5    "   �� 
�������$��1q�6=   |�.  ._�5!   &   �� #��z��6-   #��z��6W   #��z��5"   -   ._�5#   *   � $��	-1,0,-1,0q�6/    5$   .   �� 5%   2   $
��Auto �6C   �� 5&   2   $��* �6C   .+�5)   .   �� $��0,0,1,0q�65   
�������$��0��6b   $��0��6o   ._�5*   2   �� 5+   /   �� 5,   .   $ PART_RecentItemsList��6V   $��1��69   $��0��6I   .+�5-   2   �� $��Stretch=�6A   #��z 6_   #��z 5.   9   �� 5/   2   #��z 6A   #��z 50   9   $��Stretch=�51   9   $��Stretch=�6U   �� 52   2   $��Stretch=�6A   #��z 6_   #��z 53   9   54   /   56   +   57   '   58   #   �� 59   "   #��z 61   #��z 6T   5:      5;      �� 5?      .+�5@      �� $ PART_AppButtonClone��5A   %   $ 0��5@   (   $  0��68   ! .  $��37��6`   $��37��6l   $	��Top=�5A   B   $
��Left=�6Z    . ��5C   "   !� 5D   &   $
" None=�6`   $ HighQuality=�5E   -   # 5D   2   �� $$ PART_AppButton��5E   -   5G      5H      5J      5L      5M      % 5N   
   ]� 5O      & 5P      �� 5Q      ' � {�( .  65   �� 5R      ' �@ IsGrayScaleEnabled( . 6O   ./�5T      �� $) image��6O   * �@��EdgeMode$+ Unspecified=�6;   6O   �� 5U      $) image��6Q   * �@��BitmapScalingMode$
+ Fant=�6D   6Q   �� 5V      $) image��6+   * �@J�Effect+ 5W        5X      5Z      5[      P� 5\      , �@  IsOpen- .  . �5]      �� $) PART_AppButton��6D   * �@ IsDropDownPressed+ .  5^      . 5`   
   ���� 5a      '        "/  6   80   6;   5b      5d      �� 5f      "/   6   .�5g   
   �� * � ��$+ 48��6"   �� 5h   
   * � ��+ .  6&   �� 5i   
   * �@ XOffset$+ 20Q�6/   �� 5j   
   * �@ YOffset$+ 8Q�6/   �� 5k   
   * � ��
+ �����6'   �� 5l   
   * � ��
+ �����6(   �� 5m   
   * � ��$+ 1q�6,   �� 5n   
   * � ��8+   6%   �� 5o   
   * �@  MenuHeaderTemplate+ 5p      �� 5q      .R�5r      �� $��0,0,0,1q�6   
�������$��1q�6J   $	��320��6U   $	��Top=�5s   t   $��Stretch=�5t      ��5s      C� *�� *3 3 ++._�5u      �� $��Recent Documents��6$   $
��Bold"�6<   $1 Center=�6N   $��Center=�5w   !   $��Center=�6>   ��5v   !   C� *�� *3 3 ++5w   >   5x      5y      5{      5|      5~      