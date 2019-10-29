using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.Shared.CloudServiceFramework.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller
{
    /// <summary>
    /// Represents a static file controller.
    /// </summary>
    /// <seealso cref="BaseController" />
    [RoutePrefix("static-file")]
    public class StaticFileController : BaseController
    {
        private readonly IStaticFileRepository _staticFileRepository;
        private readonly IBlobDownloadService _blobDownloadService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticFileController"/> class.
        /// </summary>
        /// <param name="staticFileRepository">The static file repository.</param>
        /// <param name="blobDownloadService">The BLOB download service.</param>
        public StaticFileController(IStaticFileRepository staticFileRepository, IBlobDownloadService blobDownloadService)
        {
            _staticFileRepository = staticFileRepository;
            _blobDownloadService = blobDownloadService;
        }

        /// <summary>
        /// Gets the static file.
        /// </summary>
        /// <param name="staticFileId">The static file identifier.</param>
        /// <returns>Returns a static file with given id.</returns>
        [Route("{staticFileId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStaticFile(string staticFileId)
        {
            try
            {
                var staticFile = await _staticFileRepository.GetById(staticFileId);
                var memoryStream = (MemoryStream)(await _blobDownloadService.Download("static-files", staticFile.Name));
                var streamContent = new ByteArrayContent(memoryStream.ToArray());
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = staticFile.Name
                };
                return new ResponseMessageResult(new HttpResponseMessage
                {
                    Content = streamContent
                });
            }
            catch
                (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch
                (ArgumentException e)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Posts the specified static file.
        /// </summary>
        /// <param name="staticFile">The static file.</param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] StaticFile staticFile)
        {
            var savedStaticFile = await _staticFileRepository.Add(staticFile);

            return Created("", savedStaticFile);
        }

        /// <summary>
        /// Finds the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Return the static file with given name.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> FindBy(string name)
        {
            try
            {
                var staticFile = await _staticFileRepository.FindByName(name);
                return Ok(staticFile);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
        }
    }
}