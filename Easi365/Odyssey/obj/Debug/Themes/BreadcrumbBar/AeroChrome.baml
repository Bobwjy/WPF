   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   W   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source �� 
TargetType!    Odyssey.Controls.AeroChrome   AeroChromeTemplate ��Name J� Opacity    MouseOverBackground    MousePressedBackground J� SnapsToDevicePixels  borderShadowBrush ��Name	 �� Triggers
 P� Property P� Value N� EnterActions �� 
TargetName �� TargetProperty g� Duration	 W� To N� ExitActions �� 
TargetName �� Property �� Value �� 
TargetType  mouseOverBtnBrush  mousePressedBtnBrush  borderBrush�� 6   D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  6W   :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     5      �� 5   
   $P Jpack://application:,,,/Odyssey;Component/Themes/BreadcrumbBar/Brushes.xaml?�6   %^  &	        7  '   g    7  7  7  7   �� 5      "   6   .R�5   
   � #��z��6   .+�5	      �� $ mouseOverBtn��6   $ 0Q�6+   #��z 67   |�.  #��z��5
      #��z��6A   ._�5      �� $ innerButton��6   $��1q�6.   
�������6B   5      �� 5      $ mouseDownBtn��6   $ 0Q�6+   #��z 67   |�.  #��z��5      #��z��6A   ._�5      �� $ innerButton2��6    .  $��1.2,1.2,0,0q�6K   8��  6i   5      �� 5      $ content��67   $��Content��6   $��Center=�6H   $��Center=�6c   |�.  $��4,2,4,2q�5      #��z��6&   5      	 5   
   P� 5      
 �@  RenderMouseOver .  5      �� 5      .��5      �� .��5      W� $ mouseOverBtn��6.   $ Opacity�6S   $ 0:0:0.15D�6w   $ 1X�6�   5      5       5      �� 5      .��5      �� .��5       W� $ mouseOverBtn��6.   $ Opacity�6S   $ 0:0:0.15D�6w   $ 0X�6�   5!      5"      5$      P� 5%      
 �@  RenderPressed . . �5&      �� $ mouseDownBtn��6,    �@J�Opacity$ 1Q�6F   �� 5'      $ content��6+    � ��$ 5,3,3,1q�6@   5(      5*      �� 5,      "   6   .�5-   
   ��  �@  MouseOverBackground8   60   �� 5.   
    �@  MousePressedBackground8  63   �� 5/   
    � ��8  6(   �� 50   
    � ��8  6%   51      52      