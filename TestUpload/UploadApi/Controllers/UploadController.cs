using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace UploadApi.Controllers
{
    public class UploadController : ApiController
    {
        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            const string fileDirectory = "UploadedFiles/";
            string root = HttpContext.Current.Server.MapPath("~/" + fileDirectory);
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                
                var filepath = "";
                foreach (var file in provider.FileData)
                {
                    var filename = Guid.NewGuid() + file.Headers.ContentDisposition.FileName.Replace("\"", "");
                    var fileInfo = new FileInfo(file.LocalFileName);

                    File.Move(fileInfo.FullName, root + filename);
                }

                return new HttpResponseMessage
                {
                    Content = new StringContent(filepath)
                };
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}