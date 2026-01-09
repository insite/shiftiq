using InSite.Application.Files.Read;

using Microsoft.AspNetCore.Mvc;

using Shift.Common.File;
using Shift.Service.Content;
using Shift.Service.Feedback;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Content API: Files")]
public class FileController : ShiftControllerBase
{
    private readonly IMonitor _monitor;
    private readonly FileService _fileService;
    private readonly IShiftIdentityService _identityService;
    private readonly IStorageServiceAsync _storageService;
    private readonly ResponseService _responseService;

    private static readonly TimeSpan PublicFileMaxAge = TimeSpan.FromHours(1);

    public FileController(
        IMonitor monitor,
        FileService fileService,
        IShiftIdentityService identityService,
        IStorageServiceAsync storageService,
        ResponseService responseService
        )
    {
        _monitor = monitor;
        _fileService = fileService;
        _identityService = identityService;
        _storageService = storageService;
        _responseService = responseService;
    }

    #region Queries

    /// <summary>
    /// Checks for the existence of one specific file
    /// </summary>
    [HttpHead("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Assert)]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [EndpointName("assertFile")]
    public async Task<IActionResult> AssertAsync([FromRoute] Guid file, CancellationToken cancellation = default)
    {
        var exists = await _fileService.AssertAsync(file, cancellation);

        return exists ? Ok() : NotFound();
    }

    /// <summary>
    /// Collects the list of files that match specific criteria
    /// </summary>
    [HttpPost("content/files/collect")]
    [HybridAuthorize(Policies.Content.Files.File.Collect)]
    [ProducesResponseType<IEnumerable<FileModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFiles")]
    public async Task<IActionResult> PostCollectAsync([FromBody] CollectFiles query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    [HttpGet("content/files")]
    [HybridAuthorize(Policies.Content.Files.File.Collect)]
    [ProducesResponseType<IEnumerable<FileModel>>(StatusCodes.Status200OK)]
    [EndpointName("collectFiles_get")]
    [AliasFor("collectFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCollectAsync([FromQuery] CollectFiles query, CancellationToken cancellation = default)
    {
        return await CollectAsync(query, cancellation);
    }

    private async Task<IActionResult> CollectAsync(CollectFiles query, CancellationToken cancellation)
    {
        var models = await _fileService.CollectAsync(query, cancellation);

        var count = await _fileService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(models);
    }

    /// <summary>
    /// Counts the files that match specific criteria
    /// </summary>
    [HttpPost("content/files/count")]
    [HybridAuthorize(Policies.Content.Files.File.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFiles")]
    public async Task<IActionResult> PostCountAsync([FromBody] CountFiles query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    [HttpGet("content/files/count")]
    [HybridAuthorize(Policies.Content.Files.File.Count)]
    [ProducesResponseType<CountResult>(StatusCodes.Status200OK)]
    [EndpointName("countFiles_get")]
    [AliasFor("countFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCountAsync([FromQuery] CountFiles query, CancellationToken cancellation = default)
    {
        return await CountAsync(query, cancellation);
    }

    private async Task<IActionResult> CountAsync(CountFiles query, CancellationToken cancellation)
    {
        var count = await _fileService.CountAsync(query, cancellation);

        return Ok(new CountResult(count));
    }

    /// <summary>
    /// Downloads the list of files that match specific criteria
    /// </summary>    
    [HttpPost("content/files/download")]
    [HybridAuthorize(Policies.Content.Files.File.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFiles")]
    public async Task<FileContentResult> PostDownloadAsync([FromBody] CollectFiles query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    [HttpGet("content/files/download")]
    [HybridAuthorize(Policies.Content.Files.File.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces("application/octet-stream")]
    [EndpointName("downloadFiles_get")]
    [AliasFor("downloadFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<FileContentResult> GetDownloadAsync([FromQuery] CollectFiles query, CancellationToken cancellation = default)
    {
        return await DownloadAsync(query, cancellation);
    }

    private async Task<FileContentResult> DownloadAsync(CollectFiles query, CancellationToken cancellation)
    {
        var exporter = new ExportHelper("Content", "Files", query.Filter.Format, User);

        var models = await _fileService
            .DownloadAsync(query, cancellation)
            .ToListAsync(cancellation);

        var content = _fileService.Serialize(models, exporter.GetFileFormat(), query.Filter.Includes);

        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = exporter.CreateFileName();

        var contentType = exporter.GetContentType(fileName);

        return File(contentBytes, contentType, fileName);
    }

    /// <summary>
    /// Retrieves one specific file
    /// </summary>
    [HttpGet("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Retrieve)]
    [ProducesResponseType<FileModel>(StatusCodes.Status200OK)]
    [EndpointName("retrieveFile")]
    public async Task<IActionResult> RetrieveAsync([FromRoute] Guid file, CancellationToken cancellation = default)
    {
        var model = await _fileService.RetrieveAsync(file, cancellation);

        return model != null ? Ok(model) : NotFound();
    }

    /// <summary>
    /// Searches for the list of files that match specific criteria
    /// </summary>
    [HttpPost("content/files/search")]
    [HybridAuthorize(Policies.Content.Files.File.Search)]
    [ProducesResponseType<IEnumerable<FileMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFiles")]
    public async Task<IActionResult> PostSearchAsync([FromBody] SearchFiles query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    [HttpGet("content/files/search")]
    [HybridAuthorize(Policies.Content.Files.File.Search)]
    [ProducesResponseType<IEnumerable<FileMatch>>(StatusCodes.Status200OK)]
    [EndpointName("searchFiles_get")]
    [AliasFor("searchFiles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetSearchAsync([FromQuery] SearchFiles query, CancellationToken cancellation = default)
    {
        return await SearchAsync(query, cancellation);
    }

    private async Task<IActionResult> SearchAsync(SearchFiles query, CancellationToken cancellation)
    {
        var matches = await _fileService.SearchAsync(query, cancellation);

        var count = await _fileService.CountAsync(query, cancellation);

        Response.AddPagination(query.Filter, count);

        return Ok(matches);
    }

    #endregion Queries

    #region Commands

    [HttpPost("content/files")]
    [HybridAuthorize(Policies.Content.Files.File.Create)]
    [ProducesResponseType<FileModel>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("createFile")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFile create, CancellationToken cancellation = default)
    {
        var created = await _fileService.CreateAsync(create, cancellation);

        if (!created)
            return BadRequest($"Duplicate not permitted: FileIdentifier {create.FileIdentifier}. You cannot insert a duplicate object with the same primary key.");

        var model = await _fileService.RetrieveAsync(create.FileIdentifier, cancellation);

        return CreatedAtAction(nameof(CreateAsync), model);
    }

    [HttpDelete("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("deleteFile")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid file, CancellationToken cancellation = default)
    {
        var deleted = await _fileService.DeleteAsync(file, cancellation);

        if (!deleted)
            return NotFound();

        return Ok();
    }

    [HttpPut("content/files/{file:guid}")]
    [HybridAuthorize(Policies.Content.Files.File.Modify)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationFailure>(StatusCodes.Status400BadRequest, "application/json")]
    [EndpointName("modifyFile")]
    public async Task<IActionResult> ModifyAsync([FromRoute] Guid file, [FromBody] ModifyFile modify, CancellationToken cancellation = default)
    {
        var model = await _fileService.RetrieveAsync(file, cancellation);

        if (model is null)
            return NotFound($"File not found: FileIdentifier {modify.FileIdentifier}. You cannot modify an object that is not in the database.");

        var modified = await _fileService.ModifyAsync(modify, cancellation);

        if (!modified)
            return NotFound();

        return Ok();
    }

    #endregion Commands

    #region File upload/download

    /// <summary>
    /// Upload files (with content)
    /// </summary>
    /// <remarks>
    /// Uploads one or more files to temporary storage for authenticated users or valid survey response sessions.
    /// </remarks>
    /// <param name="responseId">Optional survey response identifier for unauthenticated respondents</param>
    /// <returns>
    /// Returns a list of <see cref="UploadFileInfo"/> objects containing file identifiers, names, and sizes, or:
    /// - 401 Unauthorized if user is not authenticated and survey session is invalid
    /// - 400 Bad Request if no files are provided in the request
    /// </returns>
    [HttpPost("content/files/temp")]
    public async Task<IActionResult> UploadTempFileAsync(string? responseId = null)
    {
        var principal = _identityService.GetPrincipal();

        if (principal.OrganizationId == null || (!principal.IsAuthenticated && !ValidateSurveyResponse()))
        {
            return Unauthorized();
        }

        var files = Request.Form.Files;
        if (files.Count == 0)
            return BadRequest("No Files");

        var userId = principal.UserId ?? Constant.UserIdentifiers.Someone;
        var organizationId = principal.OrganizationId;
        var result = new List<UploadFileInfo>();

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];

            using var stream = file.OpenReadStream();

            var model = await _storageService.CreateAsync(
                stream,
                file.FileName,
                organizationId ?? Guid.Empty,
                userId,
                ObjectIdentifiers.Temporary,
                FileObjectType.Temporary,
                new FileProperties { DocumentName = file.FileName },
                null
            );

            result.Add(new UploadFileInfo
            {
                FileIdentifier = model.FileIdentifier,
                DocumentName = model.Properties.DocumentName,
                FileName = model.FileName,
                FileSize = model.FileSize
            });
        }

        return Ok(result);

        bool ValidateSurveyResponse()
        {
            if (string.IsNullOrEmpty(responseId) || !Guid.TryParse(responseId, out var id))
                return false;

            return _responseService.Assert(id) && _responseService.IsIncomplete(id);
        }
    }

    /// <summary>
    /// Retrieve a file (content only)
    /// </summary>
    /// <remarks>
    /// Retrieves the content for a specific file using its unique ID and filename. Performs authorization checks and 
    /// supports caching. It is important to note the caller must know the name of the file, in addition to its unique 
    /// ID, or the server will return a 404 Not Found - even if the ID is valid.
    /// </remarks>
    /// <param name="id">The unique identifier of the file to retrieve</param>
    /// <param name="name">The expected filename for validation purposes</param>
    /// <param name="download">Optional parameter: set to "1" to force download, otherwise file is displayed inline</param>
    /// <returns>
    /// Returns the file content with appropriate headers, or:
    /// - 404 Not Found if file doesn't exist or filename doesn't match
    /// - 401 Unauthorized if user lacks permission to access the file
    /// - 304 Not Modified if file hasn't changed since If-Modified-Since header date
    /// </returns>
    [HttpGet("content/files/{id:guid}/{name}")]
    public async Task<IActionResult> RetrieveFileContentAsync(Guid id, string name, string? download = null)
    {
        var principal = _identityService.GetPrincipal();

        var (status, file) = await _storageService.GetFileAndAuthorizeAsync(principal, id);

        switch (status)
        {
            case FileGrantStatus.NoFile:
                return NotFound();

            case FileGrantStatus.Denied:
                return Unauthorized();

            default:
                break;
        }

        if (!string.Equals(file.FileName, name, StringComparison.OrdinalIgnoreCase)
            || !_storageService.IsRemoteFilePathValid(file)
            )
        {
            return NotFound();
        }

        var isPublic = file.Claims.NullIfEmpty() == null;
        Response.Headers.CacheControl = isPublic
            ? $"public, max-age={(int)PublicFileMaxAge.TotalSeconds}"
            : "private, no-cache";

        // Handle If-Modified-Since header for caching

        if (Request.Headers.IfModifiedSince.Count > 0)
        {
            if (DateTimeOffset.TryParse(Request.Headers.IfModifiedSince.ToString(), out var ifModifiedSince))
            {
                var fileModified = file.Uploaded.AddSeconds(-1);

                if (ifModifiedSince >= fileModified)
                {
                    return StatusCode(304); // Not Modified
                }
            }
        }

        return await SendFileAsync(id, download == "1");
    }

    /// <summary>
    /// Create a file content result
    /// </summary>
    /// <remarks>
    /// Creates an HTTP response that contains the content for a specific file, with appropriate content type and 
    /// HTTP response headers.
    /// </remarks>
    /// <param name="fileId">The unique identifier of the file to send</param>
    /// <param name="download">If true, forces file download; if false, displays inline when possible</param>
    /// <returns>
    /// Returns a <see cref="FileContentResult"/> with the file content and appropriate headers, or:
    /// - 404 Not Found if file cannot be read, doesn't exist, or directory is missing
    /// - 500 Internal Server Error if an error occurs during file processing
    /// </returns>
    private async Task<IActionResult> SendFileAsync(Guid fileId, bool download)
    {
        try
        {
            var (file, stream) = await GetFileStreamSafelyAsync(fileId);

            if (file == null || stream == null)
            {
                return NotFound("The requested file could not be located");
            }

            using (stream)
            {
                return await CreateFileResultAsync(file, stream, download);
            }
        }
        catch (FileAccessException ex)
        {
            return HandleUnexpectedError(ex, $"accessing file {fileId}");
        }
    }

    private async Task<(FileStorageModel? file, Stream? stream)> GetFileStreamSafelyAsync(Guid fileId)
    {
        try
        {
            return await _storageService.GetFileStreamAsync(fileId);
        }
        catch (ReadFileStreamFailedException fe) when (fe.InnerException is HttpRequestException)
        {
            throw new FileAccessException("The file cannot be read from the remote host");
        }
        catch (FileNotFoundException)
        {
            throw new FileAccessException("The requested file does not exist");
        }
        catch (DirectoryNotFoundException)
        {
            throw new FileAccessException("The file directory does not exist");
        }
    }

    private async Task<IActionResult> CreateFileResultAsync(FileStorageModel file, Stream stream, bool download)
    {
        var contentType = MimeMapping.GetContentType(file.FileName);

        var bytes = (file.FileSize == 0)
            ? []
            : await ReadStreamToBytesAsync(stream);

        return new FileContentResult(bytes, contentType)
        {
            FileDownloadName = download ? file.FileName : null,
            LastModified = file.Uploaded
        };
    }

    private static async Task<byte[]> ReadStreamToBytesAsync(Stream stream)
    {
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    private IActionResult HandleUnexpectedError(Exception ex, string doingWhat)
    {
        return ExceptionHandlingMiddleware
            .ReportUnexpectedProblem(ex, doingWhat, HttpContext, _monitor)
            .ToActionResult(this);
    }

    #endregion
}