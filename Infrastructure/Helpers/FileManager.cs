using Microsoft.AspNetCore.Http;

namespace Infrastructure.Helpers;

public class FileManager {

  public static string DefaultImage { get; private set; } = "default.png";
  public static string Save(IFormFile file, string root, string folder) {
    var newFileName = Guid.NewGuid() + file.FileName;
    var path = Path.Combine(root, folder, newFileName);

    using FileStream fs = new(path, FileMode.Create);
    file.CopyTo(fs);

    return newFileName;
  }

  public static bool Delete(string root, string folder, string fileName) {
    var path = Path.Combine(root, folder, fileName);

    if (!File.Exists(path)) return false;
    File.Delete(path);
    return true;
  }
}