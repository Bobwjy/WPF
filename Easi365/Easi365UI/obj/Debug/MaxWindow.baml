   M S B A M L   `   `   `  ����P  LEasi365UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ad7b111d32d14f1cG(clr-namespace:Easi365UI.Windows.ControlsEasi365UI.Windows.Controls  5      P LClientLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7ac84983712e9790J3clr-namespace:ClientLib.Entities;assembly=ClientLibClientLib.Entities 5      5clr-namespace:Easi365UI.WindowsEasi365UI.Windows  5      9!clr-namespace:Easi365UI.ExtentionEasi365UI.Extention  5      3clr-namespace:Easi365UI.ModelsEasi365UI.Models  5      ?$clr-namespace:Easi365UI.UserControlsEasi365UI.UserControls  5	      E'clr-namespace:Easi365UI.Models.CommandsEasi365UI.Models.Commands  5
      %clr-namespace:Easi365UI	Easi365UI  5          Easi365UI.MaxWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      8eui(clr-namespace:Easi365UI.Windows.Controls     5      Dmode3clr-namespace:ClientLib.Entities;assembly=ClientLib     5      3Windowsclr-namespace:Easi365UI.Windows     5      h dHardcodet.Wpf.TaskbarNotification, Version=1.0.5.0, Culture=neutral, PublicKeyToken=d6bef4a0a5ae870e)tb http://www.hardcodet.net/taskbar  5      -local!clr-namespace:Easi365UI.Extention  5      .	localUtilclr-namespace:Easi365UI.Models  5      -uc$clr-namespace:Easi365UI.UserControls  5	      6commands'clr-namespace:Easi365UI.Models.Commands  5
      :d2http://schemas.microsoft.com/expression/blend/2008  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      S OOdyssey, Version=1.4.22.21415, Culture=neutral, PublicKeyToken=07237c05cb5a69b2(odchttp://schemas.odyssey.com/wpf  5      "ttttclr-namespace:Easi365UI  5      -   5        +   %Easi365UI.Windows.Controls.EasiWindow   Name$
  MaxW��5      +   %Easi365UI.Windows.Controls.EasiWindow   Loaded$ MainWindow_Loaded��5         SizeChanged$ Window_SizeChanged��5         Closing$ MaxW_Closing��5       � Title$ Easi365网盘��5      $	��975��6#   $	��600��6/    � WindowStartupLocation$ CenterScreen=�5      $	��975��66   $	��600��6F    � WindowStyle$
 None=�5       � AllowsTransparency .   � 
ResizeMode$ NoResize=�5      	 � 	Resources	 5      5   /Easi365UI.Models.Commands.OpenMainWindowCommand   OpenMainWindowCommand  ExpanderListItemTemplete
 �� Orientation  ExpanderListBox �� 
TargetType �� Property �� Value �� DataType   Easi365UI.FolderItem%  Odyssey.Controls.BreadcrumbItem  ToggleButtonStyle �� 
TargetType J� SnapsToDevicePixels ��Name �� Right	 �� X1	 �� Y1	 �� X2	 �� Y2 �� StrokeDashArray
 �� Top �� Bottom �� Triggers P� Property P� Value �� 
TargetName  ExpanderStyle � RowDefinitions �Name  �� CornerRadius! d�Name" d� 	IsChecked# �� Mode$ �� RelativeSource% �� ElementName& �� Path  ExpanderListItemStyle��%�  &	        &	      &	 �     (�� �    5+      *, +)'  �    &	 '    &	 I	    &	 �      5   
   6)   �� 5   
   .R�5      �� $
 
Horizontal=�6   .+�5       �� $!��/assets/images/personal.png��6   $��16��6=   $��16��6I   �� 5!      $��3,0,0,0q�68   ��6   �� *
SpaceTitle+68   5"      5#      �� 5%   
   " ��6(   .�5&      ��  � ��$ 13(�6)   �� 5'       � ��$ 10,0,0,0q�6'   �� 5(       � ��
 ���� 6,   5)      �� 5+   
   "  6   .R�5,      �� ��6   �� *Folder+5-      �� 5.   
   "  6   .�5/      ��  �@ Image$ Web.png��6&   50      �� 52   
   " d�6*   .�53      ��  �@�FocusVisualStyle 61   Z� �� 54       � ��$ 30��6'   �� 55       � �� �	#7Ad2e7f46+   �� 56       � �� �	#FF217cb56+   �� 57       � ��$
 Bold"�6+   �� 58       � ��$ 0q�60   �� 59       � ��$
 Left=�6;   �� 5:       � ��$ Center=�69   �� 5;       � ��$	 5 6q�6(   �� 5<       � �� 5=      �� 5>      " d�6&   .R�5?      �� #��z��6!   #��z��5@   !   #��z��5A   !    . .+�5C      �� $ 
canvNormal��6%   $y�Visible=�69   #��z��5D   %   #��z��6E   #��z��5E   %    . $ 0��6j   .+�5F   "   �� $��15��6,   $��30��67   ���	#FFc7ebff$ 0��6T   �� 5G   "   $ 0��6'   $ 0��6.   $ 0��65   $ 15��6<   ���#297eb4$��3��6U   $
��6 10q�6i   $ 1R�6w   $ 0��6�   5H      �� 5I      $ canvChecked��6%   $y�Hidden=�6:   #��z��5J   %   #��z��6E   #��z��5K   %    . $ 0��6j   .+�5L   "   F� U������� (9� QȊ� ���Sc@Ѡ.c (9� Q0c -��� @QȊ� |?5^��@ш�� �z~ ���� (9� 	���#297eb4$ 5��6�   $ 5��6�   F� 5M   "   U������� (9� QȊ� ���Sc@Ѡ.c (9� Q0c -��� @QȊ� |?5^��@ш�� �z~ ���� (9� 	���#297eb4$ 5��6�   $ 12��6�   �� 5N   "   #��z��6,   $��1��6L   $ 0��6W   ���	#FFbacfda6i   5O      �� 5Q      #��z��6/   #��z��5R      ��. # z 5T      #��z��5U      5V       5X      P� 5Y       � �� . . �5Z   "   ��  � �� �	#FFd2e7f46?   5[      P� 5\       �@d�	IsChecked$
 trueY�6;   . �5]   "   �� $ canvChecked��6?    � y�$ Visible=�6X   �� 5^   "   $ 
canvNormal��6?    � y�$ Hidden=�6W   5_      P� 5`       � {� .  . �5a   "   �� # � ��6?    � ��5b      5d      5f      5g      �� 5j   
   " 9�6&   .�5k      ��  � �� 5l      �� 5m      " 9�6&   .R�5n      �  5o      �� 5p   "   $��30 �60   �� 5q   "   $ 
ContentRow��60   $��0 �6B   .+�5s      �� $��Border��5t   !   $��0��5u   !   $��1q�5v   !   $  5,5,0,0��5w   !   ._�5x   "   � .+�5y   &   �� $��0q�5z   )   $��Header��5{   )   ��. 5|   )   d� 5}   &   $! 
HeaderSite��63   #��z��5~   '   #��z��5   '   #��z��5�   '   #���  5�   '   " 5�   '   �� *
IsExpanded+$# TwoWay=�$ �� *TemplatedParent+5�   '   5�   #   5�      �� 5�      $��Content��5�   !   $��1��5�   !   $��1,0,1,1q�5�   !   $  0,0,4,4��5�   !   ._�5�   "   �� $��0q�63   5�      5�       5�      P� 5�       �@9�
IsExpanded . . �5�   "   �� $ 
ContentRow��6)    5�   )   �� $% Content��$& DesiredHeight� � ��5�      5�      5�      5�      �� 5�   
   " ��6.   .�5�      ��  � ��$ 12(�6)   �� 5�       � ��$ 24��6'   5�      5�      ' J� CommandBindings' 5�      )  #System.Windows.Input.CommandBinding  5�   
   -   5        (   Command$ ( ApplicationCommands.Delete��5�      5�        5�   
   -   5        $( ApplicationCommands.Cut��5�      5�        5�   
   -   5        $( ApplicationCommands.Copy��5�      5�        5�   
   -   5        $( ApplicationCommands.Paste��5�      5�        5�   
   -   5        $1( +localUtil:LocalViewModel.NavigationBackward��5�      5�        5�   
   -   5        $0( *localUtil:LocalViewModel.NavigationForward��5�      5�        5�   
   -   5        $2( ,localUtil:LocalViewModel.DownloadFileCommand��5�      5�        5�   
   -	   5        $7( 1localUtil:LocalViewModel.CancelDownloadingCommand��5�      5�        5�   
   -
   5        $2( ,localUtil:LocalViewModel.PersonRenameCommand��5�      5�        5�   
   -   5        $-( 'localUtil:LocalViewModel.ViewDocCommand��5�      5�      .��5�      � -   5        ) �Name$) MainGrid��5�      .+�5�   
   3  -Hardcodet.Wpf.TaskbarNotification.TaskbarIcon  -   5        *  Name$* tb��5�      +   
IconSource$ + /Assets/Images/easi365.ico��5�      ,   LeftClickCommand#, [  5�      -   DoubleClickCommand#- [  5�       .   LeftClickCommandParameter. 5�      �� $
% MaxW��"/   DoubleClickCommandParameter/ 5�      �� $
% MaxW��0 � ContextMenu0 5�      �� 5�      .@�5�      m� -   5        1 m�Name$1 tbMainWindowMenuItem��5�      $��	主界面��6=   6J   �� 5�      m� 5�      -   5        $1 tbExitMenuItem��5�      $��退出��67   6C   5�      5�      �� 5�   
   -   5        2 ��Name$2 
MainBorder��5�      
�������
���   �$  3��6L   $��1q�6]   $
��Auto��6q   ._�5�      � -   5        $��
HeaderGrid��5�      .+�5�      �� -   5        3 ��Name$3 ContentHeader��5�   !   $��70��68   $	��Top=�6D   .��5�      � $	��Top=�6   ��5�      �� 5�      4 �� ImageSource$4 /assets/images/top-1.png��6)   .+�5�      �� $
��Left=�6!   $��80��6<   $	��Top=�6H   .+�5�      �� -   5        5 ��Name$5 wTitle��5�   (   $��纳通医疗��68   $��13(�6D   
�������$
��Bold"�6e   6 J� RenderTransformOrigin$6 0.636,0.664&�6w   7 �� Left$7 10��6�   $ 8��6�   5�      �� 5�      $��right=�6%   $��22��6A   $	��Top=�6M   $��24��6e   $��0,0,84,0q�6p   .+�5�      +   %Easi365UI.Windows.Controls.EasiButton  -   5        8  Name$8 SettingsBtn��5�   -   +	   %Easi365UI.Windows.Controls.EasiButton9 	  Click$9 SettingsBtn_Click_1��6B   $	��Top=�6^   $��27��6v   $��22��6�   $6 -0.519,0.092&�6�   ��5�   "   �� 5�   &   $%4 /assets/images/settings_ctx.png��61   :   MyEnterBrush: 5�   "   �� 5�   &   $%4 /assets/images/settings_ctx.png��61   ;   MyMoverBrush; 5�   "   �� 5�   &   $%4 /assets/images/settings_ctx.png��61   5�      5�      �� 5�      $��Right=�6%   $��1��6A   $��28��6K   $��0,0,84,0q�6W   $	��Top=�6i   .+�5�      �� $��28��6$   $ 3��60   $��1��6?   $��/assets/images/line.png��6I   5�      �� 5�      $��right=�6%   $��22��6A   $	��Top=�6M   $��27��6e   $��0,0,56,0q�6p   .+�5�        -   5        $8 MinBtn��5�   -   $9 MinBtn_Click��6�   $	��Top=�6=   $��27��6U   $��22��6`   $6 -0.519,0.092&�6l   < � ToolTip$< 	最小化��6�   ��5�   "   �� 5�   &   $4 /assets/images/min.png��61   : 5  "   �� 5  &   $4 /assets/images/mim.png��61   ; 5  "   �� 5  &   $4 /assets/images/mim.png��61   5     5     �� 5	     $��Right=�6%   $��1��6A   $��28��6K   $��0,0,55,0q�6W   $	��Top=�6i   .+�5
     �� $��28��6$   $ 3��60   $��1��6?   $��/assets/images/line.png��6I   5     �� 5     $��right=�6%   $��22��6A   $	��Top=�6M   $��27��6e   $��0,0,29,0q�6p   .+�5       -   5        $8 MaxBtn��5  -   $9 MaxBtn_Click��6l   $	��Top=�6=   $��27��6U   $��22��6`   $< 	最大化��6�   ��5  "   �� 5  &   $4 /assets/images/max.png��61   : 5  "   �� 5  &   $4 /assets/images/maxb.png��61   ; 5  "   �� 5  &   $4 /assets/images/maxb.png��61   5     5     �� 5     $��Right=�6%   $��1��6A   $��28��6K   $��0,0,27,0q�6W   $	��Top=�6i   .+�5     �� $��28��6$   $ 3��60   $��1��6?   $��/assets/images/line.png��6I   5     �� 5     $��Right=�6%   $��22��6A   $��27��6M   $��0,0,0,0q�6X   $	��Top=�6i   .+�5       -   5        $8 CloseBtn��5  -   $9 CloseBtn_Click��6�   $��Right=�6?   $	��Top=�6[   $��27��6s   $��22��6~   $< 关闭��6�   ��5  "   �� 5  &   $4 /assets/images/close.png��61   : 5!  "   �� 5"  &   $4 /assets/images/closeb.png��61   ; 5$  "   �� 5%  &   $4 /assets/images/closeb.png��61   5'     5(     �� 5)     -   5        $��个人空间��5)  !   $
��Left=�60   $��	39,34,0,0q�6K   $	��Top=�6^   $��75��6v   6�   �� 5*     -   5        $��部门空间��5*  !   $
��Left=�60   $��
153,34,0,0q�6K   $	��Top=�6_   $��75��6w   6�   �� 5+     -   5        $��公司空间��5+  !   $
��Left=�60   $��
266,34,0,0q�6K   $	��Top=�6_   $��75��6w   6�   �� 5,     $��Right=�6$   $��
0,35,100,0q�6@   $
��Wrap=�6T   $��
欢迎您,��6h   $	��Top=�6t   $��1��6�   �� 5-     $
��Wrap=�6$   $��0,7,50,0q�6U   $��Right=�6g   $	��Top=�6�   $��1��6�   ��68   �� *DisplayName+6�   5.     5/     � 50     -   5        $) DataGrid��50     $��0,50,0,0q�6)   $	��Top=�6;   = � ColumnDefinitions= 51     �� 52     $
��568* �6+   �� 53     $
��405* �6+   .+�56     � -   5        $) persSpaceGrid��56     $��0,10,0,0q�63   $��2��6E    58     �� 59     $��30 �6,   �� 5:     $��* �6,   .+�5<     � $��0��6   $��30��6,   $6 0.501,0.502&�68   ��5>     �� 5?  "   $4 /assets/images/top-3.png��6-   = 5A     �� 5B  "   $	��150 �63   �� 5C  "   �� 5D  "   $	��200 �63   .+�5G     �� $��0��5H     $
 
Horizontal=�5I     $
��Left=�5J     $��4,0,0,0q�5K     $��90��6.   .+�5L  "   �� -   5        > ��Name$> NavHomeButton��5M     $	��Top=�5O       NavHomeButton#��[ 5P      . 5Q     �� 5R  "   $1��+localUtil:LocalViewModel.NavigationBackward��5S     $
��Left=�5T       NavBackButton#���  5U      . $��PersonalBack��5W     �� 5X  "   $0��*localUtil:LocalViewModel.NavigationForward��5Y       NavForwardButton#���  5Z      . $��3,3,3,0q�5[  9   $	��Top=�6J   $��Personalforward��6b   5\     $
  Odyssey.Controls.BreadcrumbBar
  5^     -   5        ? 
 Name$	? bar��5^  0   $y�	Collapsed=�6=   |�. $��1��5_     @ 
  TraceBinding@ 5c     �� *Folder+A 
  ImageBindingA 5d     �� *Image+5e     �� 5f     $
 
Horizontal=�6)   $
��Left=�6B   $��1��6]   $��2��6m   $	��686��6�   .+�5g  "   �� $��kaiti)�6(   $��12(�6;   
�������$��文件路径��6[   �� 5h  "   -   5        B ��Name$B file_url��5h  4   $	��400��6(   64   5j     �� 5k     $
 
Horizontal=�6)   $��Right=�6B   $��2��5l     $��0,0,6,1q�5m     $	��180��6.   .+�5n  "   �� -    5        C ��Name$C 	file_name��5n  6   $	��120��6*   6J   �� 5o  "   $,��&/assets/images/button-search-enter.png��6(   $y�	Collapsed=�6X   �� 5p  "   -!   5        .��5q  &   �� $,��&/assets/images/button-search-enter.png��6,   5r  #     5s  "   -"   5        $8 btnCollapse��5s  1   $y�	Collapsed=�6F   $��0,0,10,0q�6^   $��16��6p   $��16��6{   $< 收起侧边栏��6�   ��5t  &   �� 5u  *   $&4  /assets/images/expansion_nav.png��65   : 5w  &   �� 5x  *   $&4  /assets/images/expansion_nav.png��65   ; 5z  &   �� 5{  *   $&4  /assets/images/expansion_nav.png��65   5}  #     5~  "   -#   5        $8 btnCollapse1��5~  1   $y�	Collapsed=�6G   $��0,0,10,0q�6_   $��16��6q   $��16��6|   $< 收起侧边栏��6�   ��5  &   �� 5�  *   $&4  /assets/images/expansion_nav.png��65   : 5�  &   �� 5�  *   $&4  /assets/images/expansion_nav.png��65   ; 5�  &   �� 5�  *   $&4  /assets/images/expansion_nav.png��65   5�  #     5�  "   -$   5        $8 btnChangeView��5�  1   $9 btnChangeView_Click��6�   $��Right=�6H   $��6,0,6,1q�6d   $��16��6u   $��16��6�   $< 切换到缩略图模式��6�   $6 
-2.875,0.5&�6�   ��5�  &   �� 5�  *   $%4 /assets/images/details_view.png��65   : 5�  &   �� 5�  *   $%4 /assets/images/details_view.png��65   ; 5�  &   �� 5�  *   $%4 /assets/images/details_view.png��65   5�  #   5�     5�     � 5�     $��1��6   .+�5�     � = 5�  "   �� 5�  &   -%   5        D ��Name$D grid_Nav��5�  7   $	��180 �6I   $	��120��6U   $	��300��6d   �� 5�  &   -&   5        $D grid_NavBar��5�  7   $��2 �6L   �� 5�  &   $��* �67   .+�5�  "   �� -'   5        E ��Name$E FileDocList��5�  �   $��0,0,0,40q�6-   $
��Auto��6?   $
��Auto=�6L   $
 Vertical=�6|   5�  #   �� 5�  "   $��1��6/   $
��Left=�6?   $��1��6Z   ���#CCC6d   �� 5�  "   $��2��6-   $
 Vertical=�6=   .+�5�  &   � -(   5        $) grdFileList��5�  +   .+�5�  *   � $��0��6/   .+�5�  .   �� -)   5        $C txtViewMode��5�  6   $��Details��6K   $y�	Collapsed=�6Z   �� 5�  .   -*   5        F ��Name$F FileListView��5�  7   G �� SelectionMode$G Extended=�5�  !   H J� 	AllowDropH . *   $Easi365UI.Extention.ListViewBehaviorI   
HeaderSortI . $��Disabled=�5�  !   ��.  . $
��Auto=�5�  !   $��0q�5�  !   $��0q�5�  T   ��5�  !   �� *ListViewItems+J J� InputBindingsJ 5�  2   �� 5�  6   
K �� Key$
K Back��6A   L �� Command$1L +localUtil:LocalViewModel.NavigationBackward��6L   0 5�  2   �� 5�  6   M J� BitmapEffectM 5�  :   �� 5�  >   .@�5�  :   m� -+   5        $1 FileListCtmUploadFile��5�  u   $��上传文件...��6C   6u   m� 5�  :   -,   5        $1 FileListCtmNewFolder��5�  m   $��新建文件夹��6C   6m   �� 5�  :   m� 5�  :   --   5        $1 ViewWay��5�  O   $��查看��6C   .@�5�  >   m� -.   5        $1 ThumbnailsView��5�  T   $��	缩略图��6G   6l   m� 5�  >   -/   5        $1 DetailsView��5�  U   $��详细信息��6G   6j   5�  ;   m� 5�  :   -0   5        $��刷新��5�  C   6O   m� 5�  :   -1   5        $1 PasteMenuItem��5�  $   $��粘贴��5�  $   N m� Command$N Paste��5�  $   O m� CommandParameter$O person��64   m� 5�  :   -2   5        $ 1 ListViewRecycleBinMenuItem��5�  C   $��	回收站��6g   $y�	Collapsed=�6t   5�  7   ��5�  2   �� 5�  6   " ��6<   .�5�  :   -3   5        �� 5�  :    � ��$ Stretch=�6g   �� 5�  :    � ��$	 Top=�6^   �� 5�  :    �@�ContextMenu 5�  >   �� 5�  B   
�������.@�5�  F   m� $1 MenuItemEditOnline��5�  '   $��在线编辑��5�  '   $-N 'localUtil:LocalViewModel.ViewDocCommand��5�  C   $O ViewDoc��6u   m� 5�  F   $1 MenuItemViewOnline��5�  '   $��在线查看��5�  '   $-N 'localUtil:LocalViewModel.ViewDocCommand��5�  C   $O OnlyViewDoc��6u   m� 5�  F   $1 MenuItemCopy��5�  '   $��复制��5�  '   $
N Copy��5�  '   $O PersonalCopy��66   m� 5�  F   $1 MenuItemCut��5�  '   $��剪切��5�  '   $	N Cut��5�  '   $O PersonalCut��65   m� 5�  F   $1 	DeleteCtm��5�  '   $��删除��5�  O   $ N ApplicationCommands.Delete��6[   $O personal��5�  '   5�  '   m� 5�  F   $1 DownloadCtm��6[   $��下载��6O   $2N ,localUtil:LocalViewModel.DownloadFileCommand��6p   $O PersonalViewDownload��5�  '   m� 5�  F   $��	重命名��6O   $2N ,localUtil:LocalViewModel.PersonRenameCommand��6\   $O person��6�   �� 5�  F   5�  C   5�  ;   5�  7   ��5�  2   �� 5�  6   " ��6<   P �� TriggersP 5�  :   �� 5�  >   Q �� BindingQ 6J   �� *Text+$% txtViewMode��R �� Value	R Thumb6|   .a�5�  B   ��  � ��$ Disabled=�6   �� 5�  B    � �� 5�  F   �� 5�  J   .R�5�  N   � 5�  K   5   C   �� 5  B    � �� 5  F   �� 5  J   .R�5  N   � S � Orientation$S Vertical=�6X   $��3q�6o   $��80��6z   < 6�   �� *Name+.+�5  R   �� $��Center=�6X   $	��0,2q�6u   ��5  V   `� 5  Z    	 stringToImageT `� 	Converter#T [	 6g   .1�5	  ^   �� $
& Name�6f   �� 5
  ^   $& IsDirectory�6f   5  [   5  S   +   %Easi365UI.Extention.EditableTextBlock  5  R   U   
Lost_FocusU PersonFileRename_LostFocus6j   $��Center=�61  	V   IdV 6�   �� *ServerItem.Id+W   IsDirectoryW 6�   �� *IsDirectory+X   IsInEditModeX 6�   �� *IsInEditMode+Y   TextY 6�   �� *Name+Z   OriginalPathZ 6  �� *
FullPath+61  5  O   5  K   5  C   5  ?   �� 5  >   Q 6K   �� *Text+$% txtViewMode��R Details6}   .a�5  B   ��  �@��View 5  F   �� 5  J   .P�5  N   �� [ �� Width$	[ 200��6]   \   	SortField$\ IsDirectory��6i   $��	文件名��6�   ] �� CellTemplate] 5  R   �� 5  V   .R�5  Z   �� $
 
Horizontal=�6e   < 6~   �� *Name+.+�5  ^   �� $	��5,0q�6d   $��18��6q   $��18��6|   ��5   b   `� 5!  f   #T [	 6s   .1�5"  j   �� $
& Name�6r   �� 5#  j   $& IsDirectory�6r   5$  g   5&  _     5'  ^   $	��0,2q�6v   U PersonFileRename_LostFocus6�   $��Center=�6K  V 6�   �� *ServerItem.Id+W 6�   �� *IsDirectory+X 6�   �� *IsInEditMode+Y 6  �� *Name+Z 6)  �� *
FullPath+6K  5(  [   5)  W   5+  O   �� 54  N   $	[ 140��6]   $\ Modified��6�   $��修改时间��6�   ^ �� DisplayMemberBinding^ 6i   �� *
Modified+_ �� StringFormat$_ {0:yyyy-MM-dd HH:mm}��6�   �� 56  N   $	[ 100��6]   $��大小��6i   $
\ Size��6u   ] 57  R   �� 58  V   .R�59  Z   �� ` �� TextAlignment$` Right=�6d   $��Right=�6z   ��5:  ^   `� 5;  b    
 fileSizeFormat#T [
 6o   .1�5<  f   �� $
& Size�6n   �� 5=  f   $& IsDirectory�6n   5>  c   5@  [   5A  W   5C  O   �� 5D  N   $	[ 100��6]   ] 5E  R   �� 5F  V   .R�5G  Z   �� ��6d   �� *IsLocalCached+5H  W   5J  O   �� 5K  N   $	[ 100��6]   $�� ��6i   ] 5L  R   �� 5M  V   .R�5O  Z   �� $
 
Horizontal=�5P  ,   y�5Q  ,   �� *DownloadState.IsDownloading+  boolToVisibilitya �� 	Converter#a [ .+�5S  ^   �� $��Center=�5T  -   $��Center=�5U  -   $��60��5V  -   $` Right=�5X  -   ��5W  -   �� *DownloadState.DownloadPercent+$_ {0:P}��5X  -   �� 5Y  ^   $7��1localUtil:LocalViewModel.CancelDownloadingCommand��5Z  -     CancelButton#��[ 5\  -   
b � Tagb 5[  -   �� *DownloadState+.��5]  b   �� $��取消��6l   
����  �c � Cursor$
c Hand��6�   5^  _   5_  [   5`  W   5b  O   5c  K   5e  C   5f  ?   5h  7   5j  /   �� 5k  .   $	��Top=�5l  !   $��该文件夹为空。��5n  !   $��25��5o  !   $	��120��5p  !   $��30, 50q�5q  !   
���   �$` Center=�5s  !   y�5m  !   �� *ListViewItems.Count+d �� ConverterParameter$d FolderIsEmpty��  numberToVisibility#a [ 5s  !      Easi365UI.Windows.Loading  5t  .   -4   5        e  Name$e DeptLoading��5t  >   f   LoadingText$f 正在加载...��6S   $y�	Collapsed=�6i   5u  +   (  "System.Windows.Controls.WebBrowser  5y  *   -5   5        g  Name$g borderDocumentViewr��5y  Z   $��2��65   $	��Top=�6E   $y�	Collapsed=�6w   5z  '   5{  #   5|     5}     5~     5     �� 5�     -6   5        $ 
toolCanvas��5�     $��25��5�     $��Bottom=�5�     $
��Auto��5�     ���#F4F5F7.+�5�     � $��50��6R   ��6   �� *ActualWidth+$% 
toolCanvas��= 5�     �� 5�     $��* �6/   �� 5�     $��* �6/    5�     �� 5�     $��* �6,   �� 5�     $��* �6,   .+�5�     � $y�	Collapsed=�5�     $��0��5�     $��0��5�     $��2��5�     h � DataContexth 5�     �� *SelectedItem+$% FileListView��= 5�     �� 5�  "   $��* �63   �� 5�  "   $��* �63    5�     �� 5�  "   $��* �60   �� 5�  "   $��* �60   .+�5�     �� $
 
Horizontal=�6)   $��0��6B   $��0��6O   .+�5�  "   �� $��文件名：��6,   $��Center=�68   $��Center=�6U   �� 5�  "   $��Center=�6B   $��Center=�6_   ��6,   �� *Name+6_   5�     �� 5�     $
 
Horizontal=�6)   $��0��6B   $��1��6O   .+�5�  "   �� $��	大小：��6,   $��Center=�67   $��Center=�6T   �� 5�  "   $` Center=�6,   $��Center=�6C   $��Center=�6`   ��5�  &   `� 5�  *   #T [
 67   .1�5�  .   �� $
& Size�66   �� 5�  .   $& IsDirectory�66   5�  +   5�  #   5�     �� 5�     $
 
Horizontal=�6)   $��1��6B   $��1��6O   .+�5�  "   �� $��修改日期：��6,   $��Center=�69   $��Center=�6V   �� 5�  "   $��Center=�6k   $��Center=�6�   ��6,   �� *
Modified+$_ {0:yyyy-MM-dd HH:mm}��6�   5�     5�     � 5�     $��1��6   $��0��6/   y�5�     �� *UploadingFiles.Count+$d UploadingFilesCount��#a [ .+�5�     �� $
 
Horizontal=�6)   $��Right=�6B   .+�5�  "   �� $��还剩 ��6,   $��Center=�67   $��Center=�6T   �� 5�  "   $��Center=�6S   $��Center=�6p   ��6,   �� *UploadingFiles.Count+6p   �� 5�  "   $�� 个文件需要上传��6,   $��Center=�6=   $��Center=�6Z   �� 5�  "   -7   5        
������ $��0q�5�  
   $��	10,0,30,0q�5�  
   .��5�  &   �� $
 
Horizontal=�61   .+�5�  *   �� $*��$Assets/Images/UploadDetailButton.png��60   $
��None=�6^   �� 5�  *   $��详细��64   $��3,0,0,0q�6>   $
��Left=�6P   $��Center=�6k   5�  '   5�  #   5�     5�     5�     5�       5�     -8   5        $e fileListOperation��5�  "   $f  ��5�     $y�	Collapsed=�5�     5�     5�     5�     5�     