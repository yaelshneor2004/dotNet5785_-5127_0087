   M S B A M L   `   `   `  ����=  9PL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PLPL  5   	       PL.LoginWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   "localclr-namespace:PL     5   	     � Title$  LoginWindow��5   	   $	��450��6   $	��800��6*    � WindowState$ 	Maximized=�5   	    � DataContext 5	   	   ��  �� RelativeSource ��  �� Mode$
 Self=�.��5      � ��5   
   �� 5       �� ImageSource$  Resources/loginwindoww.jpg��6   .+�5   
   � $��Center=�6   $��Center=�6*   .+�5      �� 
���   �$��2q�6)    �� Padding$ 10q�6=    �� CornerRadius$ 10��6J   $��90,-174,-260,-171q�6\   ._�5      �� $��0q�6   .+�5      �� $��WELCOME��6   $��Center=�6.   $	��100(�6K   $��Comic Sans MS)�6Z   
���   �$��10q�6�   $��	ExtraBold"�6�   $	��100��6�   $��47��6�   �� 5      $��Enter your name��6   $
��Left=�66   $��5q�6Q   $	��Top=�6\   $��25(�6t   $
��Bold"�6�   �� 5      $
��Left=�6I   $��30��6d   $��5q�6p    �� TextWrapping$
 Wrap=�6{   $	��230��6�   $��15(�6�   ��6   �� *userNameText+	 �� Mode$	 TwoWay=�6�   �� 5      $��Enter your password��6   $
��Left=�6:   $��5q�6U   $��25(�6`   $
��Bold"�6n   � 5      $
��Left=�6   $��5q�66   $	��Top=�6A   $	��230��6Y   $��50��6e   
 � ColumnDefinitions
 5      �� 5      $��* �6/   �� 5      $
��Auto �6/   .+�5      � .+�5      G� $��30��6*   $��Center=�66   $��15(�6Q   $��0,0,0,0q�6_   y�6p   �� *passwordVisibility+$	 TwoWay=��� 5      $��30��6&   $��Center=�62   $��15(�6M   $��0,0,0,0q�6�   y�6[   �� *TextVisibility+$	 TwoWay=���6�   �� *passwordText+$	 TwoWay=�6�   5       �� 5!      -   5        
������ 
������ $��20(�5!   u   $��	186,0,2,0q�6�   ��5"      �� 5#   "   $��20��6Y   $��20��6e   ��6(   �� *ButtonImageSource+$	 TwoWay=�6e   5%      5&      �� 5'      -   5        $��Login��5'      $��34��6-   $	��100��69   $��25(�6Y    J� RenderTransformOrigin$ 
0.45,0.557&�6g   5(      5)      5*      5+      5,      