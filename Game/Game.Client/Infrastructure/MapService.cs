using System;
using System.IO;
using System.Threading.Tasks;
using Game.AdminClient.Models;

namespace Game.AdminClient.Infrastructure
{
    public class MapService : IMapService
    {
        private const int MaxFileSize = 1024 /* 1024 B */ * 1024 /* 1024 KB */; // = 1 MB 

        public async Task<Map> LoadMapAsync(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("File or path does not exist.");

            var fileInfo = new FileInfo(path);
            if (fileInfo.Length <= 0)
                throw new ArgumentException("File is empty.");

            if (fileInfo.Length > MaxFileSize)
                throw new ArgumentException("File is too big.");

            int rowLength = -1;
            int rowsCount = 0;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    var map = new Map();

                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (rowLength != -1 && rowLength != line.Length)
                            throw new ArgumentException("Map rows has different sizes.");

                        rowLength = line.Length;
                        map.Rows.Add(line);

                        rowsCount++;
                    }

                    map.Height = rowsCount;
                    map.Width = rowLength;

                    return map;
                }
            }
        }
    }
}