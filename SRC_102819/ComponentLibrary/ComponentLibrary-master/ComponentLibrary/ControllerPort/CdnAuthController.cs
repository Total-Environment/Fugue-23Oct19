using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.WindowsAzure.Storage.Blob;
using TE.ComponentLibrary.ComponentLibrary.App_Start;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.ECToken;
using TE.Shared.CloudServiceFramework.Domain;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{

    [RoutePrefix("auth")]
    public class CdnAuthController:BaseController
    {

        private readonly IBlobService _azureService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CdnAuthController"/> class.
        /// </summary>
        /// <param name="blobService">The BLOB service.</param>
        public CdnAuthController(IBlobService blobService)
        {
            _azureService = blobService;
        }

        /// <summary>
        /// Authenticates the cl.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        [ComponentLibraryAuthorize(Permissions = "Get Cdn and Saas Token")]
        [HttpGet]
        [Route("CL")]
        [ResponseType(typeof(CLAuthDto))]
        public IHttpActionResult AuthenticateCL(int duration = 60)
        {
            IDictionary<string, string> sasTokens = new Dictionary<string, string>();
            AddSasTokenForContainer("staticFiles", duration, sasTokens);
            return Ok(new CLAuthDto
            {
                CdnToken = GetCdnToken(duration),
                SasTokens = sasTokens
            });
        }

        private void AddSasTokenForContainer(string container, int duration, IDictionary<string, string> sasTokens)
        {
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(duration),
                Permissions = SharedAccessBlobPermissions.Read
            };
            var containerName = ConfigurationManager.AppSettings[container];
            var containerSasToken = _azureService.GenerateSasTokenForContainer(containerName, policy);
            sasTokens.Add(containerName, containerSasToken);
        }

        private CdnToken GetCdnToken(int duration)
        {
            var tokenGenerator = new ECTokenGenerator();
            var cdnToken = tokenGenerator.EncryptV3(ConfigurationManager.AppSettings["cdnKey"], DateTime.UtcNow.AddMinutes(duration), null, null, null,
                null, null, "https", null, null);

            return new CdnToken
            {
                paramName = ConfigurationManager.AppSettings["cdnQueryParam"],
                token = cdnToken
            };
        }
    }
}