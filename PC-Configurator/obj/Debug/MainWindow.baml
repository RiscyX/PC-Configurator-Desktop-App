   M S B A M L   `   `   `  ����J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:PC_ConfiguratorPC_Configurator  5   	        PC_Configurator.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:PC_Configurator     5   	     � Title$  PC Konfigurátor��5   	   $	��700��5	   	   $
��1000��5
   	    � WindowStartupLocation$ CenterScreen=�5   	   ���#121212
������� � WindowStyle$
 None=�5   	    � 
ResizeMode$ CanMinimize=�5   	    � AllowsTransparency .  � 	Resources 5         ModernCardButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name �� CornerRadius �� Padding J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth   Opacity   
BlurRadius � RowDefinitions ��Name �� RelativeSource �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%  &	        �� 5   
   " ��6)   .�5      ��  � �� �#1E1E1E6+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       � ��$ 18(�6)   �� 5       � ��$ SemiBold"�6+   �� 5       � ��$ 20q�6(   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� $
 PART_Border��6!   #��z��66   $ 12��5   !   # z��63   ���#333333$��1q�5    7    5!        5"   "   $ 3Q�63   $	 0.3Q�6C   $ 10Q�6Q   ._�5$      �  5%   "   �� 5&   &   $
��Auto �64   �� 5'   &   $��* �64   .+�5*   "   �� $
 PART_IconBorder��6)   $��0��6B   $��80��5+   )   $��80��64   ���#0D66D0$ 40��5-   )   $��Center=�5.   )   $��0,0,0,20q�5/   )   ._�50   &   �� $ 	PART_Icon��60   $��Segoe MDL2 Assets)�51   /   $��32(�52   /   
�������$��Center=�54   /   $��Center=�55   /   ��50   C   �� *Tag+ �� *TemplatedParent+55   /   56   #   �� 57   "   $��1��6-   .+�58   &   �� #��z��60   #��z��59   .   #��z��5:   .   #��z��5;   .   $��Center=�5<   .   $ Center=�5=   .   �� 5?   &   $ PART_Description��60   $��14(�5A   .   ���#AAAAAA$��Center=�5C   .   $ Center=�5D   .   $��0,10,0,0q�5E   .   $
��Wrap=�5F   .   ��5@   .   �� *CommandParameter+ �� *TemplatedParent+5F   .   5G   #   5H      5I       5J      P� 5K       �@J�IsMouseOver . . �5L   "   �� $ PART_Border��6)    � �� �#0D66D06Y   �� 5M   "   $ PART_Border��6)    � �� �#2222226X   5N      5P      5R      5S      5T      .��5V      �  5W   
   �� 5X      $
��Auto �6   �� 5Y      $��* �6   .+�5]   
   �� -   5        $��0��5]      ���#1E1E1E$��48��63   ._�5^      � .+�5_      �� $��PC KONFIGURÁTOR��6   $
��Left=�63   $��Center=�6N   $��20,0,0,0q�5`      $��16(�6.   $��SemiBold"�6<   
�������6R   �� 5a       �� Orientation$ 
Horizontal=�6   $��Right=�66   .+�5b      �� -   5         ��Name$ MinimizeButton��5b      $��48��65   $��48��6@   
������ $��0q�5c   6    � Cursor$
 Hand��5d   :   .��5e      �� $	����6$   $��Segoe MDL2 Assets)�64   $��12(�5f   "   ���#AAAAAA60   5g      �� 5h      -   5        $ CloseButton��5h      $��48��62   $��48��6=   
������ $��0q�5i   6   $
 Hand��5j   7   .��5k      �� $	����6$   $��Segoe MDL2 Assets)�64   $��12(�5l   "   ���#AAAAAA60   5m      5n      5o      5p      �� 5s   
   $��1��6   ���#121212._�5t      �  5u      �� 5v      $
��Auto �6$   �� 5w      $��* �6$   .+�5{      �� $��0��6   $��	0,50,0,30q�6*   .+�5|      �� $*��$Üdvözöljük a PC Konfigurátorban��6    $��32(�5}       $
��Bold"�6.   $��Center=�5~       
�������5       �� 5�      $D��>Konfigurálja, tervezze, építse meg álmai számítógépét��6    $��16(�5�       $��Center=�5�       ���#AAAAAA$��0,10,0,0q�5�       5�      � 5�      $��1��6   $��
0,20,0,100q�6$    � ColumnDefinitions 5�      �� 5�      $��* �6+   �� 5�      $
��Auto �6+   �� 5�      $
��Auto �6+   �� 5�      $��* �6+   .+�5�      �� -   5        $��btnLogin��5�      $��1��5�      $��Bejelentkezés��5�      
 � Tag$	 ��5�      $C��=Jelentkezzen be meglévő fiókjába, és folytassa a munkát��5�      #��[  5�      $	��300��5�      $	��320��6)   $��0,0,20,0q�5�      �� 5�      -   5        $��btnRegister��5�      $��2��5�      $��Regisztráció��5�      $	 ��5�      $U��ORegisztráljon új felhasználóként, és kezdje el használni az alkalmazást��5�      #��[  5�      $	��300��5�      $	��320��6)   $��20,0,0,0q�5�      5�      �� 5�      $��1��6   $6��0© 2025 PC Konfigurátor | Minden jog fenntartva��5�      $��12(�5�      ���#777777$��Center=�5�      $��Bottom=�5�      $��0,0,0,20q�5�      5�      5�      5�      5�      