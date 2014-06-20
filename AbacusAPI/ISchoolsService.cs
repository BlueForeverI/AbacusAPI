using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using AbacusAPI.Models;

namespace AbacusAPI
{
    [ServiceContract]
    public interface ISchoolsService
    {
        [OperationContract]
        IEnumerable<School> SearchSchoolsByName(string searchPhrase);

        [OperationContract]
        School GetSchoolById(Guid id);
    }
}
