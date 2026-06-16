using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ChipmeoApis.Web.Conventions;

public class V2RouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        foreach (var selector in controller.Selectors)
        {
            if (selector.AttributeRouteModel?.Template is { } template)
            {
                selector.AttributeRouteModel.Template = $"v2/{template.TrimStart('/')}";
            }
        }
    }
}
