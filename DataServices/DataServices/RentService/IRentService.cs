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
        [WebInvoke(Method = "GET", ResponseFormat=WebMessageFormat.Json, UriTemplate= "/rent/city/{city}?area={area}&project={project}&maxrecords={maxrecords}", BodyStyle=WebMessageBodyStyle.Bare)]
        IEnumerable<PropertyRentServiceDto> GetCityRents(string city, string area, string project, string maxrecords);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rent/city/{city}/averagerent?area={area}&project={project}", BodyStyle = WebMessageBodyStyle.Bare)]
        IEnumerable<PropertyAverageRentDto> GetCityCurrentAverageRents(string city, string area, string project);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rent/allcitynames", BodyStyle = WebMessageBodyStyle.Bare)]
        IEnumerable<string> GetAllCityNames();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rent/city/{city}/projectnames", BodyStyle = WebMessageBodyStyle.Bare)]
        IEnumerable<string> GetCityProjectNames(string city);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rent/city/{city}/areanames", BodyStyle = WebMessageBodyStyle.Bare)]
        IEnumerable<string> GetCityAreaNames(string city);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/rent/city/metadata?city={city}", BodyStyle = WebMessageBodyStyle.Bare)]
        Dictionary<string, CityMetadataDto> GetCitiesMetaData(string city);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/test/{numrec}", BodyStyle = WebMessageBodyStyle.Bare)]
        IEnumerable<PropertyRentServiceTrimmedDto> TestRents(string numrec);
    }
}
