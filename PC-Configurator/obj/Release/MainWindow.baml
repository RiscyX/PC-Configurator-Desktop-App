   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:PC_ConfiguratorPC_Configurator       PC_Configurator.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:PC_Configurator       � Title$  PC Konfigurátor��$	��700��$
��1000�� � WindowStartupLocation$ CenterScreen=����#121212
������� � WindowStyle$
 None=� � 
ResizeMode$ CanMinimize=� � AllowsTransparency .  � 	Resources    ModernCardButton �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name �� CornerRadius �� Padding J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth   Opacity   
BlurRadius � RowDefinitions ��Name �� RelativeSource �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%�  &	        �� " ��.���  � �� �#1E1E1E��  � ��
 �������  � ��$ 0q���  � ��$ 18(���  � ��$ SemiBold"���  � ��$ 20q���  �@�Cursor$
 Hand����  � �� �� "	 ��.R��� $
 PART_Border��#��z��$ 12��# z�����#333333$��1q�   $ 3Q�$	 0.3Q�$ 10Q�._��  �� $
��Auto ��� $��* �.+��� $
 PART_IconBorder��$��0��$��80��$��80�����#0D66D0$ 40��$��Center=�$��0,0,0,20q�._��� $ 	PART_Icon��$��Segoe MDL2 Assets)�$��32(�
�������$��Center=�$��Center=����� *Tag+ �� *TemplatedParent+�� $��1��.+��� #��z��#��z��#��z��#��z��$��Center=�$ Center=��� $ PART_Description��$��14(����#AAAAAA$��Center=�$ Center=�$��0,10,0,0q�$
��Wrap=����� *CommandParameter+ �� *TemplatedParent+ P�  �@J�IsMouseOver . . ��� $ PART_Border�� � �� �#0D66D0�� $ PART_Border�� � �� �#222222.���  �� $
��Auto ��� $��* �.+��� -   $��0�����#1E1E1E$��48��._�� .+��� $��PC KONFIGURÁTOR��$
��Left=�$��Center=�$��20,0,0,0q�$��16(�$��SemiBold"�
���������  �� Orientation$ 
Horizontal=�$��Right=�.+��� -    ��Name$ MinimizeButton��$��48��$��48��
������ $��0q� � Cursor$
 Hand��.���� $	����$��Segoe MDL2 Assets)�$��12(����#AAAAAA�� -   $ CloseButton��$��48��$��48��
������ $��0q�$
 Hand��.���� $	����$��Segoe MDL2 Assets)�$��12(����#AAAAAA�� $��1�����#121212._��  �� $
��Auto ��� $��* �.+��� $��0��$��	0,50,0,30q�.+��� $*��$Üdvözöljük a PC Konfigurátorban��$��32(�$
��Bold"�$��Center=�
��������� $D��>Konfigurálja, tervezze, építse meg álmai számítógépét��$��16(�$��Center=����#AAAAAA$��0,10,0,0q�� $��1��$��
0,20,0,100q� � ColumnDefinitions �� $��* ��� $
��Auto ��� $
��Auto ��� $��* �.+��� -   $��btnLogin��$��1��$��Bejelentkezés��
 � Tag$	 ��$C��=Jelentkezzen be meglévő fiókjába, és folytassa a munkát��#��[  $	��300��$	��320��$��0,0,20,0q��� -   $��btnRegister��$��2��$��Regisztráció��$	 ��$U��ORegisztráljon új felhasználóként, és kezdje el használni az alkalmazást��#��[  $	��300��$	��320��$��20,0,0,0q��� $��1��$6��0© 2025 PC Konfigurátor | Minden jog fenntartva��$��12(����#777777$��Center=�$��Bottom=�$��0,0,0,20q�