namespace FcmsPortalUI.Services
{
    public class UploadPathHelper
    {
        private const string UploadsFolderName = "uploads";

        public static string GetUploadRoot(IWebHostEnvironment env)
            => Path.Combine(env.ContentRootPath, UploadsFolderName);

        public static string GetCategoryPath(IWebHostEnvironment env, string category)
            => Path.Combine(GetUploadRoot(env), category);

        public static string GetPublicUrl(string category, string fileName)
            => $"/{UploadsFolderName}/{category}/{fileName}";

        public static string GetPhysicalPath(IWebHostEnvironment env, string publicUrl)
        {
            var relativePath = publicUrl.TrimStart('/');
            return Path.Combine(env.ContentRootPath, relativePath);
        }
    }
}
