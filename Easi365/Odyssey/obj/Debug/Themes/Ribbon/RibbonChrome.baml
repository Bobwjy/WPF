   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      S OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls 5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source3   -Odyssey.Controls.RoundedCornerResizeConverter   roundedCornerConverter  boolToVisible#  Odyssey.Controls.RibbonChrome �� 
TargetType �� Property �� Value  	odc:Skins  HighlightedPressedBtnBrush   HighlightedMouseOverBtnBrush   HighlightedRibbonBorderBrush  MouseOverRibbonBorderBrush �� 
TargetType �Name ��Name   CornerRadius	 �� CornerRadius
 J� Opacity   InnerBorderThickness �� RelativeSource �� 	Converter �� ConverterParameter   MouseOverBackground   MouseOverBorderBrush   MousePressedBackground  BorderShadowBrush ��Name �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value N� EnterActions �� 
TargetName �� TargetProperty g� Duration	 W� To N� ExitActions�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     6W   .sys&clr-namespace:System;assembly=mscorlib  5      &odcclr-namespace:Odyssey.Controls  5   	     5      �� 5   
   $\ Vpack://application:,,,/Odyssey;Component/Themes/Ribbon/DefaultRibbonButtonBrushes.xaml?�6   %?  &	        (��      5
      *	odc:SkinsDisabledImageOpacity+)&	 /     '  B     7   7   7      5	      6'   X�5
      0.50Q�6V   �� 5      6#   �� 5      "  6   .�5   
   ��  � ��$ 1q�6,   �� 5   
    �@ InnerBorderThickness$ 1q�61   �� 5   
    � ��$ 2q�6$   �� 5   
    � ��$ Center=�65   �� 5   
    �@ 
RenderFlat . 6'   �� 5   
    � |� .  6&   �� 5   
    � �� 6'   C� *�� *3 3 ++�� 5   
    �@ MouseOverBackground 60   C� *�� *3 3 ++�� 5   
    �@ MousePressedBackground 63   C� *�� *3 3 ++�� 5   
    � �� 6(   C� *�� *3 3 ++�� 5    
    �@ MouseOverBorderBrush 61   C� *�� *3 3 ++�� 5!   
    � �� 5"      �� 5#      "  6"   .R�5$      � $
 grid��6   |�.  .+�5%      �� $ 
defaultBtn��6!   #	 z 65   |�.  $y�	Collapsed=�6u   $��0q�5&   !   $
 1Q�6,   $��Stretch=�68   #��z��6V   #��z��5'   !   #��z��6M   ._�5(      �� $ InnerBorder��6%   |�.  #��z 6L   $��Stretch=�5)   %   ���	#80FFFFFF	 5*   %   �� *CornerRadius+ �� *TemplatedParent+8   $ -1��5+      �� 5,      $ mouseOverBtn��6!   #	 z 67   |�.  $��Stretch=�5-   !   $
 0Q�6?   $��0q�6K   #��z 6V   #��z 5.   !   #��z��6V   ._�5/      �� $ mouseOverInnerBorder��6%   |�.  $��Stretch=�6U   #��z 50   %   ���	#C0FFFFFF	 51   %   �� *CornerRadius+ �� *TemplatedParent+8  $ -1��52      �� 53      $ mouseDownBtn��6!   #	 z 67   |�.  $��Stretch=�54   !   $y�Hidden=�6?   #��z 6S   #��z��55   !   #��z��6M   ._�56      �� $ innerBorder2��60   $��0q�6%   |�.  $��Stretch=�6X   $��0.2,1.6,0.2,0q�57   %   }�. 	 58   %   �� *CornerRadius+ �� *TemplatedParent+8  $ -1����59   %   C� *�� *3 3 ++5:      �� 5;      $ content��6_   #��z��6)   |�.  #��z��5<   !   #��z��6D   #��z��5=   !   #��z��5>   !   5?       5@      P� 5A        �@ ShowInnerMouseOverBorder .  . �5B      �� $ mouseOverInnerBorder��6M    � y�$ 	Collapsed=�6;   6M   5C      P� 5D       �@ 
RenderFlat .  . �5E      �� $ 
defaultBtn��6K    � y�$ Visible=�6;   6K   5F      ]� 5G       5H      �� 5I   "    �@ RenderMouseOver . 6G   �� 5J   "    �@ AnimateTransition .  6Z   ./�5L      �� $ 
defaultBtn��6%    � ��
 ���� 6T   �� 5M      $ mouseOverBtn��6B    �@J�Opacity$ 1Q�68   6B   5N      ]� 5P       5Q      �� 5R   "    �@ RenderMouseOver . 6G   �� 5S   "    �@ AnimateTransition . 6Z    5U      �� 5V   "   .��5W   &   �� .��5X   *   W� $ mouseOverBtn��6:   $ Opacity�6_   $ 0:0:0.25D�5Y   1   $ 1X�6E   5Z   '   5[   #    5]      �� 5^   "   .��5_   &   �� .��5`   *   W� $ mouseOverBtn��6:   $ Opacity�6_   $ 0:0:0.25D�5a   1   $ 0X�6E   5b   '   5c   #   5e      P� 5g       �@ RenderPressed . . �5h      �� $ mouseDownBtn��6;    � y�$ Visible=�6U   5i      P� 5j       � {� .  . �5k      �� $ mouseDownBtn��6J    � y�$ Hidden=�6;   6J   �� 5l      $ mouseOverBtn��6J    � y�$ Hidden=�6;   6J   5m      5o      5q      5r      5t      