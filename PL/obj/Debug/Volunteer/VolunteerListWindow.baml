   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PL.VolunteerPL.Volunteer  5   	   &     PL.Volunteer.VolunteerListWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:PL.Volunteer     5   	   -   5          � Title$  VolunteerListWindow��5   	   $	��640��6%   $	��800��62    � Language$ he-IL�5
   	    � WindowState$ 	Maximized=�6H    � DataContext 5	   	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��Stretch=�6   $
��auto��6-   $��0��6;   $��Stretch=�6H   $
��auto��6d    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   5      �� 5   
   $��Label��6   $��0��6    $
��Left=�60   $��	90,10,0,0q�6K   $	��Top=�6^   �� 5   
   -   5        $��	5,0,6,540q�5   �   $��2��6�    �� SelectedValue 6   �� *SortInVolunteerInList+	 �� Mode$	 TwoWay=���6P   ��    SortCollectionMyCallTypeKey
 �� Source#
 [  6�   &   System.Windows.Controls.DataGrid  5   
   -   5        
�������$��1��5   &   $��5q�63      AutoGenerateColumns$ False��6k      SelectionUnit$ FullRow=�6�      
IsReadOnly$
 True��5         RowDetailsVisibilityMode$ VisibleWhenSelected=�6%      EnableRowVirtualization$
 True��6T   ��5   >   ��  �� Path$ VolunteerInList� �� SelectedItem 6�   �� *SelectedVolunteer+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Id��6<      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6H   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *Id+6H     5      $ 	Full Name��6B   $
 Auto 6U    6%   �� *
FullName+6U   4  .System.Windows.Controls.DataGridTemplateColumn  5       $ 	Is Active��6)   $
 Auto 6<      CellTemplate 5!      �� 5"      .R�5#      �� ${�False��6S    d� 	IsChecked 6'   �� *
IsActive+$	 OneWay=�6S   5$      5&        5'      $ Total Calls Handled��6K   $
 Auto 6h    6%   �� *TotalCallsHandled+6h     5(      $ Total Calls Cancelled��6M   $
 Auto 6l    6%   �� *TotalCallsCancelled+6l     5)      $ Total Calls Expired��6K   $
 Auto 6h    6%   �� *TotalCallsExpired+6h     5*      $ Current Call Id��6G   $
 Auto 6`    6%   �� *CurrentCallId+6`     5+      $ Current Call Type��6I   $
 Auto 6d    6%   �� *CurrentCallType+6d     5,      $ Actions��6)   $
 Auto 6:    5-      �� 5.      .R�5/      �� -   5        $��Delete��5/   %   y�6T   �� *.+ ! ConvertVisibilityDeleteVolKey �� 	Converter# [ 50      52      54      �� 55   
   -   5        $	��Add��55      $��2��6   $��Center=�6,   $��50��6I   $	��232��6U   $��0,5,0,5q�6�   56      57      