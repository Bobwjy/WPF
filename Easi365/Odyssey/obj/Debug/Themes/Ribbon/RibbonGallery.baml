   M S B A M L   `   `   `  ����a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5   	   S OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls 5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source �� 
TargetType$   Odyssey.Controls.RibbonGallery   RibbonGalleryTemplate ��Name �� CornerRadius � RowDefinitions � ColumnDefinitions ��Name �Name	 K� IsItemsHost#  Odyssey.Controls.RibbonButton
  Name   IsFlat   CornerRadius    IsDropDownEnabled)  #Odyssey.Controls.ImageRenderOptions   SmallImageScalingMode   Odyssey.Controls.RibbonBar   AffectsDropDown   SmallEdgeMode �� 	ClickMode   ImageStretch    ScrollUpCommand   Size  	odc:Skins  RibbonButtonGroupBgBrush  DefaultBorderBrush   
SmallImage  UpArrowImage    ScrollDownCommand  DownArrowImage+  %Odyssey.Controls.RibbonDropDownButton  Name   IsFlat   CornerRadius   EdgeMode   PopupPlacement   PopupAnimation    
LargeImage   
LargeImage    Title    Content!    Footer"   DropDownFooter#    FooterTemplate$   DropDownFooterTemplate%   IsDropDownPressed& �� RelativeSource' �� Mode(   
SmallImage  ExpandImage)   DropDownHeader*    Header+    HeaderTemplate, ��Name- �� Triggers. P� Property/ P� Value0 �� 
TargetName1 �� Property2 �� Value  MinimizedRibbonPanelBgBrush3 � 	Resources#  Odyssey.Controls.RibbonChrome4 �� 
TargetType  RibbonChromeStyle5 �� BasedOn6 �� Path�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5   	   &odcclr-namespace:Odyssey.Controls  5   	     5      �� 5   
   $\ Vpack://application:,,,/Odyssey;Component/Themes/Ribbon/DefaultRibbonButtonBrushes.xaml?�5      �� 5   
   $U Opack://application:,,,/Odyssey;Component/Themes/Ribbon/RibbonHLChromeStyle.xaml?�6   %�  &	        7  '   P    7   �� 5      "   6   .R�5   
   �� $ border��66   $��1q�6   $ 3��6%   #��z��6F   |�.  #��z��5      ._�5      � |�.   5      �� 5      $��24 �6$   �� 5      $��24 �6$   �� 5      $��24 �6$    5      �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5      �� $ ScrollViewer��5   B   $��3��5      $��1q�60   $��Hidden=�6;   |�.  $��Disabled=�5      $��Stretch=�6X   $
��Left=�5      .��5      � $ PART_WrapPanel��6    	 .  $��Stretch=�6L   5        5      $
 UpButton��5    ^   $��0��5   #   $��1��60    .  $ 0,3,0,0��6O   $��16��6f   |�.  #{�z 6�   $
 Fant=�5       .  $ Unspecified=�5       $ Press=�6L   $
 None=�5!      #��Z 6>   $��	0,-1,-1,0q�5$   g   $ Small=�5%      ��5"      C� *�� *3 3 ++��5#      C� *�� *3 3 ++ 5$      C� *�� *3 3 ++5%        5&      $
 
DownButton��5)   6   $��1��5&   #   $��1��60    .  $ 0��6O   $��	0,-1,-1,0q�6`   |�.  #{�z 6�   $ Unspecified=�5'      $
 Fant=�5(       .  $��16��5)      $ Press=�6$   $
 None=�6J   #��Z 5*      $ Small=�5.      ��5+      C� *�� *3 3 ++��5,      C� *�� *3 3 ++ 5-      C� *�� *3 3 ++5.        5/      $ PART_PopupBtn��52   =   $��2��5/   +   $��1��68    .  $ 0,0,3,0��6W   $��16��6n   |�.  #{�z 6�   $ Unspecified=�50      $ Unspecified=�60   $
 Fant=�51       .  $
��Auto��52      $
 Left=�6'   $
 None=�54      $
 Fade=�6>   # z 58      #  z 6C   #" z! 59      #$ z# 5:      $��
0,-1,-1,-1q�6S   $ Small=�5;      % 53      �� *IsDropDownOpen+& �� *TemplatedParent+$' TwoWay=���55      C� *�� *3 3 ++��56      C� *�� *3 3 ++( 57      C� *�� *3 3 ++) 5<      ]� 5=      $��0q�6$   $��Stretch=�6/   |�.  .+�5>      �� $	��Top=�6-   #��z* 6B   |�.  #��z+ 5?   %   �� 5@      $, PART_ItemsPresenter��6-   $��4q�6J   |�.  6V   5A      5C      5D      5E      - 5F   
   P� 5G      . �@  IsCollapsed/ . . �5H      �� $0 
DownButton��6A   1 � y�$2 	Collapsed=�6/   6A   �� 5I      $0 UpButton��6A   1 � y�$2 	Collapsed=�6/   6A   �� 5J      $0 ScrollViewer��6A   1 � y�$2 	Collapsed=�6/   6A   �� 5K      $0 PART_PopupBtn��67   1 � ��$2 0��6-   67   �� 5L      $0 PART_PopupBtn��6:   1 � ��$2 0��60   6:   �� 5M      $0 PART_PopupBtn��6;   1 � ��$2 3��61   6;   �� 5N      $0 PART_PopupBtn��6E   1 �@ Size$2 Large=�67   6E   �� 5O      $0 PART_PopupBtn��6;   1 �@ CornerRadius$2 3��61   6;   �� 5P      $0 PART_PopupBtn��66   1 � ��$2 -1q�6+   66   �� 5Q      $0 PART_PopupBtn��6B   1 �@ PopupPlacement$2 Bottom=�63   6B   �� 5R      $0 PART_PopupBtn��5T      2 5S      C� *�� *3 3 ++1 � ��5T      �� 5U      $0 PART_PopupBtn��67   1 � ��$
2 Auto��6*   67   5V      3 5X   
   ���� 5Y      '        "4  6   85   6;   5Z      5[      �� 5]      "4   6   .�5^   
   �� 1 � |�2 .  6'   �� 5_   
   1 � ��2 5`      �� 5a      .R�5b      � 5c      5e      �� 5f   
   1 � ��2 6(   C� *�� *3 3 ++�� 5g   
   1 � ��82   6%   �� 5h   
   1 �@ AnimateTransition2 6?   �� $*6 $(odc:RibbonOption.AnimateTransition)�& �� *Self+5j      5k      