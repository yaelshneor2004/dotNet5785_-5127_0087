   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:PL.CallPL.Call  5   	       PL.Call.CallListWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:PL.Call     5   	   -   5          � Title$  CallListWindow��5   	   $	��640��6    $	��800��6-    � Language$ he-IL�5
   	    � WindowState$ 	Maximized=�6X    � DataContext 5	   	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      �  � RowDefinitions 5   
   �� 5      $	��40* �6   �� 5      $
��500* �6   �� 5      $
��Auto �6   .+�5   
   � $
��auto��6   $��0��6   $
��auto��6*   $��0,-6,0,6q�67    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� -   5        $��
-3,7,6,-12q�5   �   $��2��6�    J� RenderTransformOrigin$ 0.5,0.5&�6�   	 �� SelectedValue	 5      �� *SortInCallInList+
 �� Mode$
 TwoWay=���5      ��    SortCollectionCallStatusKey �� Source# [  6�   5      &   System.Windows.Controls.DataGrid  5   
   -   5        
�������$��1,0,5,12q�5   &   $��1��68      AutoGenerateColumns$ False��6f      
IsReadOnly$
 True��6�      EnableRowVirtualization$
 True��6�      RowDetailsVisibilityMode$ VisibleWhenSelected=�6�   $	��514��6;  $��Bottom=�6H  ��6E   �� *
CallList+ �� SelectedItem 6  �� *SelectedCall+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Assignment Id��6<      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6S   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *Id+6S     5      $ Call Id��6@   $
 Auto 6Q    6%   �� *CallId+6Q     5      $
 Type��6>   $
 Auto 6L    6%   �� *Type+6L     5       $ 
Start Time��6C   $
 Auto 6W    6%   �� *	StartTime+6W     5!      $ Time Remaining��6G   $
 Auto 6_    6%   �� *TimeRemaining+6_     5"      $ L Volunteer Name��6K   $
 Auto 6e    6%   �� *LastVolunteerName+6e     5#      $ Completion Time��6H   $
 Auto 6a    6%   �� *CompletionTime+6a     5$      $ Status��6@   $
 Auto 6P    6%   �� *Status+6P     5%      $ Total Assignments��6J   $
 Auto 6e    6%   �� *TotalAssignments+6e   4  .System.Windows.Controls.DataGridTemplateColumn  5&      $ Delete��6)   $
 Auto 69      CellTemplate 5'      �� 5(      .R�5)      �� -   5        $��Delete��5)   %   y�6O   �� *.+ * &ConvertDeleteAssignmentToVisibilityKey �� 	Converter# [ 5*      5,        5.      $ Cancal Assignment��6)   $
 Auto 6D    5/      �� 50      .R�51      �� -   5        $��Cancal Assignment��51   %   y�6`   �� *Status+ * &ConvertCancalAssignmentToVisibilityKey# [ 52      54      56      �� 57   
   -   5        $	��Add��57      $��2��6   $��Center=�6,   $��50��6I   $	��257��6U   $��0,5,0,5q�6v   58      59      