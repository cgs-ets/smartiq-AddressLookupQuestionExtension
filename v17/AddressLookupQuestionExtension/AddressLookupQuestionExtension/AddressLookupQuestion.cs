using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Intelledox.Extension.CustomQuestion;
using Intelledox.QAWizard;
using Intelledox.Model;
using System.Reflection;

namespace AddressLookupQuestionExtension
{
    public class AddressLookupQuestion : Intelledox.Extension.CustomQuestion.CustomQuestionExtension
    {

        // Inputs
        private Guid _apiKeyGuid = new Guid("A423239F-4BC6-472C-839A-C3CF93C619FB");
        private Guid _defaultGuid = new Guid("5EC3A14C-96A6-4BD5-86D8-8800EFAEA578");
        // Outputs
        private Guid _addressGuid = new Guid("03D695CD-1594-499F-B319-AB0DA6F751BB");
        private Guid _addressline1Guid = new Guid("821E52C2-2217-4AE0-BC4A-25EEA4666259");
        private Guid _suburbGuid = new Guid("C904FD31-EAE5-46CA-A51A-207E7954C31E");
        private Guid _stateGuid = new Guid("68A874A2-5C06-43F1-8189-A0BE8B2B0E7F");
        private Guid _countryGuid = new Guid("8F00EBB9-3550-46BA-BE0E-D1943D59D32A");
        private Guid _postcodeGuid = new Guid("3C5B3C90-9669-4744-ACA3-6F3A1A305267");
        private Guid _isSelectedGuid = new Guid("1FFD2010-A7DE-40FA-9E21-4FACE90D1114");
        private Guid _isDefaultGuid = new Guid("462791E1-0673-4F6B-8E0E-FDDF2D9E1956");
        private Guid _tempAddressGuid = new Guid("052A2E34-AB07-49FB-A831-61FA539B98E4");

        public override ExtensionIdentity ExtensionIdentity { get; protected set; }
            = new ExtensionIdentity()
            {
                //Id = new Guid("AEF2AFAE-4AE9-49EB-A029-8466CC770477"),
                Id = new Guid("3A4BD1A8-2844-40D8-B5FF-7ACC6CC8ACCB"),
                Name = "Address Autocomplete"
            };

        public override byte[] Icon16x16Png => IconHelper.GetResourceBytes("AddressLookupQuestionExtension.location16.png");

        public override byte[] Icon48x48Png => IconHelper.GetResourceBytes("AddressLookupQuestionExtension.location48.png");

        public override void WriteHtml(string controlPrefix, CustomQuestionProperties props, TextWriter writer, TextWriter jsWriter)
        {
            if (string.IsNullOrEmpty(props.GetAttributeString(_apiKeyGuid)))
            {
                writer.Write("Error setting up Address Lookup Question. Mappify API Key missing.");
                return;
            }

            int selected;
            int.TryParse(props.GetAttributeString(_isSelectedGuid), out selected);
            if (selected == 1)
            {
                writer.Write("<div class=\"selected\" id=\"" + controlPrefix + "-wrapper\">");
            }
            else
            {
                writer.Write("<div id=\"" + controlPrefix + "-wrapper\">");
            }

            writer.Write("<input type=\"text\" name=\"" + controlPrefix + "\" id=\"" + controlPrefix + "\" placeholder=\"Start typing an address\" class=\"form-control form-control-sm q-prompt\" role=\"presentation\" autocomplete=\"off\" data-type=\"Text\" spellcheck=\"false\"");

            if (!string.IsNullOrEmpty(props.GetAttributeString(_addressGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_addressGuid)));
                writer.Write("\"");
            }
            writer.Write(" onblur=\"" + TRIGGER_REFRESH + "\"");

            writer.Write(" />");

            writer.Write("<div id=\"" + controlPrefix + "-tick\"><svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 64 64\" enable-background=\"new 0 0 64 64\"><path d=\"M32,2C15.431,2,2,15.432,2,32c0,16.568,13.432,30,30,30c16.568,0,30-13.432,30-30C62,15.432,48.568,2,32,2z M25.025,50  l-0.02-0.02L24.988,50L11,35.6l7.029-7.164l6.977,7.184l21-21.619L53,21.199L25.025,50z\" fill=\"#FFFFFF\"/></svg></div>");
            writer.Write("<a class=\"btn btn-secondary\" href=\"#\" id=\"" + controlPrefix + "-changeaddress\" >Change address</a>");

            // Address Line 1
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-addressline1\" id=\"" + controlPrefix + "-addressline1\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_addressline1Guid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_addressline1Guid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            // Suburb
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-suburb\" id=\"" + controlPrefix + "-suburb\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_suburbGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_suburbGuid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            // State
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-state\" id=\"" + controlPrefix + "-state\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_stateGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_stateGuid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            // Country
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-country\" id=\"" + controlPrefix + "-country\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_countryGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_countryGuid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            // Postcode
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-postcode\" id=\"" + controlPrefix + "-postcode\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_postcodeGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_postcodeGuid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            // Temp address
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-tempaddress\" id=\"" + controlPrefix + "-tempaddress\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_tempAddressGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_tempAddressGuid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            // Selected
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-selected\" id=\"" + controlPrefix + "-selected\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_isSelectedGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_isSelectedGuid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            // Defaulted
            writer.Write("<input type=\"hidden\" name=\"" + controlPrefix + "-defaulted\" id=\"" + controlPrefix + "-defaulted\"");
            if (!string.IsNullOrEmpty(props.GetAttributeString(_isDefaultGuid)))
            {
                writer.Write(" value=\"");
                writer.Write(WebUtility.HtmlEncode(props.GetAttributeString(_isDefaultGuid)));
                writer.Write("\"");
            }
            writer.Write(" />");

            writer.Write("<div><ul id=\"" + controlPrefix + "-address-lookup-results\"></ul></div>");

            // JS Code
            var assembly = Assembly.GetExecutingAssembly();
            string jscode = "";
            string jsfile = "AddressLookupQuestionExtension.addressLookup.js";
            using (Stream stream = assembly.GetManifestResourceStream(jsfile))
            using (StreamReader reader = new StreamReader(stream))
            {
                jscode = reader.ReadToEnd();
            }
            jscode = jscode.Replace("{{controlPrefix}}", controlPrefix);
            jscode = jscode.Replace("{{apiKey}}", props.GetAttributeString(_apiKeyGuid));
            jsWriter.Write(jscode);

            // CSS Code
            string csscode = "";
            string cssfile = "AddressLookupQuestionExtension.addressLookup.css";
            using (Stream stream = assembly.GetManifestResourceStream(cssfile))
            using (StreamReader reader = new StreamReader(stream))
            {
                csscode = reader.ReadToEnd();
            }
            csscode = csscode.Replace("[[controlPrefix]]", controlPrefix);

            writer.Write("<style>");
            writer.Write(csscode);
            writer.Write("</style>");

            writer.Write("</div");

        }

        public override List<AvailableInput> GetAvailableInputs()
        {
            return new List<AvailableInput>()
            {
                new AvailableInput()
                {
                    Id = _apiKeyGuid,
                    Name = "Mappify API Key"
                },
                new AvailableInput()
                {
                    Id = _defaultGuid,
                    Name = "Default value"
                }
            };
        }

        public override List<AvailableOutput> GetAvailableOutputs()
        {
            return new List<AvailableOutput>()
            {
                new AvailableOutput()
                {
                    Id = _addressGuid,
                    Name = "Street Address",
                    OutputType = CustomQuestionOutputType.Text
                },
                new AvailableOutput()
                {
                    Id = _addressline1Guid,
                    Name = "Address Line 1",
                    OutputType = CustomQuestionOutputType.Text
                },
                new AvailableOutput()
                {
                    Id = _suburbGuid,
                    Name = "Suburb",
                    OutputType = CustomQuestionOutputType.Text
                },
                new AvailableOutput()
                {
                    Id = _stateGuid,
                    Name = "State",
                    OutputType = CustomQuestionOutputType.Text
                },
                new AvailableOutput()
                {
                    Id = _countryGuid,
                    Name = "Country",
                    OutputType = CustomQuestionOutputType.Text
                },
                new AvailableOutput()
                {
                    Id = _postcodeGuid,
                    Name = "Postcode",
                    OutputType = CustomQuestionOutputType.Text
                },
                new AvailableOutput()
                {
                    Id = _isSelectedGuid,
                    Name = "Selected",
                    OutputType = CustomQuestionOutputType.Text
                },
                new AvailableOutput()
                {
                    Id = _isDefaultGuid,
                    Name = "Defaulted",
                    OutputType = CustomQuestionOutputType.Text
                }
            };
        }

        public override void InitialiseInputs(CustomQuestionProperties props)
        {
            props.UpdateAttribute(_isSelectedGuid, 0);
            props.UpdateAttribute(_isDefaultGuid, 0);
            foreach (CustomQuestionInput input in props.QuestionInputs)
            {
                if (input.InputTypeId == _defaultGuid)
                {
                    props.UpdateAttribute(_addressGuid, input.Value.ToString());
                    props.UpdateAttribute(_isDefaultGuid, 1);
                }
                else if (input.InputTypeId == _apiKeyGuid)
                {
                    props.UpdateAttribute(_apiKeyGuid, input.Value.ToString());
                }
            }
        }

        public override void UpdateAttributes(string controlPrefix, NameValueCollection postedFormValues, CustomQuestionProperties props)
        {
            props.UpdateAttribute(_addressGuid, postedFormValues[controlPrefix]);
            props.UpdateAttribute(_addressline1Guid, postedFormValues[controlPrefix + "-addressline1"]);
            props.UpdateAttribute(_suburbGuid, postedFormValues[controlPrefix + "-suburb"]);
            props.UpdateAttribute(_stateGuid, postedFormValues[controlPrefix + "-state"]);
            props.UpdateAttribute(_countryGuid, postedFormValues[controlPrefix + "-country"]);
            props.UpdateAttribute(_postcodeGuid, postedFormValues[controlPrefix + "-postcode"]);
            props.UpdateAttribute(_isSelectedGuid, postedFormValues[controlPrefix + "-selected"]);
            props.UpdateAttribute(_isDefaultGuid, postedFormValues[controlPrefix + "-defaulted"]);
            props.UpdateAttribute(_tempAddressGuid, postedFormValues[controlPrefix + "-tempaddress"]);
        }

    }
}