using Commerce.Api.Requests;
using Commerce.Api.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StorageDebugController(IObjectStorageService storage) : ControllerBase
{
  // POST /api/debug/storage/upload (multipart/form-data with "file")
  [HttpPost("upload")]
  [Consumes("multipart/form-data")]
  public async Task<IActionResult> Upload([FromForm] UploadObjectRequest request, CancellationToken ct)
  {
    var file = request.File;

    if (file is null || file.Length == 0)
      return BadRequest("file is required");

    var safeName = Path.GetFileName(file.FileName);
    var objectKey = $"debug/{Guid.NewGuid():N}-{safeName}";

    await using var stream = file.OpenReadStream();
    await storage.UploadAsync(objectKey, stream, file.ContentType ?? "application/octet-stream", ct);

    return Ok(new { key = objectKey, url = storage.GetPublicUrl(objectKey) });
  }

  // DELETE /api/debug/storage?key=debug/...
  [HttpDelete]
  public async Task<IActionResult> Delete([FromQuery] string key, CancellationToken ct)
  {
    if (string.IsNullOrWhiteSpace(key))
      return BadRequest("key is required");

    await storage.DeleteAsync(key, ct);
    return NoContent();
  }
}
