using FileStorage;
using FileStorage.Models;
using Athn.Helpers.MimeType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Athn.Tools.FilesToDb
{
    public class FileUploader
    {
        string _path;
        IFileRepository _repository;
        ILogger _logger;

        public FileUploader(string path, IFileRepository repository, ILogger logger)
        {
            _path = path;
            _repository = repository;
            _logger = logger;
        }

        public async Task Upload()
        {
            _logger.Log($"Import started at {DateTime.Now}");
            var counter = await ReadFilesFromDisc(_path, _repository);
            _logger.Log($"Import finished at {DateTime.Now}");
            _logger.Log($"{counter} files were imported");
        }

        private async Task<int> ReadFilesFromDisc(string path, IFileRepository repository)
        {
            int counter = 0;

            if (Directory.Exists(path) == false)
                return counter;

            var dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0)
                foreach (var dir in dirs)
                {
                    counter += await ReadFilesFromDisc(dir, repository);
                }

            var filePathes = Directory.GetFiles(path);
            if (filePathes.Length == 0) return counter;

            foreach (var filePath in filePathes)
            {
                var fileInfo = new FileInfo(filePath);

                var bytes = await ReadFileFromDisc(filePath);

                var mimeType = new MimeTypeChecker().GetMimeType(bytes);

                var fileDoc = new StoredFileModel()
                {
                    Name = filePath,
                    Content = bytes,
                    MimeType = mimeType,
                    Md5 = GetMd5Hash(bytes),
                    Sha1 = GetSha1Hash(bytes),
                    Ext = GetExtention(filePath, mimeType),
                    Size = fileInfo.Length
                };

                fileDoc.IsDuplicateMd5 = await repository.FindAnyByMd5(fileDoc.Md5);
                //fileDoc.IsDuplicateSha1 = await repository.FindAnyBySha1(fileDoc.Sha1);
                //bool isDuplicate = false;

                if (fileDoc.IsDuplicateMd5)
                {
                    var duplicateMda5Files = await repository.FindAllByMd5(fileDoc.Md5);

                    foreach (var df in duplicateMda5Files)
                    {
                        if (fileDoc.Sha1 == df.Sha1)
                        {
                            fileDoc.IsDuplicateSha1 = true;
                            //_logger.Log($"duplicate file {filePath}\n");
                            break;
                        }
                    }
                }
                //if (isDuplicate == false)
                {
                    try
                    {
                        await repository.UploadFileAsync(fileDoc);
                        counter++;
                        //_logger.Log($"{DateTime.Now} saved {filePath} \nMIME {mimeType}");
                    }
                    catch(Exception ex)
                    {
                        _logger.Log($"Exception in {filePath}");
                        _logger.Log($"{ex.Message}");
                    }
                }
            }

            return counter;
        }

        private async Task<byte[]> ReadFileFromDisc(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        private string GetExtention(string filePath, string mimeType)
        {
            if (String.IsNullOrEmpty(filePath) || String.IsNullOrEmpty(mimeType) || mimeType == "undefined") return null;

            var extFromMime = mimeType.Split(',');
            if (extFromMime.Length == 0) return null;

            var extFromFileName = Path.GetExtension(filePath).ToLower().Substring(1);

            if (extFromMime.Contains(extFromFileName))
                return extFromFileName;

            return extFromMime[0];
        }

        private string GetMd5Hash(byte[] file)
        {
            if (file == null)
                throw new Exception("FileUploader.GetMd5Hash: File content cannot be null!");

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(file);

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        private string GetSha1Hash(byte[] file)
        {
            if (file == null)
                throw new Exception("FileUploader.GetSha1Hash: File content cannot be null!");

            using (SHA1Managed sha1Hash = new SHA1Managed())
            {
                byte[] data = sha1Hash.ComputeHash(file);

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder(data.Length * 2);

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
