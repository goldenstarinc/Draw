﻿#pragma checksum "K:\MyDesktop\GitHub\Photoshop_2\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F46339AD29AB68C5E5343808EEC3F8ED06E480760B21515A42CEB0A65EC50253"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App3
{
    partial class MainWindow : 
        global::Microsoft.UI.Xaml.Window, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainWindow.xaml line 16
                {
                    global::Microsoft.UI.Xaml.Controls.Grid element2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Grid)element2).KeyDown += this.Canvas_KeyDown;
                }
                break;
            case 3: // MainWindow.xaml line 240
                {
                    this.colorPickerDialog = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ContentDialog>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ContentDialog)this.colorPickerDialog).PrimaryButtonClick += this.ColorPickerDialog_PrimaryButtonClick;
                    ((global::Microsoft.UI.Xaml.Controls.ContentDialog)this.colorPickerDialog).SecondaryButtonClick += this.ColorPickerDialog_SecondaryButtonClick;
                }
                break;
            case 4: // MainWindow.xaml line 246
                {
                    this.colorPicker = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ColorPicker>(target);
                }
                break;
            case 5: // MainWindow.xaml line 231
                {
                    this.DrawingCanvas = global::WinRT.CastExtensions.As<global::App3.CustomCanvas>(target);
                    ((global::App3.CustomCanvas)this.DrawingCanvas).PointerPressed += this.Canvas_PointerPressed;
                    ((global::App3.CustomCanvas)this.DrawingCanvas).PointerMoved += this.Canvas_PointerMoved;
                    ((global::App3.CustomCanvas)this.DrawingCanvas).PointerReleased += this.Canvas_PointerReleased;
                    ((global::App3.CustomCanvas)this.DrawingCanvas).PointerEntered += this.CustomCanvas_PointerEntered;
                    ((global::App3.CustomCanvas)this.DrawingCanvas).PointerExited += this.CustomCanvas_PointerExited;
                }
                break;
            case 6: // MainWindow.xaml line 111
                {
                    this.slider = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Slider>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Slider)this.slider).ValueChanged += this.StrokeThickness_Changed;
                }
                break;
            case 7: // MainWindow.xaml line 213
                {
                    this.myNumberButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.SplitButton>(target);
                }
                break;
            case 8: // MainWindow.xaml line 214
                {
                    this.SelectedNumber = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 9: // MainWindow.xaml line 216
                {
                    this.numberFlyout = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Flyout>(target);
                }
                break;
            case 10: // MainWindow.xaml line 217
                {
                    this.ButtonContainer = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.StackPanel>(target);
                }
                break;
            case 11: // MainWindow.xaml line 218
                {
                    global::Microsoft.UI.Xaml.Controls.Button element11 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element11).Click += this.NumberButton_Click;
                }
                break;
            case 12: // MainWindow.xaml line 219
                {
                    global::Microsoft.UI.Xaml.Controls.Button element12 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element12).Click += this.PlusButton_Click;
                }
                break;
            case 13: // MainWindow.xaml line 220
                {
                    global::Microsoft.UI.Xaml.Controls.Button element13 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element13).Click += this.MinusButton_Click;
                }
                break;
            case 14: // MainWindow.xaml line 188
                {
                    this.myColorButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.SplitButton>(target);
                }
                break;
            case 15: // MainWindow.xaml line 204
                {
                    this.OpenColorPicker = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.OpenColorPicker).Click += this.OpenColorPickerButton_Click;
                }
                break;
            case 16: // MainWindow.xaml line 189
                {
                    this.CurrentColor = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Border>(target);
                }
                break;
            case 17: // MainWindow.xaml line 191
                {
                    this.colorList = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Flyout>(target);
                }
                break;
            case 18: // MainWindow.xaml line 193
                {
                    global::Microsoft.UI.Xaml.Controls.Button element18 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element18).Click += this.ColorButton_Click;
                }
                break;
            case 19: // MainWindow.xaml line 194
                {
                    global::Microsoft.UI.Xaml.Controls.Button element19 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element19).Click += this.ColorButton_Click;
                }
                break;
            case 20: // MainWindow.xaml line 195
                {
                    global::Microsoft.UI.Xaml.Controls.Button element20 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element20).Click += this.ColorButton_Click;
                }
                break;
            case 21: // MainWindow.xaml line 196
                {
                    global::Microsoft.UI.Xaml.Controls.Button element21 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element21).Click += this.ColorButton_Click;
                }
                break;
            case 22: // MainWindow.xaml line 197
                {
                    global::Microsoft.UI.Xaml.Controls.Button element22 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element22).Click += this.ColorButton_Click;
                }
                break;
            case 23: // MainWindow.xaml line 198
                {
                    global::Microsoft.UI.Xaml.Controls.Button element23 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element23).Click += this.ColorButton_Click;
                }
                break;
            case 24: // MainWindow.xaml line 129
                {
                    this.Line = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Line).Click += this.SelectLine;
                }
                break;
            case 25: // MainWindow.xaml line 134
                {
                    this.Circle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Circle).Click += this.SelectCircle;
                }
                break;
            case 26: // MainWindow.xaml line 139
                {
                    this.Rectangle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Rectangle).Click += this.SelectRectangle;
                }
                break;
            case 27: // MainWindow.xaml line 144
                {
                    this.Triangle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Triangle).Click += this.SelectTriangle;
                }
                break;
            case 28: // MainWindow.xaml line 149
                {
                    this.RightTriangle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.RightTriangle).Click += this.SelectRightTriangle;
                }
                break;
            case 29: // MainWindow.xaml line 154
                {
                    this.Rhomb = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Rhomb).Click += this.SelectRhombus;
                }
                break;
            case 30: // MainWindow.xaml line 161
                {
                    this.GoldenStar = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.GoldenStar).Click += this.SelectGoldenStar;
                }
                break;
            case 31: // MainWindow.xaml line 166
                {
                    this.Person = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Person).Click += this.SelectPerson;
                }
                break;
            case 32: // MainWindow.xaml line 171
                {
                    this.Square = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Square).Click += this.SelectSquare;
                }
                break;
            case 33: // MainWindow.xaml line 176
                {
                    this.Octagon = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Octagon).Click += this.SelectOctagon;
                }
                break;
            case 34: // MainWindow.xaml line 67
                {
                    this.Brush = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Brush).Click += this.SelectBrush;
                }
                break;
            case 35: // MainWindow.xaml line 74
                {
                    this.Fill = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Fill).Click += this.SelectFill;
                }
                break;
            case 36: // MainWindow.xaml line 81
                {
                    this.ColorPicker = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.ColorPicker).Click += this.SelectColorPicker;
                }
                break;
            case 37: // MainWindow.xaml line 88
                {
                    this.Text = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Text).Click += this.SelectText;
                }
                break;
            case 38: // MainWindow.xaml line 94
                {
                    this.Clear = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Clear).Click += this.ClearCanvas;
                }
                break;
            case 39: // MainWindow.xaml line 101
                {
                    this.Eraser = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Eraser).Click += this.SelectEraser;
                }
                break;
            case 40: // MainWindow.xaml line 26
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element40 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element40).Click += this.NewFile_Click;
                }
                break;
            case 41: // MainWindow.xaml line 31
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element41 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element41).Click += this.OpenFile_Click;
                }
                break;
            case 42: // MainWindow.xaml line 36
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element42 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element42).Click += this.SaveFile_Click;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }


        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

