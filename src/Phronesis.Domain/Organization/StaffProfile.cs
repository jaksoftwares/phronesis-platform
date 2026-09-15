using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Organization;

public class StaffProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string EmployeeId { get; private set; }
    public Department Department { get; private set; }
    public string JobTitle { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; private set; }

    public User User { get; private set; } = null!;

    private StaffProfile() { }

    public StaffProfile(Guid userId, string employeeId, Department department, string jobTitle, DateTime hireDate)
    {
        UserId = userId;
        EmployeeId = employeeId;
        Department = department;
        JobTitle = jobTitle;
        HireDate = hireDate;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
