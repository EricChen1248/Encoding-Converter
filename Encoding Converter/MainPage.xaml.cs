using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Encoding_Converter
{
    /// <inheritdoc cref="Page" />
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private StorageFile _inputFile;
        private StorageFile _outputFile;

        private readonly SortedDictionary<string, int> _encodings;
        public MainPage()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _encodings = Encoder.EncodingDictionary();
        }

        
        private async void InputSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker {ViewMode = PickerViewMode.Thumbnail};
            filePicker.FileTypeFilter.Add("*");
            var file = await filePicker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            FileLocationText.Text = file.Path;
            _inputFile = file;
            
            
            GeneratePreview();
        }

        private async void GeneratePreview()
        {
            if (_inputFile == null || InputEncodingCombo.SelectedItem == null)
            {
                return;
            }
            
            using (var stream = await _inputFile.OpenReadAsync())
            {
                using (var classicStream = stream.AsStreamForRead())
                {
                    var codePage = ((KeyValuePair<string, int>) InputEncodingCombo.SelectedItem).Value;
                    using (var sr = new StreamReader(classicStream, Encoding.GetEncoding(codePage)))
                    {
                        var list = new List<string>();
                        var stringSize = 0;
                        while (stringSize < 1000 && sr.EndOfStream == false)
                        {
                            var str = sr.ReadLine();
                            if (str == null) continue;
                            stringSize += str.Length;
                            list.Add(str);
                        }
                        PreviewTextBlock.Text = list.Aggregate((str, s) => str + '\n' + s);
                    }
                }
            }
        }

        private async void OutputSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileSavePicker();
            if (_inputFile != null)
            {
                filePicker.FileTypeChoices.Add(_inputFile.ContentType, new List<string>{"."+_inputFile.Name.Split('.').LastOrDefault()});
            }
            else
            {
                return;
            }

            filePicker.SuggestedFileName = "Converted_File";

            var file = await filePicker.PickSaveFileAsync();
            if (file != null)
            {
                OutputLocationText.Text = file.Path;
            }

            _outputFile = file;
        }
        
        private void InputEncodingCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeneratePreview();   
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_inputFile == null || _outputFile == null || InputEncodingCombo.SelectedItem == null || OutputEncodingCombo.SelectedItem == null)
            {
                return;
            }
            
            var inCodePage = ((KeyValuePair<string, int>) InputEncodingCombo.SelectedItem).Value;
            var outCodePage = ((KeyValuePair<string, int>) OutputEncodingCombo.SelectedItem).Value;
            using (var sr = new ClassicStreamReader(Encoding.GetEncoding(inCodePage), _inputFile))
            {
                using(var sw = new ClassicStreamWriter(Encoding.GetEncoding(outCodePage), _outputFile))
                {
                    while (sr.EndOfStream == false)
                    {
                        var line = Encoding.GetEncoding(inCodePage).GetBytes(sr.ReadLine());
                        var converted = Encoder.Convert(outCodePage, inCodePage, line);
                        sw.WriteLine(converted);
                    }
                }
            }
        }
    }
}
