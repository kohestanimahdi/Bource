﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TseClient
{


    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://tsetmc.com/", ConfigurationName = "TseClient.WebServiceTseClientSoap")]
    public interface WebServiceTseClientSoap
    {

        // CODEGEN: Generating message contract since element name LastPossibleDevenResult from namespace http://tsetmc.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/LastPossibleDeven", ReplyAction = "*")]
        TseClient.LastPossibleDevenResponse LastPossibleDeven(TseClient.LastPossibleDevenRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/LastPossibleDeven", ReplyAction = "*")]
        System.Threading.Tasks.Task<TseClient.LastPossibleDevenResponse> LastPossibleDevenAsync(TseClient.LastPossibleDevenRequest request);

        // CODEGEN: Generating message contract since element name InstrumentResult from namespace http://tsetmc.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/Instrument", ReplyAction = "*")]
        TseClient.InstrumentResponse Instrument(TseClient.InstrumentRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/Instrument", ReplyAction = "*")]
        System.Threading.Tasks.Task<TseClient.InstrumentResponse> InstrumentAsync(TseClient.InstrumentRequest request);

        // CODEGEN: Generating message contract since element name insCodes from namespace http://tsetmc.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/DecompressAndGetInsturmentClosingPrice", ReplyAction = "*")]
        TseClient.DecompressAndGetInsturmentClosingPriceResponse DecompressAndGetInsturmentClosingPrice(TseClient.DecompressAndGetInsturmentClosingPriceRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/DecompressAndGetInsturmentClosingPrice", ReplyAction = "*")]
        System.Threading.Tasks.Task<TseClient.DecompressAndGetInsturmentClosingPriceResponse> DecompressAndGetInsturmentClosingPriceAsync(TseClient.DecompressAndGetInsturmentClosingPriceRequest request);

        // CODEGEN: Generating message contract since element name errorMessage from namespace http://tsetmc.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/LogError", ReplyAction = "*")]
        TseClient.LogErrorResponse LogError(TseClient.LogErrorRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/LogError", ReplyAction = "*")]
        System.Threading.Tasks.Task<TseClient.LogErrorResponse> LogErrorAsync(TseClient.LogErrorRequest request);

        // CODEGEN: Generating message contract since element name InstrumentAndShareResult from namespace http://tsetmc.com/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/InstrumentAndShare", ReplyAction = "*")]
        TseClient.InstrumentAndShareResponse InstrumentAndShare(TseClient.InstrumentAndShareRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tsetmc.com/InstrumentAndShare", ReplyAction = "*")]
        System.Threading.Tasks.Task<TseClient.InstrumentAndShareResponse> InstrumentAndShareAsync(TseClient.InstrumentAndShareRequest request);
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class LastPossibleDevenRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "LastPossibleDeven", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.LastPossibleDevenRequestBody Body;

        public LastPossibleDevenRequest()
        {
        }

        public LastPossibleDevenRequest(TseClient.LastPossibleDevenRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class LastPossibleDevenRequestBody
    {

        public LastPossibleDevenRequestBody()
        {
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class LastPossibleDevenResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "LastPossibleDevenResponse", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.LastPossibleDevenResponseBody Body;

        public LastPossibleDevenResponse()
        {
        }

        public LastPossibleDevenResponse(TseClient.LastPossibleDevenResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class LastPossibleDevenResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string LastPossibleDevenResult;

        public LastPossibleDevenResponseBody()
        {
        }

        public LastPossibleDevenResponseBody(string LastPossibleDevenResult)
        {
            this.LastPossibleDevenResult = LastPossibleDevenResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class InstrumentRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "Instrument", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.InstrumentRequestBody Body;

        public InstrumentRequest()
        {
        }

        public InstrumentRequest(TseClient.InstrumentRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class InstrumentRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
        public int DEven;

        public InstrumentRequestBody()
        {
        }

        public InstrumentRequestBody(int DEven)
        {
            this.DEven = DEven;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class InstrumentResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstrumentResponse", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.InstrumentResponseBody Body;

        public InstrumentResponse()
        {
        }

        public InstrumentResponse(TseClient.InstrumentResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class InstrumentResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string InstrumentResult;

        public InstrumentResponseBody()
        {
        }

        public InstrumentResponseBody(string InstrumentResult)
        {
            this.InstrumentResult = InstrumentResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class DecompressAndGetInsturmentClosingPriceRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "DecompressAndGetInsturmentClosingPrice", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.DecompressAndGetInsturmentClosingPriceRequestBody Body;

        public DecompressAndGetInsturmentClosingPriceRequest()
        {
        }

        public DecompressAndGetInsturmentClosingPriceRequest(TseClient.DecompressAndGetInsturmentClosingPriceRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class DecompressAndGetInsturmentClosingPriceRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string insCodes;

        public DecompressAndGetInsturmentClosingPriceRequestBody()
        {
        }

        public DecompressAndGetInsturmentClosingPriceRequestBody(string insCodes)
        {
            this.insCodes = insCodes;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class DecompressAndGetInsturmentClosingPriceResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "DecompressAndGetInsturmentClosingPriceResponse", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.DecompressAndGetInsturmentClosingPriceResponseBody Body;

        public DecompressAndGetInsturmentClosingPriceResponse()
        {
        }

        public DecompressAndGetInsturmentClosingPriceResponse(TseClient.DecompressAndGetInsturmentClosingPriceResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class DecompressAndGetInsturmentClosingPriceResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string DecompressAndGetInsturmentClosingPriceResult;

        public DecompressAndGetInsturmentClosingPriceResponseBody()
        {
        }

        public DecompressAndGetInsturmentClosingPriceResponseBody(string DecompressAndGetInsturmentClosingPriceResult)
        {
            this.DecompressAndGetInsturmentClosingPriceResult = DecompressAndGetInsturmentClosingPriceResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class LogErrorRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "LogError", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.LogErrorRequestBody Body;

        public LogErrorRequest()
        {
        }

        public LogErrorRequest(TseClient.LogErrorRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class LogErrorRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string errorMessage;

        public LogErrorRequestBody()
        {
        }

        public LogErrorRequestBody(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class LogErrorResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "LogErrorResponse", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.LogErrorResponseBody Body;

        public LogErrorResponse()
        {
        }

        public LogErrorResponse(TseClient.LogErrorResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class LogErrorResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string LogErrorResult;

        public LogErrorResponseBody()
        {
        }

        public LogErrorResponseBody(string LogErrorResult)
        {
            this.LogErrorResult = LogErrorResult;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class InstrumentAndShareRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstrumentAndShare", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.InstrumentAndShareRequestBody Body;

        public InstrumentAndShareRequest()
        {
        }

        public InstrumentAndShareRequest(TseClient.InstrumentAndShareRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class InstrumentAndShareRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(Order = 0)]
        public int DEven;

        [System.Runtime.Serialization.DataMemberAttribute(Order = 1)]
        public long LastID;

        public InstrumentAndShareRequestBody()
        {
        }

        public InstrumentAndShareRequestBody(int DEven, long LastID)
        {
            this.DEven = DEven;
            this.LastID = LastID;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class InstrumentAndShareResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "InstrumentAndShareResponse", Namespace = "http://tsetmc.com/", Order = 0)]
        public TseClient.InstrumentAndShareResponseBody Body;

        public InstrumentAndShareResponse()
        {
        }

        public InstrumentAndShareResponse(TseClient.InstrumentAndShareResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://tsetmc.com/")]
    public partial class InstrumentAndShareResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string InstrumentAndShareResult;

        public InstrumentAndShareResponseBody()
        {
        }

        public InstrumentAndShareResponseBody(string InstrumentAndShareResult)
        {
            this.InstrumentAndShareResult = InstrumentAndShareResult;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface WebServiceTseClientSoapChannel : TseClient.WebServiceTseClientSoap, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class WebServiceTseClientSoapClient : System.ServiceModel.ClientBase<TseClient.WebServiceTseClientSoap>, TseClient.WebServiceTseClientSoap
    {

        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);

        public WebServiceTseClientSoapClient(EndpointConfiguration endpointConfiguration) :
                base(WebServiceTseClientSoapClient.GetBindingForEndpoint(endpointConfiguration), WebServiceTseClientSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public WebServiceTseClientSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
                base(WebServiceTseClientSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public WebServiceTseClientSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) :
                base(WebServiceTseClientSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public WebServiceTseClientSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TseClient.LastPossibleDevenResponse TseClient.WebServiceTseClientSoap.LastPossibleDeven(TseClient.LastPossibleDevenRequest request)
        {
            return base.Channel.LastPossibleDeven(request);
        }

        public string LastPossibleDeven()
        {
            TseClient.LastPossibleDevenRequest inValue = new TseClient.LastPossibleDevenRequest();
            inValue.Body = new TseClient.LastPossibleDevenRequestBody();
            TseClient.LastPossibleDevenResponse retVal = ((TseClient.WebServiceTseClientSoap)(this)).LastPossibleDeven(inValue);
            return retVal.Body.LastPossibleDevenResult;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TseClient.LastPossibleDevenResponse> TseClient.WebServiceTseClientSoap.LastPossibleDevenAsync(TseClient.LastPossibleDevenRequest request)
        {
            return base.Channel.LastPossibleDevenAsync(request);
        }

        public System.Threading.Tasks.Task<TseClient.LastPossibleDevenResponse> LastPossibleDevenAsync()
        {
            TseClient.LastPossibleDevenRequest inValue = new TseClient.LastPossibleDevenRequest();
            inValue.Body = new TseClient.LastPossibleDevenRequestBody();
            return ((TseClient.WebServiceTseClientSoap)(this)).LastPossibleDevenAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TseClient.InstrumentResponse TseClient.WebServiceTseClientSoap.Instrument(TseClient.InstrumentRequest request)
        {
            return base.Channel.Instrument(request);
        }

        public string Instrument(int DEven)
        {
            TseClient.InstrumentRequest inValue = new TseClient.InstrumentRequest();
            inValue.Body = new TseClient.InstrumentRequestBody();
            inValue.Body.DEven = DEven;
            TseClient.InstrumentResponse retVal = ((TseClient.WebServiceTseClientSoap)(this)).Instrument(inValue);
            return retVal.Body.InstrumentResult;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TseClient.InstrumentResponse> TseClient.WebServiceTseClientSoap.InstrumentAsync(TseClient.InstrumentRequest request)
        {
            return base.Channel.InstrumentAsync(request);
        }

        public System.Threading.Tasks.Task<TseClient.InstrumentResponse> InstrumentAsync(int DEven)
        {
            TseClient.InstrumentRequest inValue = new TseClient.InstrumentRequest();
            inValue.Body = new TseClient.InstrumentRequestBody();
            inValue.Body.DEven = DEven;
            return ((TseClient.WebServiceTseClientSoap)(this)).InstrumentAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TseClient.DecompressAndGetInsturmentClosingPriceResponse TseClient.WebServiceTseClientSoap.DecompressAndGetInsturmentClosingPrice(TseClient.DecompressAndGetInsturmentClosingPriceRequest request)
        {
            return base.Channel.DecompressAndGetInsturmentClosingPrice(request);
        }

        public string DecompressAndGetInsturmentClosingPrice(string insCodes)
        {
            TseClient.DecompressAndGetInsturmentClosingPriceRequest inValue = new TseClient.DecompressAndGetInsturmentClosingPriceRequest();
            inValue.Body = new TseClient.DecompressAndGetInsturmentClosingPriceRequestBody();
            inValue.Body.insCodes = insCodes;
            TseClient.DecompressAndGetInsturmentClosingPriceResponse retVal = ((TseClient.WebServiceTseClientSoap)(this)).DecompressAndGetInsturmentClosingPrice(inValue);
            return retVal.Body.DecompressAndGetInsturmentClosingPriceResult;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TseClient.DecompressAndGetInsturmentClosingPriceResponse> TseClient.WebServiceTseClientSoap.DecompressAndGetInsturmentClosingPriceAsync(TseClient.DecompressAndGetInsturmentClosingPriceRequest request)
        {
            return base.Channel.DecompressAndGetInsturmentClosingPriceAsync(request);
        }

        public System.Threading.Tasks.Task<TseClient.DecompressAndGetInsturmentClosingPriceResponse> DecompressAndGetInsturmentClosingPriceAsync(string insCodes)
        {
            TseClient.DecompressAndGetInsturmentClosingPriceRequest inValue = new TseClient.DecompressAndGetInsturmentClosingPriceRequest();
            inValue.Body = new TseClient.DecompressAndGetInsturmentClosingPriceRequestBody();
            inValue.Body.insCodes = insCodes;
            return ((TseClient.WebServiceTseClientSoap)(this)).DecompressAndGetInsturmentClosingPriceAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TseClient.LogErrorResponse TseClient.WebServiceTseClientSoap.LogError(TseClient.LogErrorRequest request)
        {
            return base.Channel.LogError(request);
        }

        public string LogError(string errorMessage)
        {
            TseClient.LogErrorRequest inValue = new TseClient.LogErrorRequest();
            inValue.Body = new TseClient.LogErrorRequestBody();
            inValue.Body.errorMessage = errorMessage;
            TseClient.LogErrorResponse retVal = ((TseClient.WebServiceTseClientSoap)(this)).LogError(inValue);
            return retVal.Body.LogErrorResult;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TseClient.LogErrorResponse> TseClient.WebServiceTseClientSoap.LogErrorAsync(TseClient.LogErrorRequest request)
        {
            return base.Channel.LogErrorAsync(request);
        }

        public System.Threading.Tasks.Task<TseClient.LogErrorResponse> LogErrorAsync(string errorMessage)
        {
            TseClient.LogErrorRequest inValue = new TseClient.LogErrorRequest();
            inValue.Body = new TseClient.LogErrorRequestBody();
            inValue.Body.errorMessage = errorMessage;
            return ((TseClient.WebServiceTseClientSoap)(this)).LogErrorAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TseClient.InstrumentAndShareResponse TseClient.WebServiceTseClientSoap.InstrumentAndShare(TseClient.InstrumentAndShareRequest request)
        {
            return base.Channel.InstrumentAndShare(request);
        }

        public string InstrumentAndShare(int DEven, long LastID)
        {
            TseClient.InstrumentAndShareRequest inValue = new TseClient.InstrumentAndShareRequest();
            inValue.Body = new TseClient.InstrumentAndShareRequestBody();
            inValue.Body.DEven = DEven;
            inValue.Body.LastID = LastID;
            TseClient.InstrumentAndShareResponse retVal = ((TseClient.WebServiceTseClientSoap)(this)).InstrumentAndShare(inValue);
            return retVal.Body.InstrumentAndShareResult;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TseClient.InstrumentAndShareResponse> TseClient.WebServiceTseClientSoap.InstrumentAndShareAsync(TseClient.InstrumentAndShareRequest request)
        {
            return base.Channel.InstrumentAndShareAsync(request);
        }

        public System.Threading.Tasks.Task<TseClient.InstrumentAndShareResponse> InstrumentAndShareAsync(int DEven, long LastID)
        {
            TseClient.InstrumentAndShareRequest inValue = new TseClient.InstrumentAndShareRequest();
            inValue.Body = new TseClient.InstrumentAndShareRequestBody();
            inValue.Body.DEven = DEven;
            inValue.Body.LastID = LastID;
            return ((TseClient.WebServiceTseClientSoap)(this)).InstrumentAndShareAsync(inValue);
        }

        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }

        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }

        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WebServiceTseClientSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.WebServiceTseClientSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WebServiceTseClientSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://service.tsetmc.com/WebService/TseClient.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.WebServiceTseClientSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://service.tsetmc.com/WebService/TseClient.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        public enum EndpointConfiguration
        {

            WebServiceTseClientSoap,

            WebServiceTseClientSoap12,
        }
    }
}
