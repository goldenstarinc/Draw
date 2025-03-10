using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3
{
    /// <summary>
    /// Модуль для создания кнопок
    /// </summary>
    internal static class ButtonCreator
    {
        /// <summary>
        /// Создание кнопки "+"
        /// </summary>
        /// <param name="ButtonContainer">Панель, содержащая кнопки</param>
        /// <param name="PlusButton_Click">Обработчик события нажатия на кнопку "+"</param>
        internal static void CreatePlusLayerButton(ref StackPanel? ButtonContainer, RoutedEventHandler PlusButton_Click)
        {
            if (ButtonContainer == null) return;

            Button plusButton = new Button
            {
                Content = '+',
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 0, 0)
            };
            plusButton.Click += PlusButton_Click;
            ButtonContainer.Children.Add(plusButton);
        }

        /// <summary>
        /// Создание кнопки "-"
        /// </summary>
        /// <param name="ButtonContainer">Панель, содержащая кнопки</param>
        /// <param name="MinusButton_Click">Обработчик события нажатия на кнопку "-"</param>
        internal static void CreateMinusLayerButton(ref StackPanel? ButtonContainer, RoutedEventHandler MinusButton_Click)
        {
            if (ButtonContainer == null) return;

            Button minusButton = new Button
            {
                Content = '-',
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 0, 0)
            };
            minusButton.Click += MinusButton_Click;
            ButtonContainer.Children.Add(minusButton);
        }

    }
}
