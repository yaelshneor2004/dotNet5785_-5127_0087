   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:PL.CallPL.Call  5   	   f bMicrosoft.Web.WebView2.Wpf, Version=1.0.2957.106, Culture=neutral, PublicKeyToken=2a8ab48044d2601ekLclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.WpfMicrosoft.Web.WebView2.Wpf 5   	   %    PL.Call.SelectCallToTreatWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:PL.Call     5   	   \wv2Lclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf     5   	   -   5          � Title$  SelectCallToTreatWindow��5   	   $	��640��6)   $
��1000��66    � Language$ he-IL�5	   K    � WindowStartupLocation$ CenterScreen=�6�    � DataContext 6	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5
      � .+�5   
   �  � ColumnDefinitions 5      �� 5      $��2* �6#   �� 5      $��* �6#    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      � $��0��6   $��0��6    $��0,0,10,0q�60    5      �� 5      $��* �6'   �� 5      $��* �6'   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��Center=�66   $��-10,0,138,0q�6Q   $��2��6f   .+�5      �� -   5        $��43��5   �   $	��511��6�   	 J� RenderTransformOrigin$	 0.5,0.5&�6�   ���	#FF8484DC
 �� SelectedValue
 6   �� *FilterByType+ �� Mode$ TwoWay=���6S   ��    SortCollectionMyCallTypeKey �� Source# [   J� OpacityMask 5      �� 5       �� EndPoint$ 0.5,1&�62    �� 
StartPoint$ 0.5,0&�6C   .��5   "   �  � Color$ Black��6/   � 5    "    � Offset$ 1Q�6/   $ 	#FF7813F6��6:   5!      z�5#      V� 5$      .~�5%   "   �� �� 5&   "    �� AngleY$ 0.004Q�60   �� 5'   "    �� Angle$ 0.134Q�62   T� 5(   "    T� Y$ 0.009Q�65   5)      5+      5,      5-      &   System.Windows.Controls.DataGrid  5/      -   5        $��0,18,143,25q�5/      $��1��6,   $��0��69      AutoGenerateColumns$ False��6s      
IsReadOnly$
 True��6�      EnableRowVirtualization$
 True��6�      RowDetailsVisibilityMode$ VisibleWhenSelected=�6�   
�������$	 0.5,0.5&�6Y  ��6I   ��  �� Path$ OpenCallList� �� SelectedItem 6  �� *SelectedOpenCall+z�50      V� 51      .~�52      �� �� 53      $ -0.148Q�6(   �� 54      T� 55      $ -0.664Q�6-   56         Columns 58      0  *System.Windows.Controls.DataGridTextColumn  59      ,  &System.Windows.Controls.DataGridColumn   Header$ Call Id��6@      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6Q   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6)   �� *Id+6Q     5:      $
 Type��6B   $
 Auto 6P    6)   �� *Type+6P     5;      $ Description��6I   $
 Auto 6^    6)   �� *Description+6^     5<      $ Address��6E   $
 Auto 6V    6)   �� *	Address+6V     5=      $ 
Start Time��6G   $
 Auto 6[    6)   �� *	StartTime+6[     5>      $ Max End Time��6H   $
 Auto 6^    6)   �� *
MaxEndTime+6^     5?      $ Distance��6S   $
 Auto 6e    6)   �� *DistanceFromVolunteer+6e   4  .System.Windows.Controls.DataGridTemplateColumn  5@      $ Select��6-   $
 Auto 6=      CellTemplate 5A      �� 5B      .R�5C   "   �� -   5        $��Select��5C   )   6:   5D      5F      5H      �� 5J      $��Call Description:��6   $��2��62   $��0��6?   $��10,2,146,18q�6O   .��5K      �� $
��Left=�6   $��Center=�68   $	��630��6S   .+�5L      �� $��39��6I   $	��665��6U   ��6   �� *Description+$ TwoWay=�6U   5M      5N      � 5P      $��1��6   $��0��6#   $��2��60   $	��400��6A   $��45,-9,-200,25q�6P   .+�5Q      )  #Microsoft.Web.WebView2.Wpf.WebView2  -   5        $��
MapWebView��5Q      $��-175,71,212,0q�6N       Source  61   �� *	MapSource+6N   5R      �� 5T      $ 
Horizontal=�6   $
��Left=�62   $��2��6M   $	��Top=�6a   $��
578,11,0,0q�6y   $	��407��6�   $��2��6�   .+�5U      ��   ValidationErrorTemplate! =� ErrorTemplate#! [ 6   $	��230��6_   $��30��6k   .��6w   ��5V      �� 5W      $ CurrentVolunteer.Address�6"   $ TwoWay=�6B   " �� UpdateSourceTrigger$" PropertyChanged=�6P   # �� ValidationRules# 5X      �� 5Y   "   $ �� ResourceKey$$ addressValidationKey��61   5[      .��5\   %   5]      �� 5^      -   5        $��Update Address��5^      $��30��6N   5_      5`      5a      5b      