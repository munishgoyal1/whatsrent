using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WhatsRent.DomainModel;

namespace WhatsRent.DataServices.RentService
{
    [ServiceContract(Namespace = "WhatsRent.DataServices.RentService")]
    public interface IRentService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat=WebMessageFormat.Json, UriTemplate= "/rent/{numrec}?city={city}", BodyStyle=WebMessageBodyStyle.Bare)]
        IEnumerable<PropertyRentServiceDto> GetRents(string city, string numrec);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rent/average?city={city}", BodyStyle = WebMessageBodyStyle.Bare)]
        IEnumerable<PropertyAverageRentDto> GetCurrentRents(string city);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/test/{numrec}", BodyStyle = WebMessageBodyStyle.Bare)]
        IEnumerable<PropertyRentServiceTrimmedDto> TestRents(string numrec);
    }
}
