   M S B A M L   `   `   `  ����P  LEasi365UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ad7b111d32d14f1cG(clr-namespace:Easi365UI.Windows.ControlsEasi365UI.Windows.Controls  5      9!clr-namespace:Easi365UI.ExtentionEasi365UI.Extention  5      5clr-namespace:Easi365UI.WindowsEasi365UI.Windows  5      (    "Easi365UI.UserControls.OrgManageUC   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      8eui(clr-namespace:Easi365UI.Windows.Controls     5      ;staffsLstSort!clr-namespace:Easi365UI.Extention     5      3Windowsclr-namespace:Easi365UI.Windows     5        J� CommandBindings  5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$ Delete��5      6I   .��5      � -   5         �Name$ OrgManageGrid��5      .+�5   
   �  � ColumnDefinitions 5      �� 5      $	��200 �6#   $	��120��6/   $	��300��6>   �� 5      $��5 �6#   �� 5      $��* �6#   .+�5      R� -   5         R�Name$ DeptsTvw��5      $	��240��6)   $��0��65   $��1q�6E   
������ $
��Left=�6s   ��6�   �� *DepartmentList+��5      �� 5       �� 
TargetType" Q�6   .�5      ��  �� Property �@Q�
IsExpanded �� Value . 67   -   5        5      ��5!      �� 5"       �� ItemsSource 60   �� *Depts+.R�5#      �� ��6$   �� *
DeptName+	 � ContextMenu	 5$      �� 5%   "   .@�5&   &   m� $��添加员工��6/   
 m� Icon
 5'   *   �� 5(   .   $%��/Assets/images/add_contract.png��64   5*   '   m� 5+   &   $��添加子部门��6/   
 5,   *   �� 5-   .   $!��/Assets/images/add_dept.png��64   5/   '   m� 50   &   $��修改当前部门��6/   
 51   *   �� 52   .   $$��/Assets/images/spacemanage.png��64   54   '   m� 55   &   $��删除当前部门��6/    m� Command$ Delete��6@   
 56   *   �� 57   .   $��/Assets/images/remove.png��64   59   '   m� 5:   &   -   5        $��刷新部门结构��5:   /   
 5;   *   �� 5<   .   $ ��/Assets/images/Refresh.png��64   5>   '   5?   #   5A      5B      5D      �� 5E      $��1��6   $
��Left=�6+   $��1��6F   ���#CCC6P   �� 5F      -   5         ��Name$ StaffLvw��5F      *   $Easi365UI.Extention.ListViewBehavior   
HeaderSort . $��2��6Z   
������ $	��Top=�6�   ��6�   ��  �� View 5G      �� 5H      .P�5I      �� $��ID��6)    �� Width$ 50��65      	SortField$ ID��6@    �� CellTemplate 5J      �� 5K   "   .R�5L   &   �� $��0��60   ��6@   �� *ID+ � ToolTip 6T   �� *ID+5M   #   5O      �� 5P      $��姓名��6)   $	 120��65   $ UserName��6A    5Q      �� 5R   "   .R�5S   &   �� $��0��60   ��6@   �� *
UserName+ 6Z   �� *
UserName+5T   #   5V      �� 5W      $��账号��6)   $	 240��65   $ Account��6A    5X      �� 5Y   "   .R�5Z   &   �� $��0��60   ��6@   �� *	Account+ 6Y   �� *	Account+5[   #   5]      �� 5^      $��联系电话��6)   $	 120��67   $ TelPhone��6C    5_      �� 5`   "   .R�5a   &   �� $��0��60   ��6@   �� *
TelPhone+ 6Z   �� *
TelPhone+5b   #   5d      �� 5e      $��所属部门��6)   $	 180��67   $
 Dept��6C    5f      �� 5g   "   .R�5h   &   �� $��0��60   ��6@   �� *Dept+ 6V   �� *Dept+5i   #   5k      5l      	 5o      �� 5p      -   5         ��Name$ ZoneIformationList��5p   "    �� 	StaysOpen . 
������� J� BitmapEffect 5q      �� 5r      .@�5t      m� -   5         m�Name$ EditContextMenu��5t   /   $��编辑��6#   
 5u      �� 5v   "   $��/Assets/images/view.png��6(   5x      m� 5y      -	   5        $ RemoveContextMenu��5y   /   $��删除��6#   
 5z      �� 5{   "   $��/Assets/images/remove.png��6(   5}      5~      5�      5�         Easi365UI.Windows.Loading  5�   
   -
   5          Name$ OrgLoadingUC��5�         LoadingText$ 正在加载...��60   $��	0,0,0,150q�6F     5�   
   -   5        $ loading��5�      $ 正在删除...��6+   $y�	Collapsed=�6A   6Z   5�      5�      