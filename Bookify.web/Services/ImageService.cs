using Bookify.web.Core.Const;

namespace Bookify.web.Services
{
	public class ImageService : IImageService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
		private int _maxAllowedSize = 2097152;

		public ImageService(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}



		public async Task<(bool IsUploaded, string? ErrorMassage)> UploadAsync(IFormFile Image, string ImageName, string Folderpath, bool HasThumbNail)
		{
			var extension = Path.GetExtension(Image.FileName);

			if (!_allowedExtensions.Contains(extension))
				return (IsUploaded: false, ErrorMassage: Error.extensition);

			if (Image.Length > _maxAllowedSize)
				return (isUploaded: false, ErrorMessage: Error.maxsize);

			var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{Folderpath}", ImageName);

			using var stream = File.Create(path);
			await Image.CopyToAsync(stream);
			stream.Dispose();

			if (HasThumbNail)
			{
				var thumbPath = Path.Combine($"{_webHostEnvironment.WebRootPath}{Folderpath}/thumb", ImageName);

				using var loadedImage = SixLabors.ImageSharp.Image.Load(Image.OpenReadStream());
				var ratio = (float)loadedImage.Width / 200;
				var height = loadedImage.Height / ratio;
				loadedImage.Mutate(i => i.Resize(width: 200, height: (int)height));
				loadedImage.Save(thumbPath);
			}

			return (isUploaded: true, errorMessage: null);
		}
		public void Delete(string ImagePath, string? ImageThumNailPath = null)
		{
			var oldImagePath = $"{_webHostEnvironment.WebRootPath}{ImagePath}";

			if (File.Exists(oldImagePath))
				File.Delete(oldImagePath);
			if (!string.IsNullOrEmpty(ImageThumNailPath))
			{
				var oldThumbnailPath = $"{_webHostEnvironment.WebRootPath}{ImageThumNailPath}";
				if (File.Exists(oldThumbnailPath))
					File.Delete(oldThumbnailPath);

			}

		}
	}
}
