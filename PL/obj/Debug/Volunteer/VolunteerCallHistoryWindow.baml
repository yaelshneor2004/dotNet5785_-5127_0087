   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PL.VolunteerPL.Volunteer  5   	   -    'PL.Volunteer.VolunteerCallHistoryWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:PL.Volunteer     5   	   -   5          � Title$   VolunteerCallHistoryWindow��5   	   $	��350��6,   $	��800��69    � Language$ he-IL�5
   	    � WindowStartupLocation$ CenterScreen=�5       � DataContext 5	   	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��Center=�6.   .+�5      �� -   5        $��43��5      $	��789��5       �� SelectedValue 5      �� *FilterByType+	 �� Mode$	 TwoWay=���5      ��    SortCollectionMyCallTypeKey
 �� Source#
 [  5      5      &   System.Windows.Controls.DataGrid  5!   
   $��1��6   
�������   AutoGenerateColumns$ False��5"         
IsReadOnly$
 True��5$      $��0,8,0,-8q�6%   ��5#      �� *CloseCallList+$	 TwoWay=�   Columns 5%      0  *System.Windows.Controls.DataGridTextColumn  5&      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Id+  5'      $
 Type��6%    63   �� *Type+  5(      $ Address��6%    66   �� *	Address+  5)      $ 
Start Time��6%    69   �� *	StartTime+  5*      $ Start Treatment Time��6%    6C   �� *StartTreatmentTime+  5+      $ End Time��6%    67   �� *	EndTime+  5,      $ End Type��6%    67   �� *	EndType+5.      50      51      