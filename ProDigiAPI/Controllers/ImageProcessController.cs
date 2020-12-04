using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProDigiAPI.Infrastructure;
using ProDigiAPI.Model;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProDigiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageProcessController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImageProcessController> _logger;
        //private readonly IHostEnvironment _hostEnvironment;

        //public ImageProcessController(IHostEnvironment hostEnvironment, IHttpClientFactory httpClientFactory, ILogger<ImageProcessController> logger)
        //{
        //    _logger = logger;
        //    _httpClientFactory = httpClientFactory;
        //    _hostEnvironment = hostEnvironment;
        //}

        public ImageProcessController(IHttpClientFactory httpClientFactory, ILogger<ImageProcessController> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<ImageObject>> Get(string ImageURL)
        {
            try
            {
                Color imageColor = Color.Transparent;
                using (var client = _httpClientFactory.CreateClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(ImageURL, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                            {

                                imageColor = ImageProcess.ColorDetector(streamToReadFrom);

                                //If I want to create a copy in file system.
                                //string destinationPath = Path.Combine(_hostEnvironment.ContentRootPath, @"downloads\test1.png");
                                //using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
                                //{
                                //    await streamToReadFrom.CopyToAsync(streamToWriteTo);

                                //}
                            }
                        }
                        else
                        {
                            return NotFound("Check your image URL");
                        }


                    }
                }

                //ImageProcess.ColorDetector(destinationPath);
                ColorModel ProDigiColor = ImageProcess.GetColor(imageColor);

                var Output = new ImageObject()
                {
                    Key = ProDigiColor.Name,
                    Color = ProDigiColor,
                };


                return Ok(Output);
            }
            catch (Exception ex)
            {
                //I prefer to use Azure ApplicationInsight. I can follow requests, failures and performance as well set an alert.
                _logger.LogError(ex.Message, ImageURL);
                return BadRequest(ex);
            }
        }
    }
}
