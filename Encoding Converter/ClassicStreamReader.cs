using System;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Encoding_Converter
{
    public class ClassicStreamReader : IDisposable
    {
        private readonly Encoding _encoding;
        private readonly StorageFile _file;
        private IRandomAccessStreamWithContentType _accessStream;
        private Stream _classicStream;
        private StreamReader _streamReader;
        
        public bool EndOfStream => _streamReader.EndOfStream;
        
        public ClassicStreamReader(Encoding encoding, StorageFile file)
        {
            _encoding = encoding;
            _file = file;
            GenerateStream();
        }

        private async void GenerateStream()
        {
            _accessStream = await _file.OpenReadAsync();
            _classicStream = _accessStream.AsStreamForRead();
            _streamReader = new StreamReader(_classicStream, _encoding);
        }

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
            _classicStream?.Dispose();
            _accessStream?.Dispose();
        }
    }
}
