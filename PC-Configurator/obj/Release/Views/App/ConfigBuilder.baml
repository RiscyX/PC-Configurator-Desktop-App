   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PC_Configurator.Views.AppPC_Configurator.Views.App  -    'PC_Configurator.Views.App.ConfigBuilder   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     9local'clr-namespace:PC_Configurator.Views.App       � 	Resources     ComponentCard �� 
TargetType �� Property �� Value  ComponentCardSelected3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth   
BlurRadius   Color   Opacity  ComponentTypeHeader  ActionButton �� 
TargetType	 �� CornerRadius
 �� Padding �� Triggers P� Property P� Value  SecondaryButton �� BasedOn��%�  &	        &	 �     &	 �    &	     &	 �    7  �� " ��.���  � �� �#252525��  � �� �#333333��  � ��$ 1q���  �@��CornerRadius$ 8����  � ��$ 0,0,0,24q���  � ��$ 80���� " ��.���  � �� �#252525��  � �� �#0D66D0��  � ��$ 2q���  �@��CornerRadius$ 8����  � ��$ 0,0,0,24q���  � ��$ 80����  �@J�Effect   $ 1Q�$ 10Q�$ #0D66D0��$	 0.3Q��� " ��.���  � ��$ 16(���  � ��$ SemiBold"���  � �� �#AAAAAA��  � ��$ 	0,10,0,10q��� " ��.���  � �� �#0D66D0��  � �� �#FFFFFF��  � ��$ 	20,8,20,8q���  � ��$ 0,0,0,0q���  � ��$ 36����  � ��$ 14(���  � ��$ SemiBold"���  � ��$ 0q���  �@�Cursor$
 Hand����  � �� �� " ��.R��� #��z��$	 4��#
 z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � �� �#1976D2P�  � �� . . ���  � �� �#0D47A1�� " ��8   .���  � �� �#333333 P�  �@J�IsMouseOver . . ���  � �� �#444444.��� $��20,20,20,20q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+�� $��0��$��0,0,0,20q� � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��0��.+��� $*��$Új konfiguráció összeállítása��$��24(�$
��Bold"�
�������$��0,0,0,8q��� $\��VVálaszd ki a szükséges komponenseket és állítsd össze álmaid számítógépét�����#AAAAAA$��14(�$
��Wrap=��� $��1�� �� Orientation$ 
Horizontal=�$	��Top=�.+��� $��Teljes ár:��$��16(����#AAAAAA$��Center=�$��0,0,10,0q��� ���#0D66D0$	 6��$
 	15,8,15,8q�._��� -    ��Name$ TotalPriceBlock��$
��0 Ft��$��20(�$
��Bold"����#FFFFFF$��Center=��� $��1��$
��Auto=�$��Disabled=�.���  �� $��3* ��� $��1* �.+��� $��0��$��0,0,30,0q�.+��� $��Processzor (CPU)��#��[ �� -    ��Name$ CPUCard��#��[   � Cursor$
 Hand��
 � Tag$	 CPU��._�� $��20q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��40��$��40�����#333333$	 20��$��0,0,20,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��.+��� -   $ CPUNameBlock��$��Válassz processzort��$��16(�
��������� -   $ CPUDetailBlock��$��Nincs kiválasztva��$��12(����#AAAAAA$��0,4,0,0q��� $��2��$	����$��Segoe MDL2 Assets)�$��14(����#AAAAAA$��Right=�$��Center=��� $��Alaplap��#��[ �� -   $ MotherboardCard��#��[  $
 Hand��$ Motherboard��._�� $��20q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��40��$��40�����#333333$	 20��$��0,0,20,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��.+��� -   $ MotherboardNameBlock��$��Válassz alaplapot��$��16(�
��������� -   $ MotherboardDetailBlock��$��Nincs kiválasztva��$��12(����#AAAAAA$��0,4,0,0q��� $��2��$	����$��Segoe MDL2 Assets)�$��14(����#AAAAAA$��Right=�$��Center=��� $��Videókártya (GPU)��#��[ �� -   $ GPUCard��#��[  $
 Hand��$	 GPU��._�� $��20q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��40��$��40�����#333333$	 20��$��0,0,20,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��.+��� -	   $ GPUNameBlock��$��Válassz videókártyát��$��16(�
��������� -
   $ GPUDetailBlock��$��Nincs kiválasztva��$��12(����#AAAAAA$��0,4,0,0q��� $��2��$	����$��Segoe MDL2 Assets)�$��14(����#AAAAAA$��Right=�$��Center=��� $��Memória (RAM)��#��[ �� -   $ RAMCard��#��[  $
 Hand��$	 RAM��._�� $��20q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��40��$��40�����#333333$	 20��$��0,0,20,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��.+��� -   $ RAMNameBlock��$��Válassz memóriát��$��16(�
��������� -   $ RAMDetailBlock��$��Nincs kiválasztva��$��12(����#AAAAAA$��0,4,0,0q��� $��2��$	����$��Segoe MDL2 Assets)�$��14(����#AAAAAA$��Right=�$��Center=��� $��Tárhely��#��[ �� -   $ StorageCard��#��[  $
 Hand��$ Storage��._�� $��20q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��40��$��40�����#333333$	 20��$��0,0,20,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��.+��� -   $ StorageNameBlock��$��Válassz tárhelyet��$��16(�
��������� -   $ StorageDetailBlock��$��Nincs kiválasztva��$��12(����#AAAAAA$��0,4,0,0q��� $��2��$	����$��Segoe MDL2 Assets)�$��14(����#AAAAAA$��Right=�$��Center=��� $��Tápegység��#��[ �� -   $ PSUCard��#��[  $
 Hand��$	 PSU��._�� $��20q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��40��$��40�����#333333$	 20��$��0,0,20,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��.+��� -   $ PSUNameBlock��$��Válassz tápegységet��$��16(�
��������� -   $ PSUDetailBlock��$��Nincs kiválasztva��$��12(����#AAAAAA$��0,4,0,0q��� $��2��$	����$��Segoe MDL2 Assets)�$��14(����#AAAAAA$��Right=�$��Center=��� $��Számítógépház��#��[ �� -   $ CaseCard��#��[  $
 Hand��$
 Case��._�� $��20q� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��40��$��40�����#333333$	 20��$��0,0,20,0q�._��� $	����$��Segoe MDL2 Assets)�$��18(�
�������$��Center=�$��Center=��� $��1��.+��� -   $ CaseNameBlock��$#��Válassz számítógépházat��$��16(�
��������� -   $ CaseDetailBlock��$��Nincs kiválasztva��$��12(����#AAAAAA$��0,4,0,0q��� $��2��$	����$��Segoe MDL2 Assets)�$��14(����#AAAAAA$��Right=�$��Center=��� $��1��.+��� ���#252525���#333333$��1q�$	 8��$
 16q�._��� .+��� $#��Konfiguráció összefoglaló��$��18(�$��SemiBold"�
�������$��0,0,0,16q��� ���#1E1E1E���#333333$��1q�$	 6��$
 12q�$��0,0,0,16q�._��� .+��  �� $
��Auto ��� $��* �.+��� $��0��$	����$��Segoe MDL2 Assets)�$��18(����#0D66D0$��0,0,8,0q��� $��1��$"��Kompatibilitás ellenőrzés��$��14(�$��SemiBold"�
��������� -   $ CompatibilityMessage��$@��:Válassz komponenseket a kompatibilitás ellenőrzéséhez�����#AAAAAA$��12(�$��0,8,0,0q�$
��Wrap=��� $!��Teljesítmény értékelés��$��14(�$��SemiBold"�
�������$��0,0,0,8q�� $��0,0,0,16q� �� $��* ��� $
��Auto �.+�� -    �Name$ PerformanceBar��$��0�� � Value$ 0Q� � Maximum$	 100Q�$��8�����#333333���#0D66D0�� -   $ PerformanceRating��$��1��$	��N/A��
�������$��14(�$
��Bold"�$��8,0,0,0q��� $��Ajánlott felhasználás��$��14(�$��SemiBold"�
�������$��0,0,0,8q��� -   $ RecommendedUseBlock��$ ��Általános felhasználás�����#AAAAAA$��14(�$
��Wrap=��� -   $ PerformanceScoreBlock��$y�Hidden=�$��0/100���� ���#252525���#333333$��1q�$	 8��$
 16q�$��0,16,0,0q�._��� .+��� $#��Becsült teljesítményigény��$��16(�$��SemiBold"�
�������$��0,0,0,12q��  �� $
��Auto ��� $��* �.+��� $��0��$	����$��Segoe MDL2 Assets)�$��20(����#0D66D0�� $��1��$��12,0,0,0q�.+��� -   $ PowerConsumptionBlock��$	��0 W��$��22(�$
��Bold"�
��������� $��becsült használat��$��12(����#AAAAAA� $��2��$��0,20,0,0q� �� $��* ��� $
��Auto �.+��� -   $��0��$��Komponenslista törlése��#��[ $
��Left=��� $��1��$ 
Horizontal=�.+��� -   $��Konfiguráció mentése��#��[ 