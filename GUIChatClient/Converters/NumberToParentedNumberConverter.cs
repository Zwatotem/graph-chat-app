using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GraphChatApp.Converters;

public class NumberToParentedNumberConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		dynamic number = value;
		switch (value)
		{
			case int i:
				number = i;
				break;
			case double d:
				number = d;
				break;
			case decimal m:
				number = m;
				break;
			default:
				number = 0;
				break;
		}
		return "(" + number.ToString("N0") + ")";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var stripped = value.ToString().Replace("(", "").Replace(")", "");
		return decimal.Parse(stripped);
	}
}