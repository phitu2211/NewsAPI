using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NewsAPI.Controllers.V1
{
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("api/v1/image/{imageName}")]
        public IActionResult Get(String imageName)
        {
            try
            {
                Byte[] image = System.IO.File.ReadAllBytes(_webHostEnvironment.WebRootPath + "\\Images\\" + imageName);   // You can use your own method over here.         
                return File(image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
    }
}