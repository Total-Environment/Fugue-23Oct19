using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.Shared.CloudServiceFramework.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller
{
    /// <summary>
    /// Controller for all document related endpoints
    /// </summary>
    /// <seealso cref="BaseController" />
    [RoutePrefix("upload")]
    public class DocumentController : BaseController
    {
        private readonly IDocumentRepository _documentRepository;

        /// <summary>
        /// Constructor of Document controller
        /// </summary>
        /// <param name="documentRepository"></param>
        public DocumentController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        /// <summary>
        /// Creates a static file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("static-files")]
        public async Task<IHttpActionResult> Create()
        {
            var response = new List<StaticFile>();
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Not a file type");
            }
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                if(!IsValidFileName(filename))
                    return BadRequest($"Invalid file format : {filename}");

                var buffer = await file.ReadAsStreamAsync();
                try
                {
                    filename = Guid.NewGuid().ToString("D") +"."+ filename.Split('.').Last();
                    var staticFile = await _documentRepository.Upload(filename, buffer);
                    response.Add(staticFile);
                }
                catch (BlobUploadException ex)
                {
                    return StatusCode(HttpStatusCode.BadGateway);
                }
            }
            return Created("", response);
        }

        private bool IsValidFileName(string filename)
        {
            return new[] {"jpg", "JPG", "jpeg", "png", "PNG", "PDF", "pdf", "JPEG"}.Contains(filename.Split('.').Last());
        }
    }
}