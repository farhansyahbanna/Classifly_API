namespace Classifly_API.Services
{
    public interface IPhotoService
    {
        // This interface defines the contract for photo services.
        // It can be implemented by various classes to handle photo-related operations.
        // Example method signatures:
        Task<string> UploadImageAsync(IFormFile file);

        //Task<string> UploadPhotoAsync(byte[] photoData, string fileName);
        //Task<byte[]> GetPhotoAsync(string photoId);
        //Task DeletePhotoAsync(string photoId);
        //Task<IEnumerable<string>> ListPhotosAsync(int userId);
    }
}
