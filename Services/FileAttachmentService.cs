using FcmsPortal.Constants;
using FcmsPortal.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FcmsPortalUI.Services
{
    public interface IFileAttachmentService
    {
        Task<FileAttachment> UploadFileAsync(IBrowserFile file, string destinationFolder);
        Task<byte[]> GetFileContentAsync(FileAttachment attachment);
        Task<string> GetFileTypeAsync(FileAttachment attachment);
        Task DeleteFileAsync(FileAttachment attachment);
        Task<List<FileAttachment>> GetAttachmentsAsync(string category, int referenceId);
        Task SaveAttachmentReferenceAsync(FileAttachment attachment, string category, int referenceId);
    }

    public class FileAttachmentService : IFileAttachmentService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsFolder;

        public FileAttachmentService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");

            // Ensure uploads directory exists
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<FileAttachment> UploadFileAsync(IBrowserFile file, string destinationFolder)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file), "File cannot be null.");

            if (string.IsNullOrWhiteSpace(destinationFolder))
                throw new ArgumentException("Destination folder cannot be null or empty.", nameof(destinationFolder));

            if (file.Size <= 0)
                throw new ArgumentException("File size must be greater than zero.", nameof(file.Size));

            if (file.Size > FcmsConstants.MAX_FILE_SIZE)
                throw new InvalidOperationException($"File size exceeds the 10MB limit. File size: {file.Size / (1024.0 * 1024.0):F2}MB");

            // Create folder if it doesn't exist
            var targetFolder = Path.Combine(_uploadsFolder, destinationFolder);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // Generate unique filename to prevent overwrites
            var fileExtension = Path.GetExtension(file.Name);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);
            var relativePath = Path.Combine("uploads", destinationFolder, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.OpenReadStream(FcmsConstants.MAX_FILE_SIZE).CopyToAsync(stream);
            }

            // Create and return attachment record
            return new FileAttachment
            {
                FileName = file.Name,
                FilePath = relativePath.Replace('\\', '/'), // Ensure web-friendly path format
                FileSize = file.Size,
                UploadDate = DateTime.Now
            };
        }

        public async Task<byte[]> GetFileContentAsync(FileAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment), "FileAttachment cannot be null.");

            var filePath = Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/'));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {attachment.FileName}");

            return await File.ReadAllBytesAsync(filePath);
        }

        public Task<string> GetFileTypeAsync(FileAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment), "FileAttachment cannot be null.");

            var extension = Path.GetExtension(attachment.FileName).ToLowerInvariant();

            var mimeType = extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            return Task.FromResult(mimeType);
        }

        public async Task DeleteFileAsync(FileAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment), "FileAttachment cannot be null.");

            var filePath = Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        // These methods would connect to your database in a real implementation
        // For now, they're placeholders
        public Task<List<FileAttachment>> GetAttachmentsAsync(string category, int referenceId)
        {
            // In a real implementation, this would query your database
            // for attachments related to a specific category and reference ID
            return Task.FromResult(new List<FileAttachment>());
        }

        public Task SaveAttachmentReferenceAsync(FileAttachment attachment, string category, int referenceId)
        {
            // In a real implementation, this would save the attachment reference
            // to your database, associating it with the category and reference ID
            return Task.CompletedTask;
        }
    }

}