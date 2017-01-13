using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using SPMeta2.Syntax.Default;
using SPMeta2.CSOM.Services;
using System.Security;

namespace SPMetaProvisioning
{
    class Program
    {
        static void Main(string[] args)
        {
           


            // define fields
            var clientDescriptionField = new FieldDefinition
            {
                Title = "Client Description",
                InternalName = "dcs_ClientDescription",
                Group = "SPMeta2.Samples",
                Id = new Guid("06975b67-01f5-47d7-9e2e-2702dfb8c217"),
                FieldType = BuiltInFieldTypes.Note,
            };

            var clientNumberField = new FieldDefinition
            {
                Title = "Client Number",
                InternalName = "dcs_ClientNumber",
                Group = "SPMeta2.Samples",
                Id = new Guid("22264486-7561-45ec-a6bc-591ba243693b"),
                FieldType = BuiltInFieldTypes.Number,
            };


            // define content type
            var customerAccountContentType = new ContentTypeDefinition
            {
                Name = "Customer Account",
                Id = new Guid("ddc46a66-19a0-460b-a723-c84d7f60a342"),
                ParentContentTypeId = BuiltInContentTypeId.Item,
                Group = "SPMeta2.Samples",
            };


            //List and Library provision
            var genericList = new ListDefinition
            {
                Title = "Generic list",
                Description = "A generic list.",
                TemplateType = BuiltInListTemplateTypeId.GenericList,
                Url = "GenericList",
                OnQuickLaunch=false
               
            };

            var documentLibrary = new ListDefinition
            {
                Title = "Document library",
                Description = "A document library.",
                TemplateType = BuiltInListTemplateTypeId.DocumentLibrary,
                Url = "DocumentLibrary"
            };

            // Create a model and define relationships between fields and content type
            var siteModel = SPMeta2Model.NewSiteModel(site =>
            {
                site
                    .AddField(clientDescriptionField)
                    .AddField(clientNumberField)
                    .AddContentType(customerAccountContentType, contentType =>
                    {
                        contentType
                            .AddContentTypeFieldLink(clientDescriptionField)
                            .AddContentTypeFieldLink(clientNumberField);
                    });
            });

            var webModel = SPMeta2Model.NewWebModel(web =>
                {
                    web.AddList(genericList);
                    web.AddList(documentLibrary);
                });

            var clientContext = GetContext();
                    

            // deploy the model to the SharePoint site over CSOM
            var csomProvisionService = new CSOMProvisionService();
            csomProvisionService.DeploySiteModel(clientContext, siteModel);
            csomProvisionService.DeployWebModel(clientContext, webModel);

         


        }

        static ClientContext GetContext()
        {
            var securePassword = new SecureString();
            foreach (char c in Configuration.ServicePassword)
            {
                securePassword.AppendChar(c);
            }

            var onlineCredentials = new SharePointOnlineCredentials(Configuration.ServiceUserName, securePassword);

            var context = new ClientContext(Configuration.ServiceSiteUrl);
            context.Credentials = onlineCredentials;

            return context;
        }

        private class Configuration
        {
            public static string ServiceSiteUrl = "your site url";
            public static string ServiceUserName = "username";
            public static string ServicePassword = "password";
        }

    }
}
