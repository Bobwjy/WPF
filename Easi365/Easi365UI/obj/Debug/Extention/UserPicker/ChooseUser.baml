   M S B A M L   `   `   `  ����P  LEasi365UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ad7b111d32d14f1cG(clr-namespace:Easi365UI.Windows.ControlsEasi365UI.Windows.Controls  5   	   3clr-namespace:Easi365UI.ModelsEasi365UI.Models  5   	   $    Easi365UI.Extention.ChooseUser   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   8eui(clr-namespace:Easi365UI.Windows.Controls     5   	   0modelclr-namespace:Easi365UI.Models     5   	     � Title$  ChooserUser��5   	   $	��400��6   $	��320��6*    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�6.    � WindowStyle$
 None=�6D    � 	Resources 5         ShareProvider S� 
ObjectType#   Easi365UI.Models.OrgViewModel S� 
MethodName  TreeViewItemStyle �� 
TargetType �� Property	 �� Value
 �� Path �� Mode  CheckBoxItemTemplate �� ItemsSource �� Orientation d� 	IsChecked  boolToVisibility �� 	Converter��%  &	        &	 *     &	 �     7  S� 5	   
   "  63   $ Create��6\   �� 5
   
   " Q�6*   .�5      ��  �@Q�
IsExpanded	 6+   �� $
 
IsExpanded�$ TwoWay=�5      �� 5   
    6@   �� *
Children+.R�5      �� $ 
Horizontal=�6   $��0,2,0,0q�62   .+�5      �� $��TreeItemChk��6   -   5        |�.  $��Center=�5   �    6@   �� *	IsChecked+$ TwoWay=�y�6l   �� *IsUser+8   6�   �� 5      $	��2,0q�6O   ��6#   �� *DisplayText+$ OneWay=�6O   5      5      5      .��5      �� $��1q�6   
�������
���   �._�5   
   � .+�5      �� $��30��6   $	��Top=�6!   $
��Auto��69   ��5      �� 5       �� EndPoint$ 0.5,1&�6*    �� 
StartPoint$ 0.5,0&�6;   .��5      �  � Color$ 	#FF1954BB��6'    � Offset$ 0Q�69   � 5      $ #65ADCA��6'   $ 1Q�67   5      .+�5      �� -   5         ��Name$ wTitle��5      $��选择用户��6,   $��14(�68   
�������$
��Bold"�6Y    �� Left$ 10��6k   
 �� Top$ 5��6|   +   %Easi365UI.Windows.Controls.EasiButton  5      -   5          Name$ MinBtn��5   !   +   %Easi365UI.Windows.Controls.EasiButton   Click$ MinBtn_Click��6`   $	��Top=�61   $��27��6I   $��22��6T    � ToolTip$ 	最小化��6u    �� Right$ 25��6�      MyMoverBrush 5      �� 5        �� ImageSource$ /Assets/images/mim.png��6%      MyEnterBrush 5"      �� 5#      $ /Assets/images/mie.png��6%   ��5%      �� 5&      $ /Assets/images/min.png��6%   5(      �� 5)      $��28��6   $ 0��6$   $��1��63   $��/Assets/images/line.png��6=   $ 24��6^     5*      -   5        $ CloseBtn��5*   !   $ CloseWindow��6~   $��Right=�63   $	��Top=�6O   $��27��6g   $��22��6r   $ 关闭��6�   $ 0��6�   $ 0��6�    5+      �� 5,      $ /Assets/images/closeb.png��6%    5.      �� 5/      $ /Assets/images/closeb.png��6%   ��51      �� 52      $ /Assets/images/close.png��6%   54      55      � 56      $��5,30,5,5q�6    � RowDefinitions 57      �� 58      $��* �6$   �� 59      $��10 �6$   �� 5:      $��30 �6$   .+�5<      R� $��0��6   $��0q�6(   $	��Top=�6<   #��[ 6T   #��[ 6�   ��6�   ��  �� Source# [  6�   �� 5=      -   5        $��CheckedUsersLst��5=      $��1��61   $
��0,10q�6c   $	��Top=�6q   ��6>   �� *CheckedUsers+��5>      �� 5?        �� 
TargetType"  ��6*   .R�5@      � ! � Orientation$! 
Horizontal=�6(   " K� IsItemsHost" . 6A   5A      ��5C      �� 5D      .R�5E      �� .+�5F   "   �� ��6,   �� *
UserName+5G      5H      5J      �� 5K      $ 
Horizontal=�6   $��2��66   $��Right=�6C   $��Bottom=�6_   .+�5L      �� -   5        # ��Name$# btnChooseUser��5L      $ �� 	IsDefault$ . $��75��6a   $��24��6l   $��确定��6x   �� 5M      -   5        $# 	btnCancel��5M      $��取消��60   $��75��6=   $��24��6H   $��8,0,0,0q�6T   6e   5N      5O      5P      5Q      5R      