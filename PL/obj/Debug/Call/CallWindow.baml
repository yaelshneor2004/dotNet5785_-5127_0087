   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:PL.CallPL.Call  5   	       PL.Call.CallWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:PL.Call     5   	   -   5          � Title$  
CallWindow��5   	   $	��700��6   $	��500��6)    � WindowStartupLocation$ Manual=�5	   K    � Left$ 0��5
   	   
 � Top$ 0��6    � Language$ he-IL�5   	    � DataContext 5	   	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      �� $
��Auto=�6   .��5   
   �� $��10q�6   .+�5      �� $��Call Details��6   #��[B��6+   
����  �$
��Bold"�6e   $��20(�6w   $��0,0,0,10q�6�   �� 5      $	��ID:��6   #��[B��6"   
����  �$��0,5,0,5q�6\   y�6m   �� *CurrentCall.Id+    ConvertIdCalltoVisibilityKey �� 	Converter# [  �� 5      	 �� 
IsReadOnly$
	 True��6C   #��[B��6U   
���   �$��0,5,0,5q�6�   ��6   �� *CurrentCall.Id+
 �� Mode$
 TwoWay=�y�6�   �� *CurrentCall.Id+# [  �� 5      $��Type:��6   #��[B��6$   
����  �$��0,5,0,5q�6^   �� 5      #��[B��6�   
���   �$��0,5,0,5q�6  
���������6   ��   SortCollectionMyCallTypeKey �� Source# [  �� SelectedItem 6c   �� *CurrentCall.Type+ �� SelectedValue 6�   �� *CurrentCall.Type+$
 TwoWay=�{�5      �� *CurrentCall+ $  ConvertCallComboboxToReadOnlyKey# [ 6q   �� 5      $��Description:��6   #��[B��6+   
����  �$��0,5,0,5q�6e   �� 5        ValidationErrorTemplate =� ErrorTemplate# [ 6   #��[B��6Z   
���   �$��0,5,0,5q�6�   	 6�   �� *CurrentCall+  ConvertCallToReadOnlyKey# [ .��6�   ��5      �� 5       �� Path$ CurrentCall.Description�6   $
 TwoWay=�6=    �� UpdateSourceTrigger$ PropertyChanged=�6K    �� ValidationRules 5      �� 5       �� ResourceKey$ descriptionValidationKey��6-   5      .��5   !   5      �� 5       $��Address:��6   #��[B��6'   
����  �$��0,5,0,5q�6a   �� 5"      # [ 6   #��[B��6[   
���   �$��0,5,0,5q�6�   	 6�   �� *CurrentCall+# [ .��6   ��5#      �� 5$      $ CurrentCall.Address�6   $
 TwoWay=�69   $ PropertyChanged=�6G    5%      �� 5&      $ addressValidationKey��6-   5(      .��5)   !   5*      �� 5+      $��Start Time:��6   #��[B��6*   
����  �$��0,5,0,5q�6d   �� 5,      #��[B��6=   
���   �$��0,5,0,5q�6z   $
	 True��6�   ��6   �� *CurrentCall.StartTime+6�   �� 5.      $��Max End Time:��6   #��[B��6,   
����  �$��0,5,0,5q�6f   �� 5/      # [ 6   #��[B��6�   
���   �$��0,5,0,5q�6�   	 6\   �� *CurrentCall+  ConvertMaxEndToReadOnlyKey# [ .��6  ��50      �� 51      $ CurrentCall.MaxEndTime�6   $
 TwoWay=�6<   $ PropertyChanged=�6J    52      �� 53      $ dateValidationKey��6-   55      .��56   !   57      �� 59      $��Status:��6   #��[B��6&   
����  �$��0,5,0,5q�6`   �� 5:      #��[B��6:   $
	 True��6d   
���   �$��0,5,0,5q�6�   ��6   �� *CurrentCall.Status+6�   �� 5<      $��Assignment List:��6   #��[B��6/   
����  �$��0,20,0,5q�6i   y�6{   �� *CurrentCall+  ConvertCallToVisibilityKey# [ &   System.Windows.Controls.DataGrid  5=         AutoGenerateColumns$ False��6   $��0,20,0,5q�6c      
IsReadOnly$
 True��6�   ��63   �� *CurrentCall.Assignments+y�6u   �� *CurrentCall+# [    Columns 5>      0  *System.Windows.Controls.DataGridTextColumn  5?      ,  &System.Windows.Controls.DataGridColumn   Header$ Volunteer ID��6)      
Foreground
 �   �1  +System.Windows.Controls.DataGridBoundColumn   Binding 6?   �� *VolunteerId+6_     5@      $ Volunteer Name��6)   
 �   � 6A   �� *VolunteerName+6c     5A      $ Start Treatment Time��6)   
 �   � 6G   �� *StartTreatmentTime+6n     5B      $ End Treatment Time��6)   
 �   � 6E   �� *EndTreatmentTime+6j   4  .System.Windows.Controls.DataGridComboBoxColumn  5C      $ End Type��6-      ItemsSource 6?   ��  ! SortCollectionMyFinishTypeKey# [    SelectedItemBinding 6�   �� *	EndType+   SelectedValueBinding 6�   �� *	EndType+$
 TwoWay=�5E      �� 5F      -   5        $	��255��5F   4   $��Center=�6[   $��40��6x   $��	0,23,0,43q�6�   ��6   �� *
ButtonText+6�   5G      5H      5I      