   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   W   1clr-namespace:Odyssey.EffectsOdyssey.Effects  5   	   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 62   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   RibbonTextBoxTemplate �� 
TargetType$    Odyssey.Controls.RibbonTextBox � ColumnDefinitions ��Name �� EdgeMode    Image �� BitmapScalingMode �� Path	 �� RelativeSource
 ��Name    Title    
LabelWidth ��Name    ContentWidth J� Opacity ��Name �� Triggers P� Value P� Property �� 
TargetName �� Property �� Value    Odyssey.Controls.RibbonBar ]� 
Conditions �� Property �� Value#   Odyssey.Controls.RibbonChrome  	odc:Skins  DisabledImageOpacity%   Odyssey.Effects.GrayscaleEffect N� EnterActions �� 
TargetName �� TargetProperty g� Duration	 W� To N� ExitActions  W� From! O� GlowSize" O� Opacity# O� 	GlowColor  	GlowColor  DisabledForegroundBrush$ �� 
TargetType   Odyssey.Controls.KeyTip  DefaultBorderBrush�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  6W   $fxclr-namespace:Odyssey.Effects  5   	   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  62   :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     5      �� 5   
   $\ Vpack://application:,,,/Odyssey;Component/Themes/Ribbon/DefaultRibbonButtonBrushes.xaml?�6   %�  &	        '   
    7   �� 5      "   64   .R�5	   
   � |�.   5
      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   �� 5      $��* �6#   .+�5      �� $	 img��65   $ Aliased=�6   #��z 5      $��16��66   $��16��6A   $��0��6M   $��2q�6]   $��Center=�6h   $��Center=�6�   |�.   5      �� $4 .(odc:ImageRenderOptions.SmallImageScalingMode)�	 �� *TemplatedParent+5      �� 5      $

 text��6   #��z 6&   $��Center=�6E   $��1��6`   $��2,0,2,0q�6p   |�.  #��z 5      �� 5      $ outerBorder��6�   $��2��6   $��0q�6%   #��z��60   #��z��6Z   #��z 5      $��Stretch=�6<   |�.  #��z��6l   $
 0.65Q�5      �� 5      $ innerBorder��6�   
�������$��1q�6)   |�.  $��2��6O   $��1q�6_   
������ ._�5      �� $ PART_ContentHost��6   $��Stretch=�69   $��Center=�6W   5      5       5   
   P� 5       6'   Z�  �@  Image. �5       �� $	 img��6/    � y�$ 	Collapsed=�6@   �� 5!      $	 img��6+    � ��$ 0q�6<   5"      P� 5#       �@  Title$  ��6'   . �5$      �� $
 text��6+    � ��$ 0q�6=   5%      P� 5&       �@ Size$ Small=�64   . �5'      �� $
 text��6A    � y�$ 	Collapsed=�6/   6A   5(      P� 5)       �@ Size$ 	Minimized=�64   . �5*      ��  � y�$ 	Collapsed=�6/   5+      ]� 5-       5.      �� 5/       � {� .  65   �� 50       �@ IsGrayScaleEnabled . 6O   ./�52      �� $	 img��6}    6,   C� *�� *3 3 ++ �@J�Opacity6}   �� 53      $	 img��6O    �@��EdgeMode$ Unspecified=�6;   6O   �� 54      $	 img��6Q    �@��BitmapScalingMode$
 Fant=�6D   6Q   �� 55      $	 img��6+    �@J�Effect 56        57      59      5:      P� 5<       �@J�IsMouseOver .  5=      �� 5>      .��5?      �� .��5@      W� $ outerBorder��6.   $ Opacity�6R   $ 0:0:0.15D�6v   $ 1X�6�   5A      5B       5D      �� 5E      .��5F      �� .��5G      W� $ outerBorder��6.   $ Opacity�6R   $ 0:0:0.15D�6v   $  1X�6�   5H      5I      5K      P� 5L       �@J�	IsFocused . . �5M      �� $ outerBorder��68    �@J�Opacity$	 1.0Q�6,   68   �� 5N      $ outerBorder��61    �@J�BitmapEffect 5O      O� 5P      $! 2Q�60   $" 1Q�6�   # 6=   C� *�� *3 3 ++6�   5R      5S      P� 5U       � {� .  . �5V      ��  6/   C� *�� *3 3 ++ � ��5W      5Z      �� 5\      "$   6   .�5]   
   ��  �@ YOffset$ 4Q�6/   �� 5^   
    �@�FocusVisualStyle 6-   Z� �� 5_   
    � ��8   6%   �� 5`   
    � ��
 �����6'   �� 5a   
    � �� 6(   C� *�� *3 3 ++�� 5b   
    � ��$ 1q�6,   �� 5c   
    � ��$ 0q�6#   �� 5d   
    � ��$ 24��6&   �� 5e   
    � |� . 6&   5f      5g      