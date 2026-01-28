using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Requests;

public sealed class UploadObjectRequest
{
  [FromForm(Name = "file")]
  public IFormFile File { get; set; } = default!;
}
