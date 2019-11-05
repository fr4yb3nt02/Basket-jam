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


        internal static string subirImagen(Imagen img,string entidadImagen)
        {
            try
            {
                Account account = new Account(
                                     HelperCloudinary.secretName,
                                     HelperCloudinary.apiKey,
                                      HelperCloudinary.apiSecret);



                Cloudinary cloudinary = new Cloudinary(account);


                var delParams = new DelResParams()
                {
                    PublicIds = new List<string>() { entidadImagen + "/" + img.Nombre },
                    Invalidate = true
                };
                var delResult = cloudinary.DeleteResources(delParams);

                var uploadParams = new ImageUploadParams()
                {

                    File = new FileDescription(img.ImgBase64),
                    PublicId = entidadImagen+"/" + img.Nombre,
                    Overwrite = true,

                };
                // var uploadResult = cloudinary.Upload(uploadParams).Uri;
                return cloudinary.Upload(uploadParams).Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal static string buscarImagen(string id, string entidadImagen)
        {
            try
            {
                Account account = new Account(
                                     HelperCloudinary.secretName,
                                     HelperCloudinary.apiKey,
                                      HelperCloudinary.apiSecret);

                Cloudinary cloudinary = new Cloudinary(account);

                SearchResult result = cloudinary.Search().Expression(id).Execute();
                if (result.TotalCount != 0)
                    return HelperCloudinary.cloudUrl + entidadImagen + "/" + id + "." + result.Resources[0].Format;
                else
                    return HelperCloudinary.cloudUrl + "default_tnhqrz";//entidadImagen + "/" + id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
