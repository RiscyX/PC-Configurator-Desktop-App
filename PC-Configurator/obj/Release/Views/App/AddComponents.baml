   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PC_Configurator.Views.AppPC_Configurator.Views.App  -    'PC_Configurator.Views.App.AddComponents   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     9local'clr-namespace:PC_Configurator.Views.App       � 	Resources     ComponentSelectButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Padding J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth	   Opacity
   
BlurRadius � ColumnDefinitions ��Name �� RelativeSource �� Triggers P� Property P� Value��%�  &	        �� " ��.���  � �� �#1E1E1E��  � ��
 �������  � ��$ 17(���  � ��$ SemiBold"���  � ��$ 60����  � ��$ 0,0,0,16q���  �@�Cursor$
 Hand����  � ��$ 1q���  � �� �#333333��  � �� �� " ��.R��� #��z��#��z��#��z��$ 8��$ 16q�   $ 1Q�$		 0.2Q�$
 5Q�._��  �� $
��Auto ��� $��* �.+��� $ 	IconBlock��$��Segoe MDL2 Assets)�$��22(�$��Center=�$��0,0,12,0q����� *Tag+ �� *TemplatedParent+�� $��1��$��Center=�$
��Left=� P�  �@J�IsMouseOver . . ���  � �� �#252525��  � �� �#0D66D0P�  � �� . . ���  � �� �#333333.��� ���#121212$��0q� �� $	��340 ��� $��* �.+��� -    ��Name$ 	LeftPanel��$��0�����#1A1A1A$ 12��$��20,20,10,20q����#333333$��1q�   $ 2Q�$		 0.2Q�$
 10Q�._��� $��15q�$��Center=�.+��� $��Alkatrészek hozzáadása��
�������$��22(�$
��Bold"�$��Center=�$��0,0,0,30q��� -    ��Name$ AddCpuButton��$��CPU hozzáadása��
 � Tag$	 ��#��[  �� -   $ AddGpuButton��$��GPU hozzáadása��$	 ��#��[  �� -   $ AddMotherboardButton��$��Alaplap hozzáadása��$	 ��#��[  �� -   $ AddRamButton��$��RAM hozzáadása��$	 ��#��[  �� -   $ AddStorageButton��$��Meghajtó hozzáadása��$	 ��#��[  �� -   $ AddPsuButton��$��Tápegység hozzáadása��$	 ��#��[  �� -   $ AddCaseButton��$��Gépház hozzáadása��$	 ��#��[  � $��1��$��10,20,20,20q�.+��� 
������ $��Center=�$��Center=�$
��Auto��$	��800������ *ActualHeight+ �� ElementName$ 	LeftPanel��._��� $
��Auto=�$
��Auto=�.���� -	    ��Name$ FormHost��$��Center=�$	��Top=�$
��Auto��