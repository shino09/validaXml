﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Microsoft.VSDesigner generó automáticamente este código fuente, versión=4.0.30319.42000.
// 
#pragma warning disable 1591

namespace Negocio.WS_Notificaciones_Web_Cliente {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="WS_Notificaciones_Web_ClienteSoap", Namespace="http://tempuri.org/")]
    public partial class WS_Notificaciones_Web_Cliente : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback Envio_Resumen_CargaOperationCompleted;
        
        private System.Threading.SendOrPostCallback Error_ValidacionOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public WS_Notificaciones_Web_Cliente() {
            this.Url = global::Negocio.Properties.Settings.Default.Negocio_WS_Notificaciones_Web_Cliente_WS_Notificaciones_Web_Cliente;
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
        public event Envio_Resumen_CargaCompletedEventHandler Envio_Resumen_CargaCompleted;
        
        /// <remarks/>
        public event Error_ValidacionCompletedEventHandler Error_ValidacionCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Envio_Resumen_Carga", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public MENSAJERES_Type Envio_Resumen_Carga(CABECERA_Type cabecera, DATOS_Type datosWC) {
            object[] results = this.Invoke("Envio_Resumen_Carga", new object[] {
                        cabecera,
                        datosWC});
            return ((MENSAJERES_Type)(results[0]));
        }
        
        /// <remarks/>
        public void Envio_Resumen_CargaAsync(CABECERA_Type cabecera, DATOS_Type datosWC) {
            this.Envio_Resumen_CargaAsync(cabecera, datosWC, null);
        }
        
        /// <remarks/>
        public void Envio_Resumen_CargaAsync(CABECERA_Type cabecera, DATOS_Type datosWC, object userState) {
            if ((this.Envio_Resumen_CargaOperationCompleted == null)) {
                this.Envio_Resumen_CargaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEnvio_Resumen_CargaOperationCompleted);
            }
            this.InvokeAsync("Envio_Resumen_Carga", new object[] {
                        cabecera,
                        datosWC}, this.Envio_Resumen_CargaOperationCompleted, userState);
        }
        
        private void OnEnvio_Resumen_CargaOperationCompleted(object arg) {
            if ((this.Envio_Resumen_CargaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Envio_Resumen_CargaCompleted(this, new Envio_Resumen_CargaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Error_Validacion", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public MENSAJERES_Type Error_Validacion(CABECERA_Type cabecera, DATOS_Type datosWC) {
            object[] results = this.Invoke("Error_Validacion", new object[] {
                        cabecera,
                        datosWC});
            return ((MENSAJERES_Type)(results[0]));
        }
        
        /// <remarks/>
        public void Error_ValidacionAsync(CABECERA_Type cabecera, DATOS_Type datosWC) {
            this.Error_ValidacionAsync(cabecera, datosWC, null);
        }
        
        /// <remarks/>
        public void Error_ValidacionAsync(CABECERA_Type cabecera, DATOS_Type datosWC, object userState) {
            if ((this.Error_ValidacionOperationCompleted == null)) {
                this.Error_ValidacionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnError_ValidacionOperationCompleted);
            }
            this.InvokeAsync("Error_Validacion", new object[] {
                        cabecera,
                        datosWC}, this.Error_ValidacionOperationCompleted, userState);
        }
        
        private void OnError_ValidacionOperationCompleted(object arg) {
            if ((this.Error_ValidacionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Error_ValidacionCompleted(this, new Error_ValidacionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cavali.com.pe/ib/esb/")]
    public partial class CABECERA_Type {
        
        private string aPP_CONSUMIDORAField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string APP_CONSUMIDORA {
            get {
                return this.aPP_CONSUMIDORAField;
            }
            set {
                this.aPP_CONSUMIDORAField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cavali.com.pe/ib/esb/srv04001")]
    public partial class DATOSRes_Type {
        
        private string resultCodeField;
        
        private string messageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
        public string resultCode {
            get {
                return this.resultCodeField;
            }
            set {
                this.resultCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cavali.com.pe/ib/esb/srv04001")]
    public partial class DETALLERes_Type {
        
        private DATOSRes_Type dATOSField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DATOSRes_Type DATOS {
            get {
                return this.dATOSField;
            }
            set {
                this.dATOSField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cavali.com.pe/ib/esb")]
    public partial class CABECERARes_Type {
        
        private string aPP_CONSUMIDORAField;
        
        private string cOD_RESPUESTAField;
        
        private string dES_RESPUESTAField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string APP_CONSUMIDORA {
            get {
                return this.aPP_CONSUMIDORAField;
            }
            set {
                this.aPP_CONSUMIDORAField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COD_RESPUESTA {
            get {
                return this.cOD_RESPUESTAField;
            }
            set {
                this.cOD_RESPUESTAField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DES_RESPUESTA {
            get {
                return this.dES_RESPUESTAField;
            }
            set {
                this.dES_RESPUESTAField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cavali.com.pe/ib/esb/srv04001")]
    public partial class INTEGRACIONRES_Type {
        
        private CABECERARes_Type cABECERAField;
        
        private DETALLERes_Type dETALLEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CABECERARes_Type CABECERA {
            get {
                return this.cABECERAField;
            }
            set {
                this.cABECERAField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DETALLERes_Type DETALLE {
            get {
                return this.dETALLEField;
            }
            set {
                this.dETALLEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cavali.com.pe/ib/esb/srv04001")]
    public partial class MENSAJERES_Type {
        
        private INTEGRACIONRES_Type iNTEGRESField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public INTEGRACIONRES_Type INTEGRES {
            get {
                return this.iNTEGRESField;
            }
            set {
                this.iNTEGRESField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3752.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://cavali.com.pe/ib/esb/srv04002")]
    public partial class DATOS_Type {
        
        private string idPadreField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string idPadre {
            get {
                return this.idPadreField;
            }
            set {
                this.idPadreField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Envio_Resumen_CargaCompletedEventHandler(object sender, Envio_Resumen_CargaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Envio_Resumen_CargaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Envio_Resumen_CargaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public MENSAJERES_Type Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((MENSAJERES_Type)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Error_ValidacionCompletedEventHandler(object sender, Error_ValidacionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Error_ValidacionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Error_ValidacionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public MENSAJERES_Type Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((MENSAJERES_Type)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591