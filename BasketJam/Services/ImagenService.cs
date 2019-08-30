using BasketJam.Helper;
using BasketJam.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketJam.Services
{
    /*public interface IImagenService
    {
       static void subirImagen(Imagen img, string entidadImagen);
       // Task<Imagen> BuscarImagen(string id);       
    }*/

    internal class ImagenService //: IImagenService
    {


        internal static void subirImagen(Imagen img,string entidadImagen)
        {
            try
            {
                Account account = new Account(
                                     HelperCloudinary.secretName,
                                     HelperCloudinary.apiKey,
                                      HelperCloudinary.apiSecret);

                Cloudinary cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {

                    File = new FileDescription(img.ImgBase64),
                    PublicId = entidadImagen+"/" + img.Nombre,
                    Overwrite = true,

                };
                var uploadResult = cloudinary.Upload(uploadParams);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
