using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using AbacusAPI.Models;
using AbacusAPI.Repositories;

namespace AbacusAPI
{
    public class SchoolsService : ISchoolsService
    {
        public IEnumerable<School> SearchSchoolsByName(string searchPhrase)
        {
            var repo = new SchoolsRepository();
            return repo.SearchSchoolsByName(searchPhrase);
        }

        public School GetSchoolById(Guid id)
        {
            var repo = new SchoolsRepository();
            return repo.GetSchoolById(id);
        }
    }
}
