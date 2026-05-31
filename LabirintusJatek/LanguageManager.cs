using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LabirintusJatek
{
    public static class LanguageManager
    {
        public static void ChangeLanguage(string languageCode)
        {
            var dict = new ResourceDictionary();

            dict.Source = new Uri($"Resources/Strings.{languageCode}.xaml", UriKind.Relative);

            var dictionaries = Application.Current.Resources.MergedDictionaries;
            dictionaries.Clear();
            dictionaries.Add(dict);
        }
    }
}
