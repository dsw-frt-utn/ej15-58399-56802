using Dsw2026Ej15.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej15.Domain.Interfaces
{
    public interface IPersistance
    {
        IEnumerable<Speciality> GetSpecialities();
        Speciality? GetSpecialityById(Guid id);

        IEnumerable<Doctor> GetActiveDoctors();
        Doctor? GetActiveDoctorById(Guid id);

        void AddDoctor(Doctor doctor);
        void UpdateDoctor(Doctor doctor);
    }
}

