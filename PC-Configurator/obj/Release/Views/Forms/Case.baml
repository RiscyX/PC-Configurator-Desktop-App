   M S B A M L   `   `   `  ���� J  FPC-Configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PC_Configurator.Views.FormsPC_Configurator.Views.Forms  &     PC_Configurator.Views.Forms.Case   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ;local)clr-namespace:PC_Configurator.Views.Forms       � 	Resources     ModernInputBox �� 
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
BlurRadius$ 10Q�._��� $��Center=�$	��700��.+��� -     ��Name$  	FormTitle��$��Gépház hozzáadása��
�������$��32(�$
��Bold"�$��Center=�$��	0,10,0,40q��� $
��Név�����#AAAAAA$��16(�$��0,0,0,8q��� -   ! ��Name$! NameTextBox��#��[  $��0,0,0,5q��� -   $  	NameError��  ValidationErrorText#��[ �� $��	Formátum�����#AAAAAA$��16(�$��0,20,0,8q��� -   " ��Name$" FormFactorComboBox��#��[ $��0,0,0,5q�# �� SelectedIndex$# 0��.@��� $	��ATX��
����������#1E1E1E�� $��	Micro-ATX��
����������#1E1E1E�� $��Mini-ITX��
����������#1E1E1E�� $��E-ATX��
����������#1E1E1E�� -   $  FormFactorError��#��[ �� $��Szín�����#AAAAAA$��16(�$��0,20,0,8q��� -   $" ColorComboBox��#��[ $��0,0,0,5q�$# 0��.@��� $��Fekete��
����������#1E1E1E�� $��Fehér��
����������#1E1E1E�� $��Ezüst��
����������#1E1E1E�� $
��Kék��
����������#1E1E1E�� $��Piros��
����������#1E1E1E�� -   $  
ColorError��#��[ �� $��	Ár (HUF)�����#AAAAAA$��16(�$��0,20,0,8q��� -   $! PriceTextBox��#��[  $��0,0,0,5q��� -	   $  
PriceError��#��[ �� -
   $ ��Name$$ 
SaveButton��$��Mentés��$	��240��$��60��$��	0,30,0,20q����#0D66D0
�������$��20(�$��SemiBold"�$��0q�% � Cursor$
% Hand��$��Center=�  ��%9   '��       �� " ��.���  �@��CornerRadius$ 8��   $ 1Q�$	 0.3Q�$ 5Q�