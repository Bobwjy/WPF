   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� SourceO Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5	   D      progressBarTemplate �� 
TargetType � ColumnDefinitions � RowDefinitions J� OpacityMask �� 
StartPoint �� EndPoint � Color	 � Offset
 �� RadiusX �� RadiusY 
� RadiusX 
� RadiusY �� RelativeTransform�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  5   	   :x,http://schemas.microsoft.com/winfx/2006/xaml     64     5      �� 5   
   $P Jpack://application:,,,/Odyssey;Component/Themes/BreadcrumbBar/Brushes.xaml?�6   %�
  &	        �� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5	      ,s&clr-namespace:System;assembly=mscorlib  6D   " �5   2   .R�5
   
   � $��
Background��6   .+�5      �� $��
PART_Track��6   $��1,1,1,1q�6*   {� 5      $��PART_Indicator��6   $��1,1,1,1q�6.   $
��Left=�6?   ._�5      � $��
Foreground��6    5      �� 5      $��15��6+   �� 5      $
��0.1* �6+   �� 5      $��15��6+    5      �� 5      �� 5      .+�5      �� $��	Indicator��6P   #��z��6    $��3��6a   $��2��5      �� 5      $��	Animation��6P   #��z��6    $��3��5      $��2��61    5   a   Z� 5   1   �� 5      $��LeftDark��6    $��1,1,0,1q�60   $��2��6A   ��5      �� 5      $	 0,0&�62   $	 1,0&�6C   ��5   &   � $ 	#0C000000��63   $	 0Q�6E   � 5    &   $ 	#20000000��63   $		 0.3Q�6E   � 5!   &   $ 	#00000000��63   $	 1Q�6E   5#      5%      �� 5&      $��	RightDark��68   $
 1��6    $ 1��6,   $��0,1,1,1q�6I   $��2��6Z   $��2��5'      ��5(      �� 5)      $	 0,0&�62   $	 1,0&�6C   ��5+   &   � $ 	#00000000��63   $	 0Q�6E   � 5,   &   $ 	#20000000��63   $		 0.7Q�6E   � 5-   &   $ 	#0C000000��63   $	 1Q�6E   5/      51      �� 52      $��	LeftLight��6    $��0��61   $��2��6A   ��53      
� 54      $ 1Q�62   $ 1Q�6>   $ 1,0,0,1,0.5,0.5W�6J   ��56   &   � $ 	#60FFFFC4��63   $	 0Q�6E   � 57   &   $ 	#00FFFFC4��63   $	 1Q�6E   59      5;      �� 5<      $��CenterLight��6    $��1��63   $��2��6C   ��5=      �� 5>      $	 0,1&�62   $	 0,0&�6C   ��5@   &   � $ 	#60FFFFC4��63   $	 0Q�6E   � 5A   &   $ 	#00FFFFC4��63   $	 1Q�6E   5C      5E      �� 5F      $��
RightLight��6    $��2��62   $��2��6B   ��5G      
� 5H      $ 1Q�62   $ 1Q�6>   $ 1,0,0,1,-0.5,0.5W�6J   ��5J   &   � $ 	#60FFFFC4��63   $	 0Q�6E   � 5K   &   $ 	#00FFFFC4��63   $	 1Q�6E   5M      5O      �� 5P      $��
Highlight1��6   $��3��6/   $��2��6C   ��5Q      �� 5R      $	 0,0&�62   $	 0,1&�6C   ��5T   &   � $ 	#90FFFFFF��63   $	 0.5385Q�6E   � 5U   &   $ 	#00FFFFFF��63   $	 0.5385Q�6E   5W      5Y      �� 5Z      $��
Highlight2��6   $��3��6/   $��2��6C   ��5[      �� 5\      $	 0,0&�62   $	 0,1&�6C   ��5^   &   � $ 	#80FFFFFF��63   $
	 0.05Q�6E   � 5_   &   $ 	#00FFFFFF��63   $
	 0.25Q�6E   5a      5c      5d      5e      �� 5f      #��z��6   #��z��5g      5h      5i      5k      