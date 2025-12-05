namespace DemoTienda.Domain.Exceptions
{
    public class FileNotFoundInBlobException : Exception
    {
        public FileNotFoundInBlobException(string fileName)
            : base($"El archivo '{fileName}' no existe en el Blob Storage.")
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }
}
