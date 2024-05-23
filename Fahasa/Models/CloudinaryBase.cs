using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class CloudinaryBase
    {
        public static Account account = new Account(
                ConfigurationManager.AppSettings["CloudinaryAccount"],
                ConfigurationManager.AppSettings["CloudinaryAPIKey"],
                ConfigurationManager.AppSettings["CloudinarySecretKey"]
            );
        public static Cloudinary cloudinary = new Cloudinary(account);
        public static ImageUploadResult UploadImage(ImageUploadParams image)
        {
            image.UploadPreset = "fahasa";
           

            return cloudinary.Upload(image);
        }

        public static DelResResult DeleteImage(string url) {
            Uri uri = new Uri(url);
            string fileName = Path.GetFileNameWithoutExtension(uri.LocalPath);
            
            DelResParams delResParams = new DelResParams();
            delResParams.PublicIds.Add("fahasa/" + fileName);

            return cloudinary.DeleteResources(delResParams);
        }

    }
}