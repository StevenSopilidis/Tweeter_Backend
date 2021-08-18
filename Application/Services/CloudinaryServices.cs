using System.Threading.Tasks;
using Application.Helpers;
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Services
{

    //services that is used for the cloudinary functionality
    public class CloudinaryServices: ICloudinaryServices
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryServices(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }


        // func for uploading a photo to Cloudinary
        public async Task<ImageUploadResult> UploadPhoto(IFormFile file)
        {
            var result = new ImageUploadResult();

            using var stream = file.OpenReadStream();
            //specify the upload photo params 
            var uploadParams = new ImageUploadParams{
                File= new FileDescription(file.Name,stream),
                Transformation= new Transformation().Width(700).Height(600).Crop("fill")
            };
            //upload the photo to cloudinary
            result = await _cloudinary.UploadAsync(uploadParams);
            return result;
        }


        //func for deleting an Image from Cloudianry
        public async Task<DeletionResult> DeleteImage (string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            return deleteResult;
        }
    }
}