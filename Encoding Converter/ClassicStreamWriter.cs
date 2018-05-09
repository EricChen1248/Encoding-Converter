using System;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Encoding_Converter
{
    public class ClassicStreamWriter : IDisposable
    {
        private readonly Encoding _encoding;
        private readonly StorageFile _file;
        private IRandomAccessStream _accessStream;
        private Stream _classicStream;
        private StreamWriter _streamWriter;
        
        public ClassicStreamWriter(Encoding encoding, StorageFile file)
        {
            _encoding = encoding;
            _file = file;
            GenerateStream();
        }

        private async void GenerateStream()
        {
            _accessStream = await _file.OpenAsync(FileAccessMode.ReadWrite);
            _classicStream = _accessStream.AsStreamForWrite();
            _streamWriter = new StreamWriter(_classicStream, _encoding);
        }

        public void WriteLine(string line)
        {
            _streamWriter.WriteLine(line);
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
            _classicStream?.Dispose();
            _accessStream?.Dispose();
        }
    }
}
