   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PC_Configurator.Views.AppPC_Configurator.Views.App  -    'PC_Configurator.Views.App.AdminSettings   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     9local'clr-namespace:PC_Configurator.Views.App       � 	Resources     SectionTitle �� 
TargetType �� Property �� Value  
CardBorder  SettingLabel  SwitchButton �� 
TargetType ��Name �� CornerRadius ��Name �� Triggers	 P� Property
 P� Value �� 
TargetName  ActionButton �� Padding �� Triggers  DangerButton �� BasedOn  NeutralButton  StyledComboBox  StyledTextBox 	 InfoText��%�  &	        &	 s     &	 )    &	 �    &	 p    &	 :    7  &	 �    7  &	 �    &	 %    &		 �    �� " ��.���  � ��$ 20(���  � ��$ SemiBold"���  � ��
 �������  � ��$ 0,0,0,16q��� " ��.���  � �� �#252525��  � �� �#333333��  � ��$ 1q���  �@��CornerRadius$ 8����  �@��Padding$ 20q���  � ��$ 0,0,0,20q��� " ��.���  � ��$ 14(���  � ��$ Medium"���  � �� �#CCCCCC��  � ��$ 0,0,0,8q���  � ��$ Center=��� " d�.���  � �� �� " d�.R�� .+��� $ 
BackBorder��$��44��$��24�����#666666$ 12���� $ ThumbBorder��$��20��$��20��
�������$ 10��$
��Left=�$��2,0,0,0q��� $ Content��$y�	Collapsed=� P� 	 �@d�	IsChecked$

 TrueY�. ��� $ 
BackBorder�� � �� �#0D66D0�� $ ThumbBorder�� � ��$ Right=��� $ ThumbBorder�� � ��$ 0,0,2,0q���  �@�Cursor$
 Hand���� " ��.���  � �� �#0D66D0��  � �� �#FFFFFF��  � ��$ 	16,8,16,8q���  � ��$ 0,8,0,0q���  � ��$ 36����  � ��$ 14(���  � ��$ SemiBold"���  � ��$ 0q���  �@�Cursor$
 Hand����  � �� �� " ��.R��� #��z��$ 4��# z��._��� $��Center=�$��Center=� P� 	 �@J�IsMouseOver
 . . ���  � �� �#1976D2P� 	 � ��
 . . ���  � �� �#0D47A1�� " ��8   .���  � �� �#D32F2F P� 	 �@J�IsMouseOver
 . . ���  � �� �#F44336P� 	 � ��
 . . ���  � �� �#B71C1C�� " ��8   .���  � �� �#555555 P� 	 �@J�IsMouseOver
 . . ���  � �� �#666666P� 	 � ��
 . . ���  � �� �#444444�� " ��.���  � �� �#1E1E1E��  � ��
 �������  � �� �#333333��  � ��$ 1q���  � ��$ 36����  � ��$ 8,5,5,5q��� " ��.���  � �� �#1E1E1E��  � ��
 �������  � �� �#333333��  � ��$ 1q���  � ��$ 36����  � ��$ Center=���  � ��$	 8,0q��� " ��.���  � ��$ 13(���  � �� �#999999��  � ��$
 Wrap=���  � ��$ 0,4,0,12q�.���� $
��Auto=�.��� $��20q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��Admin beállítások��$��24(�$
��Bold"�
�������$��0,0,0,24q�$��0���� $��1��#��[ ._��  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��Rendszerbeállítások��#��[  $��0��� $��1��$��0,0,0,16q� � ColumnDefinitions �� $��* ��� $
��Auto � �� $
��Auto ��� $
��Auto �.+��� $��0��$��0��.+��� $��Adatbázis kapcsolat��#��[ �� $J��DAz alkalmazás által használt adatbázis kapcsolati beállításai��#��[	 �� -   $��Kapcsolat tesztelése��#��[ $��1��$��0��$	��Top=�� $��0��$��2��$��1��$��0,8,0,0q� �� $��* ��� $��* � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��Szerver��$��0��$��0��$��0,0,12,8q��� -    ��Name$ DbServerTextBox��$��	localhost��$��1��$��0��#��[ $��	0,0,12,12q��� $��Adatbázis neve��$��0��$��1��$��0,0,0,8q��� -   $ DbNameTextBox��$��pc_configurator��$��1��$��1��#��[ $��0,0,0,12q��� $��Felhasználónév��$��2��$��0��$��0,0,12,8q��� -   $ DbUserTextBox��$
��root��$��3��$��0��#��[ $��0,0,12,0q��� $��Jelszó��$��2��$��1��$��0,0,0,8q�G� -    G�Name$ DbPasswordBox��$��3��$��1�����#1E1E1E
����������#333333$��1q�$��36��$��Center=�$	��8,0q��� $��2�����#333333$��0,0,0,16q��� $��3�� �� Orientation$ Vertical=�.+�� $��0,0,0,16q� �� $��* ��� $
��Auto �.+��� $��0��.+��� $��Karbantartási mód��#��[ �� $G��AHa be van kapcsolva, csak admin felhasználók jelentkezhetnek be��#��[	 d� -    d�Name$ MaintenanceModeToggle��$��1��#��[ $	��Top=�� $��0,0,0,16q� �� $��* ��� $
��Auto �.+��� $��0��.+��� $)��#Komponens automatikus jóváhagyás��#��[ �� $@��:Az új komponensek automatikusan jóváhagyásra kerülnek��#��[	 d� -   $ AutoApproveToggle��$��1��#��[ $	��Top=� d� 	IsChecked$
 TrueY��  �� $��* ��� $
��Auto � �� $
��Auto ��� $
��Auto �.+��� $��0��$��0��.+��� $��Regisztrációs mód��#��[ �� $M��GVálaszd ki, hogyan történjenek az új felhasználói regisztrációk��#��[	 �� -    ��Name$ RegistrationModeCombo��$��0��$��2��$��1��#��[ .@��� $3��-Nyílt regisztráció (bárki regisztrálhat)���� $7��1Moderált regisztráció (admin jóváhagyással)�� �� 
IsSelected . �� $/��)Zárt rendszer (nincs új regisztráció)���  �� $��* ��� $
��Auto � �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��.+��� $$��Adatbázis biztonsági mentés��#��[ �� $0��*Automatikus adatbázis mentés ütemezése��#��[	 �� -	   $ BackupFrequencyCombo��$��0��$��2��$��1��#��[ .@��� $��Naponta���� $��Hetente�� . �� $��Havonta���� $��Kézi mentés���� -
   $��Adatbázis mentése most��#��[ $��0��$��2��$��2��$
��Left=�$��0,12,0,0q��� $��2��#��[ ._��  �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $$��Admin felhasználók kezelése��#��[  $��0��� $��1��$��0,0,0,16q� �� $��* ��� $
��Auto �.+��� $:��4Az alkalmazásban jelenleg 2 admin felhasználó van��$��Center=�$��0�����#CCCCCC�� -   $��Admin hozzáadása��#��[ $��1���� $��2�����#1E1E1E���#333333$��1q�$ 4��._��� -    ��Name$ 	AdminList��
������ $��0q����� " ��.���  � ��
 ���� ��  � ��$ 8q���  � ��$ Stretch=����� .R��� $ 8q�._��  �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��40��$��40�����#444444$ 20��$��0,0,16,0q�._��� 
�������$��16(�$��SemiBold"�$��Center=�$��Center=����� *
Initials+�� $��1��$��Center=�.+��� $��14(�$��Medium"�
����������� *DisplayName+�� $��12(����#999999$��0,4,0,0q����� *Email+�� $��2��$ 
Horizontal=�$��Center=�.+��� -   $��Jogosultságok��#��[ $��0,0,8,0q�
 � Tag �� *Id+�� -   $��Eltávolítás��#��[  �� *Id+�� $��3��#��[ ���#252020._��  �� $
��Auto ��� $
��Auto �.+��� $��Veszélyes műveletek��#��[  $��0�����#F44336�� $��1��$ Vertical=�.+�� $��0,0,0,16q� �� $��* ��� $
��Auto �.+��� $��0��.+��� $ ��Adatbázis újraépítése��#��[ ���#FF9E80�� $]��WAz adatbázis séma újraépítése. Minden adat elvész! Csak karbantartási célokra.��#��[	 ���#FF9E80�� -   $ ��Adatbázis újraépítése��#��[ $��1��$	��Top=�� $��0,0,0,16q� �� $��* ��� $
��Auto �.+��� $��0��.+��� $ ��Minden komponens törlése��#��[ ���#FF9E80�� $a��[Az összes komponens adatbázisból történő törlése. Ez a művelet nem visszavonható!��#��[	 ���#FF9E80�� -   $��Komponensek törlése��#��[ $��1��$	��Top=��  �� $��* ��� $
��Auto �.+��� $��0��.+��� $%��Minden konfiguráció törlése��#��[ ���#FF9E80�� $[��UAz összes felhasználói konfiguráció törlése. Ez a művelet nem visszavonható!��#��[	 ���#FF9E80�� -   $��Konfigurációk törlése��#��[ $��1��$	��Top=�