   M S B A M L   `   `   `  ����  Laboratory_11    Laboratory_11.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$1  +Support for multithreading and data streams��5   	   $	��525��6=   $	��800��6J   $��Comic Sans MS)�5   	   ���#333333.��5      � .+�5	   
   �� $	��Top=�6   $
��Left=�6-   $��	20,20,0,0q�6H   $	��294��6[   $	��354��6h   .+�5
      �  � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5      $��* �6'   .+�5      �� $��Enter N:��6   $��10,10,10,10q�6+   
�������$��20(�6S   �� 5      -   5        $��NTextBox��5      $��1��6*   $��10,15,25,15q�6:   $��20(�6O   .��6]    � 	Resources 5       �� 
TargetType �� Property �� Value��%^   '��       �� 5      " ��6    .�5      ��  �@��CornerRadius$ 5��6=   5      5      .��6)   5      5      � 5       5      �� 5      $
��Auto �6'   �� 5      $��* �6'   .+�5      �� $��Enter K:��6   $��10,10,10,10q�6+   
�������$��20(�6S   �� 5      -   5        $��KTextBox��5      $��1��6*   $��10,15,25,15q�6:   $��20(�6O   .��6]    5      ��%^   '��       �� 5       " ��6    .�5!      ��  �@��CornerRadius$ 5��6=   5"      5#      .��6)   5$      5%      �� 5&      -   5        $��Calculate with Task��5&      $	��326��63   $��
0,10,20,10q�6?   $��35��6u    5'      ��%^   '��       �� 5(      " ��6   .�5)      ��  �@��CornerRadius$ 5��69   5*      5+      5,      �� 5-      -   5        $��Calculate with Delegate��5-      $	��326��67   $��
0,10,20,10q�6C   $��35��6}    5.      ��%^   '��       �� 5/      " ��6   .�50      ��  �@��CornerRadius$ 5��69   51      52      53      �� 54      -   5        $ ��Calculate with Async/Await��54      $	��326��6:   $��
0,10,20,10q�6F   $��35��6}    55      ��%^   '��       �� 56      " ��6   .�57      ��  �@��CornerRadius$ 5��69   58      59      5:      5;      �� 5<   
   -   5        $��ResultTextBox��5<      $	��375��6'   $��0,35,35,190q�63    �� 
IsReadOnly$
 True��6H    �� TextWrapping$
 Wrap=�6Z   " �� VerticalScrollBarVisibility$
 Auto=�6n   $��Right=�6�   .��6�    5=      ��%^   '��       �� 5>      " ��6   .�5?      ��  �@��CornerRadius$ 5��65   5@      5A      .��6!   5B      � 5E   
   $��20,300,35,10q�6   	 � RowDefinitions	 5F      �� 5G      $
��Auto �6    $��0��6.   �� 5H      $
��Auto �6    $��84��6.   �� 5I      $
��Auto �6    �� 5J      $
��Auto �6    .+�5L      �� $��Enter I:��6   $��10,10,10,10q�6'   
�������$��20(�6O   �� 5M      -   5        $��FibonacciTermsTextBox��5M      $��120,15,400,15q�63   $��20(�6J   .��6X    5N      ��%^   '��       �� 5O      " ��6   .�5P      ��  �@��CornerRadius$ 5��69   5Q      5R      .��6%   5S      �� 5U      -   5        $!��Start Fibonacci Calculation��5U      $	��325��6;   $��35��6G   $	��Top=�6S   $
��Left=�6k   $��1��6�   $��5,0,0,0q�6�    5V      ��%^   '��       �� 5W      " ��6   .�5X      ��  �@��CornerRadius$ 5��69   5Y      5Z      5[      � 5]      -	   5        
 �Name$
 ProgressBar��5]      $��1��6/   $��35��6<   $	��Top=�6H   $��5,55,0,0q�6`   $y�Visible=�6r   $��3��6�   5^      � 5`   
   .+�5a      �� -
   5        $��CompressButton��5a      $��Compress Files��6+   $
��Left=�6b   $	��Top=�6}   $	��175��6�   $��80��6�   $��375,315,0,0q�6�    5b      ��%^   '��       �� 5c      " ��6   .�5d      ��  �@��CornerRadius$ 5��69   5e      5f      5g      �� 5h      -   5        $��DecompressButton��5h      $��Decompress Files��6-   $
��Left=�6h   $	��Top=�6�   $	��175��6�   $��80��6�   $��575,315,0,0q�6�    5i      ��%^   '��       �� 5j      " ��6   .�5k      ��  �@��CornerRadius$ 5��69   5l      5m      5n      5o      5q      5r      