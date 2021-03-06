﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace TE.ComponentLibrary.ComponentLibrary.SAPServiceReference {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="MaterialMaster_OutBinding", Namespace="http://total-environment/PI/MaterialMaster")]
    public partial class MaterialMaster_OutService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback MaterialMaster_OutOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public MaterialMaster_OutService() {
            this.Url = global::TE.ComponentLibrary.ComponentLibrary.Properties.Settings.Default.ComponentLibrary_SAPServiceReference_MaterialMaster_OutService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event MaterialMaster_OutCompletedEventHandler MaterialMaster_OutCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sap.com/xi/WebService/soap1.1", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("MaterialMasterRes", Namespace="http://total-environment/PI/MaterialMaster")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public MaterialMasterResItem[] MaterialMaster_Out([System.Xml.Serialization.XmlElementAttribute(Namespace="http://total-environment/PI/MaterialMaster")] MaterialMasterReq MaterialMasterReq) {
            object[] results = this.Invoke("MaterialMaster_Out", new object[] {
                        MaterialMasterReq});
            return ((MaterialMasterResItem[])(results[0]));
        }
        
        /// <remarks/>
        public void MaterialMaster_OutAsync(MaterialMasterReq MaterialMasterReq) {
            this.MaterialMaster_OutAsync(MaterialMasterReq, null);
        }
        
        /// <remarks/>
        public void MaterialMaster_OutAsync(MaterialMasterReq MaterialMasterReq, object userState) {
            if ((this.MaterialMaster_OutOperationCompleted == null)) {
                this.MaterialMaster_OutOperationCompleted = new System.Threading.SendOrPostCallback(this.OnMaterialMaster_OutOperationCompleted);
            }
            this.InvokeAsync("MaterialMaster_Out", new object[] {
                        MaterialMasterReq}, this.MaterialMaster_OutOperationCompleted, userState);
        }
        
        private void OnMaterialMaster_OutOperationCompleted(object arg) {
            if ((this.MaterialMaster_OutCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.MaterialMaster_OutCompleted(this, new MaterialMaster_OutCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2046.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://total-environment/PI/MaterialMaster")]
    public partial class MaterialMasterReq {
        
        private MaterialMasterReqItem[] itemField;
        
        private MaterialMasterReqItem1[] item1Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MaterialMasterReqItem[] item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("item1", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MaterialMasterReqItem1[] item1 {
            get {
                return this.item1Field;
            }
            set {
                this.item1Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2046.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://total-environment/PI/MaterialMaster")]
    public partial class MaterialMasterReqItem {
        
        private string mATNRField;
        
        private string f_MATNRField;
        
        private string mBRSHField;
        
        private string mTARTField;
        
        private string wERKSField;
        
        private string eXTN_WERKSField;
        
        private string lGORTField;
        
        private string mAKTXField;
        
        private string mEINSField;
        
        private string mATKLField;
        
        private string eKGRPField;
        
        private string eXTENSIONField;
        
        private string bKLASField;
        
        private string vPRSVField;
        
        private string sTPRSField;
        
        private string pEINHField;
        
        private string wAERSField;
        
        private string vERPRField;
        
        private string lVORMField;
        
        private string fUGUE_IDField;
        
        private string sTUECField;
        
        private string tAXIMField;
        
        private string tAXKMField;
        
        private string mEINS1Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MATNR {
            get {
                return this.mATNRField;
            }
            set {
                this.mATNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string F_MATNR {
            get {
                return this.f_MATNRField;
            }
            set {
                this.f_MATNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MBRSH {
            get {
                return this.mBRSHField;
            }
            set {
                this.mBRSHField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MTART {
            get {
                return this.mTARTField;
            }
            set {
                this.mTARTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string WERKS {
            get {
                return this.wERKSField;
            }
            set {
                this.wERKSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EXTN_WERKS {
            get {
                return this.eXTN_WERKSField;
            }
            set {
                this.eXTN_WERKSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LGORT {
            get {
                return this.lGORTField;
            }
            set {
                this.lGORTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MAKTX {
            get {
                return this.mAKTXField;
            }
            set {
                this.mAKTXField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MEINS {
            get {
                return this.mEINSField;
            }
            set {
                this.mEINSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MATKL {
            get {
                return this.mATKLField;
            }
            set {
                this.mATKLField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EKGRP {
            get {
                return this.eKGRPField;
            }
            set {
                this.eKGRPField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EXTENSION {
            get {
                return this.eXTENSIONField;
            }
            set {
                this.eXTENSIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BKLAS {
            get {
                return this.bKLASField;
            }
            set {
                this.bKLASField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VPRSV {
            get {
                return this.vPRSVField;
            }
            set {
                this.vPRSVField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STPRS {
            get {
                return this.sTPRSField;
            }
            set {
                this.sTPRSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PEINH {
            get {
                return this.pEINHField;
            }
            set {
                this.pEINHField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string WAERS {
            get {
                return this.wAERSField;
            }
            set {
                this.wAERSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VERPR {
            get {
                return this.vERPRField;
            }
            set {
                this.vERPRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LVORM {
            get {
                return this.lVORMField;
            }
            set {
                this.lVORMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FUGUE_ID {
            get {
                return this.fUGUE_IDField;
            }
            set {
                this.fUGUE_IDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STUEC {
            get {
                return this.sTUECField;
            }
            set {
                this.sTUECField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TAXIM {
            get {
                return this.tAXIMField;
            }
            set {
                this.tAXIMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TAXKM {
            get {
                return this.tAXKMField;
            }
            set {
                this.tAXKMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MEINS1 {
            get {
                return this.mEINS1Field;
            }
            set {
                this.mEINS1Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2046.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://total-environment/PI/MaterialMaster")]
    public partial class MaterialMasterReqItem1 {
        
        private string tDLINEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TDLINE {
            get {
                return this.tDLINEField;
            }
            set {
                this.tDLINEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2046.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://total-environment/PI/MaterialMaster")]
    public partial class MaterialMasterResItem {
        
        private string mATNRField;
        
        private string wERKSField;
        
        private string rETCODEField;
        
        private string mESSAGEField;
        
        private string fUGUE_IDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MATNR {
            get {
                return this.mATNRField;
            }
            set {
                this.mATNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string WERKS {
            get {
                return this.wERKSField;
            }
            set {
                this.wERKSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RETCODE {
            get {
                return this.rETCODEField;
            }
            set {
                this.rETCODEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MESSAGE {
            get {
                return this.mESSAGEField;
            }
            set {
                this.mESSAGEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FUGUE_ID {
            get {
                return this.fUGUE_IDField;
            }
            set {
                this.fUGUE_IDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    public delegate void MaterialMaster_OutCompletedEventHandler(object sender, MaterialMaster_OutCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2046.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class MaterialMaster_OutCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal MaterialMaster_OutCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public MaterialMasterResItem[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((MaterialMasterResItem[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591