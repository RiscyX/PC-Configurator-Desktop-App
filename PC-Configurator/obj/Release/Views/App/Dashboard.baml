   M S B A M L   `   `   `  ����   PC-Configurator)    #PC_Configurator.Views.App.Dashboard   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  PC Konfigurátor��$	��900��$
��1600�� � WindowStartupLocation$ CenterScreen=� � WindowState$ 	Maximized=� � WindowStyle$
 None=� � 
ResizeMode$ NoResize=����#121212
������� � AllowsTransparency .  � 	Resources    SidebarButton �� 
TargetType �� Property	 �� Value
 �� 
TargetType ��Name �� Padding �� CornerRadius ��Name �� Triggers P� Property P� Value �� 
TargetName  ActiveSidebarButton �� BasedOn  	CardStyle3  -System.Windows.Media.Effects.DropShadowEffect   Color   
BlurRadius   ShadowDepth   Opacity  	TitleText  
HeaderText��%�  &	        &	 ,    7   &	 �    &	 �    &	 �    �� " ��.���  � ��
	 ���� ��  � ��	 �#CCCCCC��  � ��$	 17(���  � ��$	 SemiBold"���  � ��$	 54����  � ��$	 0,6,0,6q���  � ��$	 20,0,0,0q���  � ��$
	 Left=���  � ��$	 Center=���  � ��$	 0q���  �@�Cursor$
	 Hand����  � ��	 �� "
 ��.R�� .+��� $ HighlightBorder��$��4��$
��Left=�
������ �� $ 54,0,0,0q�#��z��$ 0��._��� $��Center=�$
��Left=��� $ 	IconBlock��$	����$��Segoe MDL2 Assets)�$��20(�#��z��$��Center=�$
��Left=�$��20,0,0,0q� P�  �@J�IsMouseOver . . ���  � ��	 �#222222��  � ��	 �#FFFFFF�� $ HighlightBorder�� � ��	 �#0D66D0P�  � �� . . ���  � ��	 �#333333�� $ HighlightBorder�� � ��	 �#0D66D0�� " ��8   .���  � ��	 �#222222��  � ��	 �#FFFFFF��  � ��	 �� "
 ��.R�� .+��� $ HighlightBorder��$��4��$
��Left=����#0D66D0�� $ 54,0,0,0q�#��z��$ 0��._��� $��Center=�$
��Left=��� $ 	IconBlock��$	����$��Segoe MDL2 Assets)�$��20(�#��z��$��Center=�$
��Left=�$��20,0,0,0q��� " ��.���  � ��	 �#1E1E1E��  �@��CornerRadius$	 12����  �@��Padding$	 28q���  � ��$	 0,0,0,0q���  � ��	 �#333333��  � ��$	 1q���  �@J�Effect	   $ #000000��$ 12Q�$ 2Q�$	 0.3Q��� " ��.���  � ��$	 28(���  � ��$
	 Bold"���  � ��	 �#FFFFFF��  � ��$	 0,0,0,16q��� " ��.���  � ��$	 18(���  � ��$	 SemiBold"���  � ��	 �#0D66D0��  � ��$	 0,0,0,8q�.���  � ColumnDefinitions �� $	��280 ��� $��* ��� $
��Auto �.+��� $��0�����#181818���#333333$��0,0,1,0q�._��  � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��20,32,20,40q�.+��  �� $
��Auto ��� $��* �.+��� $��0��$��48��$��48�����#0D66D0$ 8��$��0,0,16,0q�._��� $	����$��Segoe MDL2 Assets)�$��24(�
�������$��Center=�$��Center=��� $��1��$��Center=�.+��� $��PC Konfigurátor��$��20(�$
��Bold"�
��������� $��Desktop App��$��12(����#999999$��0,2,0,0q��� -    ��Name$ SidebarMenu��$��1��$��0,20,0,0q��� $��2�����#222222$ 20,16,20,16q����#333333$��0,1,0,0q�._��  �� $
��Auto ��� $��* �.+��� $��0��$��36��$��36�����#0D66D0$ 18��$��0,0,12,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��$��Center=�.+��� -   $ UserEmailText��$��user@example.com��$��13(�
�������$��CharacterEllipsis=��� -   $ UserRoleText��$��	User Role��$��11(����#999999$��0,2,0,0q�� $��1�� �� $��64 ��� $��* �.+��� $��0�����#1E1E1E���#333333$��0,0,0,1q�._��  �� $��* ��� $
��Auto �.+��� -   $ 	PageTitle��$��	Dashboard��$��20(�$��SemiBold"�
�������$��Center=�$��28,0,0,0q��� $��1�� �� Orientation$ 
Horizontal=�$��Right=�$��Center=�$��0,0,16,0q�.+��� -    ��Name$ MinimizeButton��$	����$��Segoe MDL2 Assets)�$��12(�$��40��$��40��
������ ���#CCCCCC$��0q��� -   $ CloseButton��$	����$��Segoe MDL2 Assets)�$��12(�$��40��$��40��
������ ���#CCCCCC$��0q����� " ��.���  � ��
	 ���� ��  �@�Cursor$
	 Hand�� �� Triggers P�  �@J�IsMouseOver . . ���  � ��	 �#E81123��  � ��
	 ������ $��1�����#121212.+��� #��[ $��28q�$	��400��$
��1200��$	��400��$
��1400��$��Center=�$��Center=�._��� -    ��Name$ MainContentArea��