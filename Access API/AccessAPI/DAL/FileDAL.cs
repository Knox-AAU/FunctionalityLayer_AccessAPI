﻿using System.Net;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Access_API.DAL
{
    public class FileDAL
    {
        public List<byte> GetFile(int id)
        {
            List<byte> bytes = new List<byte>();

            HttpWebResponse response = Drivers.HttpRequest.GetRequest(Urls.FileTransferUrl + $"/file/{id}");
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    sr.BaseStream.CopyTo(ms);
                    byte[] b = new byte[ms.Length];
                    b = ms.ToArray();
                    bytes.AddRange(b);
                }
            }
            return bytes;
        }
    }
}
