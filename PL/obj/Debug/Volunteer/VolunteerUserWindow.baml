   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PL.VolunteerPL.Volunteer  5   	   f bMicrosoft.Web.WebView2.Wpf, Version=1.0.2957.106, Culture=neutral, PublicKeyToken=2a8ab48044d2601ekLclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.WpfMicrosoft.Web.WebView2.Wpf 5	   	   &     PL.Volunteer.VolunteerUserWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:PL.Volunteer     5   	   \wv2Lclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf     5	   	   -   5          � Title$  VolunteerUserWindow��5   	   $
��1461��6%   $	��800��63    � Language$ he-IL�5   	    � WindowState$ 	Maximized=�5       � DataContext 5
   	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      � .+�5   
   (�  (� Stretch$ Uniform=�6   .��5      �  � RowDefinitions 5      �� 5      $��* �6$   �� 5      $
��Auto �6$    � ColumnDefinitions 5      �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5      �� $��20,0,0,0q�6   $
��Left=�6-   $��0��6H   $��0��6U   $��2q�6e   ���#9370DB$	��Top=�6�   $
��Auto��6�   ��5      �� 5       $��Volunteer Details��6$   ���#9370DB$��16(�6R   .��5"      �� 	 �� Orientation$	 
Horizontal=�6!   .+�5#      �� $	 Vertical=�6%   $��	20,10,0,0q�6<   $	��Top=�6O   .+�5$      �� $	��ID:��6$   #��[B��62   
����  �6Z   �� 5%         ValidationErrorTemplate
 =� ErrorTemplate#
 [  6&   #��[B��6�   
���   � �� 
IsReadOnly 6j   �� *CurrentVolunteer.Id+  ConvertObjIdToTFKey �� 	Converter# [ .��6  ��5&   "   �� 5'   &    �� Path$ CurrentVolunteer.Id�6.    �� Mode$ TwoWay=�6I    �� UpdateSourceTrigger$ PropertyChanged=�6W    �� ValidationRules 5(   *   �� 5)   .    �� ResourceKey$ idNumValidationKey��6=   5+   '   .��5,   0   5-      �� 5/      $��
Full Name:��6$   #��[B��69   
����  �6a   �� 50      #
 [  6&   #��[B��6k   
���   �.��6�   ��51   "   �� 52   &   $ CurrentVolunteer.FullName�6.   $ TwoWay=�6O   $ PropertyChanged=�6]    53   *   �� 54   .   $ nameValidationKey��6=   56   '   .��57   0   58      �� 5:      $��Phone:��6$   #��[B��65   
����  �6]   �� 5;      #
 [  6&   #��[B��6j   
���   �.��6�   ��5<   "   �� 5=   &   $ CurrentVolunteer.Phone�6.   $ TwoWay=�6L   $ PropertyChanged=�6Z    5>   *   �� 5?   .   $ phoneValidationKey��6=   5A   '   .��5B   0   5C      �� 5E      $��Email:��6$   #��[B��65   
����  �6]   �� 5F      #
 [  6&   #��[B��6j   
���   �.��6�   ��5G   "   �� 5H   &   $ CurrentVolunteer.Email�6.   $ TwoWay=�6L   $ PropertyChanged=�6Z    5I   *   �� 5J   .   $ emailValidationKey��6=   5L   '   .��5M   0   5N      �� 5P      $��	Password:��6$   #��[B��68   
����  �6`   �� 5Q      #
 [  6&   #��[B��6j   
���   �.��6�   ��5R   "   �� 5S   &   $ CurrentVolunteer.Password�6.   $ TwoWay=�6O   $ PropertyChanged=�6]    5T   *   �� 5U   .   $ passwordValidationKey��6=   5W   '   .��5X   0   5Y      �� 5[      $��Address:��6$   #��[B��67   
����  �6_   �� 5\      #��[B��6P   
���   ���6&   �� *CurrentVolunteer.Address+��6�   �� *CurrentVolunteer.Latitude+ ! ConvertZeroDimensionsToRedKey# [ �� 5^      $��Role:��6$   #��[B��64   
����  �6\   �� 5_      #��[B��5b   #   
���   �
�������${�False��5e   #   ��5_   '   ��   SortCollectionRolesKey �� Source# [  �� SelectedItem 5`   #   �� *Role+ �� SelectedValue 5a   #   �� *CurrentVolunteer.Role+$ TwoWay=�5e   #   �� 5g      $��
Is Active:��6$   #��[B��69   
����  �$��0, 20, 0, 5q�6s   �� 5h      #��[B��6d   $	��307��6�    d� 	IsChecked 6'   �� *CurrentVolunteer.IsActive+$ TwoWay=�6�   �� 5j      $��Max Distance:��6$   #��[B��6<   
����  �6d   �� 5k      #
 [  6&   #��[B��6j   
���   �.��6�   ��5l   "   �� 5m   &   $" CurrentVolunteer.MaxDistance�6.   $ TwoWay=�6R   $ PropertyChanged=�6`    5n   *   �� 5o   .   $ maxDistanceValidationKey��6=   5q   '   .��5r   0   5s      �� 5u      $��Type Distance:��6$   #��[B��6=   
����  �6e   �� 5v      #��[B��5y   #   
���   �
���������5v   '   ��   SortCollectionDistancesKey# [  5w   #   �� *TypeDistance+ 5x   #   �� *CurrentVolunteer.TypeDistance+$ TwoWay=�5{   #   �� 5}      $��Num Treated Calls��6$   #��[B��6@   
����  �6h   �� 5~      $
 True��6g   #��[B��6y   
���   ���6&   �� *$"CurrentVolunteer.TotalCallsHandled+$ TwoWay=�6�   �� 5�      $��Num Cancel Calls��6$   #��[B��6?   
����  �6g   �� 5�      $
 True��6i   #��[B��6{   
���   ���6&   �� *&$CurrentVolunteer.TotalCallsCancelled+$ TwoWay=�6�   �� 5�      $��Num Expired Calls��6$   #��[B��6@   
����  �6h   �� 5�      $
 True��6g   #��[B��6y   
���   ���6&   �� *$"CurrentVolunteer.TotalCallsExpired+$ TwoWay=�6�   �� 5�      $	 
Horizontal=�6)   $��Center=�6B   $��10,10,10,10q�6_   .+�5�   "   �� -   5        $��Update��5�   )   $	��130��6:   $��46��6^   �� 5�   "   $	 
Horizontal=�6-   $��Center=�6F   $��10,10,10,10q�6c   .+�5�   &   �� -   5        $��Call History��5�   -   $	��130��6D   $��47��6m   �� 5�   &   -   5        $��Select call to treat��5�   -   $	��130��6L   $��10q�6t   $��47��6�   ��5�   *   �� 5�   .    �� 
TargetType" ��64    �� BasedOn# [B��6Q   .�5�   2   ��  �� Property � y� �� Value$ 	Collapsed=�6O    �� Triggers 5�   2   �� 5�   6    �� Binding 6B   �� *CurrentCall+ ( $ConvertCallInProgressToVisibilityKey# [  �� Value 	Collapsed6�   .a�5�   :   ��  � y�$ Visible=�6W   5�   7   �� 5�   6    6B   �� *CurrentCall+# [  Visible6�   .a�5�   :   ��  � y�$ 	Collapsed=�6W   5�   7   5�   /   5�   '   5�   #   5�      5�      5�      5�      �� 5�      $��20,0,0,0q�6   $��Right=�6-   $��0��6I   $��1��6V   $��2q�6f   ���#9370DB$	��Top=�6�   $
��Auto��6�   y�6�   �� *CurrentVolunteer.CurrentCall+  ConvertObjIdTovisNotVisKey# [ ��5�      �� 5�      $��Call details��6$   ���#9370DB$��16(�6N   .��5�      �  5�      �� 5�      $
��Auto �6,   �� 5�      $��* �6,   .+�5�      �� $	 
Horizontal=�6%   $	��Top=�6>   $��0��6V   .+�5�      �� $	 Vertical=�6)   $	��Top=�6@   .+�5�   "   �� $	��ID:��6(   #��[B��66   
����  �y�6p   �� *CurrentCall+# [ �� 5�   "   $
 True��6W   #��[B��6i   
���   ���6*   �� *CurrentCall.Id+$ TwoWay=�y�6�   �� *CurrentCall+# [ �� 5�   "   $��Type:��6(   #��[B��68   
����  �y�6r   �� *CurrentCall+# [ �� 5�   "   #��[B��6�   
���   �${�False��6{  ��6+   ��   SortCollectionMyCallTypeKey# [  6w   �� *CurrentCall.Type+ 6�   �� *CurrentCall.Type+$ TwoWay=�y�6  �� *CurrentCall+# [ 6{  �� 5�   "   $��Description:��6(   #��[B��6?   
����  �y�6y   �� *CurrentCall+# [ �� 5�   "   #��[B��6S   
���   �$
 True��6�   ��6*   �� *CurrentCall.Description+y�6�   �� *CurrentCall+# [ 6�   �� 5�   "   $��Address:��6(   #��[B��6;   
����  �y�6u   �� *CurrentCall+# [ �� 5�   "   #��[B��6O   
���   �$
 True��6�   ��6*   �� *CurrentCall.Address+y�6�   �� *CurrentCall+# [ 6�   �� 5�   "   $��Start Time:��6(   #��[B��6>   
����  �y�6x   �� *CurrentCall+# [ �� 5�   "   #��[B��6Q   
���   �$
 True��6�   ��6*   �� *CurrentCall.StartTime+y�6�   �� *CurrentCall+# [ 6�   �� 5�   "   $��Max End Time:��6(   #��[B��6@   
����  �y�6z   �� *CurrentCall+# [ �� 5�   "   #��[B��6R   
���   �$
 True��6�   ��6*   �� *CurrentCall.MaxEndTime+y�6�   �� *CurrentCall+# [ 6�   �� 5�   "   $��Status:��6(   #��[B��6:   
����  �y�6t   �� *CurrentCall+# [ �� 5�   "   $��	0,20,0,10q�6-   .+�5�   &   &   System.Windows.Controls.DataGrid     AutoGenerateColumns$ False��6_      CanUserAddRows$ False��6{      
IsReadOnly$
 True��6�   ��6/   �� *CurrentCall.Assignments+y�6�   �� *CurrentCall+# [     Columns  5�   *   0  *System.Windows.Controls.DataGridTextColumn  5�   .   ,  &System.Windows.Controls.DataGridColumn!   Header$! Volunteer ID��6A   "   
IsReadOnly$
" True��6w   1  +System.Windows.Controls.DataGridBoundColumn#   Binding# 6W   �� *VolunteerId+6w     5�   .   $! Volunteer Name��6A   # 6Y   �� *VolunteerName+  5�   .   $! Start Treatment Time��6A   # 6_   �� *StartTreatmentTime+  5�   .   $! End Treatment Time��6A   # 6]   �� *EndTreatmentTime+4  .System.Windows.Controls.DataGridComboBoxColumn  5�   .   $! End Type��6E   $   SelectedItemBinding$ 6W   �� *	EndType+%   ItemsSource% 5�   :   ��  ! SortCollectionMyFinishTypeKey# [ 5�   '   5�   #   �� 5�   "   $	 
Horizontal=�6-   $��Center=�6F   $��10,10,10,10q�6c   .+�5�   &   �� -   5        $��Cancel Call��5�   -   $	��130��6C   $��10q�6k   $��47��6�   y�6w   �� *CurrentCall+# [ 6�   �� 5�   &   -   5        $��Finish Call��5�   -   $	��130��6C   $��10q�6k   $��47��6�   y�6w   �� *CurrentCall+# [ 6�   5�   #   5�      5�      )  #Microsoft.Web.WebView2.Wpf.WebView2  5�      -   5        &  Name$& 
MapWebView��5�   '   $	��470��5�   -   $	��200��69   $��1��6F   '   Source' 6   �� *	MapSource+y�6S   �� *CurrentCall+# [ z�5�      V� 5�   "   .~�5�   &   �� ( �� ScaleY$( 1Q�65   �� 5�   &   �� 5�   &   T� 5�   &   5�   #   5�      5�      5�      5�      5�      5�      5�      