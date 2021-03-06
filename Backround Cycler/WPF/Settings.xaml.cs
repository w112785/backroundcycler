﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace Backround_Cycler.WPF
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : UserControl
	{
		public Settings ()
		{
			InitializeComponent ();
			//RepeatButtonDown.Content = char.ConvertFromUtf32(8595);
			//RepeatButtonUp.Content = char.ConvertFromUtf32(8593);
			//TimeChangeText.Value = 10;
		}

		private void UserControl_IsVisibleChanged (object sender, DependencyPropertyChangedEventArgs e)
		{

		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void textBox_Pasting(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent (typeof (String)))
			{
				String text = (String)e.DataObject.GetData (typeof (String));
				if (!IsTextAllowed (text))
				{
					e.CancelCommand ();
				}
			}
			else
			{
				e.CancelCommand ();
			}
		}

		private bool IsTextAllowed (string text)
		{
			Regex regex = new Regex ("[^0-9-]+"); //regex that matches disallowed text
			return !regex.IsMatch (text);
		}

		private void textBox1_PreviewTextInput (object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed (e.Text);
		}

		private void ChangeTime_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (TimeChangeText.NumValue < 1M)
			{
				TimeChangeText.NumValue = 1M;
			}
			else if (TimeChangeText.NumValue > 100000M)
			{
				TimeChangeText.NumValue = 100000M;
			}
		}
	}
}
