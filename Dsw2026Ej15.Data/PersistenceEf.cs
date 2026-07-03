using Dsw2026Ej15.Data.Context;
using Dsw2026Ej15.Data.Dtos;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Dsw2026Ej15.Data
{
    public class PersistenceEf : IPersistance
    {
        private readonly AppDbContext _context;

        public PersistenceEf(AppDbContext context)
        {
            _context = context;
            LoadSpecialities();
        }

        public IEnumerable<Speciality> GetSpecialities()
        {
            return _context.Specialities.ToList();
        }

        public Speciality? GetSpecialityById(Guid id)
        {
            return _context.Specialities.FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<Doctor> GetActiveDoctors()
        {
            return _context.Doctors
                .Include(d => d.Speciality)
                .Where(d => d.IsActive)
                .ToList();
        }

        public Doctor? GetActiveDoctorById(Guid id)
        {
            return _context.Doctors
                .Include(d => d.Speciality)
                .FirstOrDefault(d => d.Id == id && d.IsActive);
        }

        public void AddDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
        }

        public void UpdateDoctor(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            _context.SaveChanges();
        }

        private void LoadSpecialities()
        {
            if (_context.Specialities.Any())
                return;

            try
            {
                string jsonPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "specialities.json");

                string json = File.ReadAllText(jsonPath);

                List<SpecialityDto> dtos =
                    JsonSerializer.Deserialize<List<SpecialityDto>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? [];

                List<Speciality> specialities = dtos
                    .Select(s => new Speciality(s.Name, s.Description, s.Id))
                    .ToList();

                _context.Specialities.AddRange(specialities);
                _context.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
    }
}
