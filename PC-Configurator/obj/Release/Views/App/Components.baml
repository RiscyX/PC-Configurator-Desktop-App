   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:PC_Configurator.Views.AppPC_Configurator.Views.App  *    $PC_Configurator.Views.App.Components   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     9local'clr-namespace:PC_Configurator.Views.App       � 	Resources     FilterButton �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� CornerRadius �� Triggers P� Property	 P� Value
 �� 
TargetName  ActiveFilterButton �� BasedOn��%�  &	        &	 K    7   �� " ��.���  � �� �#1E1E1E��  � �� �#CCCCCC��  � ��$ 16(���  � ��$ SemiBold"���  � ��$	 110����  � ��$ 40����  � ��$	 6,0q���  �@�Cursor$
 Hand����  � ��$ 1q���  � �� �#333333��  � �� �� " ��.R��� $ ButtonBorder��#��z��#��z��#��z��$ 8��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver	 . . ��� $
 ButtonBorder�� � �� �#252525�� $
 ButtonBorder�� � �� �#0D66D0P�  � ��	 . . ��� $
 ButtonBorder�� � �� �#333333�� " ��8   .���  � �� �#0D66D0��  � �� �#FFFFFF��  � �� �#0D66D0.��� ���#121212$
��1400��$	��800�� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* �$	��400��.+��� $��0�����#1A1A1A �� Padding$ 32,20q����#333333$��0,0,0,1q�._�� .+��� $��Alkatrészek kezelése��$��28(�$
��Bold"�
�������$��Center=��� $��Right=� �� Orientation$ 
Horizontal=�.+�� $	��240��$��40��$��0,0,16,0q�.+��� -    ��Name$ SearchTextBox�����#232323
����������#444444$��1q�$��Center=�$��	12,0,40,0q�$��15(�.��  ��%9   '��       �� " ��.���  �@��CornerRadius$ 8��.���� $��Keresés név alapján...�����#777777$��Center=�$��16,0,0,0q� J� IsHitTestVisible .  ���� " ��.���  � y�$ 	Collapsed=� �� Triggers ��  �� Binding �� *Text+ �� ElementName$ SearchTextBox�� �� Value  .a���  � y�$ Visible=��� $	����$��Segoe MDL2 Assets)�$��16(����#AAAAAA$��Right=�$��Center=�$��0,0,16,0q� .  �� -    ��Name$ SearchButton��$��Keresés��$	��100��$��40�����#0D66D0
�������$��0q�  ��%9   '��       �� " ��.���  �@��CornerRadius$ 8���� -    ��Name$ TypeSelectorPanel��$ 
Horizontal=�$��Center=�$��	0,24,0,16q�$��1��.+��� -   $ BtnCPU��$	��CPU��
 � Tag$	 CPU��#��[ �� -   $ BtnGPU��$	��GPU��$	 GPU��#��[  �� -   $ BtnRAM��$	��RAM��$	 RAM��#��[  �� -   $ 
BtnStorage��$��Storage��$ Storage��#��[  �� -   $ BtnMotherboard��$��Motherboard��$ Motherboard��$	��130��#��[  �� -	   $ BtnPSU��$	��PSU��$	 PSU��#��[  �� -
   $ BtnCase��$
��Case��$
 Case��#��[  �� $��2��$��	32,0,32,0q�
������ $��0q�$ 0,10,0,0q�._��� .+�� $��0,0,0,16q�.+��� -    ��Name$ ComponentTypeTitle��$��CPU komponensek��$��24(�
�������$
��Bold"�$��Center=��� -   $��Új hozzáadása��$��Right=�$��40��$	��140�����#0D66D0
�������$��0q�$��SemiBold"�  ��%9   '��       �� " ��.���  �@��CornerRadius$ 8���� $A��;Kattintson bármelyik elemre a részletek megtekintéséhez��$��15(����#AAAAAA$��0,0,0,10q�� $��2��$��	0,120,0,0q�.+��� -    ��Name$ ComponentScroller��$
��Auto=�$��Disabled=�$��0,0,5,0q�.���� -    ��Name$ ComponentList��$��
32,0,32,32q����� .R�$� ���� .R��� $ componentBorder��-   ���#1E1E1E$ 12��$��0,8,0,12q�$ 24q� � Cursor$
 Hand�����#333333$��1q� �� *Id+ J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 2Q�   Opacity$	 0.2Q�   
BlurRadius$ 10Q�  � Triggers  :� ! :� RoutedEvent$! Border.MouseEnter��.[��� .���� .���� " �� TargetProperty$1" +(Border.Background).(SolidColorBrush.Color)�	# �� To$# #252525X�$ g� Duration$$ 0:0:0.2D��� $2" ,(Border.BorderBrush).(SolidColorBrush.Color)�$# #0D66D0X�$$ 0:0:0.2D�:� $! Border.MouseLeave��.[��� .���� .���� $1" +(Border.Background).(SolidColorBrush.Color)�$# #1E1E1EX�$$ 0:0:0.2D��� $2" ,(Border.BorderBrush).(SolidColorBrush.Color)�$# #333333X�$$ 0:0:0.2D�._�� % � ColumnDefinitions% �� $��* ��� $
��Auto �.+��� $��0��$ Vertical=�.+��� $��22(�
�������$��SemiBold"����� *Name+�� $��14(����#AAAAAA$��0,6,0,0q����� *Id+& �� StringFormat$& ID: {0}���� $ AdminButtons��$��1��$ 
Horizontal=�$��Center=�$��Right=�.+��� $ 
EditButton��-   $��42��$��42�����#0D66D0$��0q�$��0,0,10,0q� �� *Id+  ��%9   '��       �� " ��.���  �@��CornerRadius$ 8��.���� $	����$��Segoe MDL2 Assets)�$��16(�
�������$��Center=�$��Center=��� $ DeleteButton��-   $��42��$��42�����#851D1D$��0q� �� *Id+  ��%9   '��       �� " ��.���  �@��CornerRadius$ 8��.���� $	����$��Segoe MDL2 Assets)�$��16(�
�������$��Center=�$��Center=�' � ToolTip' _� ���#121212$��0q����#333333$��1q�.���� ���#121212$ 20q�$ 8��$	��300��._��� .+��� $��20(�
�������$��SemiBold"�$��0,0,0,12q����� *Name+�� $��1�����#333333$��0,0,0,12q��� $��16(�
�������$
��Wrap=�( �� 
LineHeight$( 24������ *	Tooltip+�� -   $ NoResultsPanel�����#1E1E1E$ 12��$��	32,0,32,0q�$ 30q����#333333$��1q�$y�	Collapsed=�$	��200��$	��Top=�._��� $��Center=�$��Center=�.+��� $	����$��Segoe MDL2 Assets)�$��40(����#AAAAAA$��Center=�$��0,0,0,16q��� $��Nincs találat��$��22(�$��SemiBold"�
�������$��Center=�$��0,0,0,10q��� $1��+Próbálkozzon más keresési kifejezéssel��$��16(����#AAAAAA$��Center=�$
��Wrap=�) �� TextAlignment$) Center=�