   M S B A M L   `   `   `  ����J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PC_Configurator.Views.AppPC_Configurator.Views.App  5   	   -    'PC_Configurator.Views.App.AddComponents   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:PC_Configurator.Views.App     5   	     � 	Resources  5
         ComponentSelectButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Padding J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth	   Opacity
   
BlurRadius � ColumnDefinitions ��Name �� RelativeSource �� Triggers P� Property P� Value��%�  &	        �� 5   
   " ��6.   .�5      ��  � �� �#1E1E1E6+   �� 5       � ��
 �����6+   �� 5       � ��$ 17(�6)   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$ 60��6'   �� 5       � ��$ 0,0,0,16q�6'   �� 5       �@�Cursor$
 Hand��6'   �� 5       � ��$ 1q�60   �� 5       � �� �#3333336,   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   #��z��5   !   $ 8��5   !   $ 16q�62    5        5   "   $ 1Q�63   $		 0.2Q�6C   $
 5Q�6Q   ._�5       �  5!   "   �� 5"   &   $
��Auto �67   �� 5#   &   $��* �67   .+�5%   "   �� $ 	IconBlock��6,   $��Segoe MDL2 Assets)�5'   *   $��22(�5(   *   $��Center=�5)   *   $��0,0,12,0q�5*   *   ��5&   *   �� *Tag+ �� *TemplatedParent+5*   *   �� 5+   "   $��1��63   $��Center=�5,   1   $
��Left=�5-   1   5.      5/       50      P� 51       �@J�IsMouseOver . . �52   "   ��  � �� �#2525256?   �� 53   "    � �� �#0D66D06@   54      P� 55       � �� . . �56   "   ��  � �� �#3333336?   57      59      5;      5<      5=      .��6$   � ���#121212$��0q�6>    5>   
   �� 5?      $	��340 �6   �� 5@      $��* �6   .+�5C   
   �� -   5         ��Name$ 	LeftPanel��5C      $��0��6$   ���#1A1A1A$ 12��6I   $��20,20,10,20q�6[   ���#333333$��1q�6�    5D        5E      $ 2Q�6#   $		 0.2Q�63   $
 10Q�6A   ._�5G      �� $��15q�6   $��Center=�6%   .+�5H      �� $��Alkatrészek hozzáadása��6   
�������$��22(�6M   $
��Bold"�6[   $��Center=�6m   $��0,0,0,30q�6�   �� 5J      -   5         ��Name$ AddCpuButton��5J      $��CPU hozzáadása��6/   
 � Tag$	 ��6H   #��[  6W   6�   �� 5K      -   5        $ AddGpuButton��5K      $��GPU hozzáadása��6/   $	 ��6H   #��[  6W   6�   �� 5L      -   5        $ AddMotherboardButton��5L      $��Alaplap hozzáadása��67   $	 ��6T   #��[  6c   6�   �� 5M      -   5        $ AddRamButton��5M      $��RAM hozzáadása��6/   $	 ��6H   #��[  6W   6�   �� 5N      -   5        $ AddStorageButton��5N      $��Meghajtó hozzáadása��63   $	 ��6Q   #��[  6`   6�   �� 5O      -   5        $ AddPsuButton��5O      $��Tápegység hozzáadása��6/   $	 ��6N   #��[  6]   6�   �� 5P      -   5        $ AddCaseButton��5P      $��Gépház hozzáadása��60   $	 ��6L   #��[  6[   6�   5Q      5R      � 5S   
   $��1��6   $��10,20,20,20q�6   .+�5T      �� 
������ $��Center=�5U      $��Center=�5V      $
��Auto��5W      $	��800��5X      ��5Y      �� *ActualHeight+ �� ElementName$ 	LeftPanel��._�5Z      �� $
��Auto=�6   $
��Auto=�5[      .��5\      �� -	   5         ��Name$ FormHost��5\   %   $��Center=�5]   %   $	��Top=�5^   %   $
��Auto��5_   %   5`      5a      5b      5c      5d      