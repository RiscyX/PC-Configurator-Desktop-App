   M S B A M L   `   `   `  ����   PC-Configurator,    &PC_Configurator.Views.User.LoginWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$'  !PC Konfigurátor - Bejelentkezés��$	��640��$	��600�� � WindowStartupLocation$ CenterScreen=����#121212
������� � WindowStyle$
 None=� � 
ResizeMode$ CanMinimize=� � AllowsTransparency .  � 	Resources    ModernInputBox �� 
TargetType �� Property �� Value  ModernPasswordBox  ModernCheckbox	 �� 
TargetType
 ��Name �� CornerRadius (� Stretch F�Name �� StrokeEndLineCap �� StrokeStartLineCap �� Triggers P� Property P� Value �� 
TargetName  ModernLinkButton �� Padding �� Triggers�� %�  &	        &	 =    &	 z    &	 D    �� " ��.���  � �� �#1E1E1E��  � ��
 �������  � �� �#333333��  � ��$ 1q���  � ��$ 16,12q���  � ��$ 16(���  � ��$ Segoe UI)���  � ��$ Center=���  � ��$
 Left=���  �@��
CaretBrush
 �������  �@��SelectionBrush �#0D66D0�� " G�.���  � �� �#1E1E1E��  � ��
 �������  � �� �#333333��  � ��$ 1q���  � ��$ 16,12q���  � ��$ 16(���  � ��$ Segoe UI)���  � ��$ Center=���  � ��$
 Left=���  �@G�
CaretBrush
 �������  �@G�SelectionBrush �#0D66D0�� " ��.���  � ��$ 15(���  � ��$ Segoe UI)���  � �� �#AAAAAA��  � ��$ 0,0,0,16q���  � �� �� "	 ��.R�]� ��. .+��� $
 CheckBoxBorder��$��20��$��20�����#1E1E1E���#333333$��1q�$ 4��$��0,0,10,0q�$��Center=�._�(� $ Uniform=�$��4q�.��F� $ 	CheckMark��"����    @KL � 	= @T� р�� @B 	$��2��
�������$ Round=�$ Round=�$y�	Collapsed=��� $��Center=� P�  �@d�	IsChecked$
 TrueY�. ��� $ 	CheckMark�� � y�$ Visible=��� $ CheckBoxBorder�� � �� �#0D66D0�� $ CheckBoxBorder�� � �� �#0D66D0P�  �@J�IsMouseOver . . ��� $ CheckBoxBorder�� � �� �#0D66D0�� " ��.���  � ��
 ���� ��  � �� �#0D66D0��  � ��$ 0q���  � ��$ 15(���  �@�Cursor$
 Hand����  � �� �� "	 ��.R��� 
������ $ 4q�._��� #��z��#��z��$��	Underline{�$��Center=� P�  �@J�IsMouseOver . . ���  � �� �#1E8BFF.���  � RowDefinitions �� $
��Auto ��� $��* �.+��� -   $��0�����#1E1E1E$��48��._�� .+��� $��PC KONFIGURÁTOR��$
��Left=�$��Center=�$��20,0,0,0q�$��16(�$��SemiBold"�
���������  �� Orientation$ 
Horizontal=�$��Right=�.+��� -    ��Name$ MinimizeButton��$��48��$��48��
������ $��0q� � Cursor$
 Hand��.���� $	����$��Segoe MDL2 Assets)�$��12(����#AAAAAA�� -   $ CloseButton��$��48��$��48��
������ $��0q�$
 Hand��.���� $	����$��Segoe MDL2 Assets)�$��12(����#AAAAAA�� $��1�����#1E1E1E$��1,0,1,1q�._��� ���#1E1E1E$ 12��$ 40q�$��40q����#333333$��1q� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 2Q�   Opacity$	 0.2Q�   
BlurRadius$ 10Q�._��� $��Center=�$��Center=�$	��420��.+��� $��Bejelentkezés��$��32(�$
��Bold"�$��Center=�
�������$��0,0,0,35q��� $��
Email cím��$��16(����#AAAAAA$��0,0,0,8q��� -   $��EmailTextBox��$��50��$	��420��#��[  $��0,0,0,24q��� $��Jelszó��$��16(����#AAAAAA$��0,0,0,8q�� $��0,0,0,16q�.+�G� -   $��PasswordBox��$��50��$	��420��#��[ �� -   $��PasswordVisibleBox��$��50��$	��420��#��[  $y�	Collapsed=��� -   $��ShowPasswordCheckBox��$��Jelszó megjelenítése��#��[ �� -   $��Bejelentkezés��$	��220��$��54��$��	0,24,0,24q����#0D66D0
�������$��18(�$��SemiBold"�$��0q�$
 Hand��$��Center=� ��%9   '��       �� " ��.���  �@��CornerRadius$ 8��   $ 1Q�$	 0.3Q�$ 5Q��� -	   $(��"Nincs még fiókod? Regisztráció��#��[ $��Center=�