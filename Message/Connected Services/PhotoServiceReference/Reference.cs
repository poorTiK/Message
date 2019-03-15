﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Message.PhotoServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PhotoServiceReference.IPhotoService")]
    public interface IPhotoService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPhotoService/GetPhotoById", ReplyAction="http://tempuri.org/IPhotoService/GetPhotoByIdResponse")]
        byte[] GetPhotoById(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPhotoService/GetPhotoById", ReplyAction="http://tempuri.org/IPhotoService/GetPhotoByIdResponse")]
        System.Threading.Tasks.Task<byte[]> GetPhotoByIdAsync(int id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPhotoService/SetPhotoById", ReplyAction="http://tempuri.org/IPhotoService/SetPhotoByIdResponse")]
        void SetPhotoById(int id, byte[] photoBytes);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPhotoService/SetPhotoById", ReplyAction="http://tempuri.org/IPhotoService/SetPhotoByIdResponse")]
        System.Threading.Tasks.Task SetPhotoByIdAsync(int id, byte[] photoBytes);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPhotoServiceChannel : Message.PhotoServiceReference.IPhotoService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PhotoServiceClient : System.ServiceModel.ClientBase<Message.PhotoServiceReference.IPhotoService>, Message.PhotoServiceReference.IPhotoService {
        
        public PhotoServiceClient() {
        }
        
        public PhotoServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PhotoServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PhotoServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PhotoServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public byte[] GetPhotoById(int id) {
            return base.Channel.GetPhotoById(id);
        }
        
        public System.Threading.Tasks.Task<byte[]> GetPhotoByIdAsync(int id) {
            return base.Channel.GetPhotoByIdAsync(id);
        }
        
        public void SetPhotoById(int id, byte[] photoBytes) {
            base.Channel.SetPhotoById(id, photoBytes);
        }
        
        public System.Threading.Tasks.Task SetPhotoByIdAsync(int id, byte[] photoBytes) {
            return base.Channel.SetPhotoByIdAsync(id, photoBytes);
        }
    }
}
