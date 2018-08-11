using System;
using System.Globalization;
using System.Windows.Data;

namespace Browser.Resources.Converters
{

    public class UrlToFaviconUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = (string) value;
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    if (!url.StartsWith("http"))
                    {
                        url = "http://" + url;
                    }
                        
                    var uri = new Uri(url);
                    return uri.Scheme + "://" + uri.Host + "/favicon.ico";
                }
                catch
                {
                    return null;
                }
                
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}