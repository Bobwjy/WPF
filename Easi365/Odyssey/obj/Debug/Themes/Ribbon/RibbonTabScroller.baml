   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source#    Odyssey.Controls.RibbonChrome �� 
TargetType �� Property �� Value   	odc:Skins  BorderBrush  RibbonTabBrush $  RibbonTabScrollerControlTemplate �� 
TargetType(  "Odyssey.Controls.RibbonTabScroller   Name   RibbonTabScrollerChromeStyle �Name �� Color	 �� RelativeSource
 J� OpacityMask �� 
StartPoint �� EndPoint � Color � Offset#   Odyssey.Controls.RibbonButton  Name �� 	ClickMode  InternalRibbonButton   ScrollLeftCommand   CornerRadius  LeftArrowImage   ScrollRightCommand  RightArrowImage ��Name �� Triggers P� Property P� Value �� 
TargetName�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     5      �� 5   
   $V Ppack://application:,,,/Odyssey;Component/Themes/Ribbon/InternalRibbonButton.xaml?�6   �� 5   
   $\ Vpack://application:,,,/Odyssey;Component/Themes/Ribbon/DefaultRibbonButtonBrushes.xaml?�5      %v	  (��       5   3   *	odc:SkinsRibbonTabScrollerChromeStyle+)&	 i    7  7  '  �    7  �� 6   "   6   .�5   
   ��  �@  
RenderFlat .  6'   �� 5   
    �@  CornerRadius$ 5��6)   �� 5   
    �@  MouseOverBorderBrush 61   C� *�� *3  3 ++�� 5   
    � �� 6(   C� *�� *3  3 ++�� 5   
    � �� 6'   C� *�� *3  3 ++�� 5   
    �@  InnerBorderThickness$ 0q�61   5      �� 5      "  6?   .R�5   
      $ chrome��6   |�.  $��Stretch=�5      ��5      C� *�� *3  3 ++.��5      � |�.  .+�5      � $ ColorBackground��6   $��-1q�60   ��5      �� 5       6*   �� *Color+	 �� *TemplatedParent+
 5      �� 5       $	 0,0&�6.   $	 0,1&�6?   .��5!      � $ 	#80FFFFFF��6+   $ 0Q�6=   � 5"      $ 	#15FFFFFF��6+   $
 0.05Q�6=   � 5#      $ 	#00FFFFFF��6+   $ 1Q�6=   5$      5&      ]� 5'      |�.  .+�5(        $ 
LeftButton��6'   $ Press=�6;   |�.  8��  $
��Left=�5)   N   #��Z 5*      $��12��6Z   $ 5,0,0,5��5+      $��
-2,-2,0,-2q�64   $y�	Collapsed=�6H   .��5,      �� $��2,0,0,0q�6    |�.  $
��None=�5.   3   $��Center=�6B   $��Center=�6]   ��5-   !   C� *�� *3  3 ++5.   ]   5/        50      $ RightButton��6'   $ Press=�6<   |�.  8�� $��Right=�51   N   #��Z 52      $��12��6[   $ 0,5,5,0��53      $��
0,-2,-2,-2q�64   $y�	Collapsed=�6H   .��54      �� $��2,0,0,0q�6    |�.  $
��None=�56   3   $��Center=�6B   $��Center=�6]   ��55   !   C� *�� *3  3 ++56   ]   57      �� 58      $ PART_ScrollViewer��6#   |�.  #��z��6P   $��Disabled=�59      $��Hidden=�6D   ��.  5:      5;      5<      5=       5>   
   P� 5?       �@ IsColorized . . �5@      �� $ chrome��6=    � ��
 �����5A      P� 5B       �@ IsColorized .  . �5C      �� $ ColorBackground��6A    � y�$ 	Collapsed=�6/   6A   5D      P� 5E       �@ 	Alignment$
 Left=�6+   . �5F      �� $ RightButton��6?    � y�$ Visible=�6/   6?   5G      P� 5H       �@ 	Alignment$ Right=�6+   . �5I      �� $ 
LeftButton��6?    � y�$ Visible=�6/   6?   5J      5L      �� 5N      "  6   .�5O   
   ��  � ��8   6%   �� 5P   
    � |� .  6&   5Q      5S      