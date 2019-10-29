using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using NotificationEngine;
using TE.ComponentLibrary.ComponentLibrary.AppService;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.Helpers;
using TE.ComponentLibrary.ComponentLibrary.SAPServiceReference;

namespace TE.ComponentLibrary.ComponentLibrary.ControllerPort
{
    /// <summary>
    /// Represents the SAP Syncer
    /// </summary>
    /// <seealso cref="TE.ComponentLibrary.ComponentLibrary.ControllerPort.ISapSyncer" />
    public class SapSyncer : ISapSyncer
    {
        private readonly MaterialMaster_OutService _materialMasterOut;
        private readonly INotifier _notifier;

        /// <summary>
        /// </summary>
        /// <param name="materialMasterOut"></param>
        /// <param name="notifier"></param>
        public SapSyncer(MaterialMaster_OutService materialMasterOut, INotifier notifier)
        {
            _materialMasterOut = materialMasterOut;
            _notifier = notifier;
            _materialMasterOut.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SAPUserName"], ConfigurationManager.AppSettings["SAPPassword"]);
        }

        
        /// <summary>
        /// </summary>
        /// <param name="material"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> Sync(IMaterial material, string action)
        {
            return await SyncActual(material, action);
//            Task.Run(async () => await SyncActual(material, action));
//            return Task.FromResult(true);
        }
        private async Task<bool> SyncActual(IMaterial material, string action)
        {
            Trace.TraceInformation("material of " + material.Id + " is being synced to SAP.");

            var hsnCode = Convert.ToString(
                material["General"]?.Columns
                    .FirstOrDefault(c => c.Name.ToLower() == "hsn code")?.Value);

            var shortDescription = Convert.ToString(
                material["General"]?.Columns
                    .FirstOrDefault(c => c.Name.ToLower() == "short description")?.Value);
            var unitOfMeasure = Convert.ToString(
                material["General"]?.Columns
                    .FirstOrDefault(c => c.Name.ToLower() == "unit of measure")?.Value).ToUpper();
            var gstAppProcurement = Convert.ToString(
                material["Purchase"]?.Columns
                    .FirstOrDefault(c => string.Equals(c.Name, "GST - Procurement",
                        StringComparison.CurrentCultureIgnoreCase))?.Value);
            var gstAppSales = Convert.ToString(
                material["Purchase"]?.Columns
                .FirstOrDefault(c => string.Equals(c.Name, "GST - Sales",
                    StringComparison.CurrentCultureIgnoreCase))?.Value);

            var serviceClassification = new Dictionary<string, string>
            {
                { "Applicable", "0" },
                { "Exempted", "1" },
                { "Not applicable", "2" },
                { "Not relevant", "3" }
            };

            string gstClassForProcurement = serviceClassification.TryGetValue(gstAppProcurement, out gstClassForProcurement) ? gstClassForProcurement : "";
            string gstClassForSales = serviceClassification.TryGetValue(gstAppSales, out gstClassForSales) ? gstClassForSales : "";

            Trace.TraceInformation("material of " + material.Id + " is having HSN Code: " + hsnCode +
                                   " , Short Description: " + shortDescription + " , GST - Procurement: " + gstClassForProcurement + " , GST - Sales: " + gstClassForSales + ".");

            try
            {
                var MaterialMasterReq = new MaterialMasterReq
                {
                    item = new[]
                    {
                        new MaterialMasterReqItem
                        {
                            MATNR = material.Id,
                            F_MATNR = "",
                            MBRSH = "A",
                            MTART = "ROH",
                            WERKS = "1000",
                            EXTN_WERKS = "",
                            LGORT = "1000",
                            MAKTX = shortDescription,
                            MEINS = unitOfMeasure,
                            MATKL = "17",
                            EKGRP = "1",
                            EXTENSION = "X",
                            BKLAS = "3500",
                            VPRSV = "V",
                            STPRS = "",
                            PEINH = "1",
                            WAERS = "INR",
                            VERPR = "1",
                            LVORM = "",
                            FUGUE_ID = Guid.NewGuid().ToString(),
                            STUEC = hsnCode,
                            TAXIM = gstClassForProcurement,
                            TAXKM = "",
                        }
                    },
                    item1 = new[]
                    {
                        new MaterialMasterReqItem1
                        {
                            TDLINE = ""
                        }
                    }
                };

                Trace.TraceInformation("materialMasterOutRequest -> " +
                                       "MATNR: " + MaterialMasterReq.item[0].MATNR + ", " +
                                       "F_MATNR:" + MaterialMasterReq.item[0].F_MATNR + ", " +
                                       "MBRSH:" + MaterialMasterReq.item[0].MBRSH + ", " +
                                       "MTART:" + MaterialMasterReq.item[0].MTART + ", " +
                                       "WERKS:" + MaterialMasterReq.item[0].WERKS + ", " +
                                       "EXTN_WERKS:" + MaterialMasterReq.item[0].EXTN_WERKS +
                                       ", " +
                                       "LGORT:" + MaterialMasterReq.item[0].LGORT + ", " +
                                       "MAKTX:" + MaterialMasterReq.item[0].MAKTX + ", " +
                                       "MEINS:" + MaterialMasterReq.item[0].MEINS + ", " +
                                       "MATKL:" + MaterialMasterReq.item[0].MATKL + ", " +
                                       "EKGRP:" + MaterialMasterReq.item[0].EKGRP + ", " +
                                       "EXTENSION:" + MaterialMasterReq.item[0].EXTENSION +
                                       ", " +
                                       "BKLAS:" + MaterialMasterReq.item[0].BKLAS + ", " +
                                       "VPRSV:" + MaterialMasterReq.item[0].VPRSV + ", " +
                                       "STPRS:" + MaterialMasterReq.item[0].STPRS + ", " +
                                       "PEINH:" + MaterialMasterReq.item[0].PEINH + ", " +
                                       "WAERS:" + MaterialMasterReq.item[0].WAERS + ", " +
                                       "VERPR:" + MaterialMasterReq.item[0].VERPR + ", " +
                                       "LVORM:" + MaterialMasterReq.item[0].LVORM + ", " +
                                       "FUGUE_ID:" + MaterialMasterReq.item[0].FUGUE_ID + ", " +
                                       "TDLINE:" + MaterialMasterReq.item1[0].TDLINE + "," +
                                       "TAXIM:" + MaterialMasterReq.item[0].TAXIM + "," +
                                       "TAXKM:" + MaterialMasterReq.item[0].TAXKM + ".");

                _materialMasterOut.Timeout = 60 * 60 * 1000; // 1 hour

                var materialMasterOutResponse = _materialMasterOut.MaterialMaster_Out(MaterialMasterReq);
                if (materialMasterOutResponse.Length > 0)
                    if (materialMasterOutResponse[0].RETCODE == "0")
                    {
                        Trace.TraceInformation("material of " + material.Id +
                                               " is synced to SAP. Return values are: " +
                                               "MATNR = " + materialMasterOutResponse[0].MATNR + ", " +
                                               "WERKS = " + materialMasterOutResponse[0].WERKS + ", " +
                                               "RETCODE = " + materialMasterOutResponse[0].RETCODE +
                                               ", " +
                                               "MESSAGE = " + materialMasterOutResponse[0].MESSAGE +
                                               ", " +
                                               "FUGUE_ID = " + materialMasterOutResponse[0].FUGUE_ID +
                                               ".");
                        return true;
                    }
                    else
                    {
                        Trace.TraceError("material of " + material.Id +
                                         " is NOT synced to SAP. Return values are: " +
                                         "MATNR = " + materialMasterOutResponse[0].MATNR + " , " +
                                         "WERKS = " + materialMasterOutResponse[0].WERKS + " , " +
                                         "RETCODE = " + materialMasterOutResponse[0].RETCODE +
                                         " , " +
                                         "MESSAGE = " + materialMasterOutResponse[0].MESSAGE +
                                         " , " +
                                         "WERKS = " + materialMasterOutResponse[0].FUGUE_ID +
                                         " , ");
                        await Notify(material.Id, materialMasterOutResponse[0].MESSAGE,action);
                        return false;
                    }
                Trace.TraceError("material of " + material.Id +
                                 " is NOT synced to SAP. Length of materialMasterOutResponse.MaterialMasterRes is 0.");
                
                await Notify(material.Id, materialMasterOutResponse[0].MESSAGE,action);
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("material of " + material.Id +
                                 " is NOT synced to SAP. Exception Message: " + ex.Message + " , Exception: " +
                                 ex);
                await Notify(material.Id,ex.Message,action);
                return false;
            }
        }

        private async Task Notify(string materialId,string exceptionMessage, string action)
        {
            var sapOwnerEmail = ConfigurationManager.AppSettings["SAP_OWNER_EMAIL"];
            var templateId = ConfigurationManager.AppSettings["EMAIL_TEMPLATE_ID"];
            const string subject = "SAP Sync failure";
            var placeholders = new Dictionary<string, string>
            {
                {"action", action},
                {"component", "material"},
                {"code", materialId},
                {"failureMessage", exceptionMessage}
            };
            await _notifier.SendEmailAsync(new[] { sapOwnerEmail },
                subject, templateId, placeholders);
        }
    }
}