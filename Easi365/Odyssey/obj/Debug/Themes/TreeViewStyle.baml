   M S B A M L   `   `   `  ����S  OOdyssey, Version=1.4.22.21414, Culture=neutral, PublicKeyToken=07237c05cb5a69b23clr-namespace:Odyssey.ControlsOdyssey.Controls  5   W   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   animTime  �� 
StartPoint �� EndPoint � Color � Offset �� Color  
GlyphBrush  CheckedGlyphBrush  ExpandCollapseToggleStyle �� 
TargetType �� Property �� Value �� 
TargetType	 F�Name
 �� Triggers P� Property P� Value �� 
TargetName � ColumnDefinitions � RowDefinitions d�Name �� 	ClickMode d� 	IsChecked �� Path �� RelativeSource#    Odyssey.Controls.RibbonChrome   Name    CornerRadius    RenderMouseOver    MouseOverBackground  	odc:Skins   TransparentMouseOverBtnBrush ��Name J� Opacity �� ElementName)   #Odyssey.Controls.AnimationDecorator  Name   Duration$   AnimateOnContentHeightChanged   
IsExpanded  ��Name-  'System.Windows.Input.KeyboardNavigation!   DirectionalNavigation  DeactiveItemSelectedBgBrush # DeactiveItemSelectedBorderBrush" J� IsMouseOver,   &Odyssey.Controls.ClickableTreeViewItem#   Odyssey.Controls.RibbonOption#   CloseDropDownOnClick$ ]� 
Conditions% �� Property& �� Value  TreeViewItemTemplate 	 ListViewItemTemplate !
 ClickableTreeViewItemTemplate�� 6   D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    &odcclr-namespace:Odyssey.Controls  6W   :x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   %�  &	        (��       5      *	odc:SkinsItemSelectedBgBrush+)(�� �     5      *	odc:SkinsItemSelectedBorderBrush+)(�� �     5      *	odc:SkinsDeactiveItemSelectedBgBrush+)(�� u    5      *	odc:Skins!DeactiveItemSelectedBorderBrush+)(�� �    5      *	odc:SkinsItemMouseOverBorderBrush+)(�� �    5      *	odc:SkinsItemMouseOverBgBrush+)&	 T    &	 s    &	 �    7  7  7  (�� �    5:   9   *	odc:SkinsTreeViewItemTemplate+)7  7   (�� h    5f      *	odc:SkinsListViewItemTemplate+)(�� �    5z   F   *	odc:SkinsClickableTreeViewItemTemplate+)7  7   (�� b    5�      *	odc:SkinsTreeViewItemStyle+)(�� �    5�      *	odc:SkinsListViewItemStyle+)'  ~    E�5      0:0:0.15D�6*   �� 5      $	  0,0&�6X   $	 0,1&�6i   ��5      � $ 	#FFDCEBFC��6   $ 0Q�6-   � 5	      $ 	#FFC1DBFC��6   $ 1Q�6-   5      �� 5      $ 	#FF7DA2CE��6X   �� 5      $	  0,0&�6`   $	 0,1&�6q   ��5      � $ 	#FFFAFAFA��6   $ 0Q�6-   � 5      $ 	#FFE5E5E5��6   $ 1Q�6-   5      �� 5      $ 	#FFD9D9D9��6`   �� 5      $ 	#607DA2CE��6Y   �� 5      $	  0,0&�6Y   $	 0,1&�6j   ��5      � $ 	#60DCEBFC��6   $ 0Q�6-   � 5      $ 	#60C1DBFC��6   $ 1Q�6-   5      �� 5      $ White��6)   �� 5      $ Black��60   �� 5!      " d�6.   .�5"   
   ��  � |� .  6&   �� 5#   
    � �� 5$      �� 5%      " d�6"   .R�5&      � $��15��6   $��13��6&   
������ .+�5'      F� $	 
ExpandPath��6   $
��Left=�63   $��Center=�6N   $��1,1,1,1q�6i   ���	#FFA5A5A58��  "���� 	=     � z  	= � 	=  z 	5(   D   5)      
 5*      P� 5+       �@d�	IsChecked$
 TrueY�67   . �5,      �� $ 
ExpandPath��65    � ��" ����- @KL � z @KL � z     	6M   �� 5-      $ 
ExpandPath��65   8   � ��6M   �� 5.      $ 
ExpandPath��67   8   � ��6O   5/      P� 50       �@J�IsMouseOver . . �51      �� $ 
ExpandPath��67    � �� �	#FF1CC4F76O   �� 52      $ 
ExpandPath��65    � �� �	#FF82DFFB6M   53      55      57      58      �� 5:      " Q�6   .R�5;   
   �  5<      �� 5=      $��12��6#   $
��Auto �61   �� 5>      $
��Auto �6#   �� 5?      $
��Auto �6#    5A      �� 5B      $
��Auto �6    �� 5C      .+�5E      d� $ Expander��6   8��  $ Press=�5F   Q   |�.   5E   `   �� $ 
IsExpanded� �� *TemplatedParent+5F   c      5G      $ border��6C   $��1��6   $��2��6/   $��Stretch=�6S   $ 3��5H   c    6   �� *IsMouseOver+ �� *Self+ 5I      C� *�� *3 3 ++.��5J      �� $ PART_Header��6#   $��Header��68   5K      �� 5L      $��1��6   $��1��6"   $��2��62   #��z��5M       5L   F   �� $ 	decorator��$ AnimationOpacity�._�5N        $ 	decorator��6)   8   .   5O      ��  �� *TemplatedParent+$ 
IsExpanded�._�5P      �� $  	ItemsHost��6%   $! 	Contained=�68   5Q      5R      5S      
 5T   
   P� 5U       �@��HasItems .  . �5V      �� $ Expander��6    � y�$ Hidden=�6E   5W      P� 5Y       �@Q�
IsSelected . . �5Z      �� $ border��6=    �@  
RenderFlat .  �� 5[      $ border��6B    �@  RenderMouseOver .  5\      P� 5]       �@Q�IsSelectionActive .  . �5^      �� $ border��6    5_      C� *�� *3 3 ++ � ���� 5`      $ border��6    5a      C� *�� *3 3 ++ � ��5b      5d      �� 5f      " ��6U   .R�5g   
      $ border��6   # z" 6+   $ 3��6[   $��Stretch=�5h       5i      C� *�� *3 3 ++.��5j      �� $ PART_Content��6   $��Content��65   5k      
 5l   
   P� 5m       �@��
IsSelected . . �5n      �� $ border��6=    �@  
RenderFlat .  �� 5o      $ border��6B    �@  RenderMouseOver .  5p      P� 5q       �@J�	IsFocused .  . �5r      �� $ border��6    5s      C� *�� *3 3 ++ � ���� 5t      $ border��6    5u      C� *�� *3 3 ++ � ��5v      5x      �� 5z      "  6   .R�5{   
   �  5|      �� 5}      $��12��6#   $
��Auto �61   �� 5~      $
��Auto �6#   �� 5      $��* �6#    5�      �� 5�      $
��Auto �6    �� 5�      .+�5�      d� $ Expander��6   8��  $ Press=�5�   Q   |�.  # .   5�   `   �� $ 
IsExpanded� �� *TemplatedParent+5�   u      5�      $ border��6C   $��1��6   $��2��6/   $��Stretch=�6S   $ 3��5�   c    6   �� *IsMouseOver+ �� *Self+ 5�      C� *�� *3 3 ++.��5�      �� $ PART_Header��6#   $��Header��68   5�      �� 5�      $��1��6   $��1��6"   $��2��62   #��z��5�       5�   F   �� $ 	decorator��$ AnimationOpacity�._�5�        $ 	decorator��6)   8   .   5�      ��  �� *TemplatedParent+$ 
IsExpanded�._�5�      �� $  	ItemsHost��6%   $! 	Contained=�68   5�      5�      5�      
 5�   
   P� 5�       �@��HasItems .  . �5�      �� $ Expander��6    � y�$ Hidden=�6E   5�      P� 5�       �@Q�
IsSelected . . �5�      �� $ border��6=    �@  
RenderFlat .  �� 5�      $ border��6B    �@  RenderMouseOver .  5�      P� 5�       �@J�	IsFocused .  . �5�      �� $ border��6    5�      C� *�� *3 3 ++ � ���� 5�      $ border��6    5�      C� *�� *3 3 ++ � ��5�      ]� 5�      $ 5�      �� 5�      % �@ 	IsPressed& . 65   �� 5�      % �@J�IsMouseOver& . 67   ./�5�      �� $ border��6?    �@  RenderPressed . �� 5�      $ border��6B    �@  RenderMouseOver .  5�      5�      �� 5�      " Q�6H   .�5�   
   ��  � �� 6%   C� *�� *3 3 ++�� 5�   
    �@�FocusVisualStyle 6-   Z� 5�      �� 5�      " ��6H   .�5�   
   ��  � �� 6%   C� *�� *3 3	 ++�� 5�   
    �@�FocusVisualStyle 6-   Z� 5�      �� 5�      "  6   .�5�   
   ��  � �� 6%   C� *�� *3 3
 ++�� 5�   
    �@�FocusVisualStyle 6-   Z� 5�      5�      