   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089$    Odyssey.Controls.BoolConverter   boolConverter ! SelectedRibbonTabItemTemplate  �� 
TargetType$   Odyssey.Controls.RibbonTabItem � ColumnDefinitions F�Name J� SnapsToDevicePixels  	odc:Skins  RibbonTabItemBrush  DefaultBorderBrush ��Name �Name �� Color �� RelativeSource J� OpacityMask	 �� 
StartPoint
 �� EndPoint � Color � Offset ��Name   Title � ToolTip " TabItemSelectedForegroundBrush �� CornerRadius J� Opacity J� Effect-  'System.Windows.Media.Effects.BlurEffect   
KernelType   Radius   RenderingBias �� Triggers P� 
SourceName P� Property P� Value N� EnterActions �� 
TargetName �� TargetProperty g� Duration	 W� To N� ExitActions  �� Value   HighlightedRibbonBorderBrush! �� Property   HighlightedMouseOverBtnBrush" �� 
TargetName  RibbonTabItemTemplate#   Odyssey.Controls.RibbonChrome#   CornerRadius$   AnimateTransition%   InnerBorderThickness& J� IsMouseOver'   RenderMouseOver(   MouseOverBackground  	 TransparentMouseOverBtnBrush)   
RenderFlat* �� 	Converter 
 TabItemForegroundBrush+ �� 
TargetType, �� Triggers�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     64   %�  &	        &	      &	 �    7   '  �
    7  7     5      6   �� 5      "   6<   .R�5   
   � $
��grid��6   $��1,2,1,3q�6   $��23��6,   #��z��68   |�.   5      �� 5	      $
��Auto �6#   �� 5
      $��* �6#   �� 5      $
��Auto �6#   .+�5      F� $
 Left��5      $��1��5      $��0,0,-2,0q�6'    .  $��0��6U   |�.  M����    ��^���-  -1���- ��- ׀�[     ���- ��- @�����j     ���j ��^	��5      C� *�� *3 3 ++��5      C� *�� *3 3 ++F� 5      $ Right��5      $��1��5      $��
-1.5,0,0,0q�6'    .  $��2��6W   |�.  M����    ��^�        �@B     � 	= ��- ���- ��- @��� 	=  -1���j ��^	��5      C� *�� *3 3 ++��5      C� *�� *3 3 ++�� 5      $ Border��6Q   $��0,1,0,0q�6   $��1��6/   |�.  #��z��5      #��z��5      ._�5      � $	 tab��6   .+�5      � $ ColorBackground��6   $��	-2,0,-2,0q�64   ��5      �� 5       6.   �� *Color+ �� *TemplatedParent+ 5      �� 5       $		 0,0&�62   $	
 0,1&�6C   .��5!   "   � $ 	#30FFFFFF��6/   $ 0Q�6A   � 5"   "   $ 	#80FFFFFF��6/   $ 1Q�6A   5#      5%      �� 5&      $ tabtext��6    #��z 61   $	��Top=�6P   $��4,3,4,1q�6h   |�.  $��CharacterEllipsis=�5'   /   # z 6P   ��5(      C� *�� *3 3 ++5)      5*      �� 5+      $��MouseOverBorder��6   |�.  ���	#A0FFFF00$��3,3,3,0q�6V   $ 3,3,0,0��5,      $��1,0,2,2q�6,   $��3��6=   $ 0Q�6Q    5-        5.      $ Gaussian=�6!   $ 4Q�67   $ Performance=�6B   50      51       53   
   P� 54      $	 tab��6:    �@J�IsMouseOver .  55      �� 56      .��57      �� .��58      W� $ MouseOverBorder��6.   $ Opacity�6V   $ 0:0:0.25D�59   %   $ 1X�69   5:      5;       5=      �� 5>      .��5?      �� .��5@      W� $ MouseOverBorder��6.   $ Opacity�6V   $ 0:0:0.25D�5A   %   $ 0X�69   5B      5C      5E      P� 5F       �@J�	IsFocused . . �5G      ��   60   C� *�� *3 3 ++! � ���� 5H        6/   C� *�� *3 3 ++! � ���� 5I      $" ColorBackground��6>   ! � y�$  Hidden=�6/   6>   5J      5M      �� 5O      "   64   .R�5P   
     $# 3,3,0,0��6   $��4,0,4,0q�62   $��23��6D   $��1,0,1,0q�6P   $ . $��8��5Q      $% 0q�6   $��Stretch=�5S      #' z& 6/   $��1,1,1,0q�5U      ( 5R      C� *�� *3 3	 ++) 5T      �� *
IsSelected+ �� *TemplatedParent+8*   .��5V      �� #��z 6   $��4,0,4,0q�67   $��Stretch=�6H   # z 5W      $��NoWrap=�5Y      $��CharacterEllipsis=�6+   ��5X      C� *�� *3 3
 ++5Y   +   5Z      5[      �� 5]      "+  6   .�5^   
   �� ! � ��  6(   C� *�� *3 3 ++�� 5_   
   ! � ��  6'   C� *�� *3 3 ++�� 5`   
   ! � ��$  28��6&   �� 5a   
   ! � ��  .  6&   �� 5b   
   ! �@�FocusVisualStyle  6-   Z� �� 5c   
   ! � ��8    6%   , 5d   
   P� 5e       �@ 
IsSelected . . �5f      �� ! � ��8   6-   5g      5i      5j      