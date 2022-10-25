using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFTextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool _isAutoSaveUsed = false;
        private bool _isBold = false;
        private bool _isItalic = false;
        private bool _isUnderLined = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fonts = new InstalledFontCollection();

            foreach (System.Drawing.FontFamily font in fonts.Families)
            {
                cBoxFontStyle.Items.Add(font.Name);
            }

            for (int i = 9; i < 73; i++)
                cBoxFontSize.Items.Add(i);
        }


        private void btnFileDialog_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Text|*.txt"
            };

            if (fileDialog.ShowDialog() is true)
            {
                using StreamReader streamReader = new(fileDialog.FileName);
                txt.Selection.Text = streamReader.ReadToEnd();
            }

        }

        private void btnSaveDialog_Click(object sender, RoutedEventArgs e)
        {
            var saveFile = new SaveFileDialog();



            if (saveFile.ShowDialog() is true)
            {
                txt.SelectAll();
                using StreamWriter streamWriter = new(saveFile.FileName);
                streamWriter.Write(txt.Selection);
            }
        }


        

        private void cpTextColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {


            if (cpTextColor.SelectedColor is System.Windows.Media.Color color)
            {
                if (txt.Selection.IsEmpty)
                    txt.Foreground = new SolidColorBrush(cpTextColor.SelectedColor.Value);
                else
                    txt.Selection.ApplyPropertyValue(ForegroundProperty, new System.Windows.Media.SolidColorBrush(cpTextColor.SelectedColor.Value));

            }
        }

        private void cpBackColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (cpBackColor.SelectedColor is System.Windows.Media.Color color)
                txt.Background = new SolidColorBrush(color);
        }

        private void cpHighLightColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (cpHighLightColor.SelectedColor is System.Windows.Media.Color color)
                txt.SelectionBrush = new SolidColorBrush(color);
        }

        private void cBoxFontStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txt.Selection.IsEmpty)
                txt.FontFamily = new System.Windows.Media.FontFamily(cBoxFontStyle.SelectedItem.ToString());
            else
                txt.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new System.Windows.Media.FontFamily(cBoxFontStyle.SelectedItem.ToString()));
        }

        private void cBoxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txt.Selection.IsEmpty)
                txt.FontSize = double.Parse(cBoxFontSize.SelectedItem.ToString()!);
            else
                txt.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, cBoxFontSize.SelectedItem.ToString());
        }

        private void ButtonStyle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {

                switch (btn.Content.ToString())
                {
                    case "B":
                        _isBold = !_isBold;
                        break;

                    case "I":
                        _isItalic = !_isItalic;
                        break;

                    default:
                        _isUnderLined = !_isUnderLined;
                        break;
                }

                if (txt.Selection.IsEmpty)
                {
                    txt.FontWeight = _isBold ? FontWeights.Bold : FontWeights.Normal;
                    txt.FontStyle = _isItalic ? FontStyles.Italic : FontStyles.Normal;
                }
                else
                {
                    txt.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, _isBold ? FontWeights.Bold : FontWeights.Normal);
                    txt.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, _isItalic ? FontStyles.Italic : FontStyles.Normal);
                }

            }

        }

        private void ButtonAlignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                txt.HorizontalContentAlignment = btn.Content.ToString() switch
                {
                    "C" => HorizontalAlignment.Center,
                    "R" => HorizontalAlignment.Right,
                    _ => HorizontalAlignment.Left,
                };

            }
        }

        private void chkAutoSave_Checked(object sender, RoutedEventArgs e)
        {
            if (_isAutoSaveUsed == false)
            {

                var result = MessageBox.Show("Tour Text Will be automatically saved on Desktop with Name KepaText.txt \nDo you accept It?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _isAutoSaveUsed = true;
                    return;
                }

                chkAutoSave.IsChecked = false;
            }


        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (chkAutoSave.IsChecked == null || chkAutoSave.IsChecked == false)
                return;

            txt.SelectAll();

            File.WriteAllText(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\KepaText.txt", txt.Selection.Text);

        }
    }
}
