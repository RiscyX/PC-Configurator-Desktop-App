   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PC_Configurator.Views.FormsPC_Configurator.Views.Forms  %    PC_Configurator.Views.Forms.CPU   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ;local)clr-namespace:PC_Configurator.Views.Forms       � 	Resources     ModernInputBox �� 
TargetType �� Property �� Value  ModernComboBox �� 
TargetType d�Name �� 	ClickMode d� 	IsChecked �� Mode	 �� RelativeSource
 ��Name �� CornerRadius � ColumnDefinitions F�Name �� Triggers P� Property P� Value �� 
TargetName �� Color �Name �� IsDropDownOpen � AllowsTransparency J� SnapsToDevicePixels-  'System.Windows.Input.KeyboardNavigation   DirectionalNavigation �� SelectionBoxItem �� SelectionBoxItemTemplate J� IsHitTestVisible��%�  &	        &	 S    �� " ��.���  � �� �#1E1E1E��  � ��
 �������  � �� �#333333��  � ��$ 1q���  � ��$ 16,12q���  � ��$ 16(���  � ��$ Segoe UI)���  � ��$ Center=���  � ��$
 Left=���  �@��
CaretBrush
 �������  �@��SelectionBrush �#0D66D0��  � ��$ 50���� " ��.���  � �� �#1E1E1E��  � ��
 �������  � �� �#333333��  � ��$ 1q���  � ��$ 16,12q���  � ��$ 16(���  � ��$ Segoe UI)���  � ��$ Center=���  � ��$ 50����  � �� �� " ��.R�� .+�d� $ ToggleButton��$ Press=� �� *IsDropDownOpen+$ TwoWay=�	 �� *TemplatedParent+���� " d�.R��� $
 Border��#��z��#��z��#��z��$ 6��._��  �� �� $��30 �.+��� #��z��$
��Left=�$��Center=�F� $ Arrow��$��1��
�������$��Center=�$��Center=�"����        � z     � 	=  	= 	 P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#0D66D0P�  �@d�	IsChecked$
 trueY�. ��� $ Border�� � �� �#0D66D0���� $ #1E1E1E������ $ #333333����u�1q�� $ Popup��#��z |�.   . $��Slide=�$��Bottom=�.���� $
 PopupBorder�����#1E1E1E���#333333$��1q�$ 6��$��0,1,0,0q�._��� $��0q� . .���� $ 	Contained=��� #��z #��z #��z��$��Center=�$
��Left=�#��z�� .  .��� ���#121212.+��� ���#1E1E1E$ 12�� �� Padding$ 40q�$	��800��$��Center=�$��Center=����#333333$��1q�$	��600�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 2Q�   Opacity$	 0.2Q�   
BlurRadius$ 10Q�._��� $��Center=�$	��700��.+��� $��CPU hozzáadása��
�������$��32(�$
��Bold"�$��Center=�$��	0,10,0,40q��� $
��Név�����#AAAAAA$��16(�$��0,0,0,8q��� -     ��Name$  NameTextBox��#��[  $��0,0,0,5q��� -   ! ��Name$! NameErrorBlock��  ValidationErrorText#��[ �� $��Gyártó�����#AAAAAA$��16(�$��0,0,0,8q��� -   " ��Name$" ManufacturerComboBox��#��[ $��0,0,0,5q�# �� SelectedIndex$# 0��.@��� $��Válasszon...��
����������#1E1E1E�� $	��AMD��
����������#1E1E1E�� $��Intel��
����������#1E1E1E�� -   $! ManufacturerErrorBlock��#��[ �� $��Magok száma�����#AAAAAA$��16(�$��0,0,0,8q��� -   $" CoresComboBox��#��[ $��0,0,0,5q�$# 0��.@��� $��Válasszon...��
����������#1E1E1E�� $��2��
����������#1E1E1E�� $��4��
����������#1E1E1E�� $��6��
����������#1E1E1E�� $��8��
����������#1E1E1E�� $��10��
����������#1E1E1E�� $��12��
����������#1E1E1E�� $��14��
����������#1E1E1E�� $��16��
����������#1E1E1E�� $��18��
����������#1E1E1E�� $��20��
����������#1E1E1E�� $��22��
����������#1E1E1E�� $��24��
����������#1E1E1E�� $��26��
����������#1E1E1E�� $��28��
����������#1E1E1E�� $��30��
����������#1E1E1E�� $��32��
����������#1E1E1E�� -   $! CoresErrorBlock��#��[ �� $��Szálak száma�����#AAAAAA$��16(�$��0,0,0,8q��� -   $" ThreadsComboBox��#��[ $��0,0,0,5q�$# 0��.@��� $��Válasszon...��
����������#1E1E1E�� $��2��
����������#1E1E1E�� $��4��
����������#1E1E1E�� $��6��
����������#1E1E1E�� $��8��
����������#1E1E1E�� $��10��
����������#1E1E1E�� $��12��
����������#1E1E1E�� $��14��
����������#1E1E1E�� $��16��
����������#1E1E1E�� $��18��
����������#1E1E1E�� $��20��
����������#1E1E1E�� $��22��
����������#1E1E1E�� $��24��
����������#1E1E1E�� $��26��
����������#1E1E1E�� $��28��
����������#1E1E1E�� $��30��
����������#1E1E1E�� $��32��
����������#1E1E1E�� -   $! ThreadsErrorBlock��#��[ �� $��Alap órajel (GHz)�����#AAAAAA$��16(�$��0,0,0,8q��� -	   $  BaseClockTextBox��#��[  $��0,0,0,5q��� -
   $! BaseClockErrorBlock��#��[ �� $��Max órajel (GHz)�����#AAAAAA$��16(�$��0,0,0,8q��� -   $  BoostClockTextBox��#��[  $��0,0,0,5q��� -   $! BoostClockErrorBlock��#��[ �� $��Foglalat�����#AAAAAA$��16(�$��0,0,0,8q��� -   $" SocketComboBox��#��[ $��0,0,0,5q��� -   $! SocketErrorBlock��#��[ �� $��	Ár (HUF)�����#AAAAAA$��16(�$��0,0,0,8q��� -   $  PriceTextBox��#��[  $��0,0,0,5q��� -   $! PriceErrorBlock��#��[ �� $��Fogyasztás (W)�����#AAAAAA$��16(�$��0,0,0,8q��� -   $  PowerConsumptionTextBox��#��[  $��0,0,0,5q��� -   $ ! PowerConsumptionErrorBlock��#��[ �� -   $! 	FormTitle��$��CPU hozzáadása��$y�	Collapsed=��� -   $ ��Name$$ 
SaveButton��$��Mentés��$	��240��$��60��$��	0,10,0,20q����#0D66D0
�������$��20(�$��SemiBold"�$��0q�% � Cursor$
% Hand��$��Center=�  ��%9   '��       �� " ��.���  �@��CornerRadius$ 8��   $ 1Q�$	 0.3Q�$ 5Q�