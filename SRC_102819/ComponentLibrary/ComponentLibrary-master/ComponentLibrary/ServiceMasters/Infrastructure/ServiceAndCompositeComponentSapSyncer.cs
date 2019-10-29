using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NotificationEngine;
using TE.ComponentLibrary.ComponentLibrary.Helpers;
using TE.ComponentLibrary.ComponentLibrary.ServiceSAPServiceReference;

namespace TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IServiceAndCompositeComponentSapSyncer" />
    public class ServiceAndCompositeComponentSapSyncer : IServiceAndCompositeComponentSapSyncer
    {
        private readonly ServiceMaster_OutService _serviceMasterOut;
        private readonly INotifier _notifier;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceMasterOut"></param>
        /// <param name="notifier"></param>
        public ServiceAndCompositeComponentSapSyncer(ServiceMaster_OutService serviceMasterOut, INotifier notifier)
        {
            _serviceMasterOut = serviceMasterOut;
            _notifier = notifier;

            _serviceMasterOut.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SAPUserName"], ConfigurationManager.AppSettings["SAPPassword"]);
        }

        public async Task<bool> Sync(ServiceAndCompositeComponentRequest request)
        {
            return await SyncActual(request);
            //Task.Run(async () => await SyncActual(request));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<bool> SyncActual(ServiceAndCompositeComponentRequest request)
        {
            Trace.TraceInformation(request.ComponentType + " of " + request.Code + " is being synced to SAP.");


            var serviceClassification = new Dictionary<string, string>
            {
                { "Applicable", "0" },
                { "Exempted", "1" },
                { "Not applicable", "2" },
                { "Not relevant", "3" }
            };

            string gstClass = serviceClassification.TryGetValue(request.GSTApplicability, out gstClass) ? gstClass : "";


            Trace.TraceInformation($"{request.ComponentType} {request.Code} having  Short Description: {request.ShortDescription}, SAC Code: {request.SACCode}, GST Applicability: {gstClass}");

            try
            {
                var ServiceMasterReq = new ServiceMasterReq
                {
                    item = new[]
                    {
                        new ServiceMasterReqItem
                        {
                            SERVICENUMBER = request.Code,
                            MATL_GROUP = "17",
                            BASE_UOM = request.UnitOfMeasure,
                            SERV_CAT = "SWM",
                            P_STATUS = "Z3",
                            VALID_FROM = request.CreatedAt.ToString("dd.MM.yyyy"),
                            VAL_CLASS = "3200",
                            SERV_TYPE = "",
                            SHORT_TEXT =
                                request.ShortDescription.Length > 40
                                    ? request.ShortDescription.Substring(0, 40)
                                    : request.ShortDescription,
                            CHANGE_ID = request.Update ? "U" : "",
                            EAN11 = "",
                            USERF1_TXT = Guid.NewGuid().ToString(),
                            TAXTARIFFCODE = request.SACCode,
                            TAXIM = gstClass
                        }
                    },
                    item1 = ChunkString(request.ShortDescription, 132).Select(chunk =>
                        new ServiceMasterReqItem1
                        {
                            TDLINE = chunk
                        }
                    ).ToArray()
                };

                Trace.TraceInformation($"serviceMasterOutRequest ->" +
                                       $"SERVICENUMBER: {ServiceMasterReq.item[0].SERVICENUMBER}, " +
                                       $"MATL_GROUP: {ServiceMasterReq.item[0].MATL_GROUP}, " +
                                       $"BASE_UOM: {ServiceMasterReq.item[0].BASE_UOM}, " +
                                       $"SERV_CAT: {ServiceMasterReq.item[0].SERV_CAT}, " +
                                       $"P_STATUS: {ServiceMasterReq.item[0].P_STATUS}, " +
                                       $"VALID_FROM: {ServiceMasterReq.item[0].VALID_FROM}, " +
                                       $"VAL_CALSS: {ServiceMasterReq.item[0].VAL_CLASS}, " +
                                       $"SERV_TYPE: {ServiceMasterReq.item[0].SERV_TYPE}, " +
                                       $"SHORT_TEXT: {ServiceMasterReq.item[0].SHORT_TEXT}, " +
                                       $"CHANGE_ID: {ServiceMasterReq.item[0].CHANGE_ID}, " +
                                       $"EAN11: {ServiceMasterReq.item[0].EAN11}, " +
                                       $"USERF1_TXT: {ServiceMasterReq.item[0].USERF1_TXT}, " +
                                       $"TAXTARIFFCODE: {ServiceMasterReq.item[0].TAXTARIFFCODE}, " +
                                       $"TAXIM: {ServiceMasterReq.item[0].TAXIM}. ");

                var serviceMasterOutResponse = _serviceMasterOut.ServiceMaster_Out(ServiceMasterReq);

                if (serviceMasterOutResponse.Length > 0)
                {
                    if (serviceMasterOutResponse[0].RETCODE == "0")
                    {
                        // Success
                        Trace.TraceInformation(request.ComponentType + " of " + request.Code +
                                     " is synced to SAP. Return values are: " +
                                     "ASNUM = " + serviceMasterOutResponse[0].ASNUM + ", " +
                                     "EAN11 = " + serviceMasterOutResponse[0].EAN11 + ", " +
                                     "USERF1_TXT = " + serviceMasterOutResponse[0].USERF1_TXT + ", " +
                                     "MESSAGE = " + serviceMasterOutResponse[0].MESSAGE + ", " +
                                     "RETCODE = " + serviceMasterOutResponse[0].RETCODE + ".");
                        return true;
                    }
                    else
                    {
                        // Failure. 
                        Trace.TraceError(request.ComponentType + "service of " + request.Code +
                                      " is NOT synced to SAP. Return values are: " +
                                     "ASNUM = " + serviceMasterOutResponse[0].ASNUM + ", " +
                                     "EAN11 = " + serviceMasterOutResponse[0].EAN11 + ", " +
                                     "USERF1_TXT = " + serviceMasterOutResponse[0].USERF1_TXT + ", " +
                                     "MESSAGE = " + serviceMasterOutResponse[0].MESSAGE + ", " +
                                     "RETCODE = " + serviceMasterOutResponse[0].RETCODE + ".");
                        await Notify(request, serviceMasterOutResponse[0].MESSAGE);
                        return false;
                    }
                }
                Trace.TraceError(request.ComponentType + " of " + request.Code +
                                 " is NOT synced to SAP. Length of materialMasterOutResponse.MaterialMasterRes is 0.");

                await Notify(request, serviceMasterOutResponse[0].MESSAGE);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError(request.ComponentType + " of " + request.Code +
                              " is NOT synced to SAP. Exception Message: " + ex.Message + " , Exception: " +
                              ex);

                await Notify(request,ex.Message);
                return false;
            }
        }

        private IEnumerable<string> ChunkString(string str, int chunkLength)
        {
            if (str.Length <= chunkLength)
            {
                return new List<string> { str };
            }
            return new List<string> { str.Substring(0, chunkLength) }.Concat(ChunkString(str.Substring(chunkLength), chunkLength));
        }

        private async Task Notify(ServiceAndCompositeComponentRequest request,string exceptionMessage)
        {
            var sapOwnerEmail = ConfigurationManager.AppSettings["SAP_OWNER_EMAIL"];
            var templateId = ConfigurationManager.AppSettings["EMAIL_TEMPLATE_ID"];
            const string subject = "SAP Sync failure";

            var placeHolders = new Dictionary<string, string>
            {
                {"action", request.Update ? "update" : "create"},
                {"component", request.ComponentType},
                {"code", request.Code},
                {"failureMessage", exceptionMessage}
            };
            await _notifier.SendEmailAsync(new[] { sapOwnerEmail },
                subject, templateId, placeHolders);
        }
    }
}