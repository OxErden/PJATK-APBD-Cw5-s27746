namespace WebApplicationDBFirst.DTOs;

public record PatientResponse(
    string Pesel,
    string FirstName,
    string LastName,
    int Age,
    string Sex,
    List<AdmissionResponse> Admissions,
    List<BedAssignmentResponse> BedAssignments);