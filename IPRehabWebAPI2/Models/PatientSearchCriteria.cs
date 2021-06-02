using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using IModelBinder = Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder;

namespace IPRehabWebAPI2.Models
{
  public class PatientSearchModelBinder : IModelBinder
  {
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
      throw new NotImplementedException();
    }

    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      bindingContext.ValueProvider = new FormValueProvider(controllerContext);
      PatientSearchCriteria model = (PatientSearchCriteria)bindingContext.Model ?? new PatientSearchCriteria();
      model.SSN = GetValue(bindingContext, "SSN");
      return model;
    }

    private string GetValue(ModelBindingContext context, string name)
    {
      ValueProviderResult result = context.ValueProvider.GetValue(name);
      if (result.Values == "")
      {
        return "<Not Specified>";
      }
      return result.Values;
    }
  }
}
