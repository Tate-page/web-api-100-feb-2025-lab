using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SoftwareCatalog.Api.Technicians;

public class TechniciansController(IDocumentSession session) : ControllerBase
{
    [HttpGet("/technicians")]
    public ActionResult GetTheStatus()
    {
        var savedEntity = session.Query<TechnicianResponse>().GetEnumerator();

        return StatusCode(200, savedEntity);
    }

    [Authorize(Roles = "software-center, manager")]
    [HttpPost("/technicians")]
    public async Task<ActionResult> AddTechnician([FromBody] TechnicianBeforeDBCreation request, [FromServices] IHttpContextAccessor _httpContextAccessor)
    {
        var sub = _httpContextAccessor.HttpContext.User.Identity.Name;
        //TODO: validation
        //save it to the db
        var entityToSave = new TechnicianEntity
        {
            Id = Guid.NewGuid(),
            email = request.email,
            addedBy = sub
        };


        session.Store(entityToSave);
        await session.SaveChangesAsync();
        // send them the reponse
        var response = new TechnicianResponse()
        {
            Id = entityToSave.Id,
            email = entityToSave.email,
            phone = entityToSave.phone,
            fName = entityToSave.fName
        };
        return StatusCode(201, response);
    }
}

public class TechnicianEntity
{
    public Guid Id { get; set; }

    public string? email { get; set; }

    public string? phone { get; set; }

    public string fName { get; set; }

    public string addedBy { get; set; }
}

public class TechnicianBeforeDBCreation
{
    public string? email { get; set; }

    public string? phone { get; set; }

    public string fName { get; set; }
}

public class TechnicianResponse
{
    public Guid Id { get; set; }

    public string? email { get; set; }

    public string? phone { get; set; }
    public string fName { get; set; }
}

public class AddTechnicianRequestValidator : AbstractValidator<TechnicianBeforeDBCreation>
{
    public AddTechnicianRequestValidator()
    {
        RuleFor(x => x.email).NotNull().When(y => string.IsNullOrEmpty(y.phone));
        RuleFor(x => x.phone).NotNull().When(y => string.IsNullOrEmpty(y.email));
        RuleFor(x => x.fName).NotEmpty();
        RuleFor(x => x.fName).NotNull();
    }
}

