﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Message.ContactsServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="User", Namespace="http://schemas.datacontract.org/2004/07/ServerWCF.Model")]
    [System.SerializableAttribute()]
    public partial class User : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Message.ContactsServiceReference.ApplicationSettings ApplicationSettingsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] AvatarField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BioField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Message.ContactsServiceReference.Contact[] ContactsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EmailField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FirstNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime LastOnlineField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LoginField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Message.ContactsServiceReference.MessageT[] MessagesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Message.ContactsServiceReference.Contact[] OwnersField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PasswordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PhoneField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Message.ContactsServiceReference.ApplicationSettings ApplicationSettings {
            get {
                return this.ApplicationSettingsField;
            }
            set {
                if ((object.ReferenceEquals(this.ApplicationSettingsField, value) != true)) {
                    this.ApplicationSettingsField = value;
                    this.RaisePropertyChanged("ApplicationSettings");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Avatar {
            get {
                return this.AvatarField;
            }
            set {
                if ((object.ReferenceEquals(this.AvatarField, value) != true)) {
                    this.AvatarField = value;
                    this.RaisePropertyChanged("Avatar");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Bio {
            get {
                return this.BioField;
            }
            set {
                if ((object.ReferenceEquals(this.BioField, value) != true)) {
                    this.BioField = value;
                    this.RaisePropertyChanged("Bio");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Message.ContactsServiceReference.Contact[] Contacts {
            get {
                return this.ContactsField;
            }
            set {
                if ((object.ReferenceEquals(this.ContactsField, value) != true)) {
                    this.ContactsField = value;
                    this.RaisePropertyChanged("Contacts");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Email {
            get {
                return this.EmailField;
            }
            set {
                if ((object.ReferenceEquals(this.EmailField, value) != true)) {
                    this.EmailField = value;
                    this.RaisePropertyChanged("Email");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName {
            get {
                return this.FirstNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FirstNameField, value) != true)) {
                    this.FirstNameField = value;
                    this.RaisePropertyChanged("FirstName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName {
            get {
                return this.LastNameField;
            }
            set {
                if ((object.ReferenceEquals(this.LastNameField, value) != true)) {
                    this.LastNameField = value;
                    this.RaisePropertyChanged("LastName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastOnline {
            get {
                return this.LastOnlineField;
            }
            set {
                if ((this.LastOnlineField.Equals(value) != true)) {
                    this.LastOnlineField = value;
                    this.RaisePropertyChanged("LastOnline");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Login {
            get {
                return this.LoginField;
            }
            set {
                if ((object.ReferenceEquals(this.LoginField, value) != true)) {
                    this.LoginField = value;
                    this.RaisePropertyChanged("Login");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Message.ContactsServiceReference.MessageT[] Messages {
            get {
                return this.MessagesField;
            }
            set {
                if ((object.ReferenceEquals(this.MessagesField, value) != true)) {
                    this.MessagesField = value;
                    this.RaisePropertyChanged("Messages");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Message.ContactsServiceReference.Contact[] Owners {
            get {
                return this.OwnersField;
            }
            set {
                if ((object.ReferenceEquals(this.OwnersField, value) != true)) {
                    this.OwnersField = value;
                    this.RaisePropertyChanged("Owners");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Phone {
            get {
                return this.PhoneField;
            }
            set {
                if ((object.ReferenceEquals(this.PhoneField, value) != true)) {
                    this.PhoneField = value;
                    this.RaisePropertyChanged("Phone");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ApplicationSettings", Namespace="http://schemas.datacontract.org/2004/07/ServerWCF.Model")]
    [System.SerializableAttribute()]
    public partial class ApplicationSettings : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Contact", Namespace="http://schemas.datacontract.org/2004/07/ServerWCF.Model")]
    [System.SerializableAttribute()]
    public partial class Contact : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Message.ContactsServiceReference.User UserOwnedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Message.ContactsServiceReference.User UserOwnerField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Message.ContactsServiceReference.User UserOwned {
            get {
                return this.UserOwnedField;
            }
            set {
                if ((object.ReferenceEquals(this.UserOwnedField, value) != true)) {
                    this.UserOwnedField = value;
                    this.RaisePropertyChanged("UserOwned");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Message.ContactsServiceReference.User UserOwner {
            get {
                return this.UserOwnerField;
            }
            set {
                if ((object.ReferenceEquals(this.UserOwnerField, value) != true)) {
                    this.UserOwnerField = value;
                    this.RaisePropertyChanged("UserOwner");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MessageT", Namespace="http://schemas.datacontract.org/2004/07/ServerWCF.Model")]
    [System.SerializableAttribute()]
    public partial class MessageT : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] ContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime DateOfSendingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int SenderIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TypeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Content {
            get {
                return this.ContentField;
            }
            set {
                if ((object.ReferenceEquals(this.ContentField, value) != true)) {
                    this.ContentField = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DateOfSending {
            get {
                return this.DateOfSendingField;
            }
            set {
                if ((this.DateOfSendingField.Equals(value) != true)) {
                    this.DateOfSendingField = value;
                    this.RaisePropertyChanged("DateOfSending");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SenderId {
            get {
                return this.SenderIdField;
            }
            set {
                if ((this.SenderIdField.Equals(value) != true)) {
                    this.SenderIdField = value;
                    this.RaisePropertyChanged("SenderId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Type {
            get {
                return this.TypeField;
            }
            set {
                if ((this.TypeField.Equals(value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ContactsServiceReference.IContactsService")]
    public interface IContactsService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IContactsService/GetContacts", ReplyAction="http://tempuri.org/IContactsService/GetContactsResponse")]
        Message.ContactsServiceReference.Contact[] GetContacts(Message.ContactsServiceReference.User user);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IContactsService/GetContacts", ReplyAction="http://tempuri.org/IContactsService/GetContactsResponse")]
        System.Threading.Tasks.Task<Message.ContactsServiceReference.Contact[]> GetContactsAsync(Message.ContactsServiceReference.User user);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IContactsServiceChannel : Message.ContactsServiceReference.IContactsService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ContactsServiceClient : System.ServiceModel.ClientBase<Message.ContactsServiceReference.IContactsService>, Message.ContactsServiceReference.IContactsService {
        
        public ContactsServiceClient() {
        }
        
        public ContactsServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ContactsServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ContactsServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ContactsServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Message.ContactsServiceReference.Contact[] GetContacts(Message.ContactsServiceReference.User user) {
            return base.Channel.GetContacts(user);
        }
        
        public System.Threading.Tasks.Task<Message.ContactsServiceReference.Contact[]> GetContactsAsync(Message.ContactsServiceReference.User user) {
            return base.Channel.GetContactsAsync(user);
        }
    }
}
