using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopMVC.Models
{
    public class CategoryViewModel
    {
        Category _ctgrEntity;

        public CategoryViewModel()
        {

        }

        public CategoryViewModel(Category ctgrEntity)
        {
            _ctgrEntity = ctgrEntity;
        }

        public IEnumerable<CategoryViewModel> Children { get; set; }

        public int Id 
        {
            get { return _ctgrEntity.id; }
        }

        public int ParentId
        {
            get { return _ctgrEntity.parent_id; }
        }

        public string Name 
        {
            get { return _ctgrEntity.title; }
        }
    }
}