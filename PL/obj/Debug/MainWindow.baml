   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PLPL  5          PL.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      "localclr-namespace:PL     5      -   5          � Title$  
MainWindow��5      $	��450��6    $	��800��6-    � Language$ he-IL�5       � WindowState$ 	Maximized=�6    � DataContext 5	      ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      (�  (� Stretch$ Uniform=�6   .��5   
   �  � ColumnDefinitions 5      �� 5      $
��161* �6#   �� 5      $
��863* �6#   .+�5      �� $��4q�6   ���#9370DB$	��Top=�6A   $
��Left=�6Y   $	��360��6t   $��
194,42,0,0q�6�   $	��282��6�    J� RenderTransformOrigin$ 0.863,0.466&�6�   $��1��6�   ��5      �� 5      $��
Clock menu��6    ���#9370DB$��20(�6G   .��5      �� $��	32,0,38,0q�6   .+�5       �� $��72(�6U   $��	ExtraBold"�6c   $��48��6z   $	��183��6�   $��Center=�6�   
���   ���6   �� *CurrentTime+	 �� StringFormat$	 
MM/dd/yyyy��6�   �� 5!      -   5        $��Add One Minute��5!      $��Center=�66   $��42��6S   $	��150��6_   {�6�   �� *IsSimulatorRunning+ #  BooleanToIsEnabeldyConverterKey
 �� 	Converter#
 [  �� 5"      -   5        $��Add One Hour��5"      $��Center=�64   $��42��6Q   $	��150��6]   {�6�   �� *IsSimulatorRunning+#
 [  �� 5#      -   5        $��Add One Day��5#      $��Center=�63   $��42��6P   $	��150��6\   {�6�   �� *IsSimulatorRunning+#
 [  �� 5$      -   5        $��Add One Month��5$      $��Center=�65   $��42��6R   $	��150��6^   {�6�   �� *IsSimulatorRunning+#
 [  �� 5%      -   5        $��Add One Year��5%      $��Center=�64   $��42��6Q   $	��150��6]   {�6�   �� *IsSimulatorRunning+#
 [  5&      5'      �� 5)      $��Status Quantities��6   $
��Left=�60   $��15(�6K   $��11,115,0,292q�6Y   $
��Bold"�6o   $	��130��6�   $��2��6�   &   System.Windows.Controls.DataGrid  5*      -   5           AutoGenerateColumns$ False��5*         
IsReadOnly$
 True��63   
������ 
������ $��0,145,651,99q�6�   $��Right=�6N  $	��237��6j  $��2��6v  ��6E   �� *
StatusList+ �� Mode$ TwoWay=� �� SelectedItem 6�   �� *selectedValue+$ TwoWay=�   Columns 5+      0  *System.Windows.Controls.DataGridTextColumn  5,      ,  &System.Windows.Controls.DataGridColumn   Header$ Status��6C      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6S   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6)   �� *value+6S     5-      $ Quantity��6C   $ * 6U    6)   �� *index+6U   5/      �� 50      -   5          SimulatorButtonStyle#��[ 50      $
��Left=�6D   $��36��6_   $��8,57,0,0q�6k   $	��Top=�6}   $	��136��6�   $ -3.54,7.601&�6�   $��1��6�   �� 51      $
��Left=�6   $��30��61   $��	41,15,0,0q�6=    �� TextWrapping$
 Wrap=�6P   $	��Top=�6�   $	��190��6�   $��2��6  ��6d   �� *
Interval+$ TwoWay=� �� 
IsReadOnly 6�   �� *IsSimulatorRunning+ ! BooleanToReadOnlyConverterKey#
 [ 6  �� 52      $$��Enter the time in milliseconds��6   $
��Left=�6=   $��15��6X   $��
256,15,0,0q�6d   $	��Top=�6x   $	��190��6�   $��1��6�   �� 55      $��4q�6   ���#9370DB$	��Top=�6A   $
��Left=�6Y   $	��360��6t   $��
506,42,0,0q�6�   $	��255��6�   $ 0.863,0.466&�6�   $��1��6�   ��56      �� 57      $��Central buttons menu��6    ���#9370DB$��20(�6Q   .��59      �� $��	27,0,15,0q�6   $	��234��60   .+�5:      �� -	   5        $��Init DB��5:      $��Center=�6/   $��40��6L   $	��150��6X   {�6}   �� *IsSimulatorRunning+#
 [  �� 5;      -
   5        $��Reset DB��5;      $��Center=�60   $��40��6M   $	��150��6Y   {�6   �� *IsSimulatorRunning+#
 [  �� 5<      -   5        $��Handle Volunteers��5<      $��Center=�69   $��40��6V   $	��150��6b   6n   �� 5=      -   5        $��Handle Calls��5=      $��Center=�64   $��40��6Q   $	��150��6]   6i   5>      5?      �� 5C      $��
Max Range:��6   $
��Left=�6)   $	��Top=�6D   $��0��6\   $��
14,340,0,0q�6l   $��0��6�   $��2��6�   �� 5D      $��0��64   $��22,366,782,41q�6D   $��2��6[   ��6   �� *MaxRiskRange+6[   �� 5E      -   5        $��Update��5E      $
��Left=�6&   $��86��6A   $��30��6L   $��72,366,0,41q�6p   $��1��6�   5G      5H      5I      