using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NewsAPI.Controllers.V1
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public class FileModel
        {
            public IFormFile Files { get; set; }
        }



        [HttpPost("api/v1/file")]
        public async Task<String> Post([FromForm] FileModel model)
        {
            try
            {
                if (model.Files.Length > 0)
                {
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\Images\\"))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\Images\\");
                    }

                    using (FileStream fileStream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\Images\\" + model.Files.FileName))
                    {
                        await model.Files.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        return "Upload Success";
                    }
                }
                else
                {
                    return "Upload Failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}