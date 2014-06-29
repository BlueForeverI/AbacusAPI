using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusAPI.DataContext;
using AbacusAPI.Models;

namespace AbacusAPI.Repositories
{
    public class SchoolsRepository
    {
        public IEnumerable<School> SearchSchoolsByName(string searchPhrase)
        {
            using(AbacusEntities context = new AbacusEntities())
            {
                return context.utSchoolNames
                    .Where(s => s.Name.ToLower().Contains(searchPhrase.ToLower()))
                    .Select(GetSchoolFromEntity).ToList();
            }
        }

        public School GetSchoolById(Guid id)
        {
            using(AbacusEntities context = new AbacusEntities())
            {
                utSchoolName entity = context.utSchoolNames.SingleOrDefault(s => s.Id == id);
                return GetSchoolFromEntity(entity);
            }
        }

        private School GetSchoolFromEntity(utSchoolName entity)
        {
            return new School()
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Street = entity.Street,
                Suburb = entity.Suburb,
                Town = entity.Town
            };
        }
    }
}
