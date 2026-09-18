using Phronesis.Domain.Academic;
using Phronesis.Domain.Common;
using Phronesis.Domain.Identity;

namespace Phronesis.Domain.Tuition;

public enum ClassType
{
    OneOnOne,
    Group
}

public class VirtualClass : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ClassType ClassType { get; private set; }
    public int MaxCapacity { get; private set; }
    public bool IsActive { get; private set; }
    
    // Monetization
    public bool IsSubscriptionIncluded { get; private set; }
    public decimal Price { get; private set; }

    // Foreign Keys
    public Guid TeacherId { get; private set; }
    public Guid SubjectId { get; private set; }

    // Navigations
    public User Teacher { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;
    public ICollection<ClassEnrollment> Enrollments { get; private set; } = new List<ClassEnrollment>();
    public ICollection<ClassSession> Sessions { get; private set; } = new List<ClassSession>();

    private VirtualClass() { } // EF Core

    public VirtualClass(string name, string description, ClassType type, int capacity, Guid teacherId, Guid subjectId, bool isSubIncluded, decimal price)
    {
        Name = name;
        Description = description;
        ClassType = type;
        MaxCapacity = capacity;
        TeacherId = teacherId;
        SubjectId = subjectId;
        IsSubscriptionIncluded = isSubIncluded;
        Price = price;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
