   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PC_Configurator.Views.AppPC_Configurator.Views.App  %    PC_Configurator.Views.App.Users   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     9local'clr-namespace:PC_Configurator.Views.App     .���   � RowDefinitions  �� $
��Auto ��� $��* �.+�� $��0��$��0,0,0,15q� � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��Felhasználók kezelése��$��24(�$��SemiBold"����#333333$��Center=��� -    ��Name$ RefreshButton��$��1��$��Frissítés��$
��15,8q����#0078D4
�������$��0q� � 	Resources  �� 
TargetType �� Property �� Value��%9   '��       �� " ��.���  �@��CornerRadius$ 4���� -    ��Name$ UsersListView��$��1��$��1q����#DDDDDD �� View �� .P��� $��Iniciálék��	 �� Width$	 70��
 �� CellTemplate
 �� .R��� $��40��$��40�� �� CornerRadius$ 20�����#0078D4._��� 
�������$��SemiBold"�$��Center=�$��Center=����� *
Initials+�� $��Email��$		 200��
 �� .R��� $��Center=����� *Email+�� $
��Név��$		 150��
 �� .R��� $��Center=����� *
FullName+�� $��Jogosultság��$		 100��
 �� .R��� $ 4�� �� Padding$	 8,4q����� *	IsAdmin+   BoolToColorConverter �� 	Converter# [  ._��� 
�������$��Center=�$��Center=����� *Role+�� $��Regisztrált��$		 100��
 �� .R��� $��Center=����� *CreatedString+�� $��Utolsó belépés��$		 150��
 �� .R��� $��Center=����� *LastLoginString+�� $��
Műveletek��$		 180��
 �� .R���  �� Orientation$ 
Horizontal=�.+��� -   $��0,0,5,0q�$	��8,3q����#0078D4
�������$��0q����� *	IsAdmin+  AdminRoleConverter# [  ��%9   '��       �� " ��.���  �@��CornerRadius$ 4���� -   $��Törlés��$	��8,3q����#E81123
�������$��0q� ��%9   '��       �� " ��.���  �@��CornerRadius$ 4������ " ��.���  � ��$ Stretch=� 4   .PC_Configurator.Views.App.BoolToColorConverter6   0PC_Configurator.Views.App.AdminRoleTextConverter��%   &	        &	          