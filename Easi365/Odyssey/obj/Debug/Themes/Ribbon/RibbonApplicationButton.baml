   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   W   a ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes 5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   SilverButton  
� GradientOrigin 
� RadiusX � Color � Offset  OrangeButton  OrangeLightButton �� 
TargetType+    %Odyssey.Controls.RibbonDropDownButton A�Name J� Opacity �� Stretch    Content	 � 	StaysOpen
 � VerticalOffset    MaxDropDownHeight    PopupAnimation � AllowsTransparency �� RelativeSource �� Mode � PlacementTarget5  /Microsoft.Windows.Themes.SystemDropShadowChrome   Color   CornerRadius � ActualWidth    DropDownHeader    DropDownHeaderTemplate �� Triggers P� Property P� Value N� EnterActions	 W� To �� TargetProperty �� 
TargetName g� Duration N� ExitActions �� Value  �� 
TargetName! �� Property�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  6W   RmwtJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   %�  &	        &	 �     &	 h    (�� 4    5      *	odc:SkinsApplicationMenuButtonTemplate+)7   7  7  7  7  
� 5      $  0.5,0.95&�6/   $	 0.8Q�6I   .��5   
   � $ White��6   $ 0Q�6%   � 5   
   $ 	#FF6C7E87��6   $ 1Q�6)   5	      
� 5      $  0.5,0.95&�6/   $	 0.9Q�6I   .��5   
   � $ 	#FFF2CF94��6   $ 0Q�6)   � 5   
   $ 	#FFE97304��6   $	 0.5Q�6)   � 5   
   $ 	#FFA66107��6   $ 1Q�6)   5      
� 5      $  0.5,0.95&�64   $	 0.9Q�6N   .��5   
   � $ 	#FFFDFABE��6   $ 0Q�6)   � 5   
   $ Orange��6   $	 0.5Q�6&   � 5   
   $ 	OrangeRed��6   $ 1Q�6)   5      �� 5      "   6^   .R�5   
   � $��37��6   $��37��6   .+�5      A� ���	#A0000000$��1,1,0,0q�6'   $��36��68   $��36��6C   $
��Left=�6O   5      A� 5      $ elipse��6   $��0��6&   ���	#FFE0E0FF$��36��6M   $��36��6X   $
��Left=�6d   8��  5      A� 5       $ 	mouseOver��6   $��0��6)   ���	#FFE0E0FF$��36��6P   $��36��6[   $
��Left=�6g   8�� $ 0Q�5!   ?   F� 5"      $��0��6%   $
 None=�69   K����    ��� Q%������� ��
� Q%���    ��  � ����J]	��6   Z� ��5#      
� 5$      $  0.5,0.10&�6*   $ 1Q�6D   .��5%      � $ 	#F0FFFFFF��6'   $ 0Q�69   � 5&      $ 	#20FFFFFF��6'   $	 0.5Q�69   � 5'      $ 	#40FFFFFF��6'   $ 1Q�69   5)      5+      �� 5,      $
 0.85Q�6   #��z 6.   $��Center=�6R   $��Center=�6m   A� 5.      $	��1.5��6   ���	#94FFFFFF$��36��6?   $��36��6J   $
��Left=�6V   $	��Top=�6q   � 50      $��
PART_Popup��53   {   $��Bottom=�50   v   	 .  $	
 -21��51   '   #��z 6<   #��z 52       . |�.  ��50      �� *IsDropDownPressed+ �� *TemplatedParent+$ TwoWay=� 53      ��  �� *TemplatedParent+.��55        $
��Shdw��6P   $��0,0,5,5q�6-   $ 	#71000000��6>   $ 3��6\   #��z 56      ._�57      �� $��Bottom=�6%   #��z 6=   #��z 58      59      5:      5<       5=   
   P� 5>       �@J�IsMouseOver .  5?      �� 5@      .��5A      �� .��5B      W� $ 1X�6.   $ Opacity�65   $ 	mouseOver��6Y   $ 0:0:0.10D�5C   %   5D      5E       5G      �� 5H      .��5I      �� .��5J      W� $ 0X�6.   $ Opacity�65   $ 	mouseOver��6Y   $ 0:0:0.25D�5K   %   5L      5M      5O      P� 5P       �@  	IsPressed . . �5Q      �� 8  $  elipse��6O   ! � ���� 5R      $  	mouseOver��6>   ! � y�$ Hidden=�6/   6>   5S      P� 5T       �@  	IsChecked . . �5U      �� 8  $  elipse��6O   ! � ���� 5V      $  	mouseOver��6>   ! � y�$ Hidden=�6/   6>   5W      P� 5X       �@  IsDropDownPressed . . �5Y      �� 8  $  elipse��6O   ! � ���� 5Z      $  	mouseOver��6>   ! � y�$ Hidden=�6/   6>   5[      5]      5_      