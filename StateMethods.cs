using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3
{
    /// <summary>
    /// Модуль, содержащий методы изменения состояния булевых переменных
    /// </summary>
    internal static class StateMethods
    {
        /// <summary>
        /// Метод, делающий состояние булевой переменной неактивным
        /// </summary>
        internal static void MakeInactive(ref bool state) => state = false;

        /// <summary>
        /// Метод, делающий состояние булевой переменной активным
        /// </summary>
        internal static void MakeActive(ref bool state) => state = true;

        /// <summary>
        /// Метод, проверяющий состояние булевой переменной
        /// </summary>
        internal static bool IsActive(bool state) => state;
    }
}
