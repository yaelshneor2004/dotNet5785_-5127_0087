   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PL.VolunteerPL.Volunteer  5   	   "    PL.Volunteer.VolunteerWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:PL.Volunteer     5   	   -   5          � Title$  VolunteerWindow��5   	   $	��900��6!   $	��500��6.    � WindowStartupLocation$ Manual=�6:    � Left$ 0��5	   	   
 � Top$ 0��6    � Language$ he-IL�5   	    � DataContext 5
   	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      �� $
��Auto=�6   .��5   
   �� .+�5      �� $��Volunteer Details��6   #��[B��60   
����  �$
��Bold"�6j   $��16(�6|   $��0,0,0,10q�6�   �� 5      $	��ID:��6   #��[B��6"   
����  �6J   �� 5         ValidationErrorTemplate =� ErrorTemplate# [  6   #��[B��6�   
���   �	 �� 
IsReadOnly	 6[   �� *CurrentVolunteer.Id+  ConvertObjIdToTFKey
 �� 	Converter#
 [ .��6�   ��5      �� 5       �� Path$ CurrentVolunteer.Id�6    �� Mode$ TwoWay=�69    �� UpdateSourceTrigger$ PropertyChanged=�6G    �� ValidationRules 5      �� 5       �� ResourceKey$ idNumValidationKey��6-   5      .��5   !   5      �� 5      $��
Full Name:��6   #��[B��6)   
����  �6Q   �� 5      # [  6   #��[B��6[   
���   �.��6�   ��5      �� 5      $ CurrentVolunteer.FullName�6   $ TwoWay=�6?   $ PropertyChanged=�6M    5      �� 5      $ nameValidationKey��6-   5!      .��5"   !   5#      �� 5$      $��Phone:��6   #��[B��6%   
����  �6M   �� 5%      # [  6   #��[B��6[   
���   �.��6�   ��5&      �� 5'      $ CurrentVolunteer.Phone�6   $ TwoWay=�6<   $ PropertyChanged=�6J    5(      �� 5)      $ phoneValidationKey��6-   5+      .��5,   !   5-      �� 5.      $��Email:��6   #��[B��6%   
����  �6M   �� 5/      # [  6   #��[B��6[   
���   �.��6�   ��50      �� 51      $ CurrentVolunteer.Email�6   $ TwoWay=�6<   $ PropertyChanged=�6J    52      �� 53      $ emailValidationKey��6-   55      .��56   !   57      �� 58      $��	Password:��6   #��[B��6(   
����  �6P   �� 59      # [  6   #��[B��6Z   
���   �.��6�   ��5:      �� 5;      $ CurrentVolunteer.Password�6   $ TwoWay=�6?   $ PropertyChanged=�6M    5<      �� 5=      $ passwordValidationKey��6-   5?      .��5@   !   5A      �� 5B      $��Address:��6   #��[B��6'   
����  �6O   �� 5C      # [  6   #��[B��6[   
���   �.��6�   ��5D      �� 5E      $ CurrentVolunteer.Address�6   $ TwoWay=�6>   $ PropertyChanged=�6L    5F      �� 5G      $ addressValidationKey��6-   5I      .��5J   !   5K      �� 5L      $��Role:��6   #��[B��6$   
����  �6L   �� 5M      #��[B��5P      
���   ���5M      ��   SortCollectionRolesKey �� Source# [  �� SelectedItem 5N      �� *Role+ �� SelectedValue 5O      �� *CurrentVolunteer.Role+$ TwoWay=�5Q      �� 5S      $��
Is Active:��6   #��[B��6)   
����  �6Q   �� 5T      #��[B��6T   $	��409��6�    d� 	IsChecked 6   �� *CurrentVolunteer.IsActive+$ TwoWay=�{�6   �� *CurrentVolunteer+ ! ConvertVolunteerToReadOnlyKey#
 [ 6�   �� 5U      $��Max Distance:��6   #��[B��6,   
����  �6T   �� 5V      # [  6   #��[B��6[   
���   �.��6�   ��5W      �� 5X      $" CurrentVolunteer.MaxDistance�6   $ TwoWay=�6B   $ PropertyChanged=�6P    5Y      �� 5Z      $ maxDistanceValidationKey��6-   5\      .��5]   !   5^      �� 5_      $��Type Distance:��6   #��[B��6-   
����  �6U   �� 5`      #��[B��6�   
���   ���6   ��   SortCollectionDistancesKey# [  6b   �� *TypeDistance+ 6�   �� *CurrentVolunteer.TypeDistance+$ TwoWay=�6�   �� 5b      $
��Left=�6   $��Center=�64   $
��Auto��6O   $��13,0,0,0q�6]   $	��320��6o   y�6{   �� *CurrentVolunteer.Id+  ConvertObjIdToVisibilityKey#
 [ .+�5c      �� $��Details of Current Call��6   #��[B��6:   
����  �$
��Bold"�6t   $��14(�6�   $��0,0,0,10q�6�   y�6�   �� *CurrentVolunteer+ # ConvertVolunteerToVisibilityKey#
 [ �� 5d      $��
Call Type:��6   #��[B��6-   
����  �y�6g   �� *CurrentVolunteer+#
 [ �� 5e      ${�False��6�   #��[B��6�   
���   ���6   ��   SortCollectionMyCallTypeKey# [  6g   �� *
CallType+ 6�   �� *'%CurrentVolunteer.CurrentCall.CallType+$ TwoWay=�y�6&  �� *CurrentVolunteer+#
 [ �� 5f      $��Description:��6   #��[B��6/   
����  �y�6i   �� *CurrentVolunteer+#
 [ �� 5g      $
	 True��6T   #��[B��6f   
���   ���6   �� **(CurrentVolunteer.CurrentCall.Description+y�6�   �� *CurrentVolunteer+#
 [ �� 5h      $��Max End Time:��6   #��[B��60   
����  �y�6j   �� *CurrentVolunteer+#
 [ �� 5i      $
	 True��6S   #��[B��6e   
���   ���6   �� *)'CurrentVolunteer.CurrentCall.MaxEndTime+y�6�   �� *CurrentVolunteer+#
 [ �� 5j      $��Call Address:��6   #��[B��60   
����  �y�6j   �� *CurrentVolunteer+#
 [ �� 5k      $
	 True��6P   #��[B��6b   
���   ���6   �� *&$CurrentVolunteer.CurrentCall.Address+y�6�   �� *CurrentVolunteer+#
 [ �� 5l      $��Status:��6   #��[B��6*   
����  �y�6d   �� *CurrentVolunteer+#
 [ �� 5m      ${�False��6�   #��[B��6�   
���   ���6   ��  * &SortCollectionCallStatusByVolunteerKey# [  6r   �� *Status+ 6�   �� *%#CurrentVolunteer.CurrentCall.Status+$ TwoWay=�y�6-  �� *CurrentVolunteer+#
 [ �� 5n      $��Start Time:��6   #��[B��6.   
����  �y�6h   �� *CurrentVolunteer+#
 [ �� 5o      $
	 True��6R   #��[B��6d   
���   ���6   �� *(&CurrentVolunteer.CurrentCall.StartTime+y�6�   �� *CurrentVolunteer+#
 [ �� 5p      $��Distance From Volunteer:��6   #��[B��6;   
����  �y�6u   �� *CurrentVolunteer+#
 [ �� 5q      $
	 True��6^   #��[B��6p   
���   ���6   �� *42CurrentVolunteer.CurrentCall.DistanceFromVolunteer+y�6�   �� *CurrentVolunteer+#
 [ 5r      �� 5s      -   5        $	��200��5s   4   $��40��6@   $��Center=�6g   $��	0,23,0,43q�6�   ��6   �� *
ButtonText+6�   5t      5u      5v      