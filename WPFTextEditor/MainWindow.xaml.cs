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
        private string _tempStr = string.Empty;

        public void CopyMethod(object sender, ExecutedRoutedEventArgs e) => _tempStr = txt.SelectedText;

        public void PasteMethod(object sender, ExecutedRoutedEventArgs e)=> txt.Text.Insert(txt.SelectionStart, _tempStr);

        public void SelectAllMethod(object sender, ExecutedRoutedEventArgs e) => txt.SelectAll();

        public void CutMethod(object sender, ExecutedRoutedEventArgs e)
        {
            _tempStr = txt.SelectedText;
            txt.Text.Remove(txt.SelectionStart, txt.SelectionLength);
        }

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
                txt.Text = streamReader.ReadToEnd();
            }

        }

        private void btnSaveDialog_Click(object sender, RoutedEventArgs e)
        {
            var saveFile = new SaveFileDialog();

            if (saveFile.ShowDialog() is true)
            {
                using StreamWriter streamWriter = new(saveFile.FileName);
                streamWriter.Write(txt.Text);
            }
        }


        private void btnFileSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFileSave.Text))
            {
                MessageBox.Show("Enter File Name", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            File.WriteAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{txtFileSave.Text}.txt", txt.Text);
            MessageBox.Show("File Succesfully Saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            txtFileSave.Text = string.Empty;
        }

        private void btnFileLoad_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFileLoad.Text))
            {
                MessageBox.Show("Load Path Cannot Be Empty", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var fileInfo = new FileInfo(txtFileLoad.Text);

            if (fileInfo.Exists)
            {
                using StreamReader streamReader = new(fileInfo.FullName);
                txt.Text = streamReader.ReadToEnd();
                txtFileLoad.Text = string.Empty;
                return;
            }

            MessageBox.Show("File Not Found", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void cpTextColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {


            if (cpTextColor.SelectedColor is System.Windows.Media.Color color)
            {
                if(txt.SelectionLength>0)
                    txt.SelectionTextBrush = new SolidColorBrush(color);

                txt.Foreground = new SolidColorBrush(color);
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

        private void cBoxFontStyle_SelectionChanged(object sender, SelectionChangedEventArgs e) => txt.FontFamily = new System.Windows.Media.FontFamily(cBoxFontStyle.SelectedItem.ToString());

        private void cBoxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e) => txt.FontSize = double.Parse(cBoxFontSize.SelectedItem.ToString()!);

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

                txt.FontWeight = _isBold ? FontWeights.Bold : FontWeights.Normal;
                txt.FontStyle = _isItalic ? FontStyles.Italic : FontStyles.Normal;
                txt.TextDecorations = _isUnderLined ? TextDecorations.Underline : txt.TextDecorations = null;
            }

        }

        private void ButtonAlignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                txt.TextAlignment = btn.Content.ToString() switch
                {
                    "L" => TextAlignment.Left,
                    "C" => TextAlignment.Center,
                    "R" => TextAlignment.Right,
                    _ => txt.TextAlignment
                };

            }
        }

        private void chkAutoSave_Checked(object sender, RoutedEventArgs e)
        {
            if (_isAutoSaveUsed == false)
            {

                var result = MessageBox.Show("Tour Text Will be automatically saved on Desktop with Name KepaText.txt \nDo you accept It?","Information",MessageBoxButton.YesNo,MessageBoxImage.Question);

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

            File.WriteAllText(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\KepaText.txt", txt.Text);

        }
    }
}
