   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b21clr-namespace:Odyssey.EffectsOdyssey.Effects  5   	   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 62   3clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� 
StartPoint �� EndPoint � Color � Offset    RibbonToolTipControlTemplate �� 
TargetType$    Odyssey.Controls.RibbonToolTip5  /Microsoft.Windows.Themes.SystemDropShadowChrome   Color   CornerRadius �� CornerRadius � RowDefinitions	 � ColumnDefinitions
    Title    Image    Description ��Name  	odc:Skins  DefaultBorderBrush �� Orientation    FooterImage    Footer �� Triggers ]� 
Conditions �� Value �� Property �� 
TargetName �� Property �� Value �� 
TargetType  RibbonToolTipBackground�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     6W   $fxclr-namespace:Odyssey.Effects  5   	   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  62   &odcclr-namespace:Odyssey.Controls  5   	   %�  (��       5      *	odc:SkinsRibbonToolTipBackground+)&	  �     '   �    7   �� 6   $	  0,0&�6\   $	 0,1&�6m   .��5   
   � $
 #FFF��6   $ 0Q�6$   � 5   
   $ 	#FFC9D9EF��6   $ 1Q�6)   5      �� 5
      "   6;   .R�5   
     $
��Shdw��6H   $��0,0,5,5q�6%   $ 	#41000000��66   $ 4��6T   ._�5      �� 
�������$��1q�6(   $ 3��6<   #��z��6M   ._�5      � $��4q�6    5      �� 5      �� 5      �� 5      �� 5      	 5      �� 5      �� 5      .+�5      �� $��2��6    #��z
 64   $
��Bold"�6S   $��0,0,0,4q�6e   �� 5      $��1��6   $��0��6)   #��z 69   $
��None=�6Z   $��4q�6i   $	��Top=�6t   �� 5      $��1��6    $��1��6-   #��z 6=   $
��Wrap=�6b   $��CharacterEllipsis=�6v   $��4q�6�   $	��Top=�5      �� 5      $ 	separator��6!   $��2��64   $��2��6A   .+�5      �� $��0,1,0,0q�6v   ��6!   C� *�� *3 3 ++6v   �� 5      $��0,1,0,0q�6!   
�������$��0,1,0,0q�6F   5      �� 5!      $��3��6!   $��2��6.   $ 
Horizontal=�6B   .+�5"      �� #��z 6    $
��None=�6G   $��Center=�6V   �� 5#      #��z 6$   $��4q�6D   $��Center=�6O   $��CharacterEllipsis=�6j   $
��Wrap=�6�   $
��Bold"�5$   !   5%      5&      5'      5(       5)   
   ]� 5*       5+      �� 5,       62   Z�  �@  Footer�� 5-       67   Z�  �@  FooterImage./�5/      �� $ 	separator��6    � y�$ 	Collapsed=�6F   50      52      �� 54      "   6   .�55   
   ��  � ��$	 320��6%   �� 56   
    � �� 6(   C� *�� *3 3 ++�� 57   
    � ��8   6%   �� 58   
    � �� 6'   C� *�� *3 3 ++59      5;      