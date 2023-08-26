namespace Bookify.web.Services
{
	public interface IImageService
	{
		Task<(bool IsUploaded, string ErrorMassage)> UploadAsync(IFormFile Image,string ImageName,string Folderpath,bool HasThumbNail);
		void Delete(string ImagePath,string? ImageThumNailPath =null);

	}
}
