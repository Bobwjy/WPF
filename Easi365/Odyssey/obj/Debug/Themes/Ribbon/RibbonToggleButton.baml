   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b21clr-namespace:Odyssey.EffectsOdyssey.Effects  5   	   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 62   3clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  
� Center � Color � Offset'    !Odyssey.Controls.TwoLineConverter   twoLineConverter-  'Odyssey.Controls.RoundedCornerConverter  roundedCornerConverter $  SmallRibbonButtonControlTemplate �� 
TargetType)   #Odyssey.Controls.RibbonToggleButton#   Odyssey.Controls.RibbonChrome  Name   IsFlat   
RenderFlat   RenderEnabled   CornerRadius	   CornerRadius
   RenderPressed   RenderMouseOver �� RelativeSource ��Name   
SmallImage �� BitmapScalingMode �� Path ��Name �� Triggers P� Value P� Property �� 
TargetName �� Property �� Value    Odyssey.Controls.RibbonBar ]� 
Conditions �� Property �� Value  	odc:Skins  DisabledImageOpacity%   Odyssey.Effects.GrayscaleEffect  DisabledForegroundBrush $  LargeRibbonButtonControlTemplate �� EdgeMode   
LargeImage �� 	Converter �� 
TargetType   HighlightedRibbonBorderBrush  RibbonButtonGroupBgBrush �� Triggers�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     6W   $fxclr-namespace:Odyssey.Effects  5   	   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  62   &odcclr-namespace:Odyssey.Controls  5   	   %,  (��       5      *	odc:SkinsDefaultMousePressedBtnBrush+)&	  �     &	     &	     &	 �    7   '      7  7  
� 6   $  0.5,0.9&�6`   .��5   
   � $ 	#FFFDE499��6   $	 0.0Q�6)   � 5   
   $ 	#FFFBAE60��6   $	 0.8Q�6)   � 5   
   $ 	#FFF9972F��6   $	 1.1Q�6)   � 5	   
   $ 	#FFFDD4A8��6   $ 2Q�6)   5
         5      6     5      6!   �� 5      "  6?   .R�5   
     $ chrome��6   # z 6+   # z{�6Q   #	 z 5      #��z��5      #��z��6=   #��z��5      #
 z��6[    5   ?   �� *IsMouseOver+ �� *Self+��5      ]� 5      $��0q�6   .+�5      �� $ image��6   #��z 6+   $��Center=�5      $��2,0,2,0q�68   $��16��6I   $��16��6T   $
��Left=�6`   }�.  5      �� $4 .(odc:ImageRenderOptions.SmallImageScalingMode)� �� *TemplatedParent+5   v   �� 5      $ content��6'   #��z��68   $��Center=�6\   $��2,0,2,0q�6w   $y�Visible=�5      5      5       5   
   P� 5       6,   Z�  �@ 
SmallImage. �5       �� $ image��6A    � y�$ 	Collapsed=�6/   6A   5!      P� 5"       �@ Size$ Small=�64   . �5#      �� $ content��6A    � y�$ 	Collapsed=�6/   6A   �� 5$      $ content��64    � ��$ 0��6*   64   5%      P� 5&       �@d�	IsChecked$
 TrueY�6+   . �5'      �� $ chrome��6?    �@ RenderPressed . 5(      P� 5)       �@ Size$ 	Minimized=�64   . �5*      ��  � y�$ 	Collapsed=�6/   5+      ]� 5,       5-      �� 5.       � {� .  65   �� 5/       �@ IsGrayScaleEnabled . 6O   ./�51      �� $ image��6O    �@��EdgeMode$ Unspecified=�6;   6O   �� 52      $ image��6Q    �@��BitmapScalingMode$
 Fant=�6D   6Q   �� 53      $ image��6}    6,   C� *�� *3 3 ++ �@J�Opacity6}   �� 54      $ image��6+    �@J�Effect 55        56      58      59      P� 5:       � {� .  . �5;      ��  6/   C� *�� *3 3 ++ � ��5<      5>      �� 5@      "  6?   .R�5A   
     $ chrome��6   $��72��6+   # z{�67   #��z��5B      # z 6=   $��Center=�6c   $	��Top=�5C      #	 z 60   #
 z��5D   _    6   �� *IsMouseOver+ �� *Self+��5E      ]� 5F      $��Stretch=�6   $��2,4,2,4q�6:   $��64��6K   .+�5G      �� $ image��6   $ Aliased=�5I      #��z 6>   $��Center=�6d   $��2,0,2,0q�5J      $��32��6.   $��32��69   $	��Top=�6E   $��UniformToFill=�6Z   }�.  5H      �� $4 .(odc:ImageRenderOptions.LargeImageScalingMode)� �� *TemplatedParent+5J   r   �� 5K      $ content��6'   $��Center=�5M      $��4,0,0,0q�68   $��Center=�6I   $��Bottom=�6f   ��5L      �� *	Content+ �� *TemplatedParent+8   5M   f   5N      5P       5Q   
   P� 5R       �@ Size$ Small=�64   . �5S      �� $ content��6A    � y�$ 	Collapsed=�6/   6A   5T      P� 5U       �@d�	IsChecked$
 TrueY�6+   . �5V      �� $ chrome��6?    �@ RenderPressed . 5W      ]� 5X       5Y      �� 5Z       � {� .  65   �� 5[       �@ IsGrayScaleEnabled . 6O   ./�5]      �� $ image��6O    �@��EdgeMode$ Unspecified=�6;   6O   �� 5^      $ image��6Q    �@��BitmapScalingMode$
 Fant=�6D   6Q   �� 5_      $ image��6}    6,   C� *�� *3 3 ++ �@J�Opacity6}   �� 5`      $ image��6+    �@J�Effect 5a        5b      5d      5e      P� 5f       � {� .  . �5g      ��  6/   C� *�� *3 3 ++ � ��5h      5j      �� 5l      "  6   .�5m   
   ��  � �� .  6&   �� 5n   
    � �� 6(   C� *�� *3 3 ++�� 5o   
    � �� 6'   C� *�� *3 3 ++�� 5p   
    � ��8   6%   �� 5q   
    �@ AnimateTransition 6?   �� $* $(odc:RibbonOption.AnimateTransition)� �� *Self+ 5r   
   P� 5s       �@ Size$ Large=�64   . �5t      ��  � ��8  6-   5u      5w      5x      