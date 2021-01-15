namespace MasterAppStoreLoadFiles.Controllers
{
    using MasterAppStoreLoadFiles.Helpers;
    using MasterAppStoreLoadFiles.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly AzureStorageConfig storageConfig;

        public ImageController(IOptions<AzureStorageConfig> config)
        {
            storageConfig = config.Value;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            try
            {
                List<string> results = await StorageHelper.GetImagesUrls(storageConfig);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(ICollection<IFormFile> files, string idProduct)
        {
            bool isUploaded = false;

            try
            {
                if (files.Count == 0)
                    return BadRequest("No files received from the upload");

                if (idProduct == null) 
                    return BadRequest("No productId received from the upload");

                foreach (var formFile in files)
                {
                    if (StorageHelper.IsImage(formFile))
                    {
                        using (Stream stream = formFile.OpenReadStream())
                        {
                            isUploaded = await StorageHelper.UploadFileToStorage(stream, formFile.FileName, storageConfig, idProduct);
                        }
                    }
                    else
                    {
                        return new UnsupportedMediaTypeResult();
                    }
                }
                if (isUploaded)
                {
                    return new AcceptedResult();
                }
                else
                {
                    return BadRequest("Look like the image couldnt upload to the image");
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}

